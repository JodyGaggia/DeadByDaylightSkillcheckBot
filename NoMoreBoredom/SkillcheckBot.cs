using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NoMoreBoredom
{
    public static class SkillcheckBot
    {
        // https://www.pinvoke.net/default.aspx/user32.mouse_event
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_XDOWN = 0x0080;
        private const uint MOUSEEVENTF_XUP = 0x0100;
        private const uint XBUTTON2 = 0x00000002; // Pass in dwData parameter

        // http://www.pinvoke.net/default.aspx/user32/keybd_event.html
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        private const byte VK_SPACE = 0x20;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;

        // Controlled by GlobalHotkey.cs and determine if the bot is active and the skillcheck timing
        public static bool isActive = false;
        public static int skillcheckTime = 1100;

        // Set on launch by MainWindow.xaml.cs
        public static float scaleFactorX = 1; // Ratio between screen res and 1920x1080
        public static float scaleFactorY = 1;
        public static float stretchFactorX = 1; // Stretch factor = scale factor if current res is 16:9 otherwise 1 (since skillchecks dont resize if res is not 16:9)
        public static float stretchFactorY = 1;
        public static bool normalRatio = false; // Stores if screen res is 16:9 (true) or not (false)

        public async static void ScanScreen()
        {
            if (isActive) mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0); // M1 Hook
            else return;

            await Task.Run(() =>
            {
                // Determine skillcheck position and size on screen
                int startX = normalRatio ? (int)(894f * scaleFactorX) : (int)(894f - (1920f - (1920f * scaleFactorX)) / 2f);
                int startY = normalRatio ? (int)(474f * scaleFactorY) : (int)(474f - (1920f - (1920f * scaleFactorY)) / 2f);
                int skillcheckBoundX = normalRatio ? (int)(132f * scaleFactorX) : 132;
                int skillcheckBoundY = normalRatio ? (int)(132f * scaleFactorY) : 132;

                Rectangle area = new Rectangle(startX, startY, skillcheckBoundX, skillcheckBoundY);

                while (isActive)
                {
                    DateTime startTime = DateTime.Now; // Used in compensation (time taken for program to work out how far the ss is)

                    // Screenshot skillcheck
                    Bitmap bmp = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bmp);
                    g.CopyFromScreen(area.Left, area.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

                    int wait = Analyse(bmp); // Check if skillcheck is on screen

                    if (wait != 0) HitSkillcheck(startTime, wait); // If skillcheck is on screen, hit it

                    // Clean up
                    g.Dispose();
                    bmp.Dispose();
                    
                    Thread.Sleep(225);
                }
            });
        }

        private static int Analyse(Bitmap bmp)
        {
            // Initialise variables
            int needlePos = 0;
            int greatPos = 0;
            bool needleFound = false;
            bool greatFound = false;

            for (int i = 270; i < 630; i++)
            {
                // Co-ordinates of circumference of circle
                double x = (65 * stretchFactorX) + (65 * Math.Cos(i * (Math.PI / 180)) * stretchFactorX);
                double y = (65 * stretchFactorY) + (65 * Math.Sin(i * (Math.PI / 180)) * stretchFactorY);

                Color color = bmp.GetPixel((int)x, (int)y);
                //bmp.SetPixel((int)x, (int)y, Color.FromArgb(0, 0, 255)); // debug for checking what pixels are being checked

                if (!needleFound) // Try to find skillcheck needle
                {
                    //if (color.R > 250 && color.G < 60 && color.B < 60) // jinksee reshade
                    if (color.R > 190 && color.G < 10 && color.B < 10) // default
                    {
                        bmp.SetPixel((int)x, (int)y, Color.FromArgb(0, 255, 0));
                        needlePos = i;
                        needleFound = true;
                    }
                }

                if (!greatFound) // Try to find great zone
                {
                    //if (color.R > 245 && color.G > 245 && color.B > 245) // jinksee reshade
                    if (color.R == 255 && color.G == 255 && color.B == 255) // default
                    {
                        bmp.SetPixel((int)x, (int)y, Color.FromArgb(0, 255, 0));
                        greatPos = i + 2;
                        greatFound = true;
                    }
                }

                if (needleFound && greatFound) // If all data found, update UI and break out of loop
                {
                    MainWindow.main.UpdateSkillcheckUI(greatPos, (greatPos - 270) * (1100 / 360));
                    break;
                }
            }

            // Final calculations to determine time to wait before hitting skillcheck (performed in HitSkillcheck function)
            int distance = greatPos - needlePos;
            int waitTime = distance * (skillcheckTime / 360);

            return waitTime;
        }

        private static void HitSkillcheck(DateTime startTime, int waitTime)
        {
            // Calculate compensation (time taken for program to work)
            DateTime nowTime = DateTime.Now;
            TimeSpan diff = nowTime - startTime;
            int compensation = (int)diff.TotalSeconds;

            // Hit skillcheck
            Thread.Sleep(Math.Abs(waitTime - compensation));
            keybd_event(VK_SPACE, 0xB9, KEYEVENTF_KEYDOWN, 0);
            Thread.Sleep(50);
            keybd_event(VK_SPACE, 0xB9, KEYEVENTF_KEYUP, 0);

            Thread.Sleep(1000);
        }
    }
}
