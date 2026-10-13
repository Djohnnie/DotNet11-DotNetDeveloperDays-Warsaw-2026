using System.Windows;

namespace Wpf11.DesktopWindowManagerCrashGuard;

// .NET 11 Preview 2 release notes (wpf.md):
// "WPF now guards against crashes caused by Desktop Window Manager (DWM) failures."
//
// Before .NET 11, a transient failure in the Desktop Window Manager - the OS component
// responsible for compositing, live thumbnails, Aero glass/Fluent effects, etc. - could take
// down a WPF application along with it. This release makes WPF resilient to that: it's a
// runtime-level fix, not a library feature, so there is no MessageBoxButton-style enum or
// AppContext switch to demonstrate here - just a window explaining what changed and why.
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
