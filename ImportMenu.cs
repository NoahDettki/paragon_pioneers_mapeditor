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
            //removing whitespaces and line breaks
            string json = Regex.Replace(textBox1.Text, @"\s+", "");

            if (IsValidJson(json))
            {
                int[,] jsonArray = JsonConvert.DeserializeObject<int[,]>(@json);

                //show map form and pass the validated array
                Map map = new Map(jsonArray);
                map.Show();

                //hides import menu
                this.Hide();
            }
            else
            {
                MessageBox.Show("Your JSON is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidJson(string json)
        {
            try
            {
                //tries to create a int[,] from json. If it fails it returns false
                int[,] jsonArray = JsonConvert.DeserializeObject<int[,]>(json);
                //returns if the json string only contains [ ] 0 1 ,
                return Regex.IsMatch(json, @"^[\[\], 01]*$"); ;
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
