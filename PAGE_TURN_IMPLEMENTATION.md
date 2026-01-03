# Page Turning Animation - Implementation Details

## Overview
This document explains how the page turning animation feature works in the SimplePDFViewerApp.

## Architecture

The page turning effect is implemented using three main components:

### 1. PageTurnControl (Custom WPF UserControl)
**Location**: `SimplePDFViewer/Controls/PageTurnControl.xaml` and `.xaml.cs`

This custom control provides the visual page-turning effect using WPF's 3D projection capabilities.

**Key Features**:
- Uses `PlaneProjection` for 3D rotation around the Y-axis
- Displays two images: `FrontPageImage` (current page being flipped) and `BackPageImage` (the next/previous page revealed)
- Provides two animation methods: `TurnPageForward()` and `TurnPageBackward()`

**XAML Structure**:
```xml
<Grid ClipToBounds="True">
    <Image x:Name="imgBackPage" Source="{Binding BackPageImage}"/>
    <Grid x:Name="FrontPageGrid">
        <Grid.Projection>
            <PlaneProjection x:Name="planeProjection" RotationY="0"/>
        </Grid.Projection>
        <Image x:Name="imgFrontPage" Source="{Binding FrontPageImage}"/>
    </Grid>
</Grid>
```

**Animation Code**:
```csharp
public void TurnPageForward(Action? onCompleted = null)
{
    var animation = new DoubleAnimation
    {
        From = 0,
        To = -180,  // Rotate 180 degrees to flip the page
        Duration = TimeSpan.FromMilliseconds(600),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };
    
    animation.Completed += (s, e) =>
    {
        planeProjection.RotationY = 0;  // Reset for next animation
        onCompleted?.Invoke();
    };
    
    planeProjection.BeginAnimation(PlaneProjection.RotationYProperty, animation);
}
```

### 2. MainViewModel Updates
**Location**: `SimplePDFViewer/ViewModels/MainViewModel.cs`

The ViewModel was enhanced to support the animation workflow:

**New Properties**:
- `NextPdfDocumentImage` - Holds the image for the page being navigated to
- `pendingPageValue` - Tracks the page number during animation

**New Methods**:
- `PreparePageTurnForward(Action)` - Pre-renders the next page and starts the animation
- `PreparePageTurnBackward(Action)` - Pre-renders the previous page and starts the animation
- `CompletePageTurn()` - Updates the current page after animation completes
- `RenderPage(int pageIndex)` - Renders a specific page to a BitmapImage

**Workflow**:
```csharp
// 1. User clicks "Next" button
PreparePageTurnForward(() => {
    // 2. Pre-render next page as BackPageImage
    // 3. Start animation
    pageTurnControl.TurnPageForward(() => {
        // 4. Animation completes, update current page
        CompletePageTurn();
    });
});
```

### 3. MainWindow Integration
**Location**: `SimplePDFViewer/MainWindow.xaml` and `.xaml.cs`

**XAML Changes**:
- Added `xmlns:controls="clr-namespace:SimplePDFViewer.Controls"` namespace
- Replaced `<Image>` with `<controls:PageTurnControl>`
- Bound both `FrontPageImage` and `BackPageImage` properties

**Code-Behind Changes**:
```csharp
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
```

## How It Works (Step by Step)

1. **User clicks "Next" or "Previous" button**
   - The button click handler in MainWindow.xaml.cs is triggered

2. **ViewModel prepares for page turn**
   - `PreparePageTurnForward()` or `PreparePageTurnBackward()` is called
   - The next/previous page is rendered and set as `NextPdfDocumentImage`
   - This becomes the `BackPageImage` in the PageTurnControl

3. **Animation starts**
   - The callback provided to PreparePageTurn methods is executed
   - `pageTurnControl.TurnPageForward()` or `TurnPageBackward()` is called
   - The `PlaneProjection.RotationY` property is animated from 0° to ±180°

4. **During animation**
   - The front page (current page) rotates around the Y-axis
   - As it rotates past 90°, the back page becomes visible
   - The effect looks like turning a page in a book

5. **Animation completes**
   - The animation's Completed event fires
   - `CompletePageTurn()` is called
   - The current page index is updated
   - The `SelectedPdfDocumentImage` is updated to the new page
   - UI elements (page counter, buttons) are updated

## Customization

You can customize the page turning effect by modifying:

- **Duration**: Change `TimeSpan.FromMilliseconds(600)` to make the animation faster or slower
- **Easing**: Change `QuadraticEase` to other easing functions like `CubicEase`, `ExponentialEase`, etc.
- **Rotation Angle**: Modify the `To` property of the animation for different effects
- **Direction**: The sign of the rotation angle determines the direction (positive = left to right, negative = right to left)

## Technical Notes

- **Why PlaneProjection?**: WPF's `PlaneProjection` provides simple 3D transformations without requiring complex 3D scenes
- **Pre-rendering**: Pages are rendered before animation starts to ensure smooth playback
- **Reset**: The projection is reset to 0° after each animation to prepare for the next turn
- **Callbacks**: Action callbacks ensure proper sequencing of operations
