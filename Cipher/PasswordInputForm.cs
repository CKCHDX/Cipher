using System;
using System.Windows.Forms;

namespace Cipher
{
    public partial class PasswordInputForm : Form
    {
        public string Password { get; private set; }

        public PasswordInputForm()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
