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

        // Dragging and zooming the map
        private const float ZOOM_FACTOR = 0.005f;
        private bool isDragging = false;
        private Point lastDragPoint;
        private Point dragOffset;

        private int selectedTile = 0;

        private readonly Dictionary<int, Image> tileImagesOld = new Dictionary<int, Image>()
        {
            [0] = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Water.png")),
            [1] = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land.png")),
        };

        private readonly Dictionary<int, Image[]> tileImages = new Dictionary<int, Image[]>() {
            [0] = new []
            { 
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "WaterTBLR.png")),
            },
            [1] = new[]
            {
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land____.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandT___.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land_B__.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandTB__.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land__L_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandT_L_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land_BL_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandTBL_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land___R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandT__R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land_B_R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandTB_R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land__LR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandT_LR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "Land_BLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "Images", "LandTBLR.png")),
            }
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

            mapPanel.SetMapImages(PopulateGrid());
        }

        private Image[,] PopulateGrid()
        {
            int cols = tiles.GetLength(0);
            int rows = tiles.GetLength(1);

            Image[,] mapImages = new Image[cols, rows];

            for (int col = 0; col < cols; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    //int tileId = tiles[col, row];
                    mapImages[col, row] = getDependendImage(col, row);
                }
            }
            return mapImages;
        }

        private Image getDependendImage(int x, int y) {
            int type = tiles[x, y];
            // Get the surrounding tiles' types. Border is indicated by -1.
            int typeTop = y > 0 ? tiles[x, y - 1] : -1;
            int typeBottom = y < tiles.GetLength(1) - 1 ? tiles[x, y + 1] : -1;
            int typeLeft = x > 0 ? tiles[x - 1, y] : -1;
            int typeRight = x < tiles.GetLength(0) - 1 ? tiles[x + 1, y] : -1;

            // Return image based on type and surrounding tiles
            int index = 0;
            index += typeTop == type ? 1 : 0;
            index += typeBottom == type ? 2 : 0;
            index += typeLeft == type ? 4 : 0;
            index += typeRight == type ? 8 : 0;
            return tileImages[type][index];
        }

        /// <summary>
        /// Starts dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                isDragging = true;
                lastDragPoint = e.Location;
            } else if (e.Button == MouseButtons.Left) {
                Point? gridPos = mapPanel.MouseToGrid(e.Location);
                
                if (gridPos != null)
                {
                    tiles[gridPos.Value.X, gridPos.Value.Y] = selectedTile;
                    mapPanel.SetImageAt(getDependendImage(gridPos.Value.X, gridPos.Value.Y), gridPos.Value.X, gridPos.Value.Y);

                    // Update surrounding tiles
                    if (gridPos.Value.X > 0) {
                        mapPanel.SetImageAt(getDependendImage(gridPos.Value.X - 1, gridPos.Value.Y), gridPos.Value.X - 1, gridPos.Value.Y);
                    }
                    if (gridPos.Value.X < tiles.GetLength(0) - 1) {
                        mapPanel.SetImageAt(getDependendImage(gridPos.Value.X + 1, gridPos.Value.Y), gridPos.Value.X + 1, gridPos.Value.Y);
                    }
                    if (gridPos.Value.Y > 0) {
                        mapPanel.SetImageAt(getDependendImage(gridPos.Value.X, gridPos.Value.Y - 1), gridPos.Value.X, gridPos.Value.Y - 1);
                    }
                    if (gridPos.Value.Y < tiles.GetLength(1) - 1) {
                        mapPanel.SetImageAt(getDependendImage(gridPos.Value.X, gridPos.Value.Y + 1), gridPos.Value.X, gridPos.Value.Y + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Called while the user drags the map. This function updates the map position in the MapPanel.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseMove(object sender, MouseEventArgs e) {
            if (isDragging) {
                Point currentPos = e.Location;
                dragOffset = new Point(currentPos.X - lastDragPoint.X, currentPos.Y - lastDragPoint.Y);
                mapPanel.MoveMap(dragOffset.X, dragOffset.Y);

                lastDragPoint = currentPos;
            }
        }

        /// <summary>
        /// Ends dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                isDragging = false;
            }
        }

        private void Map_MouseWheel(object sender, MouseEventArgs e) {
            // Custom behavior based on scroll direction
            if (e.Delta != 0) {
                mapPanel.Zoom(e.Delta * ZOOM_FACTOR, e.Location);
            }
        }

        private void waterButton_Click(object sender, EventArgs e)
        {
            selectedTile = 0;
        }

        private void landButton_Click(object sender, EventArgs e)
        {
            selectedTile = 1;
        }

        //protected override CreateParams CreateParams {
        //    get {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}
    }
}
