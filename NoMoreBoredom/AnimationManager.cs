using System;
using System.Windows.Media;

namespace NoMoreBoredom
{
    public static class AnimationManager
    {
        public static void UpdateSkillcheckImage(int needlePos, int greatPos)
        {
            RotateTransform needleDegrees = new RotateTransform(needlePos);
            RotateTransform greatDegrees = new RotateTransform(greatPos);

            MainWindow.main.Dispatcher.Invoke(new Action(delegate ()
            {
                MainWindow.main.skillcheckNeedle.RenderTransform = needleDegrees;
                MainWindow.main.skillcheckZone.RenderTransform = greatDegrees;
            }));
        }
    }
}
