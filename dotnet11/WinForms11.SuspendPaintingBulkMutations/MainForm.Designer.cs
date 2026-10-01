namespace WinForms11.SuspendPaintingBulkMutations;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private Button rearrangeButton;
    private Panel labelsPanel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        rearrangeButton = new Button();
        labelsPanel = new Panel();

        rearrangeButton.Text = "Rearrange labels";
        rearrangeButton.Location = new Point(20, 20);
        rearrangeButton.Size = new Size(160, 32);

        labelsPanel.Location = new Point(20, 60);
        labelsPanel.Size = new Size(360, 240);
        labelsPanel.BorderStyle = BorderStyle.FixedSingle;
        for (var i = 1; i <= 8; i++)
        {
            labelsPanel.Controls.Add(new Label
            {
                Text = $"Item {i}",
                AutoSize = true,
                Left = 10,
                Top = 10 + (i - 1) * 20
            });
        }

        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 340);
        Controls.Add(rearrangeButton);
        Controls.Add(labelsPanel);
        Text = "MainForm";
    }

    #endregion
}
