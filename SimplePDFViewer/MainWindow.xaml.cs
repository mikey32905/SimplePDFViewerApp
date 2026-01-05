using Microsoft.Win32;
using SimplePDFViewer.ViewModels;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimplePDFViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members

        MainViewModel viewModel;

        #endregion Members

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

            //pageTurnControl.SetupControl(viewModel.CurrentPageImage, viewModel.NextPageImage, viewModel.PreviousPageImage);
            viewModel.CurrentPageRendered += ViewModel_CurrentPageRendered;
            viewModel.RequestPageTurn += ViewModel_RequestPageTurn;
        }


        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            { 
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select a PDF file"
            };

            if ( openFileDialog.ShowDialog() == true)
            {
                viewModel.LoadPdf(openFileDialog.FileName);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            viewModel.MoveToPrevious();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            viewModel.MoveToNext();
        }

        private void buttonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            viewModel.PerformZoomIn();
        }        

        private void buttonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            viewModel.PerformZoomOut();
        }

        private void ButtonFitWidth_Click(object sender, RoutedEventArgs e)
        {
            double viewerWidth = scrollViewer.ActualWidth - 20;
            viewModel.PerformFitWidth(viewerWidth);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.RequestPageTurn -= ViewModel_RequestPageTurn;
            viewModel.Dispose();
            base.OnClosed(e);
        }

        #region Methods

        private void ViewModel_RequestPageTurn(object? sender, bool forward)
        {
            // Trigger the appropriate animation in the control
            if (forward)
            {
                pageTurnControl.TurnPageForward(viewModel.CurrentPageImage, viewModel.NextPageImage);
                pageTurnControl.SetNextPage(viewModel.NextPageImage);
            }
            else
            {
                pageTurnControl.TurnPageBackward(viewModel.CurrentPageImage, viewModel.PreviousPageImage);
                pageTurnControl.SetPreviousPage(viewModel.PreviousPageImage);
            }
        }
        private void ViewModel_CurrentPageRendered(object? sender, BitmapImage aCurrentPageImage)
        {
            pageTurnControl.SetPage(aCurrentPageImage);
        }

        #endregion Methods


    }
}