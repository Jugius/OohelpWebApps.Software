using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace OohelpWebApps.Software.Updater.Dialogs.Controls;

internal class ProgressBarSmoother
{
    public static readonly DependencyProperty SmoothValueProperty =
        DependencyProperty.RegisterAttached("SmoothValue", typeof(double), typeof(ProgressBarSmoother), new PropertyMetadata(0.0, changing));
    private static readonly TimeSpan duration = TimeSpan.FromMilliseconds(250);
    public static double GetSmoothValue(DependencyObject obj)
    {
        return (double)obj.GetValue(SmoothValueProperty);
    }
    public static void SetSmoothValue(DependencyObject obj, double value)
    {
        obj.SetValue(SmoothValueProperty, value);
    }
    private static void changing(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var anim = new DoubleAnimation((double)e.NewValue, duration);// new DoubleAnimation((double)e.OldValue, (double)e.NewValue, duration2);
        (d as System.Windows.Controls.ProgressBar).BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, anim);//, HandoffBehavior.Compose);
    }
}
