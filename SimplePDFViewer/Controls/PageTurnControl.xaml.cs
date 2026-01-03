using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SimplePDFViewer.Controls
{
    /// <summary>
    /// Interaction logic for PageTurnControl.xaml
    /// </summary>
    public partial class PageTurnControl : UserControl
    {
        public PageTurnControl()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public static readonly DependencyProperty FrontPageImageProperty =
            DependencyProperty.Register("FrontPageImage", typeof(BitmapImage), typeof(PageTurnControl),
                new PropertyMetadata(null));

        public BitmapImage? FrontPageImage
        {
            get { return (BitmapImage?)GetValue(FrontPageImageProperty); }
            set { SetValue(FrontPageImageProperty, value); }
        }

        public static readonly DependencyProperty BackPageImageProperty =
            DependencyProperty.Register("BackPageImage", typeof(BitmapImage), typeof(PageTurnControl),
                new PropertyMetadata(null));

        public BitmapImage? BackPageImage
        {
            get { return (BitmapImage?)GetValue(BackPageImageProperty); }
            set { SetValue(BackPageImageProperty, value); }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Animates the page turn forward (next page)
        /// </summary>
        public void TurnPageForward(Action? onCompleted = null)
        {
            AnimatePageTurn(-180, onCompleted);
        }

        /// <summary>
        /// Animates the page turn backward (previous page)
        /// </summary>
        public void TurnPageBackward(Action? onCompleted = null)
        {
            AnimatePageTurn(180, onCompleted);
        }

        /// <summary>
        /// Animates the page turn with the specified rotation angle
        /// </summary>
        private void AnimatePageTurn(double rotationAngle, Action? onCompleted = null)
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = rotationAngle,
                Duration = TimeSpan.FromMilliseconds(600),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            animation.Completed += (s, e) =>
            {
                // Reset for next animation
                planeProjection.RotationY = 0;
                onCompleted?.Invoke();
            };

            planeProjection.BeginAnimation(PlaneProjection.RotationYProperty, animation);
        }

        /// <summary>
        /// Sets the page images without animation
        /// </summary>
        public void SetPageWithoutAnimation(BitmapImage? image)
        {
            FrontPageImage = image;
            BackPageImage = null;
            planeProjection.RotationY = 0;
        }

        #endregion
    }
}
