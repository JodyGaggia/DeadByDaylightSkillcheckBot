using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoMoreBoredom
{
    public partial class GlobalHotkey : Form
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        public GlobalHotkey()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, 0, 0, Keys.F2.GetHashCode());
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                SkillcheckBot.isActive = !SkillcheckBot.isActive;

                if (SkillcheckBot.isActive) SkillcheckBot.ScanScreen();
            }
        }
    }
}
