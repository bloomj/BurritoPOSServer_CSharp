namespace BurritoPOSServer.presentation
{
    partial class StatusUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startSvrBtn = new System.Windows.Forms.Button();
            this.stopSvrBtn = new System.Windows.Forms.Button();
            this.statusMsgs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startSvrBtn
            // 
            this.startSvrBtn.Location = new System.Drawing.Point(160, 13);
            this.startSvrBtn.Name = "startSvrBtn";
            this.startSvrBtn.Size = new System.Drawing.Size(75, 23);
            this.startSvrBtn.TabIndex = 0;
            this.startSvrBtn.Text = "Start Server";
            this.startSvrBtn.UseVisualStyleBackColor = true;
            this.startSvrBtn.Click += new System.EventHandler(this.startSvrBtn_Click);
            // 
            // stopSvrBtn
            // 
            this.stopSvrBtn.Enabled = false;
            this.stopSvrBtn.Location = new System.Drawing.Point(241, 12);
            this.stopSvrBtn.Name = "stopSvrBtn";
            this.stopSvrBtn.Size = new System.Drawing.Size(75, 23);
            this.stopSvrBtn.TabIndex = 1;
            this.stopSvrBtn.Text = "Stop Server";
            this.stopSvrBtn.UseVisualStyleBackColor = true;
            this.stopSvrBtn.Click += new System.EventHandler(this.stopSvrBtn_Click);
            // 
            // statusMsgs
            // 
            this.statusMsgs.FormattingEnabled = true;
            this.statusMsgs.Location = new System.Drawing.Point(78, 43);
            this.statusMsgs.Name = "statusMsgs";
            this.statusMsgs.Size = new System.Drawing.Size(377, 186);
            this.statusMsgs.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Status:";
            // 
            // StatusUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 245);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusMsgs);
            this.Controls.Add(this.stopSvrBtn);
            this.Controls.Add(this.startSvrBtn);
            this.Name = "StatusUI";
            this.Text = "Burrito POS Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startSvrBtn;
        private System.Windows.Forms.Button stopSvrBtn;
        private System.Windows.Forms.ListBox statusMsgs;
        private System.Windows.Forms.Label label1;
    }
}