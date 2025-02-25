using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ParagonPioneers
{
    public partial class ImportMenu : Form
    {
        public ImportMenu()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;

            // Make sure that the input is not empty.
            if (input.Equals(""))
            {
                MessageBox.Show("Import field is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate the input. Only specific characters are allowed.
            if (!IsValidInput(input))
            {
                MessageBox.Show("Your data is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Make sure that the input is rectangular. Every line must have the same length.
            string[] lines = input.Split('\n');
            int firstLineLength = lines[0].TrimEnd('\r').Length; // Windows is weird. Trim '\r' from the first line
            Console.WriteLine(firstLineLength);

            foreach (string l in lines)
            {
                int lineLength = l.TrimEnd('\r').Length; // Windows is still weird. Trim '\r' from each line
                Console.WriteLine(lineLength);
                if (lineLength != firstLineLength)
                {
                    MessageBox.Show("Every line must have the same number of characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Make sure that all mountain ranges are valid. Each mountain tile must have exactly two neighbours.
            for (int y = 0; y < lines.Length; y++) 
            {
                for (int x = 0; x < lines[y].Length; x++) 
                {
                    if (lines[y][x] == 'G')
                    {
                        int neighbourCount = 0;
                        if (y > 0 && lines[y - 1][x] == 'G') neighbourCount++;
                        if (y < lines.Length - 1 && lines[y + 1][x] == 'G') neighbourCount++;
                        if (x > 0 && lines[y][x - 1] == 'G') neighbourCount++;
                        if (x < lines[y].Length - 1 && lines[y][x + 1] == 'G') neighbourCount++;
                        if (neighbourCount != 2) {
                            MessageBox.Show("Each mountain tile must have exactly two neighbours.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            // The input is now definitively valid


            char[,] dataArray = DataToArray(input);

            //show map form and pass the validated array
            Map map = new Map(dataArray);
            map.Show();

            //hides import menu
            this.Hide();

            map.FormClosed += ShowImportMenu;

        }

        private void ShowImportMenu(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }

        private char[,] DataToArray(string data)
        {
            // Split the input into lines
            string[] rows = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            char[,] result = new char[rows.Length, rows[0].Length];

            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    result[i, j] = rows[i][j];
                }
            }
            return result;
        }

        private bool IsValidInput(string data)
        {
            try
            {
                //returns if the json string only contains W K G 0 1 2 3
                return Regex.IsMatch(data, @"^[WKG0123\n\r]*$");
            }
            catch
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select a Text File"
            };

            // Show the dialog and check if a file is selected
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    // Read file content
                    string fileContent = File.ReadAllText(filePath);

                    // Display file content in a TextBox
                    textBox1.Text = fileContent;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., file access issues
                    MessageBox.Show($"Error reading file: {ex.Message}");
                }
            }
        }
    }
}
