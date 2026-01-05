using SimplePDFViewer.Utilities;
using System.Windows.Media.Imaging;

namespace SimplePDFViewer.ViewModels
{
    public class PageTurnControlViewModel : ObservableObject
    {
        #region Members

       private BitmapImage? _currentPageImage;
       private BitmapImage? _previousPageImage;
        private BitmapImage? _nextPageImage;

        private double _currentPageTransformAngle = 0;
        private double _nextPageTransformAngle = 90;
        private double _previousPageTransformAngle = -90;

        #endregion Members

        public PageTurnControlViewModel()
        {
            
        }

        #region Properties

 
        public BitmapImage? CurrentPageImage
        {
            get { return _currentPageImage; }
            set {
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

        public double CurrentPageTransformAngle
        {
            get { return _currentPageTransformAngle; }
            set
            { 
                _currentPageTransformAngle = value;
                OnPropertyChanged(nameof(CurrentPageTransformAngle));
            }
        }

        public double NextPageTransformAngle
        {
            get { return _nextPageTransformAngle; }
            set 
            {
                _nextPageTransformAngle = value;
                OnPropertyChanged(nameof(NextPageTransformAngle));
            }
        }


        public double PreviousPageTransformAngle
        {
            get { return _previousPageTransformAngle; }
            set 
            { 
                _previousPageTransformAngle = value;
                OnPropertyChanged(nameof(PreviousPageTransformAngle));
            }
        }

        #endregion Properties


        #region Methods


        #endregion Methods



    }
}
