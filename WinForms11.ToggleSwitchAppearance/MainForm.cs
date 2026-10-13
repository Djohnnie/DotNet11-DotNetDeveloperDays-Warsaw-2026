namespace WinForms11.ToggleSwitchAppearance;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        // CheckBox and RadioButton gain a new Appearance value: ToggleSwitch.
        // Checked state, CheckedChanged, and data binding all keep working exactly as before -
        // only the rendering changes.
        toggleSwitchCheckBox.Appearance = Appearance.ToggleSwitch;
        toggleSwitchCheckBox.CheckedChanged += (_, _) =>
            Text = $"Toggle switch is now: {(toggleSwitchCheckBox.Checked ? "On" : "Off")}";
    }
}
