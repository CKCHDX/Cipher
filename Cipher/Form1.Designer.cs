using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Cipher
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.btnEncryptFile = new System.Windows.Forms.Button();
            this.progressBarFileEncryption = new System.Windows.Forms.ProgressBar();
            this.progressBarFileDecryption = new System.Windows.Forms.ProgressBar();
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.progressBarFolderEncryption = new System.Windows.Forms.ProgressBar();
            this.progressBarFolderDecryption = new System.Windows.Forms.ProgressBar();
            this.progressBarPasswordEncryption = new System.Windows.Forms.ProgressBar();
            this.btnDecryptFile = new System.Windows.Forms.Button();
            this.btnEncryptFolder = new System.Windows.Forms.Button();
            this.btnDecryptFolder = new System.Windows.Forms.Button();
            this.panelDragDrop = new System.Windows.Forms.Panel();
            this.lstDragDrop = new System.Windows.Forms.ListBox();
            this.lblStatusSignal = new System.Windows.Forms.Label();
            this.btnEncryptDrive = new System.Windows.Forms.Button();
            this.btnDecryptDrive = new System.Windows.Forms.Button();
            this.btnTurnToSecurityKey = new System.Windows.Forms.Button();
            this.progressBarDriveEncryption = new System.Windows.Forms.ProgressBar();
            this.progressBarDriveDecryption = new System.Windows.Forms.ProgressBar();
            this.comboBoxDrives = new System.Windows.Forms.ComboBox();
            this.UnlockDevice = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.btnBruteForce = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEncryptFile
            // 
            this.btnEncryptFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnEncryptFile.Location = new System.Drawing.Point(12, 21);
            this.btnEncryptFile.Name = "btnEncryptFile";
            this.btnEncryptFile.Size = new System.Drawing.Size(75, 23);
            this.btnEncryptFile.TabIndex = 0;
            this.btnEncryptFile.Text = "Encrypt File";
            this.btnEncryptFile.UseVisualStyleBackColor = false;
            this.btnEncryptFile.Click += new System.EventHandler(this.BtnEncryptFile_Click);
            this.btnEncryptFile.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnEncryptFile.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnEncryptFile.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // progressBarFileEncryption
            // 
            this.progressBarFileEncryption.Location = new System.Drawing.Point(727, 21);
            this.progressBarFileEncryption.Name = "progressBarFileEncryption";
            this.progressBarFileEncryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarFileEncryption.TabIndex = 1;
            // 
            // progressBarFileDecryption
            // 
            this.progressBarFileDecryption.Location = new System.Drawing.Point(727, 50);
            this.progressBarFileDecryption.Name = "progressBarFileDecryption";
            this.progressBarFileDecryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarFileDecryption.TabIndex = 2;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.BackColor = System.Drawing.Color.LightCyan;
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStatus.ForeColor = System.Drawing.Color.Black;
            this.txtStatus.HideSelection = false;
            this.txtStatus.Location = new System.Drawing.Point(1196, 21);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtStatus.Size = new System.Drawing.Size(460, 372);
            this.txtStatus.TabIndex = 3;
            this.txtStatus.Text = "";
            this.txtStatus.ZoomFactor = 2F;
            // 
            // progressBarFolderEncryption
            // 
            this.progressBarFolderEncryption.Location = new System.Drawing.Point(727, 79);
            this.progressBarFolderEncryption.Name = "progressBarFolderEncryption";
            this.progressBarFolderEncryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarFolderEncryption.TabIndex = 4;
            // 
            // progressBarFolderDecryption
            // 
            this.progressBarFolderDecryption.BackColor = System.Drawing.Color.PaleTurquoise;
            this.progressBarFolderDecryption.Location = new System.Drawing.Point(727, 137);
            this.progressBarFolderDecryption.Name = "progressBarFolderDecryption";
            this.progressBarFolderDecryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarFolderDecryption.TabIndex = 5;
            // 
            // progressBarPasswordEncryption
            // 
            this.progressBarPasswordEncryption.Location = new System.Drawing.Point(727, 108);
            this.progressBarPasswordEncryption.Name = "progressBarPasswordEncryption";
            this.progressBarPasswordEncryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarPasswordEncryption.TabIndex = 6;
            // 
            // btnDecryptFile
            // 
            this.btnDecryptFile.Location = new System.Drawing.Point(103, 21);
            this.btnDecryptFile.Name = "btnDecryptFile";
            this.btnDecryptFile.Size = new System.Drawing.Size(75, 23);
            this.btnDecryptFile.TabIndex = 9;
            this.btnDecryptFile.Text = "Decrypt File";
            this.btnDecryptFile.UseVisualStyleBackColor = true;
            this.btnDecryptFile.Click += new System.EventHandler(this.btnDecryptFile_Click);
            this.btnDecryptFile.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnDecryptFile.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnDecryptFile.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnEncryptFolder
            // 
            this.btnEncryptFolder.Location = new System.Drawing.Point(12, 72);
            this.btnEncryptFolder.Name = "btnEncryptFolder";
            this.btnEncryptFolder.Size = new System.Drawing.Size(87, 23);
            this.btnEncryptFolder.TabIndex = 10;
            this.btnEncryptFolder.Text = "Encrypt Folder";
            this.btnEncryptFolder.UseVisualStyleBackColor = true;
            this.btnEncryptFolder.Click += new System.EventHandler(this.btnEncryptFolder_Click);
            this.btnEncryptFolder.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnEncryptFolder.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnEncryptFolder.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnDecryptFolder
            // 
            this.btnDecryptFolder.Location = new System.Drawing.Point(104, 72);
            this.btnDecryptFolder.Name = "btnDecryptFolder";
            this.btnDecryptFolder.Size = new System.Drawing.Size(87, 23);
            this.btnDecryptFolder.TabIndex = 11;
            this.btnDecryptFolder.Text = "Decrypt Folder";
            this.btnDecryptFolder.UseVisualStyleBackColor = true;
            this.btnDecryptFolder.Click += new System.EventHandler(this.btnDecryptFolder_Click);
            this.btnDecryptFolder.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnDecryptFolder.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnDecryptFolder.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // panelDragDrop
            // 
            this.panelDragDrop.AllowDrop = true;
            this.panelDragDrop.BackColor = System.Drawing.Color.LightCyan;
            this.panelDragDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDragDrop.Location = new System.Drawing.Point(727, 247);
            this.panelDragDrop.Name = "panelDragDrop";
            this.panelDragDrop.Size = new System.Drawing.Size(463, 146);
            this.panelDragDrop.TabIndex = 12;
            this.panelDragDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.PanelDragDrop_DragDrop);
            this.panelDragDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.PanelDragDrop_DragEnter);
            // 
            // lstDragDrop
            // 
            this.lstDragDrop.BackColor = System.Drawing.Color.LightCyan;
            this.lstDragDrop.FormattingEnabled = true;
            this.lstDragDrop.Location = new System.Drawing.Point(12, 103);
            this.lstDragDrop.Name = "lstDragDrop";
            this.lstDragDrop.Size = new System.Drawing.Size(709, 290);
            this.lstDragDrop.TabIndex = 0;
            // 
            // lblStatusSignal
            // 
            this.lblStatusSignal.BackColor = System.Drawing.Color.White;
            this.lblStatusSignal.Location = new System.Drawing.Point(727, 221);
            this.lblStatusSignal.Name = "lblStatusSignal";
            this.lblStatusSignal.Size = new System.Drawing.Size(463, 23);
            this.lblStatusSignal.TabIndex = 13;
            // 
            // btnEncryptDrive
            // 
            this.btnEncryptDrive.Location = new System.Drawing.Point(184, 21);
            this.btnEncryptDrive.Name = "btnEncryptDrive";
            this.btnEncryptDrive.Size = new System.Drawing.Size(100, 23);
            this.btnEncryptDrive.TabIndex = 12;
            this.btnEncryptDrive.Text = "Encrypt Drive";
            this.btnEncryptDrive.UseVisualStyleBackColor = true;
            this.btnEncryptDrive.Click += new System.EventHandler(this.btnEncryptDrive_Click);
            this.btnEncryptDrive.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnEncryptDrive.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnEncryptDrive.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnDecryptDrive
            // 
            this.btnDecryptDrive.Location = new System.Drawing.Point(197, 72);
            this.btnDecryptDrive.Name = "btnDecryptDrive";
            this.btnDecryptDrive.Size = new System.Drawing.Size(100, 23);
            this.btnDecryptDrive.TabIndex = 13;
            this.btnDecryptDrive.Text = "Decrypt Drive";
            this.btnDecryptDrive.UseVisualStyleBackColor = true;
            this.btnDecryptDrive.Click += new System.EventHandler(this.btnDecryptDrive_Click);
            this.btnDecryptDrive.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnDecryptDrive.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnDecryptDrive.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnTurnToSecurityKey
            // 
            this.btnTurnToSecurityKey.Location = new System.Drawing.Point(12, 46);
            this.btnTurnToSecurityKey.Name = "btnTurnToSecurityKey";
            this.btnTurnToSecurityKey.Size = new System.Drawing.Size(150, 23);
            this.btnTurnToSecurityKey.TabIndex = 14;
            this.btnTurnToSecurityKey.Text = "Turn to Security Key";
            this.btnTurnToSecurityKey.UseVisualStyleBackColor = true;
            this.btnTurnToSecurityKey.Click += new System.EventHandler(this.btnTurnToSecurityKey_Click);
            this.btnTurnToSecurityKey.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnTurnToSecurityKey.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnTurnToSecurityKey.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // progressBarDriveEncryption
            // 
            this.progressBarDriveEncryption.Location = new System.Drawing.Point(727, 195);
            this.progressBarDriveEncryption.Name = "progressBarDriveEncryption";
            this.progressBarDriveEncryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarDriveEncryption.TabIndex = 15;
            // 
            // progressBarDriveDecryption
            // 
            this.progressBarDriveDecryption.Location = new System.Drawing.Point(727, 166);
            this.progressBarDriveDecryption.Name = "progressBarDriveDecryption";
            this.progressBarDriveDecryption.Size = new System.Drawing.Size(463, 23);
            this.progressBarDriveDecryption.TabIndex = 16;
            // 
            // comboBoxDrives
            // 
            this.comboBoxDrives.BackColor = System.Drawing.Color.LightCyan;
            this.comboBoxDrives.Location = new System.Drawing.Point(168, 48);
            this.comboBoxDrives.Name = "comboBoxDrives";
            this.comboBoxDrives.Size = new System.Drawing.Size(120, 21);
            this.comboBoxDrives.TabIndex = 17;
            // 
            // UnlockDevice
            // 
            this.UnlockDevice.Location = new System.Drawing.Point(601, 72);
            this.UnlockDevice.Name = "UnlockDevice";
            this.UnlockDevice.Size = new System.Drawing.Size(120, 23);
            this.UnlockDevice.TabIndex = 18;
            this.UnlockDevice.Text = "Unlock Device";
            this.UnlockDevice.UseVisualStyleBackColor = true;
            this.UnlockDevice.Click += new System.EventHandler(this.UnlockDevice_Click);
            this.UnlockDevice.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.UnlockDevice.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.UnlockDevice.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.LightCyan;
            this.textBox1.Location = new System.Drawing.Point(601, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(120, 20);
            this.textBox1.TabIndex = 19;
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.Location = new System.Drawing.Point(495, 46);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(100, 23);
            this.btnStartCapture.TabIndex = 20;
            this.btnStartCapture.Text = "StartCapture";
            this.btnStartCapture.UseVisualStyleBackColor = true;
            this.btnStartCapture.Click += new System.EventHandler(this.BtnStartCapture_Click);
            this.btnStartCapture.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnStartCapture.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnStartCapture.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // btnBruteForce
            // 
            this.btnBruteForce.Location = new System.Drawing.Point(495, 74);
            this.btnBruteForce.Name = "btnBruteForce";
            this.btnBruteForce.Size = new System.Drawing.Size(100, 23);
            this.btnBruteForce.TabIndex = 20;
            this.btnBruteForce.Text = "BruteForce";
            this.btnBruteForce.UseVisualStyleBackColor = true;
            this.btnBruteForce.Click += new System.EventHandler(this.btnBruteForce_Click);
            this.btnBruteForce.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Paint);
            this.btnBruteForce.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
            this.btnBruteForce.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            // 
            // MainMenu
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1662, 638);
            this.Controls.Add(this.btnStartCapture);
            this.Controls.Add(this.btnBruteForce);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.UnlockDevice);
            this.Controls.Add(this.comboBoxDrives);
            this.Controls.Add(this.lstDragDrop);
            this.Controls.Add(this.lblStatusSignal);
            this.Controls.Add(this.panelDragDrop);
            this.Controls.Add(this.btnDecryptFolder);
            this.Controls.Add(this.btnEncryptFolder);
            this.Controls.Add(this.btnDecryptFile);
            this.Controls.Add(this.progressBarPasswordEncryption);
            this.Controls.Add(this.progressBarFolderDecryption);
            this.Controls.Add(this.progressBarFolderEncryption);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.progressBarFileDecryption);
            this.Controls.Add(this.progressBarFileEncryption);
            this.Controls.Add(this.btnEncryptFile);
            this.Controls.Add(this.btnEncryptDrive);
            this.Controls.Add(this.btnDecryptDrive);
            this.Controls.Add(this.btnTurnToSecurityKey);
            this.Controls.Add(this.progressBarDriveEncryption);
            this.Controls.Add(this.progressBarDriveDecryption);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainMenu";
            this.Text = "File Encryption App";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnEncryptFile;
        private System.Windows.Forms.ProgressBar progressBarFileEncryption;
        private System.Windows.Forms.ProgressBar progressBarFileDecryption;
        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.ProgressBar progressBarFolderEncryption;
        private System.Windows.Forms.ProgressBar progressBarFolderDecryption;
        private System.Windows.Forms.ProgressBar progressBarPasswordEncryption;
        private System.Windows.Forms.Button btnDecryptFile;
        private System.Windows.Forms.Button btnEncryptFolder;
        private System.Windows.Forms.Button btnDecryptFolder;
        private System.Windows.Forms.Panel panelDragDrop;
        private System.Windows.Forms.Label lblStatusSignal;
        private System.Windows.Forms.ListBox lstDragDrop;
        private System.Windows.Forms.Button btnEncryptDrive;
        private System.Windows.Forms.Button btnDecryptDrive;
        private System.Windows.Forms.Button btnTurnToSecurityKey;
        private System.Windows.Forms.ProgressBar progressBarDriveEncryption;
        private System.Windows.Forms.ProgressBar progressBarDriveDecryption;
        private System.Windows.Forms.Button UnlockDevice;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnBruteForce;
        private System.Windows.Forms.Button btnStartCapture;
    }

}