namespace OohelpWebApps.Software.Updater.Dialogs;

partial class UpdateDownloadDialog
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
            this.lblCurrentOperation = new System.Windows.Forms.Label();
            this.lblCurrentProgress = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.lblUpdateVersion = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lblCurrentOperation
            // 
            this.lblCurrentOperation.AutoSize = true;
            this.lblCurrentOperation.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCurrentOperation.Location = new System.Drawing.Point(107, 192);
            this.lblCurrentOperation.Name = "lblCurrentOperation";
            this.lblCurrentOperation.Size = new System.Drawing.Size(134, 15);
            this.lblCurrentOperation.TabIndex = 2;
            this.lblCurrentOperation.Text = "Загрузка обновления...";
            // 
            // lblCurrentProgress
            // 
            this.lblCurrentProgress.AutoSize = true;
            this.lblCurrentProgress.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCurrentProgress.Location = new System.Drawing.Point(107, 238);
            this.lblCurrentProgress.Name = "lblCurrentProgress";
            this.lblCurrentProgress.Size = new System.Drawing.Size(192, 15);
            this.lblCurrentProgress.TabIndex = 3;
            this.lblCurrentProgress.Text = "Завершено: 30% (13 Мб / 125 Мб)";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(-5, 433);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(728, 2);
            this.label1.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.button1.Location = new System.Drawing.Point(587, 455);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 30);
            this.button1.TabIndex = 5;
            this.button1.Text = "Отмена";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblCurrentVersion.Location = new System.Drawing.Point(27, 451);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(131, 15);
            this.lblCurrentVersion.TabIndex = 6;
            this.lblCurrentVersion.Text = "Текущая версия: 17.3.1";
            // 
            // lblUpdateVersion
            // 
            this.lblUpdateVersion.AutoSize = true;
            this.lblUpdateVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblUpdateVersion.Location = new System.Drawing.Point(27, 476);
            this.lblUpdateVersion.Name = "lblUpdateVersion";
            this.lblUpdateVersion.Size = new System.Drawing.Size(152, 15);
            this.lblUpdateVersion.TabIndex = 7;
            this.lblUpdateVersion.Text = "Версия обновления: 17.3.2";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(107, 221);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(475, 5);
            this.pbProgress.TabIndex = 8;
            // 
            // UpdateDownloadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            this.ClientSize = new System.Drawing.Size(719, 506);
            this.Controls.Add(this.lblUpdateVersion);
            this.Controls.Add(this.lblCurrentVersion);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCurrentProgress);
            this.Controls.Add(this.lblCurrentOperation);
            this.Controls.Add(this.pbProgress);
            this.Name = "UpdateDownloadDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UpdateDownloadDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label lblCurrentOperation;
    private System.Windows.Forms.Label lblCurrentProgress;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label lblCurrentVersion;
    private System.Windows.Forms.Label lblUpdateVersion;
    private System.Windows.Forms.ProgressBar pbProgress;
}