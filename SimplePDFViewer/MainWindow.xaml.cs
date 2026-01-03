using Microsoft.Win32;
using SimplePDFViewer.ViewModels;
using System.Windows;

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

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            viewModel.PreparePageTurnBackward(() =>
            {
                pageTurnControl.TurnPageBackward(() =>
                {
                    viewModel.CompletePageTurn();
                });
            });
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            viewModel.PreparePageTurnForward(() =>
            {
                pageTurnControl.TurnPageForward(() =>
                {
                    viewModel.CompletePageTurn();
                });
            });
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
            viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}