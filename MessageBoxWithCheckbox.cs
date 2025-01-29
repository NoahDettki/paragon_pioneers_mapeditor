using System;
using System.Drawing;
using System.Windows.Forms;

public class MessageBoxWithCheckbox : Form
{
    private CheckBox checkBox;
    private Button okButton;
    private Label messageLabel;
    private PictureBox iconPictureBox;

    public bool IsChecked => checkBox.Checked;

    public MessageBoxWithCheckbox(string message, string title, Image icon)
    {
        this.Text = title;
        this.Size = new System.Drawing.Size(450, 230);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // PictureBox for the icon
        iconPictureBox = new PictureBox
        {
            Image = icon,
            Size = new System.Drawing.Size(150, 150),
            Location = new System.Drawing.Point(20, 20),
            SizeMode = PictureBoxSizeMode.Zoom
        };
        this.Controls.Add(iconPictureBox);

        // Label for the message
        messageLabel = new Label
        {
            Text = message,
            AutoSize = false,
            Location = new System.Drawing.Point(190, 20),
            Size = new System.Drawing.Size(200, 100),
            TextAlign = ContentAlignment.TopLeft
        };
        this.Controls.Add(messageLabel);

        // Checkbox
        checkBox = new CheckBox
        {
            Text = "Do not show this again",
            AutoSize = true,
            Location = new System.Drawing.Point(192, 143)
        };
        this.Controls.Add(checkBox);

        // OK button
        okButton = new Button
        {
            Text = "OK",
            Location = new System.Drawing.Point(330, 140),
            DialogResult = DialogResult.OK
        };
        this.Controls.Add(okButton);

        this.AcceptButton = okButton;
    }

    public static bool Show(string message, string title, Image icon)
    {
        using (var box = new MessageBoxWithCheckbox(message, title, icon))
        {
            return box.ShowDialog() == DialogResult.OK && box.IsChecked;
        }
    }
}
