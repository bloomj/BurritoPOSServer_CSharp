using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Spring;
using Spring.Context;
using Spring.Context.Support;
using BurritoPOSServer.domain;
using BurritoPOSServer.service;
using BurritoPOSServer.presentation;

namespace BurritoPOSServer.business
{
    /// <summary>
    /// 
    /// </summary>
    class SocketManager
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Socket socket;
        private byte[] receiveBuffer = new byte[4096];
        private byte[] writeBuffer = new byte[4096];
        private volatile Boolean exit = false;
        private Boolean auth = false;
        private User tUser = new User();
        private List<User> users = new List<User>();
        private ConnectionManager parent;
        private IUserSvc userSvc;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        public SocketManager(Socket socket)
        {
            setDefaults();
            this.socket = socket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="_par"></param>
        public SocketManager(Socket socket, ConnectionManager _par)
        {
            setDefaults();
            this.socket = socket;
            this.parent = _par;
        }

        /// <summary>
        /// 
        /// </summary>
        private void setDefaults()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));

            //will comment out after Spring.NET implementation
            //userSvc = (IUserSvc)factory.getService("IUserSvc");

            //Spring.NET
            XmlApplicationContext ctx = new XmlApplicationContext("config/spring.cfg.xml");
            userSvc = (IUserSvc)ctx.GetObject("userSvc");

            users = userSvc.getAllUsers();
        }

        public void run()
        {
            try
            {
                String inputStr = "";
                writeObject("Burrito POS Server Connected. Enter Username: ");

                inputStr = readObject();

                while (isConnected() && !inputStr.Equals("exit") && !this.exit)
                {
                    if (tUser.userName == null)
                    {
                        tUser.userName = inputStr;
                        writeObject("OK User " + tUser.userName + ", enter password: ");
                    }
                    else if (tUser.password == null)
                    {
                        dLog.Debug("Username: " + tUser.userName + " | Password: " + inputStr);
                        for (int n = 0; n < users.Count; n++)
                        {
                            dLog.Debug("Stored user: " + users[n].userName + " | Password: " + users[n].password);
                            //if (users[n].userName.Equals(tUser.userName) && users[n].password.Equals(inputStr))
                            if(users[n].userName.Equals(tUser.userName) && BCrypt.CheckPassword(inputStr, users[n].password))
                            {
                                // set password in user object
                                tUser.password = inputStr;
                                break;
                            }
                        }

                        if (tUser.password != null)
                        {
                            writeObject("OK User verified. Enter command: ");
                            auth = true;
                        }
                        else
                        {
                            tUser = new User();
                            writeObject("ERROR Invalid Credentials. Enter Username: ");
                        }
                    }
                    else if (auth)
                    {
                        if (!inputStr.Equals("exit"))
                        {
                            writeObject("OK Command " + tUser.userName + " entered. Enter command: ");
                        }
                    }

                    inputStr = readObject();
                }
            }
            catch (Exception e)
            {
                dLog.Error("Exception in run: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                setExit(true);
            }
        }

        private void writeObject(String msg) {
            try
            {
                writeBuffer = Encoding.ASCII.GetBytes(msg);
                socket.Send(writeBuffer);
                dLog.Debug("OUPUT | " + msg);
                updateStatus("OUTPUT | " + msg);
            }
            catch (SocketException se)
            {

            }
            catch (Exception e)
            {
                dLog.Error("Exception in writeObject: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private String readObject() {
            String msg = "";

            try
            {
                dLog.Debug("In readObject");
                while (isConnected() && !this.exit && msg == "")
                {
                    receiveBuffer = new byte[4096];
                    int recBytes = socket.Receive(receiveBuffer);
                    msg = Encoding.ASCII.GetString(receiveBuffer, 0, recBytes).Trim();
                }

                dLog.Debug("INPUT | " + msg);
                updateStatus("INPUT | " + msg);
            }
            catch (SocketException se)
            {
                msg = "";
            }
            catch (Exception e)
            {
                dLog.Error("Exception in readObject: " + e.Message + "\n" + e.StackTrace);
                msg = "";
            }

            return msg;
        }

        private void updateStatus(String msg) {
            try {
                if (parent != null && socket != null)
                    parent.updateStatus("  - " + ((IPEndPoint)socket.RemoteEndPoint).Address + ":" + ((IPEndPoint)socket.RemoteEndPoint).Port + " | " + msg);
            }
            catch(Exception e) {
                dLog.Error("Exception in updateStatus: " + e.Message + "\n" + e.StackTrace);
            }
        }

        public Boolean setExit(Boolean exit)
        {
            Boolean result = false;

            try
            {
                dLog.Debug("setExit: " + exit);
                this.exit = exit;
                updateStatus("Disconnecting client");

                if (socket != null)
                {
                    if (socket.Connected)
                        socket.Shutdown(SocketShutdown.Both);

                    if (socket.IsBound)
                    {
                        socket.Close();
                        socket = null;
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                dLog.Error("Exception in setExit: " + e.Message + "\n" + e.StackTrace);
            }

            return result;
        }

        public Boolean isConnected()
        {
            Boolean result = false;

            try {
                if (socket != null)
                {
                    bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (socket.Available == 0);
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
