using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;
using log4net.Config;
using System.Threading;
using BurritoPOSServer.business;

namespace BurritoPOSServer.presentation
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StatusUI : Form
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConnectionManager neatoServer;
        private Thread neatoServerThread;

        /// <summary>
        /// 
        /// </summary>
        public StatusUI()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
            InitializeComponent();

            this.FormClosing += Window_Closing;

            statusMsgs.Items.Add("Burrito POS Server UI Started");
        }

        private void startSvrBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if(neatoServerThread == null || !neatoServerThread.IsAlive) {
                    dLog.Debug("Creating Connection Manger object");
                    neatoServer = new ConnectionManager();
                    neatoServer.StatusUpdate += new EventHandler<MessageArgs>(OnStatusUpdate);
                    neatoServerThread = new Thread(neatoServer.startServer);
                    neatoServerThread.Name = "Server Thread";
                }

                dLog.Debug("Starting server thread");
                neatoServerThread.Start();
                startSvrBtn.Enabled = false;
                stopSvrBtn.Enabled = true;
            }
            catch (Exception e1)
            {
                dLog.Error("Exception in startSvrBtn_Click: " + e1.Message + "\n" + e1.StackTrace);
            }
        }

        private void stopSvrBtn_Click(object sender, EventArgs e)
        {
            try
            {
                dLog.Debug("Ending server thread");
                neatoServer.setExit(true);
                if(neatoServerThread.IsAlive)
                    neatoServerThread.Join();
                startSvrBtn.Enabled = true;
                stopSvrBtn.Enabled = false;
            }
            catch (Exception e1)
            {
                dLog.Error("Exception in stopSvrBtn_Click: " + e1.Message + "\n" + e1.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void updateStatus(String msg) 
        {
            try
            {
                statusMsgs.Items.Add(msg);
                statusMsgs.SelectedIndex = statusMsgs.Items.Count - 1;
                statusMsgs.Invalidate();
            }
            catch (Exception e1)
            {
                dLog.Error("Exception in updateStatus: " + e1.Message + "\n" + e1.StackTrace);
            }
        }

        /// <summary>
        /// Updates statusMsgs listbox with status from child threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStatusUpdate(object sender, MessageArgs e)
        {
            //cross thread - so you don't get the cross theading exception
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    OnStatusUpdate(sender, e);
                });
                return;
            }

            //update status
            statusMsgs.Items.Add(e.msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dLog.Debug("Ending server thread");
            neatoServer.setExit(true);
            if (neatoServerThread.IsAlive)
                neatoServerThread.Join();
        }
    }

    /// <summary>
    /// Class used to hold EventArgs to pass status messages back to UI
    /// </summary>
    public class MessageArgs : EventArgs
    {
        /// <summary>
        /// Status Message to display in UI
        /// </summary>
        public String msg { get; private set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_msg">Status Message</param>
        public MessageArgs(String _msg)
        {
            msg = _msg;
        }
    }
}
