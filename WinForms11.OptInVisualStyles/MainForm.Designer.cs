namespace WinForms11.OptInVisualStyles;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private Button modernButton;
    private Button classicButton;
    private CheckBox sampleCheckBox;
    private GroupBox sampleGroupBox;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        modernButton = new Button();
        classicButton = new Button();
        sampleCheckBox = new CheckBox();
        sampleGroupBox = new GroupBox();

        modernButton.Text = "Net11 rendering";
        modernButton.Location = new Point(20, 20);
        modernButton.Size = new Size(160, 32);

        classicButton.Text = "Classic rendering";
        classicButton.Location = new Point(20, 60);
        classicButton.Size = new Size(160, 32);

        sampleCheckBox.Text = "Sample check box";
        sampleCheckBox.Location = new Point(20, 100);
        sampleCheckBox.AutoSize = true;

        sampleGroupBox.Text = "Sample group box";
        sampleGroupBox.Location = new Point(20, 130);
        sampleGroupBox.Size = new Size(200, 60);

        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(320, 220);
        Controls.Add(modernButton);
        Controls.Add(classicButton);
        Controls.Add(sampleCheckBox);
        Controls.Add(sampleGroupBox);
        Text = "MainForm";
    }

    #endregion
}
