using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoMoreBoredom
{
    public partial class GlobalHotkey : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private bool shouldToggleActiveState = true; // Determines if the proceeding hotkey press should toggle the active state of the program

        private string activationKey = "F2"; // Simplifies changing of text in main window

        public GlobalHotkey()
        {
            InitializeComponent();

            // Register hotkeys to control program
            RegisterHotKey(this.Handle, 0, 0, Keys.F2.GetHashCode());
            RegisterHotKey(this.Handle, 1, 4, Keys.F2.GetHashCode());

            MainWindow.main.UpdateText("Hit " + activationKey + " to activate the bot!");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                if (shouldToggleActiveState)
                { 
                    SkillcheckBot.isActive = !SkillcheckBot.isActive; // Toggle active state
                    MainWindow.main.UpdateBackgroundColor(SkillcheckBot.isActive); // Update UI
                    
                    if (SkillcheckBot.isActive)
                    {
                        // Update variables
                        shouldToggleActiveState = false;
                        SkillcheckBot.skillcheckTime = 1100; // Set to gen skillcheck speed

                        MainWindow.main.UpdateText("Mode: Generator (" + activationKey + " for healing mode)"); // Update UI

                        SkillcheckBot.ScanScreen(); // Start scanning screen (can only happen if the program was previously disabled)

                    } else MainWindow.main.UpdateText("Hit " + activationKey + " to activate the bot!");
                }
                else
                {
                    // Change to heal speed variables
                    shouldToggleActiveState = true;
                    SkillcheckBot.skillcheckTime = 1200;
                    MainWindow.main.UpdateText("Mode: Healing (" + activationKey + " to disable bot)"); // Update UI
                }
            }
        }

        private void ExampleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unregister hotkeys when the program closes
            UnregisterHotKey(this.Handle, 0);
            UnregisterHotKey(this.Handle, 1);
        }
    }
}
