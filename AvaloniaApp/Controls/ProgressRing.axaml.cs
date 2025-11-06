using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace AvaloniaApp.Controls;

public partial class ProgressRing : UserControl
{
    private RotateTransform _rotate;
    private DispatcherTimer _timer;

    public static readonly StyledProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<ProgressRing, double>(nameof(Thickness), 4);

    public static readonly StyledProperty<IBrush> ForegroundProperty =
        AvaloniaProperty.Register<ProgressRing, IBrush>(nameof(Foreground), Brushes.DeepSkyBlue);

    public static readonly StyledProperty<IBrush> BackgroundStrokeProperty =
        AvaloniaProperty.Register<ProgressRing, IBrush>(nameof(BackgroundStroke), Brushes.Gray);

    public double Thickness
    {
        get => GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    public IBrush Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush BackgroundStroke
    {
        get => GetValue(BackgroundStrokeProperty);
        set => SetValue(BackgroundStrokeProperty, value);
    }

    public ProgressRing()
    {
        InitializeComponent();

        var ring = this.FindControl<Ellipse>("PART_Ring");

        // Создаём RotateTransform в коде
        _rotate = new RotateTransform();
        ring.RenderTransform = _rotate;
        ring.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _timer.Tick += (s, e) =>
        {
            _rotate.Angle = (_rotate.Angle + 3) % 360;
        };
        _timer.Start();
    }
}