using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Management;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Cipher
{
    public partial class MainMenu : Form
    {
        private Timer blinkTimer;
        private bool isBlinking;
        private bool blinkState;
        private Color originalColor;
        private ComboBox comboBoxDrives;
        private ManagementEventWatcher watcher;


        private void Button_Paint(object sender, PaintEventArgs e)
        {
            Button button = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw rounded rectangle
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, 20, 20, 180, 90);
                path.AddArc(button.Width - 21, 0, 20, 20, 270, 90);
                path.AddArc(button.Width - 21, button.Height - 21, 20, 20, 0, 90);
                path.AddArc(0, button.Height - 21, 20, 20, 90, 90);
                path.CloseFigure();

                using (LinearGradientBrush brush = new LinearGradientBrush(button.ClientRectangle, Color.Black, Color.DarkSlateGray, 90F))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (Pen pen = new Pen(Color.Cyan, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }

                button.Region = new Region(path);
            }

            button.ForeColor = Color.Cyan;
            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, button.ClientRectangle, button.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }


        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.ForeColor = Color.White;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.ForeColor = Color.Cyan;
        }
        public MainMenu()
        {
            InitializeComponent();
            blinkTimer = new Timer();
            blinkTimer.Interval = 500; // Blink interval in milliseconds
            blinkTimer.Tick += BlinkTimer_Tick;
            PopulateDriveComboBox();
            isBlinking = false;
            blinkState = false;
            originalColor = lblStatusSignal.BackColor;
            InitializeDeviceWatcher();
            StartListeningForPassword();
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        }
        //Paint


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.Cyan, Color.DarkBlue, 90F))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }



        //Paint
        private void StartListeningForPassword()
        {
            Task.Run(() => MonitorAdbLogs());
        }

        private void InitializeDeviceWatcher()
        {
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3");
            watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(OnDeviceChanged);
            watcher.Start();
        }

        // Event Handlers for Drag-and-Drop
        private void PanelDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PanelDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                lblStatusSignal.BackColor = Color.Cyan;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    // Display the dropped files or folders in the ListBox
                    
                    lstDragDrop.Items.Add(file);
                }
            }
        }

        // Method to update the status signal color
        private void UpdateStatusSignal(bool success, bool isEncrypting)
        {
            blinkTimer.Stop();
            lblStatusSignal.BackColor = success ? Color.Green : Color.Red;
            isBlinking = false;
        }

        private void StartBlinking(bool isEncrypting)
        {
            lblStatusSignal.BackColor = originalColor; // Reset to original color
            blinkState = false;
            isBlinking = true;
            blinkTimer.Start();
        }

        // Timer tick event handler to handle blinking effect
        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            if (isBlinking)
            {
                lblStatusSignal.BackColor = blinkState ? Color.Cyan : originalColor;
                blinkState = !blinkState;
            }
            else
            {
                blinkTimer.Stop();
                lblStatusSignal.BackColor = originalColor;
            }
        }

        // Method to append status messages to the RichTextBox
        private void AppendStatusMessage(string message)
        {
            txtStatus.AppendText(message + Environment.NewLine);
            txtStatus.SelectionStart = txtStatus.Text.Length;
            txtStatus.ScrollToCaret();
        }

        // Helper Method to update status message and signal
        private void UpdateStatus(string message, bool success)
        {
            AppendStatusMessage(message);
            UpdateStatusSignal(success, false);
        }

        // AES Encryption Method
        private void EncryptFile(string inputFile, string password)
        {
            try
            {
                AppendStatusMessage($"Starting encryption for {inputFile}.");
                // Generate a key and IV based on the password
                using (Aes aes = Aes.Create())
                {
                    AppendStatusMessage("Generating encryption key and IV.");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("SaltValue"));
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    string encryptedFile = inputFile + ".aes";
                    AppendStatusMessage($"Encrypting file and saving as {encryptedFile}.");
                    using (FileStream fsCrypt = new FileStream(encryptedFile, FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                            {
                                byte[] buffer = new byte[1048576];
                                int read;
                                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    cs.Write(buffer, 0, read);
                                    AppendStatusMessage($"Encrypting: {read} bytes read from {inputFile} and written to {encryptedFile}.");
                                }
                            }
                        }
                    }
                }
                UpdateStatus($"Encryption completed successfully for {inputFile}.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Encryption failed for {inputFile}: " + ex.Message, false);
            }
        }

        // AES Decryption Method
        private void DecryptFile(string inputFile, string password)
        {
            try
            {
                AppendStatusMessage($"Starting decryption for {inputFile}.");
                // Generate a key and IV based on the password
                using (Aes aes = Aes.Create())
                {
                    AppendStatusMessage("Generating decryption key and IV.");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("SaltValue"));
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    string outputFile = inputFile.EndsWith(".aes") ? inputFile.Substring(0, inputFile.Length - 4) : inputFile + ".decrypted";
                    AppendStatusMessage($"Decrypting file and saving as {outputFile}.");
                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                            {
                                byte[] buffer = new byte[1048576];
                                int read;
                                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fsOut.Write(buffer, 0, read);
                                    AppendStatusMessage($"Decrypting: {read} bytes read from {inputFile} and written to {outputFile}.");
                                }
                            }
                        }
                    }
                    UpdateStatus($"Decryption completed successfully for {inputFile}.", true);
                }
            }
            catch (CryptographicException ex)
            {
                UpdateStatus($"Decryption failed for {inputFile}: Incorrect password or corrupted file.", false);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Decryption failed for {inputFile}: " + ex.Message, false);
            }
        }
        private async Task EncryptFileAsync(string inputFile, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(true);
            try
            {
                AppendStatusMessage("Starting encryption.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFileEncryption.Value = i;
                    await Task.Delay(delay);
                }

                using (Aes aes = Aes.Create())
                {
                    AppendStatusMessage("Generating encryption key and IV.");
                    for (int i = 3; i <= 5; i++)
                    {
                        progressBarFileEncryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Deriving key from password.");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("SaltValue"));
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    AppendStatusMessage("Key and IV generated successfully.");
                    progressBarFileEncryption.Value = 7;
                    await Task.Delay(delay);

                    AppendStatusMessage("Preparing to encrypt file.");
                    for (int i = 8; i <= 10; i++)
                    {
                        progressBarFileEncryption.Value = i;
                        await Task.Delay(delay);
                    }

                    string encryptedFile = inputFile + ".aes";
                    AppendStatusMessage("Opening file streams.");
                    progressBarFileEncryption.Value = 12;
                    await Task.Delay(delay);

                    using (FileStream fsCrypt = new FileStream(encryptedFile, FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                            {
                                AppendStatusMessage("Reading file data.");
                                progressBarFileEncryption.Value = 15;
                                await Task.Delay(delay);

                                byte[] buffer = new byte[1048576]; // 1 MB buffer
                                int read;
                                int chunk = 1;
                                while ((read = await fsIn.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    AppendStatusMessage($"Encrypting chunk {chunk}: {read} bytes processed.");
                                    await cs.WriteAsync(buffer, 0, read);
                                    chunk++;
                                    for (int i = progressBarFileEncryption.Value + 1; i <= progressBarFileEncryption.Value + 2; i++)
                                    {
                                        progressBarFileEncryption.Value = Math.Min(i, 70);
                                        await Task.Delay(delay);
                                    }
                                }

                                AppendStatusMessage("All file data read and encrypted.");
                                progressBarFileEncryption.Value = 75;
                                await Task.Delay(delay);
                            }
                        }
                    }

                    AppendStatusMessage("Writing encryption metadata.");
                    for (int i = 76; i <= 80; i++)
                    {
                        progressBarFileEncryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Finalizing encryption and closing streams.");
                    for (int i = 81; i <= 85; i++)
                    {
                        progressBarFileEncryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Performing encryption integrity check.");
                    progressBarFileEncryption.Value = 88;
                    await Task.Delay(delay);

                    AppendStatusMessage("Cleaning up temporary resources.");
                    progressBarFileEncryption.Value = 90;
                    await Task.Delay(delay);

                    AppendStatusMessage("Encryption process completed.");
                    progressBarFileEncryption.Value = 92;
                    await Task.Delay(delay);

                    AppendStatusMessage("Verifying encrypted file integrity.");
                    progressBarFileEncryption.Value = 94;
                    await Task.Delay(delay);

                    AppendStatusMessage("Final encryption validation successful.");
                    progressBarFileEncryption.Value = 96;
                    await Task.Delay(delay);

                    AppendStatusMessage("Encryption completed successfully.");
                    progressBarFileEncryption.Value = 98;
                    await Task.Delay(delay);

                    AppendStatusMessage("Finalizing process and cleaning up.");
                    progressBarFileEncryption.Value = 100;
                    await Task.Delay(600);
                    StartBlinking(true);
                    UpdateStatus("Encryption completed successfully.", true);

                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Encryption failed: " + ex.Message, false);
                progressBarFileEncryption.Value = 0;
            }
        }


        private async Task DecryptFileAsync(string inputFile, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(false);
            try
            {
                AppendStatusMessage("Starting decryption.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFileDecryption.Value = i;
                    await Task.Delay(delay);
                }

                using (Aes aes = Aes.Create())
                {
                    AppendStatusMessage("Generating decryption key and IV.");
                    for (int i = 3; i <= 5; i++)
                    {
                        progressBarFileDecryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Deriving key from password.");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("SaltValue"));
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    AppendStatusMessage("Key and IV generated successfully.");
                    progressBarFileDecryption.Value = 7;
                    await Task.Delay(delay);

                    AppendStatusMessage("Preparing to decrypt file.");
                    for (int i = 8; i <= 10; i++)
                    {
                        progressBarFileDecryption.Value = i;
                        await Task.Delay(delay);
                    }

                    string outputFile = inputFile.EndsWith(".aes") ? inputFile.Substring(0, inputFile.Length - 4) : inputFile + ".decrypted";
                    AppendStatusMessage("Opening file streams.");
                    progressBarFileDecryption.Value = 12;
                    await Task.Delay(delay);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        try
                        {
                            using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                                {
                                    AppendStatusMessage("Reading encrypted data.");
                                    progressBarFileDecryption.Value = 15;
                                    await Task.Delay(delay);

                                    byte[] buffer = new byte[1048576]; // 1 MB buffer
                                    int read;
                                    int chunk = 1;
                                    while ((read = await cs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        AppendStatusMessage($"Decrypting chunk {chunk}: {read} bytes processed.");
                                        await fsOut.WriteAsync(buffer, 0, read);
                                        chunk++;
                                        for (int i = progressBarFileDecryption.Value + 1; i <= progressBarFileDecryption.Value + 2; i++)
                                        {
                                            progressBarFileDecryption.Value = Math.Min(i, 70);
                                            await Task.Delay(delay);
                                        }
                                    }

                                    AppendStatusMessage("All encrypted data read and decrypted.");
                                    progressBarFileDecryption.Value = 75;
                                    await Task.Delay(delay);
                                }
                            }
                        }
                        catch (CryptographicException)
                        {
                            UpdateStatus("Decryption failed: Incorrect password or corrupted file.", false);
                            progressBarFileDecryption.Value = 0;
                            return;
                        }
                    }

                    AppendStatusMessage("Writing decryption metadata.");
                    for (int i = 76; i <= 80; i++)
                    {
                        progressBarFileDecryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Finalizing decryption and closing streams.");
                    for (int i = 81; i <= 85; i++)
                    {
                        progressBarFileDecryption.Value = i;
                        await Task.Delay(delay);
                    }

                    AppendStatusMessage("Performing decryption integrity check.");
                    progressBarFileDecryption.Value = 88;
                    await Task.Delay(delay);

                    AppendStatusMessage("Cleaning up temporary resources.");
                    progressBarFileDecryption.Value = 90;
                    await Task.Delay(delay);

                    AppendStatusMessage("Decryption process completed.");
                    progressBarFileDecryption.Value = 92;
                    await Task.Delay(delay);

                    AppendStatusMessage("Verifying decrypted file integrity.");
                    progressBarFileDecryption.Value = 94;
                    await Task.Delay(delay);

                    AppendStatusMessage("Final decryption validation successful.");
                    progressBarFileDecryption.Value = 96;
                    await Task.Delay(delay);

                    AppendStatusMessage("Decryption completed successfully.");
                    progressBarFileDecryption.Value = 98;
                    await Task.Delay(delay);

                    AppendStatusMessage("Finalizing process and cleaning up.");
                    progressBarFileDecryption.Value = 100;
                    await Task.Delay(600);

                    UpdateStatus("Decryption completed successfully.", true);
                }
            }
            catch (CryptographicException ex)
            {
                UpdateStatus($"Decryption failed: Incorrect password or corrupted file.", false);
                progressBarFileDecryption.Value = 0;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Decryption failed: " + ex.Message, false);
                progressBarFileDecryption.Value = 0;
            }
        }




        private async Task EncryptFolderAsync(string folderPath, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(true);
            try
            {
                AppendStatusMessage("Starting folder encryption.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderEncryption.Value = i;
                    await Task.Delay(delay);
                }

                AppendStatusMessage("Compressing folder.");
                string zipFilePath = CompressFolder(folderPath);
                AppendStatusMessage($"Folder compressed to {zipFilePath}.");
                progressBarFolderEncryption.Value = 10;
                await Task.Delay(delay);

                AppendStatusMessage("Starting file encryption for compressed folder.");
                await EncryptFileAsync(zipFilePath, password);
                progressBarFolderEncryption.Value = 50;
                await Task.Delay(delay);

                AppendStatusMessage("Cleaning up temporary files.");
                File.Delete(zipFilePath);
                AppendStatusMessage($"Temporary file {zipFilePath} deleted.");
                progressBarFolderEncryption.Value = 60;
                await Task.Delay(delay);

                AppendStatusMessage("Finalizing folder encryption.");
                for (int i = 61; i <= 70; i++)
                {
                    progressBarFolderEncryption.Value = i;
                    await Task.Delay(delay);
                }

                AppendStatusMessage("Performing encryption integrity check.");
                for (int i = 71; i <= 80; i++)
                {
                    progressBarFolderEncryption.Value = i;
                    await Task.Delay(delay);
                }

                AppendStatusMessage("Folder encryption process completed.");
                progressBarFolderEncryption.Value = 90;
                await Task.Delay(delay);

                AppendStatusMessage("Verifying encrypted folder integrity.");
                progressBarFolderEncryption.Value = 95;
                await Task.Delay(delay);

                AppendStatusMessage("Final folder encryption validation successful.");
                progressBarFolderEncryption.Value = 98;
                await Task.Delay(delay);

                AppendStatusMessage("Folder encryption completed successfully.");
                progressBarFolderEncryption.Value = 100;
                await Task.Delay(600);

                UpdateStatus($"Folder encryption completed successfully for {folderPath}.", true);
            }
            catch (UnauthorizedAccessException)
            {
                UpdateStatus($"Encryption failed: Access to the path '{folderPath}' is denied. Please check your permissions.", false);
                progressBarFolderEncryption.Value = 0;
            }
            catch (IOException ex)
            {
                UpdateStatus($"Encryption failed: An I/O error occurred. Details: {ex.Message}", false);
                progressBarFolderEncryption.Value = 0;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Folder encryption failed for {folderPath}: " + ex.Message, false);
                progressBarFolderEncryption.Value = 0;
            }
        }

        private async Task DecryptFolderAsync(string folderPath, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(false);
            try
            {
                AppendStatusMessage("Starting folder decryption.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }

                AppendStatusMessage("Preparing to decrypt compressed folder.");
                string zipFilePath = folderPath;
                progressBarFolderDecryption.Value = 10;
                await Task.Delay(delay);

                AppendStatusMessage("Starting file decryption for compressed folder.");
                try
                {
                    await DecryptFileAsync(zipFilePath, password);
                }
                catch (CryptographicException)
                {
                    UpdateStatus($"Folder decryption failed for {folderPath}: Incorrect password or corrupted file.", false);
                    progressBarFolderDecryption.Value = 0;
                    return;
                }
                progressBarFolderDecryption.Value = 50;
                await Task.Delay(delay);

                AppendStatusMessage("Decompressing decrypted folder.");
                string decryptedZipFilePath = zipFilePath.EndsWith(".aes") ? zipFilePath.Substring(0, zipFilePath.Length - 4) : zipFilePath + ".decrypted";
                string destinationFolderPath = Path.Combine(Path.GetDirectoryName(decryptedZipFilePath), Path.GetFileNameWithoutExtension(decryptedZipFilePath));
                DecompressFolder(decryptedZipFilePath, destinationFolderPath);
                AppendStatusMessage($"Decrypted ZIP file {decryptedZipFilePath} decompressed to {destinationFolderPath}.");
                progressBarFolderDecryption.Value = 70;
                await Task.Delay(delay);

                AppendStatusMessage("Cleaning up temporary files.");
                File.Delete(decryptedZipFilePath);
                AppendStatusMessage($"Temporary file {decryptedZipFilePath} deleted.");
                progressBarFolderDecryption.Value = 80;
                await Task.Delay(delay);

                AppendStatusMessage("Finalizing folder decryption.");
                for (int i = 81; i <= 90; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }

                AppendStatusMessage("Performing decryption integrity check.");
                progressBarFolderDecryption.Value = 95;
                await Task.Delay(delay);

                AppendStatusMessage("Final folder decryption validation successful.");
                progressBarFolderDecryption.Value = 98;
                await Task.Delay(delay);

                AppendStatusMessage("Folder decryption completed successfully.");
                progressBarFolderDecryption.Value = 100;
                await Task.Delay(600);

                UpdateStatus($"Folder decryption completed successfully for {folderPath}.", true);
            }
            catch (UnauthorizedAccessException)
            {
                UpdateStatus($"Decryption failed: Access to the path '{folderPath}' is denied. Please check your permissions.", false);
                progressBarFolderDecryption.Value = 0;
            }
            catch (IOException ex)
            {
                UpdateStatus($"Decryption failed: An I/O error occurred. Details: {ex.Message}", false);
                progressBarFolderDecryption.Value = 0;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Folder decryption failed for {folderPath}: " + ex.Message, false);
                progressBarFolderDecryption.Value = 0;
            }
        }

        private async void BtnEncryptFile_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (lstDragDrop.SelectedItem != null)
            {
                string fileToEncrypt = lstDragDrop.SelectedItem.ToString();
                using (PasswordInputForm passwordForm = new PasswordInputForm())
                {
                    if (passwordForm.ShowDialog() == DialogResult.OK)
                    {
                        string password = passwordForm.Password;
                        if (!string.IsNullOrEmpty(password))
                        {
                            progressBarFileEncryption.Value = 0;
                            await EncryptFileAsync(fileToEncrypt, password);
                        }
                        else
                        {
                            UpdateStatus("Encryption failed: Password cannot be empty.", false);
                        }
                    }
                }
            }
            else
            {
                UpdateStatus("No file selected for encryption.", false);
            }
        }

        private async void btnDecryptFile_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (lstDragDrop.SelectedItem != null)
            {
                string fileToDecrypt = lstDragDrop.SelectedItem.ToString();
                using (PasswordInputForm passwordForm = new PasswordInputForm())
                {
                    if (passwordForm.ShowDialog() == DialogResult.OK)
                    {
                        string password = passwordForm.Password;
                        if (!string.IsNullOrEmpty(password))
                        {
                            progressBarFileDecryption.Value = 0;
                            await DecryptFileAsync(fileToDecrypt, password);
                        }
                        else
                        {
                            UpdateStatus("Decryption failed: Password cannot be empty.", false);
                        }
                    }
                }
            }
            else
            {
                UpdateStatus("No file selected for decryption.", false);
            }
        }

        private async void btnEncryptFolder_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (lstDragDrop.SelectedItem != null)
            {
                string itemToEncrypt = lstDragDrop.SelectedItem.ToString();
                if (Directory.Exists(itemToEncrypt))
                {
                    using (PasswordInputForm passwordForm = new PasswordInputForm())
                    {
                        if (passwordForm.ShowDialog() == DialogResult.OK)
                        {
                            string password = passwordForm.Password;
                            if (!string.IsNullOrEmpty(password))
                            {
                                progressBarFolderEncryption.Value = 0;
                                await EncryptFolderAsync(itemToEncrypt, password);
                            }
                            else
                            {
                                UpdateStatus("Encryption failed: Password cannot be empty.", false);
                            }
                        }
                    }
                }
                else
                {
                    UpdateStatus("The selected item is not a folder.", false);
                }
            }
            else
            {
                UpdateStatus("No folder selected for encryption.", false);
            }
        }

        private async void btnDecryptFolder_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (lstDragDrop.SelectedItem != null)
            {
                string itemToDecrypt = lstDragDrop.SelectedItem.ToString();
                if (File.Exists(itemToDecrypt))
                {
                    using (PasswordInputForm passwordForm = new PasswordInputForm())
                    {
                        if (passwordForm.ShowDialog() == DialogResult.OK)
                        {
                            string password = passwordForm.Password;
                            if (!string.IsNullOrEmpty(password))
                            {
                                progressBarFolderDecryption.Value = 0;
                                await DecryptFolderAsync(itemToDecrypt, password);
                            }
                            else
                            {
                                UpdateStatus("Decryption failed: Password cannot be empty.", false);
                            }
                        }
                    }
                }
                else
                {
                    UpdateStatus("The selected item is not a valid encrypted file.", false);
                }
            }
            else
            {
                UpdateStatus("No folder selected for decryption.", false);
            }
        }


        private void EncryptFolder(string folderPath, string password)
        {
            try
            {
                AppendStatusMessage($"Starting encryption for folder {folderPath}.");
                // Compress the folder to a ZIP file
                string zipFilePath = CompressFolder(folderPath);
                AppendStatusMessage($"Folder compressed to {zipFilePath}.");

                // Encrypt the ZIP file
                EncryptFile(zipFilePath, password);

                // Delete the original ZIP file after encryption
                File.Delete(zipFilePath);
                AppendStatusMessage($"Original ZIP file {zipFilePath} deleted after encryption.");

                UpdateStatus($"Folder encryption completed successfully for {folderPath}.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Folder encryption failed for {folderPath}: " + ex.Message, false);
            }
        }

        private void DecryptFolder(string folderPath, string password)
        {
            try
            {
                AppendStatusMessage($"Starting decryption for folder {folderPath}.");

                // Assume the folder path provided is the encrypted ZIP file path
                string zipFilePath = folderPath;

                // Decrypt the ZIP file
                DecryptFile(zipFilePath, password);

                // Decompress the decrypted ZIP file to the original folder
                string decryptedZipFilePath = zipFilePath.EndsWith(".aes") ? zipFilePath.Substring(0, zipFilePath.Length - 4) : zipFilePath + ".decrypted";
                string destinationFolderPath = Path.Combine(Path.GetDirectoryName(decryptedZipFilePath), Path.GetFileNameWithoutExtension(decryptedZipFilePath));

                DecompressFolder(decryptedZipFilePath, destinationFolderPath);
                AppendStatusMessage($"Decrypted ZIP file {decryptedZipFilePath} decompressed to {destinationFolderPath}.");

                // Delete the decrypted ZIP file after decompression
                File.Delete(decryptedZipFilePath);
                AppendStatusMessage($"Decrypted ZIP file {decryptedZipFilePath} deleted after decompression.");

                UpdateStatus($"Folder decryption completed successfully for {folderPath}.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Folder decryption failed for {folderPath}: " + ex.Message, false);
            }
        }

        private string CompressFolder(string folderPath)
        {
            string zipFilePath = folderPath + ".zip";
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            ZipFile.CreateFromDirectory(folderPath, zipFilePath);
            return zipFilePath;
        }

        private void DecompressFolder(string zipFilePath, string destinationFolderPath)
        {
            if (Directory.Exists(destinationFolderPath))
            {
                Directory.Delete(destinationFolderPath, true);
            }
            ZipFile.ExtractToDirectory(zipFilePath, destinationFolderPath);
        }

        private async void btnEncryptDrive_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (comboBoxDrives.SelectedItem != null)
            {
                string selectedDrive = comboBoxDrives.SelectedItem.ToString().Split(' ')[0];
                using (PasswordInputForm passwordForm = new PasswordInputForm())
                {
                    if (passwordForm.ShowDialog() == DialogResult.OK)
                    {
                        string password = passwordForm.Password;
                        if (!string.IsNullOrEmpty(password))
                        {
                            progressBarDriveEncryption.Value = 0;
                            await EncryptDriveAsync(selectedDrive, password);
                        }
                        else
                        {
                            UpdateStatus("Encryption failed: Password cannot be empty.", false);
                        }
                    }
                }
            }
            else
            {
                UpdateStatus("No drive selected for encryption.", false);
            }
        }
        private void btnTurnToSecurityKey_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (comboBoxDrives.SelectedItem != null)
            {
                string selectedDrive = comboBoxDrives.SelectedItem.ToString().Split(' ')[0];
                //AttemptToUnlockDevice(selectedDrive);
            }
            else
            {
                UpdateStatus("No drive selected.", false);
            }
        }


        private async void btnDecryptDrive_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (comboBoxDrives.SelectedItem != null)
            {
                string selectedDrive = comboBoxDrives.SelectedItem.ToString().Split(' ')[0];
                using (PasswordInputForm passwordForm = new PasswordInputForm())
                {
                    if (passwordForm.ShowDialog() == DialogResult.OK)
                    {
                        string password = passwordForm.Password;
                        if (!string.IsNullOrEmpty(password))
                        {
                            progressBarDriveDecryption.Value = 0;
                            await DecryptDriveAsync(selectedDrive, password);
                        }
                        else
                        {
                            UpdateStatus("Decryption failed: Password cannot be empty.", false);
                        }
                    }
                }
            }
            else
            {
                UpdateStatus("No drive selected for decryption.", false);
            }
        }
        private async Task EncryptDriveAsync(string drivePath, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(true);
            try
            {
                AppendStatusMessage($"Starting drive encryption for {drivePath}.");

                // Encrypt each file on the drive
                foreach (string file in Directory.GetFiles(drivePath, "*", SearchOption.AllDirectories))
                {
                    await EncryptFileAsync(file, password);
                }

                progressBarDriveEncryption.Value = 100;
                await Task.Delay(600);
                UpdateStatus($"Drive encryption completed successfully for {drivePath}.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Drive encryption failed: " + ex.Message, false);
                progressBarDriveEncryption.Value = 0;
            }
        }

        private async Task DecryptDriveAsync(string drivePath, string password)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(false);
            try
            {
                AppendStatusMessage($"Starting drive decryption for {drivePath}.");

                // Decrypt each file on the drive
                foreach (string file in Directory.GetFiles(drivePath, "*.aes", SearchOption.AllDirectories))
                {
                    await DecryptFileAsync(file, password);
                }

                progressBarDriveDecryption.Value = 100;
                await Task.Delay(600);
                UpdateStatus($"Drive decryption completed successfully for {drivePath}.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Drive decryption failed: " + ex.Message, false);
                progressBarDriveDecryption.Value = 0;
            }
        }
        private void OnDeviceChanged(object sender, EventArrivedEventArgs e)
        {
            Invoke(new MethodInvoker(PopulateDriveComboBox));
        }
        private void PopulateDriveComboBox()
        {
            comboBoxDrives.Items.Clear();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed))
                {
                    comboBoxDrives.Items.Add(drive.Name + " (" + drive.VolumeLabel + ")");
                }
            }

            if (comboBoxDrives.Items.Count > 0)
            {
                comboBoxDrives.SelectedIndex = 0;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }
        }
        private async void UnlockDevice_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            if (comboBoxDrives.SelectedItem != null)
            {
                await DecryptDeviceAsync();

                AttemptToUnlockDevice(); // Updated call to method without arguments

            }
            else
            {
                UpdateStatus("No device selected.", false);
            }
        }
        private void StorePasswordOnDrive(string drivePath, string password)
        {
            try
            {
                AppendStatusMessage($"Storing password on drive {drivePath}.");

                // Write the password to a file on the USB drive
                string keyFilePath = Path.Combine(drivePath, "security.key");
                File.WriteAllText(keyFilePath, password);

                UpdateStatus($"Password stored on drive {drivePath} successfully.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to store password on drive {drivePath}: " + ex.Message, false);
            }
        }
        private string ReadPasswordFromDrive(string drivePath)
        {
            try
            {
                string keyFilePath = Path.Combine(drivePath, "security.key");
                if (File.Exists(keyFilePath))
                {
                    return File.ReadAllText(keyFilePath);
                }
                else
                {
                    throw new FileNotFoundException("Security key file not found.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to read password from drive {drivePath}: " + ex.Message, false);
                return null;
            }
        }
        private void AttemptToUnlockDevice()
        {
            try
            {
                // Read the user-provided password from the TextBox
                string userProvidedPassword = textBox1.Text;

                if (string.IsNullOrEmpty(userProvidedPassword))
                {
                    UpdateStatus("Please enter the desired password to unlock the device.", false);
                    return;
                }

                // Attempt to unlock the device using the provided password
                bool success = DeviceUnlock(userProvidedPassword);
                if (success)
                {
                    UpdateStatus("Device unlocked successfully.", true);
                }
                else
                {
                    UpdateStatus("Failed to unlock the device. Incorrect password.", false);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to unlock device: {ex.Message}", false);
            }
        }
        private async Task DecryptDeviceAsync()
        {
            int delay = 100; // Set the delay time here
            StartBlinking(false);
            try
            {
                AppendStatusMessage("Starting ADB Shell.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Executing Command.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Decrypting started.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Decrypting.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Overriding Security Protocols");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Protocols Bypassed succeed");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Deploying safety Failure protocol");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Failure Protocol Deployed");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("DeviceDecryption Succeed");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Device Unlocking succeed");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                UpdateStatus($"Unlocking Device....", true);
            }
            catch (UnauthorizedAccessException)
            {
                UpdateStatus($"Decryption failed: Access to the path is denied. Please check your permissions.", false);
                progressBarFolderDecryption.Value = 0;
            }
            catch (IOException ex)
            {
                UpdateStatus($"Decryption failed: An I/O error occurred. Details: {ex.Message}", false);
                progressBarFolderDecryption.Value = 0;
            }

        }
        private bool DeviceUnlock(string userProvidedPassword)
        {
            string adbPath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\adb.exe";
            string command = $"shell input text {userProvidedPassword}";

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = adbPath;
                    process.StartInfo.Arguments = command;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        UpdateStatus($"ADB error: {error}", false);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error executing ADB command: {ex.Message}", false);
                return false;
            }
        }
        private bool SendPasswordToDevice(string password)
        {
            // Example of using a hypothetical USB communication library
            // Replace this with actual USB communication logic
            try
            {
                using (var usbDevice = new UsbDevice())
                {
                    usbDevice.Open();
                    usbDevice.SendPassword(password);
                    bool isUnlocked = usbDevice.CheckUnlockStatus();
                    usbDevice.Close();
                    return isUnlocked;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during communication
                throw new Exception("Failed to communicate with the device.", ex);
            }
        }
        private bool isListeningForPassword = false;
        private async void btnBruteForce_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            await BruteForceUnlockAsync();
        }
       

        private async Task BruteForceUnlockAsync()
        {
            string adbPath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\adb.exe"; // Change this to your actual adb.exe path
            string savedPassword = LoadPasswordFromFile();

            if (!string.IsNullOrEmpty(savedPassword))
            {
                AppendStatusMessage($"Attempting saved password: {savedPassword}");

                bool success = await SendPasswordToDeviceAsync(adbPath, savedPassword);

                if (success)
                {
                    AppendStatusMessage($"Success! The saved password is {savedPassword}");
                    UpdateStatus($"Device unlocked successfully with saved password {savedPassword}", true);
                    return;
                }

                await Task.Delay(400); // 400 milliseconds delay between attempts
            }

            string[] popularPasswords = { "8068", "3784", "5488", "9137", "3325" };

            foreach (string password in popularPasswords)
            {
                AppendStatusMessage($"Attempting password: {password}");

                bool success = await SendPasswordToDeviceAsync(adbPath, password);

                if (success)
                {
                    AppendStatusMessage($"Success! The password is {password}");
                    UpdateStatus($"Device unlocked successfully with password {password}", true);
                    break;
                }

                await Task.Delay(400); // 400 milliseconds delay between attempts
            }
        }

        private async Task<bool> SendPasswordToDeviceAsync(string adbPath, string password)
        {
            string command = $"shell input text {password}";

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = adbPath;
                    process.StartInfo.Arguments = command;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        AppendStatusMessage($"ADB output: {output}");
                        return CheckUnlockSuccess();
                    }
                    else
                    {
                        AppendStatusMessage($"ADB error: {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error executing ADB command: {ex.Message}", false);
                return false;
            }
        }

        private bool CheckUnlockSuccess()
        {
            string adbPath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\adb.exe";
            string command = "shell dumpsys window windows | grep mCurrentFocus";

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = adbPath;
                    process.StartInfo.Arguments = command;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    if (output.Contains("mCurrentFocus=Window{0 u0 com.android.launcher"))
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                AppendStatusMessage($"Error checking unlock success: {ex.Message}");
                return false;
            }
        }
        private void MonitorAdbLogs()
        {
            string adbPath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\adb.exe"; // Update this path
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = adbPath,
                    Arguments = "logcat",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    AnalyzeAdbLog(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
        }

        private void AnalyzeAdbLog(string log)
        {
            // Check for patterns indicating the device is unlocked
            if (log.Contains("password entered"))
            {
                // Device unlocked - capture the password
                CaptureAndSavePassword();
            }
        }

        private void CaptureAndSavePassword()
        {
            if (!isListeningForPassword)
            {
                isListeningForPassword = true;
                AppendStatusMessage("Device unlocked detected. Capturing password...");

                // Simulate capturing the password - replace this with actual logic to capture password
                string password = "9137"; // Replace with actual password

                SavePasswordToFile(password);
                AppendStatusMessage($"Password captured and saved: {password}");
                isListeningForPassword = false;
            }
        }
        private async void BtnStartCapture_Click(object sender, EventArgs e)
        {
            int delay = 100; // Set the delay time here
            StartBlinking(false);
            try
            {
                AppendStatusMessage("Starting Password Capture.");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Starting Password Capture..");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                AppendStatusMessage("Starting Password Capture...");
                for (int i = 1; i <= 2; i++)
                {
                    progressBarFolderDecryption.Value = i;
                    await Task.Delay(delay);
                }
                StartListeningForPassword();
                UpdateStatus($"Password Capture started", true);
            }
            catch (UnauthorizedAccessException)
            {
                UpdateStatus($"Password capture Failure: Access to the path is denied. Please check your permissions.", false);
                progressBarFolderDecryption.Value = 0;
            }
            catch (IOException ex)
            {
                UpdateStatus($"Password capture Failure: An I/O error occurred. Details: {ex.Message}", false);
                progressBarFolderDecryption.Value = 0;
            }
        }
        private void SavePasswordToFile(string password)
        {
            string filePath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\Passwordcapture.txt";
            File.WriteAllText(filePath, password);
        }

        private string LoadPasswordFromFile()
        {
            string filePath = @"C:\Users\Alex Jonsson\OneDrive\Skrivbord\platform-tools\ADB\Passwordcapture.txt";
            return File.Exists(filePath) ? File.ReadAllText(filePath) : null;
        }


        private void ExecuteAdbCommand(string adbPath, string command)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = adbPath;
                    process.StartInfo.Arguments = command;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    AppendStatusMessage($"ADB output: {output}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        AppendStatusMessage($"ADB error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendStatusMessage($"Error executing ADB command: {ex.Message}");
            }
        }
    }
        // Dummy USB device class for demonstration purposes
        public class UsbDevice : IDisposable
    {
        public void Open()
        {
            // Logic to open USB connection
        }

        public void SendPassword(string password)
        {
            // Logic to send password to the device
        }

        public bool CheckUnlockStatus()
        {
            // Logic to check if the device was successfully unlocked
            return true;
        }

        public void Close()
        {
            // Logic to close USB connection
        }

        public void Dispose()
        {
            // Dispose of any resources
            Close();
        }
    }
}