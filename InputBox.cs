public class InputBox : Form
{
    private TextBox inputTextBox;
    private Button okButton;
    private Button cancelButton;

    public string InputText => inputTextBox.Text;

    public InputBox(string title, string prompt)
    {
        Text = title;
        Size = new Size(400, 150);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        Label promptLabel = new Label
        {
            Text = prompt,
            Location = new Point(10, 10),
            Size = new Size(360, 20)
        };
        Controls.Add(promptLabel);

        inputTextBox = new TextBox
        {
            Location = new Point(10, 40),
            Size = new Size(360, 20)
        };
        Controls.Add(inputTextBox);

        okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(210, 70),
            Size = new Size(75, 25)
        };
        Controls.Add(okButton);

        cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(295, 70),
            Size = new Size(75, 25)
        };
        Controls.Add(cancelButton);

        AcceptButton = okButton;
        CancelButton = cancelButton;
    }
}
