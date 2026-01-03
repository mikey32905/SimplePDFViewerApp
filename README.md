# SimplePDFViewerApp

Created this app as something to open PDFs without the memory hogging of the bigger programs.  It uses the PdfiumViewer Nuget package.  Also need to add the PdfiumViewer.Native.x86_64.v8-xfa to get the necessary DLL file to go with it.

## Features

- Lightweight PDF viewing
- Page navigation with animated page-turning effect
- Zoom in/out functionality
- Fit to width option

### Page Turning Animation

The app now includes a realistic page-turning animation effect when navigating between pages. This is implemented using WPF's 3D projection capabilities (`PlaneProjection`) to create a smooth flip animation:

- **Next Page**: The current page flips from right to left (RotationY: 0° to -180°)
- **Previous Page**: The current page flips from left to right (RotationY: 0° to 180°)
- **Animation Duration**: 600ms with QuadraticEase for natural motion
- **Pre-rendering**: The next/previous page is pre-rendered before the animation starts for a seamless transition

The implementation is in `SimplePDFViewer/Controls/PageTurnControl.xaml` and its code-behind. 
