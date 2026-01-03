# Page Turning Animation - Example Usage

## Quick Start

The page turning animation is now integrated into the SimplePDFViewerApp. When you click the "Next" or "Previous" buttons, the PDF pages will flip with a realistic 3D animation effect.

## Example Code Snippets

### Basic Animation Usage

If you want to use the PageTurnControl in your own WPF project, here's how:

#### 1. Add the Control to Your XAML

```xml
<Window xmlns:controls="clr-namespace:SimplePDFViewer.Controls"
        ...>
    <controls:PageTurnControl x:Name="pageTurnControl"
                             FrontPageImage="{Binding CurrentImage}"
                             BackPageImage="{Binding NextImage}"/>
</Window>
```

#### 2. Trigger the Animation in Code-Behind

```csharp
// For Next Page (flip right to left)
private void NextButton_Click(object sender, RoutedEventArgs e)
{
    // Set the back page image (the page we're navigating to)
    viewModel.NextImage = LoadNextPageImage();
    
    // Start the animation
    pageTurnControl.TurnPageForward(() =>
    {
        // Animation complete - update current page
        viewModel.CurrentImage = viewModel.NextImage;
        viewModel.NextImage = null;
    });
}

// For Previous Page (flip left to right)
private void PreviousButton_Click(object sender, RoutedEventArgs e)
{
    // Set the back page image (the page we're navigating to)
    viewModel.NextImage = LoadPreviousPageImage();
    
    // Start the animation
    pageTurnControl.TurnPageBackward(() =>
    {
        // Animation complete - update current page
        viewModel.CurrentImage = viewModel.NextImage;
        viewModel.NextImage = null;
    });
}
```

#### 3. Without Animation (Initial Load)

```csharp
private void LoadPdf(string filename)
{
    var firstPage = RenderPage(0);
    pageTurnControl.SetPageWithoutAnimation(firstPage);
}
```

## Customization Examples

### Change Animation Speed

Edit `PageTurnControl.xaml.cs` line 56 (in the `AnimatePageTurn` method):

```csharp
// Default: 600ms
Duration = TimeSpan.FromMilliseconds(600)

// Faster (300ms)
Duration = TimeSpan.FromMilliseconds(300)

// Slower (1000ms - 1 second)
Duration = TimeSpan.FromMilliseconds(1000)
```

### Change Animation Easing

Edit `PageTurnControl.xaml.cs` line 57:

```csharp
// Default: Quadratic (smooth)
EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }

// Cubic (more dramatic)
EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }

// Exponential (very dramatic)
EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut }

// Elastic (bouncy)
EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut }

// Back (slight overshoot)
EasingFunction = new BackEase { EasingMode = EasingMode.EaseInOut }

// No easing (linear)
EasingFunction = null
```

### Change Rotation Angle (Different Effect)

Edit the calls in `TurnPageForward()` and `TurnPageBackward()`:

```csharp
// Default: 180 degrees (full flip)
public void TurnPageForward(Action? onCompleted = null)
{
    AnimatePageTurn(-180, onCompleted);
}

// 90 degrees (quarter turn)
public void TurnPageForward(Action? onCompleted = null)
{
    AnimatePageTurn(-90, onCompleted);
}

// 270 degrees (more than full flip)
public void TurnPageForward(Action? onCompleted = null)
{
    AnimatePageTurn(-270, onCompleted);
}
```

## Integration Pattern

The implementation follows a three-step pattern:

```
1. PREPARE → 2. ANIMATE → 3. COMPLETE
```

### Step 1: Prepare
```csharp
viewModel.PreparePageTurnForward(() => {
    // Pre-render the next page
    // Set it as BackPageImage
    // Call the animation callback
    ...
});
```

### Step 2: Animate
```csharp
pageTurnControl.TurnPageForward(() => {
    // Animate the 3D rotation
    // Call completion callback when done
    ...
});
```

### Step 3: Complete
```csharp
viewModel.CompletePageTurn();
    // Update current page index
    // Update UI (page counter, buttons)
    // Swap images
```

## Testing Your Implementation

To test the page turning animation:

1. **Build and run the application**
   ```bash
   dotnet build SimplePDFViewer/SimplePDFViewer.csproj
   dotnet run --project SimplePDFViewer/SimplePDFViewer.csproj
   ```

2. **Open a PDF with multiple pages**
   - Click "Open PDF" button
   - Select any PDF file with 2+ pages

3. **Test the animations**
   - Click "Next" - You should see the page flip from right to left
   - Click "Previous" - You should see the page flip from left to right
   - Try clicking rapidly - Animations should queue properly
   - Test with zoom in/out - Animation should work at all zoom levels

4. **Expected behavior**
   - Smooth 600ms animation
   - The front page rotates around its center
   - The back page becomes visible as rotation passes 90°
   - No flicker or visual artifacts
   - Page counter updates after animation

## Troubleshooting

### Animation is too fast/slow
- Adjust the `Duration` in `AnimatePageTurn()` method

### Animation looks jerky
- Check if the PC has enough resources
- Try a simpler `EasingFunction` or set it to `null`
- Reduce the PDF rendering DPI in `RenderPage()`

### Page appears before animation completes
- Ensure `CompletePageTurn()` is called in the animation callback, not before

### Animation doesn't work at all
- Verify `PlaneProjection` is properly set in XAML
- Check that `x:Name="planeProjection"` matches the code-behind
- Ensure callbacks are properly chained

## Advanced: Create Your Own Page Turn Effect

You can create custom effects by modifying the animation:

```csharp
// Curl effect (combine with ScaleTransform)
private void AnimateCurlEffect(Action? onCompleted = null)
{
    var rotationAnim = new DoubleAnimation
    {
        From = 0, To = -180,
        Duration = TimeSpan.FromMilliseconds(600)
    };
    
    var scaleAnim = new DoubleAnimation
    {
        From = 1, To = 0.8,
        Duration = TimeSpan.FromMilliseconds(300),
        AutoReverse = true
    };
    
    planeProjection.BeginAnimation(PlaneProjection.RotationYProperty, rotationAnim);
    // Apply scale animation to create curl effect
}

// Slide effect (use TranslateTransform instead)
private void AnimateSlideEffect(Action? onCompleted = null)
{
    var slideAnim = new DoubleAnimation
    {
        From = 0, To = ActualWidth,
        Duration = TimeSpan.FromMilliseconds(400),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };
    // Apply to TranslateTransform.X
}
```

## Summary

The page turning animation adds visual appeal to the PDF viewer with minimal code changes:
- **Simple API**: Just call `TurnPageForward()` or `TurnPageBackward()`
- **Customizable**: Easy to adjust speed, easing, and rotation angle
- **Performant**: Uses hardware-accelerated WPF 3D transforms
- **Integrated**: Works seamlessly with existing zoom and navigation features
