namespace WinForms11.OptInVisualStyles;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        // VisualStylesMode also cascades per-control, and can be overridden for a single
        // control even though the application default is Net11 (the default is VisualStylesMode.Inherit,
        // which follows the parent - here the form - up to the application default).
        modernButton.VisualStylesMode = VisualStylesMode.Net11;
        classicButton.VisualStylesMode = VisualStylesMode.Classic;

        Text = $"Application default: {Application.DefaultVisualStylesMode}";
    }
}
