namespace WinForms11.SuspendPaintingBulkMutations;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        rearrangeButton.Click += (_, _) => RearrangeLabels();
    }

    private void RearrangeLabels()
    {
        // Every Control now implements ISupportSuspendPainting, and the ControlMutationExtensions.SuspendPainting
        // extension method returns an IDisposable scope that suppresses repaint/layout notifications while several
        // controls are moved or resized in bulk - eliminating the flicker you'd otherwise see doing this one control
        // at a time. TargetAndDescendants also suspends painting for the container's child controls, not just the
        // container itself, whereas SuspendLayout/ResumeLayout only defers layout logic, not painting.
        using (labelsPanel.SuspendPainting(LayoutSuspendTraversal.TargetAndDescendants))
        {
            var random = new Random();
            foreach (Control label in labelsPanel.Controls)
            {
                label.Left = random.Next(0, labelsPanel.Width - label.Width);
                label.Top = random.Next(0, labelsPanel.Height - label.Height);
            }
        }
    }
}
