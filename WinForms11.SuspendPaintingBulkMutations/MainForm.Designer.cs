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
        SuspendLayout();
        // 
        // rearrangeButton
        // 
        rearrangeButton.Location = new Point(20, 20);
        rearrangeButton.Name = "rearrangeButton";
        rearrangeButton.Size = new Size(295, 80);
        rearrangeButton.TabIndex = 0;
        rearrangeButton.Text = "Rearrange labels";
        // 
        // labelsPanel
        // 
        labelsPanel.BorderStyle = BorderStyle.FixedSingle;
        labelsPanel.Location = new Point(20, 238);
        labelsPanel.Name = "labelsPanel";
        labelsPanel.Size = new Size(360, 240);
        labelsPanel.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(850, 635);
        Controls.Add(rearrangeButton);
        Controls.Add(labelsPanel);
        Name = "MainForm";
        Text = "MainForm";
        ResumeLayout(false);
    }

    #endregion
}
