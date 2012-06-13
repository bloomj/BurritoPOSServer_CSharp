using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using BurritoPOSServer.presentation;

namespace BurritoPOSServer.business
{
    /// <summary>
    /// 
    /// </summary>
    class ConnectionManager
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Socket server;
        private volatile Boolean exit = false;
        private List<Thread> sThreads = new List<Thread>();
        private List<SocketManager> sManagers = new List<SocketManager>();
        private ManualResetEvent allDone = new ManualResetEvent(true);
        public event EventHandler<MessageArgs> StatusUpdate;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionManager()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
        }

        /// <summary>
        /// 
        /// </summary>
        public void startServer()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 8000);
            foreach (IPAddress ip in ipHostInfo.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    localEP = new IPEndPoint(ip, 8000);
            
            dLog.Debug("Local address and port: " + localEP.Address.ToString() + " | " + localEP.Port.ToString());
            updateStatus(new MessageArgs("Local address and port: " + localEP.Address.ToString() + " | " + localEP.Port.ToString()));

            server = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Bind(localEP);
                server.Listen(100);

                while (!this.exit)
                {
                    allDone.Reset();

                    dLog.Debug("Waiting for a connection...");
                    updateStatus(new MessageArgs("Waiting for connection..."));
                    server.BeginAccept(new AsyncCallback(this.acceptCallback), server);

                    allDone.WaitOne();
                }

                // set exit on remaining socket managers
                foreach (SocketManager s in sManagers)
                    s.setExit(true);

                // close any remaining threads
                foreach (Thread t in sThreads)
                {
                    if(t.IsAlive)
                        t.Join();
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in startServer: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                dLog.Debug("Closing the listener...");
                updateStatus(new MessageArgs("Closing listener..."));

                if (server != null)
                {
                    if (server.Connected)
                        server.Disconnect(true);

                    if (server.IsBound)
                    {
                        server.Close();
                        server = null;
                    }
                }
            }
        }

        void acceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                if (server != null)
                {
                    dLog.Debug("Got a new connection");
                    Socket tSock = server.EndAccept(ar);
                    updateStatus(new MessageArgs("Got a new connection: " + ((IPEndPoint)tSock.RemoteEndPoint).Address + " | Port: " + ((IPEndPoint)tSock.RemoteEndPoint).Port));

                    sManagers.Add(new SocketManager(tSock, this));
                    sThreads.Add(new Thread(sManagers[sManagers.Count - 1].run));
                    sThreads[sThreads.Count - 1].Name = "Socket Thread " + sThreads.Count;
                    sThreads[sThreads.Count - 1].Start();

                    dLog.Debug("sManagers Count: " + sManagers.Count);
                    dLog.Debug("sThreads Count: " + sThreads.Count);
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in acceptCallback: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public Boolean setExit(Boolean exit)
        {
            Boolean result = false;

            try
            {
                dLog.Debug("setting exit: " + exit);
                updateStatus(new MessageArgs("setting exit: " + exit));
                this.exit = exit;

                allDone.Set();
                result = true;
            }
            catch (Exception e)
            {
                dLog.Error("Exception in setExit: " + e.Message + "\n" + e.StackTrace);
            }

            return result;
        }

        // probably not thread-safe for children to call back up but works for now
        public void updateStatus(String msg)
        {
            dLog.Debug("In public status update | msg: " + msg);
            updateStatus(new MessageArgs(msg));
        }

        protected void updateStatus(MessageArgs e)
        {
            if (StatusUpdate != null)
            {
                StatusUpdate(this, e);
            }
        }

        public Int32 getNumberOfConnections()
        {
            Int32 result = 0;

            try
            {
                result = sManagers.Count;
            }
            catch (Exception e)
            {
                dLog.Error("Exception in getNumberOfConnections: " + e.Message + "\n" + e.StackTrace);
            }

            return result;
        }

        public Boolean isConnected()
        {
            Boolean result = false;

            try
            {
                if (server != null)
                {
                    bool part1 = server.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (server.Available == 0);
                    if (part1 & part2)
                        result = false;
                    else
                        result = true;
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in isConnected: " + e.Message + "\n" + e.StackTrace);
            }

            return result;
        }
    }
}
