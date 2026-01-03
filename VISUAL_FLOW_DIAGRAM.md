# Page Turn Animation - Visual Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         USER INTERACTION                                 │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
                    ┌───────────────────────────┐
                    │   User Clicks "Next" or   │
                    │    "Previous" Button      │
                    └───────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                      MAINWINDOW.XAML.CS                                 │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  buttonNext_Click() OR buttonPrevious_Click()                          │
│         │                                                                │
│         ▼                                                                │
│  viewModel.PreparePageTurnForward(() => {                              │
│      pageTurnControl.TurnPageForward(() => {                           │
│          viewModel.CompletePageTurn();                                 │
│      });                                                                 │
│  });                                                                     │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴───────────────┐
                    ▼                               ▼
┌─────────────────────────────────┐   ┌─────────────────────────────────┐
│    MAINVIEWMODEL.CS             │   │   PAGETURNCONTROL.XAML.CS       │
├─────────────────────────────────┤   ├─────────────────────────────────┤
│                                 │   │                                 │
│ STEP 1: PreparePageTurn         │   │ STEP 2: TurnPageForward         │
│ ┌─────────────────────────────┐ │   │ ┌─────────────────────────────┐ │
│ │ • Check page bounds         │ │   │ │ • Call AnimatePageTurn()    │ │
│ │ • Calculate next page index │ │   │ │   with angle = -180         │ │
│ │ • Render next page image    │ │   │ │                             │ │
│ │ • Set NextPdfDocumentImage  │ │   │ │ AnimatePageTurn():          │ │
│ │ • Store pendingPageValue    │ │   │ │ ┌─────────────────────────┐ │ │
│ │ • Invoke callback           │ │   │ │ │ • Create DoubleAnimation│ │ │
│ └─────────────────────────────┘ │   │ │ │ • From: 0, To: angle    │ │ │
│                                 │   │ │ │ • Duration: 600ms       │ │ │
│         │                       │   │ │ │ • Easing: QuadraticEase │ │ │
│         └─────────────────────────────▶ │ • BeginAnimation on     │ │ │
│                                 │   │ │ │   PlaneProjection       │ │ │
│                                 │   │ │ └─────────────────────────┘ │ │
│                                 │   │ └─────────────────────────────┘ │
│                                 │   │                                 │
│                                 │   │         Animation Runs          │
│                                 │   │ ┌─────────────────────────────┐ │
│                                 │   │ │  0ms: RotationY = 0°        │ │
│                                 │   │ │ 150ms: RotationY = -45°     │ │
│                                 │   │ │ 300ms: RotationY = -90°     │ │
│                                 │   │ │ 450ms: RotationY = -135°    │ │
│                                 │   │ │ 600ms: RotationY = -180°    │ │
│                                 │   │ └─────────────────────────────┘ │
│                                 │   │                                 │
│                                 │   │ Animation.Completed Event       │
│                                 │   │ ┌─────────────────────────────┐ │
│                                 │   │ │ • Reset RotationY = 0       │ │
│         ┌───────────────────────────────│ • Invoke onCompleted()    │ │
│         │                       │   │ └─────────────────────────────┘ │
│         ▼                       │   └─────────────────────────────────┘
│                                 │
│ STEP 3: CompletePageTurn        │
│ ┌─────────────────────────────┐ │
│ │ • Update currentPageValue   │ │
│ │ • Set pendingPageValue = -1 │ │
│ │ • Update CurrentImage       │ │
│ │ • Clear NextImage           │ │
│ │ • Update page counter       │ │
│ │ • Update button states      │ │
│ └─────────────────────────────┘ │
│                                 │
└─────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                              UI UPDATE                                   │
├─────────────────────────────────────────────────────────────────────────┤
│  • Page counter shows new page number                                   │
│  • Previous/Next buttons update enabled state                           │
│  • New page is visible (animation complete)                             │
└─────────────────────────────────────────────────────────────────────────┘
```

## Visual Representation of the Animation

```
BEFORE ANIMATION (Page 1 visible, going to Page 2)
┌─────────────────┐
│                 │
│    PAGE 1       │  ← FrontPageImage (will rotate)
│   (Current)     │
│                 │
└─────────────────┘
     [Behind]
   ┌─────────┐
   │ PAGE 2  │       ← BackPageImage (revealed during rotation)
   └─────────┘


DURING ANIMATION (RotationY = -90°, halfway)
      ┌─────┐
      │ PAG │         ← FrontPage seen edge-on
      │  E  │
      │  1  │
      └─────┘
┌─────────────────┐
│                 │
│    PAGE 2       │  ← BackPage becoming visible
│                 │
│                 │
└─────────────────┘


