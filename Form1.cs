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

            ScrollableControl1.AutoScroll = false;
            ScrollableControl1.HorizontalScroll.Maximum = 0;
            ScrollableControl1.VerticalScroll.Maximum = 0;
            ScrollableControl1.AutoScroll = true;

            // Register the events for drag detection
            ScrollableControl1.MouseDown += Map_MouseDown;
            ScrollableControl1.MouseMove += Map_MouseMove;
            ScrollableControl1.MouseUp += Map_MouseUp;

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

            picture = new PictureBox {
                Image = images[4],
                SizeMode = PictureBoxSizeMode.AutoSize, // Auto-size: The PictureBox is sized to fit the image
                Location = new Point(50, 50)
            };

            picture.MouseDown += Map_MouseDown;
            picture.MouseMove += Map_MouseMove;
            picture.MouseUp += Map_MouseUp;

            // Add the PictureBox to the panel
            ScrollableControl1.Controls.Add(picture);
        }


        //// Add columns
        //for (int x = 0; x < map.Cells.Count(); x++) {
        //    DataGridViewImageColumn column = new DataGridViewImageColumn();
        //    column.Width = 128;
        //    dataGridView1.Columns.Add(column);
        //}

        //// Add rows
        //for (int x = 0; x < map.Cells.Count(); x++) {
        //    Object[] row = new Object[map.Cells.Count()];
        //    for (int y = 0; y < map.Cells[x].Count(); y++) {
        //        row[y] = images[map.Cells[x][y].ImageId];
        //    }
        //    dataGridView1.Rows.Add(row);
        //}


        /// <summary>
        /// If the user clicks a PictureBox e will only contain the cursor position relative to itself,
        /// while clicking the Panel results in the cursor position relative to the panel.
        /// This funciton brings both points to screen positions so that they function the same.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        /// <returns>A Point relative to the client coordinate system.</returns>
        private Point GetCursorPosition(object sender, MouseEventArgs e) {
            if (sender is PictureBox) {
                return ((PictureBox)sender).Parent.PointToClient(Cursor.Position);
            } else if (sender is Panel) {
                return ((Panel)sender).PointToClient(Cursor.Position);
            }
            return new Point(); ;
        }

        /// <summary>
        /// Starts dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = true;
                lastDragPoint = GetCursorPosition(sender, e);
            }
        }

        /// <summary>
        /// Called while the user drags the map. This function places the map cells to their new positions.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                Point currentPos = GetCursorPosition(sender, e);
                dragOffset = new Point(currentPos.X - lastDragPoint.X, currentPos.Y - lastDragPoint.Y);
                picture.Location = new Point(picture.Location.X + dragOffset.X, picture.Location.Y + dragOffset.Y);
                lastDragPoint = GetCursorPosition(sender, e);
            }
        }

        /// <summary>
        /// Ends dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                isDragging = false;
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
