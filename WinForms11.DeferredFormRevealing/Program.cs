namespace WinForms11.DeferredFormRevealing;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Before .NET 11, a form could briefly flash unstyled (e.g. a white background)
        // before dark-mode theming and layout had finished applying, especially on first show.
        // FormRevealMode.Deferred keeps the form concealed until its initial layout and theming
        // have settled, then reveals it in one step. The application-wide default is set here;
        // an individual form can still override it via its own FormRevealMode property.
        Application.SetDefaultFormRevealMode(FormRevealMode.Deferred);

        Console.WriteLine($"Application.IsFormRevealDeferred = {Application.IsFormRevealDeferred}");

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
