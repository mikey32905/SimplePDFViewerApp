using SimplePDFViewer.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SimplePDFViewer.Views
{
    /// <summary>
    /// Interaction logic for PageTurnControl.xaml
    /// </summary>
    public partial class PageTurnControl : UserControl
    {
        #region Members

        PageTurnControlViewModel pageTurnViewModel;

        #endregion Members

        public PageTurnControl()
        {
            InitializeComponent();
            pageTurnViewModel = new PageTurnControlViewModel();
            this.DataContext = pageTurnViewModel;
        }

        #region Properties


        #endregion

        public void TurnPageForward(BitmapImage currentPage, BitmapImage nextPage)
        {
            CurrentPageImage.Source = currentPage;
            NextPageImage.Source = nextPage;

            var storyboard = (Storyboard)this.Resources["PageTurnForward"];
            storyboard.Completed += (s, e) =>
            {
               // BitmapImage newImage = nextPage;
                // After animation, update current page
                CurrentPageImage.Source = nextPage;
                CurrentPageTransform.Angle = 0;
                NextPageTransform.Angle = 90;
                //SetPage(newImage);
            };
            storyboard.Begin();
        }

        public void TurnPageBackward(BitmapImage currentPage, BitmapImage previousPage)
        {
            CurrentPageImage.Source = currentPage;
            PreviousPageImage.Source = previousPage;

            var storyboard = (Storyboard)this.Resources["PageTurnBackward"];
            storyboard.Completed += (s, e) =>
            {
                // After animation, update current page
                //BitmapImage newImage = previousPage;
                CurrentPageImage.Source = previousPage;
                CurrentPageTransform.Angle = 0;
                PreviousPageTransform.Angle = -90;
                //SetPage(newImage);
            };
            storyboard.Begin();


        }

        public void SetPage(BitmapImage page)
        {
            CurrentPageImage.Source = page;
            CurrentPageTransform.Angle = 0;
        }

        public void SetPreviousPage(BitmapImage previousPage) 
        {
            PreviousPageImage.Source = previousPage;
            PreviousPageTransform.Angle = -90;
        }

        public void SetNextPage(BitmapImage nextPage)
        {
            NextPageImage.Source = nextPage;
            NextPageTransform.Angle = 90;
        }
    }
}
