namespace Updater.NetFramework.WinForms.UITests
{
    partial class Form1
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
            this.btnManual = new System.Windows.Forms.Button();
            this.btnAutomatic = new System.Windows.Forms.Button();
            this.btnAutomaticDownload_UpdateOnRequest = new System.Windows.Forms.Button();
            this.btnDownloadAndUpdateOnRequest = new System.Windows.Forms.Button();
            this.btnGetStatus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnManual
            // 
            this.btnManual.Location = new System.Drawing.Point(12, 12);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(234, 23);
            this.btnManual.TabIndex = 0;
            this.btnManual.Text = "Manual";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
            // 
            // btnAutomatic
            // 
            this.btnAutomatic.Location = new System.Drawing.Point(12, 41);
            this.btnAutomatic.Name = "btnAutomatic";
            this.btnAutomatic.Size = new System.Drawing.Size(234, 23);
            this.btnAutomatic.TabIndex = 1;
            this.btnAutomatic.Text = "Automatic";
            this.btnAutomatic.UseVisualStyleBackColor = true;
            this.btnAutomatic.Click += new System.EventHandler(this.btnAutomatic_Click);
            // 
            // btnAutomaticDownload_UpdateOnRequest
            // 
            this.btnAutomaticDownload_UpdateOnRequest.Location = new System.Drawing.Point(12, 70);
            this.btnAutomaticDownload_UpdateOnRequest.Name = "btnAutomaticDownload_UpdateOnRequest";
            this.btnAutomaticDownload_UpdateOnRequest.Size = new System.Drawing.Size(234, 23);
            this.btnAutomaticDownload_UpdateOnRequest.TabIndex = 2;
            this.btnAutomaticDownload_UpdateOnRequest.Text = "AutomaticDownload_UpdateOnRequest";
            this.btnAutomaticDownload_UpdateOnRequest.UseVisualStyleBackColor = true;
            this.btnAutomaticDownload_UpdateOnRequest.Click += new System.EventHandler(this.btnAutomaticDownload_UpdateOnRequest_Click);
            // 
            // btnDownloadAndUpdateOnRequest
            // 
            this.btnDownloadAndUpdateOnRequest.Location = new System.Drawing.Point(12, 99);
            this.btnDownloadAndUpdateOnRequest.Name = "btnDownloadAndUpdateOnRequest";
            this.btnDownloadAndUpdateOnRequest.Size = new System.Drawing.Size(234, 23);
            this.btnDownloadAndUpdateOnRequest.TabIndex = 3;
            this.btnDownloadAndUpdateOnRequest.Text = "DownloadAndUpdateOnRequest";
            this.btnDownloadAndUpdateOnRequest.UseVisualStyleBackColor = true;
            this.btnDownloadAndUpdateOnRequest.Click += new System.EventHandler(this.btnDownloadAndUpdateOnRequest_Click);
            // 
            // btnGetStatus
            // 
            this.btnGetStatus.Location = new System.Drawing.Point(12, 128);
            this.btnGetStatus.Name = "btnGetStatus";
            this.btnGetStatus.Size = new System.Drawing.Size(234, 23);
            this.btnGetStatus.TabIndex = 4;
            this.btnGetStatus.Text = "Get Status";
            this.btnGetStatus.UseVisualStyleBackColor = true;
            this.btnGetStatus.Click += new System.EventHandler(this.btnGetStatus_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 165);
            this.Controls.Add(this.btnGetStatus);
            this.Controls.Add(this.btnDownloadAndUpdateOnRequest);
            this.Controls.Add(this.btnAutomaticDownload_UpdateOnRequest);
            this.Controls.Add(this.btnAutomatic);
            this.Controls.Add(this.btnManual);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Button btnAutomatic;
        private System.Windows.Forms.Button btnAutomaticDownload_UpdateOnRequest;
        private System.Windows.Forms.Button btnDownloadAndUpdateOnRequest;
        private System.Windows.Forms.Button btnGetStatus;
    }
}

