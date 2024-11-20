using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ParagonPioneers {
    public partial class Form1 : Form {
        private Dictionary<int, Image> images;

        private bool isDragging = false;
        private Point lastDragPoint;
        private Point dragOffset;

        PictureBox picture;

        public Form1() {
            InitializeComponent();

            // Register the events for drag detection
            ScrollableControl1.MouseDown += ScrollableControl1_MouseDown;
            ScrollableControl1.MouseMove += ScrollableControl1_MouseMove;
            ScrollableControl1.MouseUp += ScrollableControl1_MouseUp;

            // Enable the panel to receive mouse wheel events
            ScrollableControl1.MouseWheel += ScrollableControl1_MouseWheel;

            // Ensure the panel can gain focus
            ScrollableControl1.TabStop = true;
            ScrollableControl1.Focus();

            images = new Dictionary<int, Image>();
            images.Add(0, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy1.png")));
            images.Add(1, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy2.png")));
            images.Add(2, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy3.png")));
            images.Add(3, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy4.png")));
            images.Add(4, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy5.png")));
            images.Add(5, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\Cells\\dummy6.png")));
            //displayMap(new Map());

            picture = new PictureBox {
                Image = images[4],
                SizeMode = PictureBoxSizeMode.AutoSize, // Auto-size: The PictureBox is sized to fit the image
                Location = new Point(50, 50)
            };

            // Forward mouse events from pictures to panel
            picture.MouseDown += (s, e) => ScrollableControl1_MouseDown(ScrollableControl1, e);
            picture.MouseMove += (s, e) => ScrollableControl1_MouseMove(ScrollableControl1, e);
            picture.MouseUp += (s, e) => ScrollableControl1_MouseUp(ScrollableControl1, e);

            // Add the PictureBox to the panel
            ScrollableControl1.Controls.Add(picture);
        }

        /// <summary>
        /// The pngs from the map array are transferred to the DataGridView
        /// </summary>
        private void displayMap(Map map) {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.RowTemplate.Height = 128;
            dataGridView1.AllowUserToAddRows = false;

            // Add columns
            for (int x = 0; x < map.Cells.Count(); x++) {
                DataGridViewImageColumn column = new DataGridViewImageColumn();
                column.Width = 128;
                dataGridView1.Columns.Add(column);
            }

            // Add rows
            for (int x = 0; x < map.Cells.Count(); x++) {
                Object[] row = new Object[map.Cells.Count()];
                for (int y = 0; y < map.Cells[x].Count(); y++) {
                    row[y] = images[map.Cells[x][y].ImageId];
                }
                dataGridView1.Rows.Add(row);
            }
        }

        private void ScrollableControl1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = true;
                lastDragPoint = e.Location;
            }
        }

        private void ScrollableControl1_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                //this.Text = $"Dragging: {e.Location}";
                dragOffset = new Point(e.Location.X - lastDragPoint.X, e.Location.Y - lastDragPoint.Y);
                picture.Location = new Point(picture.Location.X + dragOffset.X, picture.Location.Y + dragOffset.Y);
                lastDragPoint = e.Location;
            }
        }

        private void ScrollableControl1_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = false;
                //dragDirection = new Point(e.Location.X - dragStart.X, e.Location.Y - dragStart.Y);
                //MessageBox.Show($"Drag direction: {dragDirection}");
            }
        }

        private void ScrollableControl1_MouseWheel(object sender, MouseEventArgs e) {
            // Custom behavior based on scroll direction
            if (e.Delta > 0) {
                MessageBox.Show("Mouse wheel scrolled up!");
            } else if (e.Delta < 0) {
                MessageBox.Show("Mouse wheel scrolled down!");
            }
        }
    }
}
