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
    /// <param name="disposing">true if managed resources should
    /// be disposed; otherwise, false.</param>
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
        SuspendLayout();
        // 
        // modernButton
        // 
        modernButton.Location = new Point(20, 20);
        modernButton.Name = "modernButton";
        modernButton.Size = new Size(355, 62);
        modernButton.TabIndex = 0;
        modernButton.Text = "Net11 rendering";
        // 
        // classicButton
        // 
        classicButton.Location = new Point(20, 88);
        classicButton.Name = "classicButton";
        classicButton.Size = new Size(355, 62);
        classicButton.TabIndex = 1;
        classicButton.Text = "Classic rendering";
        // 
        // sampleCheckBox
        // 
        sampleCheckBox.AutoSize = true;
        sampleCheckBox.Location = new Point(20, 211);
        sampleCheckBox.Name = "sampleCheckBox";
        sampleCheckBox.Size = new Size(239, 36);
        sampleCheckBox.TabIndex = 2;
        sampleCheckBox.Text = "Sample check box";
        // 
        // sampleGroupBox
        // 
        sampleGroupBox.Location = new Point(20, 323);
        sampleGroupBox.Name = "sampleGroupBox";
        sampleGroupBox.Size = new Size(489, 253);
        sampleGroupBox.TabIndex = 3;
        sampleGroupBox.TabStop = false;
        sampleGroupBox.Text = "Sample group box";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(577, 634);
        Controls.Add(modernButton);
        Controls.Add(classicButton);
        Controls.Add(sampleCheckBox);
        Controls.Add(sampleGroupBox);
        Name = "MainForm";
        Text = "MainForm";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
