using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;
using System.Net;
using System.Net.Sockets;
using Spring;
using Spring.Context;
using Spring.Context.Support;
using NUnit.Framework;
using BurritoPOSServer.service;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.business.test
{
    /// <summary>
    /// Unit test fixture for ConnectionManager / SocketManager unit tests
    /// </summary>
    [TestFixture]
    class ConnectionManagerTestCase
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConnectionManager neatoServer;
        private Thread neatoServerThread;
        private Socket client;
        private byte[] receiveBuffer = new byte[4096];
        private byte[] writeBuffer = new byte[4096];
        private IUserSvc userSvc;

        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));

            neatoServer = new ConnectionManager();

            //will comment out after Spring.NET implementation
            //userSvc = (IUserSvc)factory.getService("IUserSvc");

            //Spring.NET
            XmlApplicationContext ctx = new XmlApplicationContext("config/spring.cfg.xml");
            userSvc = (IUserSvc)ctx.GetObject("userSvc");
        }

        /// <summary>
        /// 
        /// </summary>
        [TearDown]
        protected void tearDown()
        {

        }

        /// <summary>
        /// Unit test for ConnectionManager
        /// </summary>
        [Test]
        public void testConnectionManager()
        {
            try
            {
                neatoServerThread = new Thread(neatoServer.startServer);
                neatoServerThread.Name = "Server Thread";
                neatoServerThread.Start();

                //sleep thread before checking
                dLog.Debug("Connection Manager thread started.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is connected
                Assert.True(neatoServer.isConnected());

                // test if we can connect
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 8000);
                foreach (IPAddress ip in ipHostInfo.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        localEP = new IPEndPoint(ip, 8000);

                dLog.Debug("Local address and port: " + localEP.Address.ToString() + " | " + localEP.Port.ToString());

                client = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(localEP.Address, 8000);

                //sleep thread before checking
                dLog.Debug("Client connected.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if we now have one connection
                Assert.AreEqual(1, neatoServer.getNumberOfConnections());

                // disconnect client
                if (client.Connected)
                    client.Shutdown(SocketShutdown.Both);

                if (client.IsBound)
                {
                    client.Close();
                    client = null;
                }

                // test setExit
                Assert.True(neatoServer.setExit(true));

                //sleep thread before checking
                dLog.Debug("Set exit.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is now disconnected
                Assert.False(neatoServer.isConnected());

                // test thread termination
                if (neatoServerThread.IsAlive)
                {
                    if (!neatoServerThread.Join(5000))
                    {
                        Assert.Fail("Thread did not return in timely fashion");
                    }
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testConnectionManager: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        /// <summary>
        /// Unit test for ConnectionManager improper shutdown
        /// </summary>
        [Test]
        public void testImproperShutdown()
        {
            try
            {
                neatoServerThread = new Thread(neatoServer.startServer);
                neatoServerThread.Name = "Server Thread";
                neatoServerThread.Start();

                //sleep thread before checking
                dLog.Debug("Connection Manager thread started.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is connected
                Assert.True(neatoServer.isConnected());

                // test if we can connect
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 8000);
                foreach (IPAddress ip in ipHostInfo.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        localEP = new IPEndPoint(ip, 8000);

                dLog.Debug("Local address and port: " + localEP.Address.ToString() + " | " + localEP.Port.ToString());

                client = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(localEP.Address, 8000);

                //sleep thread before checking
                dLog.Debug("Client connected.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if we now have one connection
                Assert.AreEqual(1, neatoServer.getNumberOfConnections());

                // test setExit
                Assert.True(neatoServer.setExit(true));

                //sleep thread before checking
                dLog.Debug("Set exit.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is now disconnected
                Assert.False(neatoServer.isConnected());

                // test thread termination
                if (neatoServerThread.IsAlive)
                {
                    if (!neatoServerThread.Join(5000))
                    {
                        Assert.Fail("Thread did not return in timely fashion");
                    }
                }

                if (client.Connected)
                    client.Shutdown(SocketShutdown.Both);

                if (client.IsBound)
                {
                    client.Close();
                    client = null;
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testImproperShutdown: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        /// <summary>
        /// Unit test for SocketManager
        /// </summary>
        [Test]
        public void testSocketManager()
        {
            try
            {
                // create new user and pass to service layer to store
                User tUser = new User(1, "JimB", BCrypt.HashPassword("pass123", BCrypt.GenerateSalt()));
                Assert.True(userSvc.storeUser(tUser));

                neatoServerThread = new Thread(neatoServer.startServer);
                neatoServerThread.Name = "Server Thread";
                neatoServerThread.Start();

                //sleep thread before checking
                dLog.Debug("Connection Manager thread started.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is connected
                Assert.True(neatoServer.isConnected());

                // test if we can connect
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 8000);
                foreach (IPAddress ip in ipHostInfo.AddressList)
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        localEP = new IPEndPoint(ip, 8000);

                dLog.Debug("Local address and port: " + localEP.Address.ToString() + " | " + localEP.Port.ToString());

                client = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(localEP.Address, 8000);

                //sleep thread before checking
                dLog.Debug("Client connected.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if we now have one connection
                Assert.AreEqual(1, neatoServer.getNumberOfConnections());

                // test Socket Manager
                String inputStr = readObject();
                writeObject(tUser.userName);
                inputStr = readObject();
                writeObject("pass123");
                inputStr = readObject();

                //check out input
                Assert.True(inputStr.Split(' ')[0].Equals("OK"));

                writeObject("exit");

                // disconnect client
                if (client.Connected)
                    client.Shutdown(SocketShutdown.Both);

                if (client.IsBound)
                {
                    client.Close();
                    client = null;
                }

                // test setExit
                Assert.True(neatoServer.setExit(true));

                //sleep thread before checking
                dLog.Debug("Set exit.  Sleeping 3 secs before checking status");
                Thread.Sleep(3000);

                // test if socket is now disconnected
                Assert.False(neatoServer.isConnected());

                // test thread termination
                if (neatoServerThread.IsAlive)
                {
                    if (!neatoServerThread.Join(5000))
                    {
                        Assert.Fail("Thread did not return in timely fashion");
                    }
                }

                // remove the test user
                Assert.True(userSvc.deleteUser(tUser.id));
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testConnectionManager: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        private void writeObject(String msg)
        {
            try
            {
                writeBuffer = Encoding.ASCII.GetBytes(msg);
                client.Send(writeBuffer);
                dLog.Debug("OUPUT | " + msg);
            }
            catch (Exception e)
            {
                dLog.Error("Exception in writeObject: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private String readObject()
        {
            String msg = "";

            try
            {
                dLog.Debug("In readObject");
                while (isConnected() && msg == "")
                {
                    receiveBuffer = new byte[4096];
                    int recBytes = client.Receive(receiveBuffer);
                    msg = Encoding.ASCII.GetString(receiveBuffer, 0, recBytes).Trim();
                }

                dLog.Debug("INPUT | " + msg);
            }
            catch (Exception e)
            {
                dLog.Error("Exception in readObject: " + e.Message + "\n" + e.StackTrace);
                msg = "";
            }

            return msg;
        }

        private bool isConnected()
        {
            bool part1 = client.Poll(1000, SelectMode.SelectRead);
            bool part2 = (client.Available == 0);
            if (part1 & part2)
                return false;
            else
                return true;

        }
    }
}
