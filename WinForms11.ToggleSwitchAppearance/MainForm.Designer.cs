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
        SuspendLayout();
        // 
        // normalCheckBox
        // 
        normalCheckBox.AutoSize = true;
        normalCheckBox.Location = new Point(20, 37);
        normalCheckBox.Name = "normalCheckBox";
        normalCheckBox.Size = new Size(255, 36);
        normalCheckBox.TabIndex = 0;
        normalCheckBox.Text = "Normal appearance";
        // 
        // toggleSwitchCheckBox
        // 
        toggleSwitchCheckBox.AutoSize = true;
        toggleSwitchCheckBox.Location = new Point(20, 96);
        toggleSwitchCheckBox.Name = "toggleSwitchCheckBox";
        toggleSwitchCheckBox.Size = new Size(317, 36);
        toggleSwitchCheckBox.TabIndex = 1;
        toggleSwitchCheckBox.Text = "ToggleSwitch appearance";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(887, 231);
        Controls.Add(normalCheckBox);
        Controls.Add(toggleSwitchCheckBox);
        Name = "MainForm";
        Text = "MainForm";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
