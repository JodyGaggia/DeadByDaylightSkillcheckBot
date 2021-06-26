using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace NoMoreBoredom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static MainWindow main;

        public MainWindow()
        {
            InitializeComponent();
            main = this;
            GlobalHotkey inputHandler = new GlobalHotkey();

            // Calculate screen res factors (see SkillcheckBot.cs)
            SkillcheckBot.scaleFactorX = Screen.PrimaryScreen.Bounds.Width / 1920f;
            SkillcheckBot.scaleFactorY = Screen.PrimaryScreen.Bounds.Height / 1080f;

            if (SkillcheckBot.scaleFactorX / SkillcheckBot.scaleFactorY == 1)
            {
                SkillcheckBot.normalRatio = true;
                SkillcheckBot.stretchFactorX = SkillcheckBot.scaleFactorX;
                SkillcheckBot.stretchFactorY = SkillcheckBot.scaleFactorY;
            }
            else
            {
                SkillcheckBot.stretchFactorX = 1;
                SkillcheckBot.stretchFactorY = 1;
            }
        }

        public void UpdateSkillcheckUI(int greatPos, double duration) // Animate hitting of skillcheck in program
        {
            Dispatcher.Invoke(new Action(() => {

                skillcheckNeedle.RenderTransform = new RotateTransform(-90);

                var needlePosAnim = new DoubleAnimation()
                {
                    To = greatPos - 360,
                    Duration = TimeSpan.FromMilliseconds(duration),
                };

                skillcheckZone.RenderTransform = new RotateTransform(greatPos - 160);

                Storyboard.SetTarget(needlePosAnim, skillcheckNeedle);
                Storyboard.SetTargetProperty(needlePosAnim, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                var sb = new Storyboard();
                sb.Children.Add(needlePosAnim);
                sb.Begin();
            }));
        }

        public void UpdateBackgroundColor(bool active) // Sets background colour to green when active and red while not
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if(active)
                {
                    Brush activeBrush = new RadialGradientBrush()
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(35, 205, 150), 0),
                            new GradientStop(Color.FromRgb(60, 110, 50), 1.0),
                        }
                    };

                    gridBG.Background = activeBrush;
                } else
                {
                    Brush inactiveBrush = new RadialGradientBrush()
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(45, 0, 0), 0),
                            new GradientStop(Color.FromRgb(75, 10, 10), 1.0),
                        }
                    };

                    gridBG.Background = inactiveBrush;
                }

            }));
        }

        public void UpdateText(string text) // Updates bottom text of program
        {
            Dispatcher.Invoke(new Action(() =>
            {
                activateText.Text = text;
            }));
        }
    }
}
