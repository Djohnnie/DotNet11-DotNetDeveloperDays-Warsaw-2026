namespace WinForms11.DeferredFormRevealing;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        // A form can opt out of the application default (or opt in explicitly) with its own
        // FormRevealMode property, and observe when the mode is resolved via FormRevealModeChanged.
        FormRevealMode = FormRevealMode.Deferred;
        FormRevealModeChanged += (_, _) => Text = $"FormRevealMode is now: {FormRevealMode}";
    }
}
