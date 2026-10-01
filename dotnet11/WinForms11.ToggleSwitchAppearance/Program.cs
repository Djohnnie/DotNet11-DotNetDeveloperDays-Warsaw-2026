namespace WinForms11.ToggleSwitchAppearance;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // The new Appearance.ToggleSwitch value (below, on the CheckBox controls in MainForm)
        // only renders as a modern toggle switch under VisualStylesMode.Net11 - under the
        // classic renderer it still falls back to a normal check box.
        Application.SetDefaultVisualStylesMode(VisualStylesMode.Net11);

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
