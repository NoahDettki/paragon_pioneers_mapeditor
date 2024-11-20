using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ParagonPioneers
{
    public partial class Map : Form
    {
        private int[,] tiles;
        private const int IMAGE_SIZE = 16;

        // Map tile pictures
        PictureBox[,] mapPictures;

        // Dragging and zooming the map
        private bool isDragging = false;
        private Point lastDragPoint;
        private Point dragOffset;
        private float zoom = 3;
        private float currentTileSize;

        private readonly Dictionary<int, Image> tileImages = new Dictionary<int, Image>()
        {
            [0] = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Water.png")),
            [1] = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land.png")),
        };

        public Map(int[,] tiles)
        {
            this.tiles = tiles;

            InitializeComponent();

            Init();
        }

        private void Init()
        {
            // Window header
            this.Text = "Tile Grid";

            // -- Panel settings (mapPanel) ----------------------

            // Removing the ScrollBars
            mapPanel.AutoScroll = false;
            mapPanel.HorizontalScroll.Maximum = 0;
            mapPanel.VerticalScroll.Maximum = 0;
            mapPanel.AutoScroll = true;

            // Register the events for drag detection
            mapPanel.MouseDown += Map_MouseDown;
            mapPanel.MouseMove += Map_MouseMove;
            mapPanel.MouseUp += Map_MouseUp;

            // Enable the panel to receive mouse wheel events
            mapPanel.MouseWheel += Map_MouseWheel;

            // Ensure the panel can gain focus
            mapPanel.TabStop = true;
            mapPanel.Focus();

            // -- Panel settings end -----------------------------

            currentTileSize = IMAGE_SIZE * zoom;
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);

            mapPictures = new PictureBox[rows, cols];

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    int tileId = tiles[row, col];
                    mapPictures[col, row] = new PictureBox {
                        Image = tileImages[tileId],
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size((int)currentTileSize, (int)currentTileSize),
                        Location = new Point((int)(col * currentTileSize), (int)(row * currentTileSize)),
                    };

                    mapPictures[col, row].MouseDown += Map_MouseDown;
                    mapPictures[col, row].MouseMove += Map_MouseMove;
                    mapPictures[col, row].MouseUp += Map_MouseUp;

                    // Add the PictureBox to the panel
                    mapPanel.Controls.Add(mapPictures[col, row]);
                }
            }
        }

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
                return ((PictureBox)sender).Parent.PointToScreen(Cursor.Position);
            } else if (sender is Panel) {
                return ((Panel)sender).PointToScreen(Cursor.Position);
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

                // Move all PictureBoxes of the map
                for (int col = 0; col < mapPictures.GetLength(0); col++) {
                    for (int row = 0; row < mapPictures.GetLength(1); row++) {
                        mapPictures[row, col].Location = new Point(mapPictures[row, col].Location.X + dragOffset.X, mapPictures[row, col].Location.Y + dragOffset.Y);
                    }
                }

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

        private void Map_MouseWheel(object sender, MouseEventArgs e) {
            // Custom behavior based on scroll direction
            if (e.Delta > 0) {
                MessageBox.Show("Mouse wheel scrolled up!");
            } else if (e.Delta < 0) {
                MessageBox.Show("Mouse wheel scrolled down!");
            }
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
