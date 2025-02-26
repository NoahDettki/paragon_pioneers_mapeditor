﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace ParagonPioneers {
    public partial class ImportMenu : Form {
        public ImportMenu() {
            InitializeComponent();
        }

        private void SubmitImportButton_Click(object sender, EventArgs e)
        {
            string input = textBox.Text;

            // Make sure that the input is not empty.
            if (input.Equals("")) {
                MessageBox.Show("Import field is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate the input. Only specific characters are allowed.
            if (!IsValidInput(input)) {
                MessageBox.Show("Your data is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Make sure that the input is rectangular. Every line must have the same length.
            string[] lines = input.Split('\n');
            int firstLineLength = lines[0].TrimEnd('\r').Length; // Windows is weird. Trim '\r' from the first line

            foreach (string l in lines) {
                int lineLength = l.TrimEnd('\r').Length; // Windows is still weird. Trim '\r' from each line
                if (lineLength != firstLineLength) {
                    MessageBox.Show("Every line must have the same number of characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
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

        private void FileImportButton_Click(object sender, EventArgs e)
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
                    textBox.Text = fileContent;
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., file access issues
                    MessageBox.Show($"Error reading file: {ex.Message}");
                }
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
    }
}
