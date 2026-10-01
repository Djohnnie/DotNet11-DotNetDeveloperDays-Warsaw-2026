namespace WinForms11.OptInVisualStyles;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // .NET 11 Preview 7 introduces a new, opt-in rendering pipeline for Button, CheckBox,
        // RadioButton, GroupBox, TextBox and RichTextBox: VisualStylesMode.Net11.
        // It is opt-in application-wide (existing apps keep rendering exactly as before)
        // via Application.SetDefaultVisualStylesMode, called before Application.Run.
        Application.SetDefaultVisualStylesMode(VisualStylesMode.Net11);

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
