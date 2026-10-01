namespace WinForms11.ToggleSwitchAppearance;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private CheckBox normalCheckBox;
    private CheckBox toggleSwitchCheckBox;

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
        normalCheckBox = new CheckBox();
        toggleSwitchCheckBox = new CheckBox();

        normalCheckBox.Text = "Normal appearance";
        normalCheckBox.Location = new Point(20, 20);
        normalCheckBox.AutoSize = true;

        toggleSwitchCheckBox.Text = "ToggleSwitch appearance";
        toggleSwitchCheckBox.Location = new Point(20, 60);
        toggleSwitchCheckBox.AutoSize = true;

        components = new System.ComponentModel.Container();
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(280, 120);
        Controls.Add(normalCheckBox);
        Controls.Add(toggleSwitchCheckBox);
        Text = "MainForm";
    }

    #endregion
}
