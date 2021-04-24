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
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;

        // http://www.pinvoke.net/default.aspx/user32/keybd_event.html
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        const byte VK_SPACE = 0x20;
        const uint KEYEVENTF_KEYUP = 0x0002;

        public static bool isActive = false;

        public async static void ScanScreen()
        {
            if (isActive) mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            else return;

            await Task.Run(() =>
            {
                Rectangle area = new Rectangle(894, 474, 132, 132);

                while (isActive)
                {
                    DateTime startTime = DateTime.Now;

                    Bitmap bmp = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(bmp);
                    g.CopyFromScreen(area.Left, area.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                    g.Dispose();

                    int wait = Analyse(bmp);

                    if(wait != 0)
                    {
                        HitSkillcheck(startTime, wait);
                    }

                    bmp.Save("test.png", ImageFormat.Jpeg);
                }
            });

            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private static int Analyse(Bitmap bmp)
        {
            int needlePos = 0;
            int greatPos = 0;
            bool needleFound = false;
            bool greatFound = false;

            for (int i = 270; i < 630; i++)
            {
                double x = 65 + 65 * Math.Cos(i * (Math.PI / 180));
                double y = 65 + 65 * Math.Sin(i * (Math.PI / 180));

                Color color = bmp.GetPixel((int)x, (int)y);

                if (!needleFound)
                {
                    if (color.R > 190 && color.G < 10 && color.B < 10)
                    {
                        needlePos = i;
                        needleFound = true;
                    }
                }

                if (!greatFound)
                {
                    if (color.R == 255 && color.G == 255 && color.B == 255)
                    {
                        greatPos = i + 5;
                        greatFound = true;
                    }
                }

                if (needleFound && greatFound) break;
            }

            int distance = greatPos - needlePos;
            int waitTime = distance * (1100 / 360);

            AnimationManager.UpdateSkillcheckImage(needlePos - 90, greatPos - 90);

            return waitTime;
        }

        private static void HitSkillcheck(DateTime startTime, int waitTime)
        {
            DateTime nowTime = DateTime.Now;
            TimeSpan diff = nowTime - startTime;
            int compensation = (int)diff.TotalSeconds;

            Thread.Sleep(Math.Abs(waitTime - compensation));
            keybd_event(VK_SPACE, 0, 0, 0);
            Thread.Sleep(50);
            keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