AFTER ANIMATION (Page 2 visible)
┌─────────────────┐
│                 │
│    PAGE 2       │  ← Now FrontPageImage
│    (Current)    │
│                 │
└─────────────────┘
     [Behind]
   ┌─────────┐
   │  Empty  │       ← BackPageImage cleared
   └─────────┘
```

## Data Flow During Animation

```
┌──────────────────┐
│  Initial State   │
├──────────────────┤
│ currentPageValue │──────┐
│        = 0       │      │
│                  │      │
│ SelectedPdfDoc   │      │
│   Image = Page0  │      │
│                  │      │
│ NextPdfDoc       │      │
│   Image = null   │      │
└──────────────────┘      │
                          │
                          ▼
              User clicks "Next"
                          │
                          ▼
┌──────────────────┐      │
│ Prepare Phase    │◀─────┘
├──────────────────┤
│ pendingPageValue │
│        = 1       │
│                  │
│ SelectedPdfDoc   │
│   Image = Page0  │ (unchanged)
│                  │
│ NextPdfDoc       │
│   Image = Page1  │ (pre-rendered)
└──────────────────┘
         │
         ▼
┌──────────────────┐
│ Animation Phase  │
├──────────────────┤
│ • RotationY      │
│   animates from  │
│   0° to -180°    │
│                  │
│ • User sees      │
│   Page0 flip to  │
│   reveal Page1   │
└──────────────────┘
         │
         ▼
┌──────────────────┐
│ Complete Phase   │
├──────────────────┤
│ currentPageValue │
│        = 1       │
│                  │
│ SelectedPdfDoc   │
│   Image = Page1  │ (updated)
│                  │
│ NextPdfDoc       │
│   Image = null   │ (cleared)
│                  │
│ pendingPageValue │
│        = -1      │
└──────────────────┘
```

## XAML Structure in PageTurnControl

```
┌────────────────────────────────────────┐
│ <Grid> (LayoutRoot, ClipToBounds)     │
├────────────────────────────────────────┤
│                                        │
│  ┌──────────────────────────────────┐ │
│  │ <Image> (imgBackPage)            │ │
│  │ Source = BackPageImage           │ │
│  │ (Visible behind front page)      │ │
│  └──────────────────────────────────┘ │
│                                        │
│  ┌──────────────────────────────────┐ │
│  │ <Grid> (FrontPageGrid)           │ │
│  │ ┌────────────────────────────────┤ │
│  │ │ <Grid.Projection>              │ │
│  │ │   <PlaneProjection>            │ │
│  │ │     RotationY = "0"  ◄──────┐  │ │
│  │ │   </PlaneProjection>         │  │ │
│  │ │ </Grid.Projection>           │  │ │
│  │ └────────────────────────────────┤ │
│  │                                  │  │ │
│  │ ┌──────────────────────────────┐│  │ │
│  │ │ <Image> (imgFrontPage)       ││  │ │
│  │ │ Source = FrontPageImage      ││  │ │
│  │ │ (This image rotates)         ││  │ │
│  │ └──────────────────────────────┘│  │ │
│  └──────────────────────────────────┘ │
│                                        │
└────────────────────────────────────────┘
                │
                └──────── Animation modifies
                          this RotationY property
```

## Key Concepts

### PlaneProjection
- Provides simple 3D transformations in 2D space
- RotationY rotates around vertical axis (Y-axis)
- Positive angle = rotate left to right
- Negative angle = rotate right to left

### DoubleAnimation
- Smoothly animates a numeric property over time
- From: starting value (0°)
- To: ending value (±180°)
- Duration: how long the animation takes (600ms)
- EasingFunction: controls acceleration/deceleration

### QuadraticEase
- Starts slow, speeds up in middle, slows down at end
- More natural than linear animation
- EaseInOut mode affects both start and end

### Pre-rendering
- Renders destination page before animation
- Ensures smooth animation without pauses
- BackPageImage is set before animation starts
- Prevents flicker or blank frames

## Performance Considerations

```
┌────────────────────────────────────┐
│ Optimizations                      │
├────────────────────────────────────┤
│                                    │
│ ✓ Pre-render pages                │
│   └─► No rendering during anim    │
│                                    │
│ ✓ Freeze BitmapImages             │
│   └─► Can be used across threads  │
│                                    │
│ ✓ Hardware acceleration           │
│   └─► PlaneProjection uses GPU    │
│                                    │
│ ✓ ClipToBounds                    │
│   └─► Prevents overdraw           │
│                                    │
│ ✓ Callback pattern                │
│   └─► Proper resource cleanup     │
│                                    │
└────────────────────────────────────┘
```
