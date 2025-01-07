using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ParagonPioneers {
    public partial class ImportMenu : Form {
        public ImportMenu() {
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
            if (IsValidInput(input))
            {
                char[,] jsonArray = JsonConvert.DeserializeObject<char[,]>(@json);

                //show map form and pass the validated array
                Map map = new Map(dataArray);
                map.Show();

                //hides import menu
                this.Hide();

                map.FormClosed += ShowImportMenu;
            }
            else
            {
                MessageBox.Show("Your data is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
