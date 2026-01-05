using PdfiumViewer;
using SimplePDFViewer.Utilities;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SimplePDFViewer.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region Members

        private string _currentPage = "0";
        private double _zoomFactor = 1.0;
      // private BitmapImage? _selectedPdfDocumentImage;
        private BitmapImage? _currentPageImage;
        private BitmapImage? _previousPageImage;
        private BitmapImage? _nextPageImage;
        private PdfDocument? _pdfDocument;

        int currentPageValue = 0;
        int totalPagesValue = 0;

        private string _totalPages = "0";
       private string _pdfDocumentStatus = "Ready. Open a PDF file to begin.";
        private bool _isNextButtonEnabled = false;
        private bool _isPreviousButtonEnabled = false;
        private bool _isZoomInEnabled = false;
        private bool _isZoomOutEnabled = false;
        private bool _isFitWidthEnabled = false;
        private bool _isPDFLoaded = false;

        public bool CanNavigatePrevious => IsPDFLoaded && currentPageValue > 0;
        public bool CanNavigateNext => IsPDFLoaded && currentPageValue < totalPagesValue - 1;


        #region Event Methods

        public event EventHandler<bool> RequestPageTurn;
        public event EventHandler<BitmapImage> CurrentPageRendered;
        public event EventHandler PreviousAndNextPagesPreloaded;

        #endregion Event Methods

        #endregion Members

        public MainViewModel()
        {
            //_pdfDocument = new PdfDocument();
        }

        #region Initializing Routines


        #endregion Initializing Routines

        #region Properties

        public double ZOOM_STEP { get; private set; } = 0.25;


        public bool IsPDFLoaded
        {
            get { return _isPDFLoaded; }
            set
            { 
                _isPDFLoaded = value;
                OnPropertyChanged(nameof(IsPDFLoaded));
            }
        }


        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }


        public double ZoomFactor
        {
            get { return _zoomFactor; }
            set
            {
                _zoomFactor = value;
                OnPropertyChanged(nameof(ZoomFactor));
            }
        }
 
        //public BitmapImage? SelectedPdfDocumentImage
        //{
        //    get { return _selectedPdfDocumentImage; }
        //    set
        //    { 
        //        _selectedPdfDocumentImage = value;
        //        OnPropertyChanged(nameof(SelectedPdfDocumentImage));
        //    }
        //}


        public string TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

 
        public string PdfDocumentStatus
        {
            get { return _pdfDocumentStatus; }
            set 
            {
                _pdfDocumentStatus = value;
                OnPropertyChanged(nameof(PdfDocumentStatus));
            }
        }


        public bool IsNextButtonEnabled
        {
            get { return _isNextButtonEnabled; }
            set 
            {
                _isNextButtonEnabled = value;
                OnPropertyChanged(nameof(IsNextButtonEnabled));
            }
        }

        public bool IsPreviousButtonEnabled
        {
            get { return _isPreviousButtonEnabled; }
            set
            {
                _isPreviousButtonEnabled = value;
                OnPropertyChanged(nameof(IsPreviousButtonEnabled));
            }
        }


        public bool IsZoomInEnabled
        {
            get { return _isZoomInEnabled; }
            set 
            {
                _isZoomInEnabled = value;
                OnPropertyChanged($"{nameof(IsZoomInEnabled)}");
            }
        }


        public bool IsZoomOutEnabled
        {
            get { return _isZoomOutEnabled; }
            set
            {
                _isZoomOutEnabled = value;
                OnPropertyChanged($"{nameof(IsZoomOutEnabled)}");
            }
        }

 
        public bool IsFitWidthEnabled
        {
            get { return _isFitWidthEnabled; }
            set
            {
                _isFitWidthEnabled = value;
                OnPropertyChanged($"{nameof(IsFitWidthEnabled)}");
            }
        }

        public BitmapImage? CurrentPageImage
        {
            get { return _currentPageImage; }
            set
            {
                _currentPageImage = value;
                OnPropertyChanged(nameof(CurrentPageImage));
            }
        }

        public BitmapImage? PreviousPageImage
        {
            get { return _previousPageImage; }
            set
            {
                _previousPageImage = value;
                OnPropertyChanged(nameof(PreviousPageImage));
            }
        }

        public BitmapImage? NextPageImage
        {
            get { return _nextPageImage; }
            set
            {
                _nextPageImage = value;
                OnPropertyChanged(nameof(NextPageImage));
            }
        }



        #endregion Properties 

        #region Methods

        public void LoadPdf(string fileName)
        {
            try
            {
                _pdfDocument = PdfDocument.Load(fileName);

                CurrentPage = $"{currentPageValue}";
                ZoomFactor = 1.0;
                //UI Update
                totalPagesValue = _pdfDocument.PageCount;
                TotalPages = $"{totalPagesValue}";
                PdfDocumentStatus = $"Loaded: {Path.GetFileName(fileName)}";

                //Navigation buttons
                IsNextButtonEnabled = _pdfDocument.PageCount > 1;
                IsPreviousButtonEnabled = false;
                IsZoomInEnabled = true;
                IsZoomOutEnabled = true;
                IsFitWidthEnabled = true;
                IsPDFLoaded = true;

                //Display first page
                DisplayCurrentPage(0);
                PreloadAdjacentPages(0);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PDF: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PdfDocumentStatus = "Error loading PDF file.";
            }
        }

        private void DisplayCurrentPage(int currentPageValue)
        {
            if (_pdfDocument is null)
            {
                return;
            }

            try
            {
                //calculate dpi based on zoom factor
                int dpi = (int)(96 * ZoomFactor);
                CurrentPageImage = RenderPage(currentPageValue, dpi);

                CurrentPageRendered?.Invoke(this, CurrentPageImage);//EventArgs.Empty

                CurrentPage = $"{(currentPageValue + 1)}";//
                IsPreviousButtonEnabled = currentPageValue > 0;
                IsNextButtonEnabled = currentPageValue < totalPagesValue - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying page: {ex.Message}", "Error",
                     MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private BitmapImage RenderPage(int pageIndex, int dpi)
        {
            using (var image = _pdfDocument.Render(pageIndex, dpi, dpi, false))
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var memory = new MemoryStream())
                {
                    image.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            }
        }


        public void MoveToPrevious()
        {
            if (CanNavigatePrevious)
            {
                currentPageValue--;
                DisplayCurrentPage(currentPageValue);
                PreloadAdjacentPages(currentPageValue);
                RequestPageTurn?.Invoke(this, false);
            }
        }

        public void MoveToNext()
        {
            if (CanNavigateNext)
            {
                currentPageValue++;
                DisplayCurrentPage(currentPageValue);
                PreloadAdjacentPages(currentPageValue);
                RequestPageTurn?.Invoke(this, true);
            }

            //RequestPageTurn
        }

        private void PreloadAdjacentPages(int currentPage)
        {
            if (_pdfDocument == null) return;

            try
            {
                int dpi = (int)(96 * ZoomFactor);
              //  int currentPage = currentPageValue;//int.Parse(CurrentPage)
                int totalPages = int.Parse(TotalPages);

                // Preload next page
                if (currentPage < totalPages - 1)
                { 
                    int nextPage = currentPage + 1;
                    NextPageImage = RenderPage(nextPage, dpi);
                }

                // Preload previous page
                if (currentPage > 1)
                {
                    int previousPage = currentPage - 1;
                    PreviousPageImage = RenderPage(previousPage, dpi);
                }

                PreviousAndNextPagesPreloaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error preloading pages: {ex.Message}");
            }
        }


        public void PerformZoomIn()
        { 
            if (ZoomFactor > 0.25) // Min zoom 25%
            {
                ZoomFactor -= ZOOM_STEP;
                DisplayCurrentPage(currentPageValue);
                PdfDocumentStatus = $"Zoom: {(ZoomFactor * 100):F0}%";
            }
        }

        public void PerformZoomOut()
        {
            if (ZoomFactor < 3.0) //Max zoom is 300%
            {
                ZoomFactor += ZOOM_STEP;
                DisplayCurrentPage(currentPageValue);
                PdfDocumentStatus = $"Zoom: {(ZoomFactor * 100):F0}%";
            }
        }

        public void PerformFitWidth(double scrollViewerWidth)
        {
            if (_pdfDocument is null)
            {
                return;
            }

            //Calculate the fit width zoom factor based on the width of the PDF page
            using (var image = _pdfDocument.Render(currentPageValue, 96, 96, false))
            {

                double availableWidth = scrollViewerWidth - 20; // Subtract padding
                ZoomFactor = availableWidth / image.Width;
                DisplayCurrentPage(currentPageValue);
                PdfDocumentStatus = $"Fit to width: {(ZoomFactor * 100):F0}%";
            }

        }

        public void Dispose()
        {
            _pdfDocument?.Dispose();
        }

        #endregion Methods
    }
}
