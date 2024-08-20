using System;
using System.Windows.Forms;

namespace Cipher
{
    public partial class Startup : Form
    {
        private Timer transitionTimer;
        private Timer fadeInTimer;
        private Timer fadeOutTimer;
        private const int fadeInterval = 50; // interval in ms
        private const double fadeStep = 0.05; // step for each interval

        public Startup()
        {
            InitializeComponent();
            this.Load += Startup_Load;
        }

        private void Startup_Load(object sender, EventArgs e)
        {
            // Load the GIF from embedded resources
            pictureBoxLoading.Image = Properties.Resources._9beda78c6eb197e9e962a50e7f6ff09c; // Ensure this matches the resource name
            pictureBoxLoading.Enabled = true;

            // Initialize the fade-in timer
            fadeInTimer = new Timer();
            fadeInTimer.Interval = fadeInterval;
            fadeInTimer.Tick += FadeInTimer_Tick;
            fadeInTimer.Start();

            transitionTimer = new Timer();
            transitionTimer.Interval = 5000; // 5 seconds
            transitionTimer.Tick += TransitionTimer_Tick;
            transitionTimer.Start();
        }

        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1)
            {
                this.Opacity += fadeStep;
            }
            else
            {
                fadeInTimer.Stop();
            }
        }

        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            transitionTimer.Stop();

            // Initialize the fade-out timer
            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = fadeInterval;
            fadeOutTimer.Tick += FadeOutTimer_Tick;
            fadeOutTimer.Start();
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= fadeStep;
            }
            else
            {
                fadeOutTimer.Stop();
                this.DialogResult = DialogResult.OK; // Signal to Program.cs to open MainMenu
                this.Hide(); // Hide the startup form
                MainMenu mainMenu = new MainMenu();
                mainMenu.Show(); // Show the main menu form
            }
        }
    }
}
