using Newtonsoft.Json;
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

        private Image missingImage = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Missing.png"));

        private readonly Dictionary<int, Image[]> tileImages = new Dictionary<int, Image[]>() {
            [0] = new []
            {
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water____.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterT___.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water_B__.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterTB__.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water__L_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterT_L_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water_BL_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterTBL_.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water___R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterT__R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water_B_R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterTB_R.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water__LR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterT_LR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water_BLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "WaterTBLR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water-TL.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water-TR.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water-BL.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Water-BR.png")),



            },
            [1] = new []
            {
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "LandTBLR.png")),
            },
            [2] = new []
            {
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Tree.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Tree2.png")),
                Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Tree3.png"))
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
                    mapImages[col, row] = GetDependendImage(col, row);
                }
            }
            return mapImages;
        }

        /// <summary>
        /// Checks wether the specified coordinate is in the bounds of the tile map.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>true if inbound, else false</returns>
        private bool IsInbounds(int x, int y) {
            if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1)) {
                return false;
            } else return true;
        }

        /// <summary>
        /// Checks wether the type of the tile at the specified coordinate is the same type as the specified type
        /// Out of bounds coordinates return true
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="type">the tile type to compare with</param>
        /// <returns></returns>
        private bool IsTypeAt(int x, int y, int type) {
            // Check for out of bounds
            if (!IsInbounds(x, y)) return true;

            // Compare type
            return tiles[x, y] == type;
        }

        private Image GetDependendImage(int x, int y) {
            if (!IsInbounds(x, y)) return missingImage;

            int type = tiles[x, y];

            if (type != 0)
            {
                return tileImages[type][0];
            }

            // Handling water tiles
            if (type == 0) {
                // Are the neighbouring tiles land tiles?
                bool top = IsInbounds(x, y - 1) && tiles[x, y - 1] != 0;
                bool bottom = IsInbounds(x, y + 1) && tiles[x, y + 1] != 0;
                bool left = IsInbounds(x - 1, y) && tiles[x - 1, y] != 0;
                bool right = IsInbounds(x + 1, y) && tiles[x + 1, y] != 0;

                bool topLeft = IsInbounds(x - 1, y - 1) && tiles[x - 1, y - 1] != 0;
                bool topRight = IsInbounds(x + 1, y - 1) && tiles[x + 1, y - 1] != 0;
                bool bottomLeft = IsInbounds(x - 1, y + 1) && tiles[x - 1, y + 1] != 0;
                bool bottomRight = IsInbounds(x + 1, y + 1) && tiles[x + 1, y + 1] != 0;

                // Return water image based on neighbouring tiles
                int index = 0;
                index += top ? 1 : 0;
                index += bottom ? 2 : 0;
                index += left ? 4 : 0;
                index += right ? 8 : 0;
                
                if (index == 0) {
                    if (topLeft) return tileImages[type][16];
                    if (topRight) return tileImages[type][17];
                    if (bottomLeft) return tileImages[type][18];
                    if (bottomRight) return tileImages[type][19];
                }

                return tileImages[type][index];
            }

            //// Get the surrounding tiles' types. Border is indicated by -1.
            //int typeTop = y > 0 ? tiles[x, y - 1] : -1;
            //int typeBottom = y < tiles.GetLength(1) - 1 ? tiles[x, y + 1] : -1;
            //int typeLeft = x > 0 ? tiles[x - 1, y] : -1;
            //int typeRight = x < tiles.GetLength(0) - 1 ? tiles[x + 1, y] : -1;

            //// Return image based on type and surrounding tiles
            //int index = 0;
            //index += typeTop == type ? 1 : 0;
            //index += typeBottom == type ? 2 : 0;
            //index += typeLeft == type ? 4 : 0;
            //index += typeRight == type ? 8 : 0;
            return tileImages[type][0];
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
                    int x = gridPos.Value.X;
                    int y = gridPos.Value.Y;

                    if (selectedTile == 0) {
                        // Rules for water placement
                        if (!IsTypeAt(x + 1, y, 0) && !IsTypeAt(x - 1, y, 0)) return;
                        if (!IsTypeAt(x, y + 1, 0) && !IsTypeAt(x, y - 1, 0)) return;
                    }

                    tiles[gridPos.Value.X, gridPos.Value.Y] = selectedTile;
                    mapPanel.SetImageAt(GetDependendImage(x, y), x, y);

                    // Update surrounding tiles
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X - 1, gridPos.Value.Y), gridPos.Value.X - 1, gridPos.Value.Y);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X + 1, gridPos.Value.Y), gridPos.Value.X + 1, gridPos.Value.Y);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X, gridPos.Value.Y - 1), gridPos.Value.X, gridPos.Value.Y - 1);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X, gridPos.Value.Y + 1), gridPos.Value.X, gridPos.Value.Y + 1);

                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X - 1, gridPos.Value.Y - 1), gridPos.Value.X - 1, gridPos.Value.Y - 1);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X - 1, gridPos.Value.Y + 1), gridPos.Value.X - 1, gridPos.Value.Y + 1);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X + 1, gridPos.Value.Y - 1), gridPos.Value.X + 1, gridPos.Value.Y - 1);
                    mapPanel.SetImageAt(GetDependendImage(gridPos.Value.X + 1, gridPos.Value.Y + 1), gridPos.Value.X + 1, gridPos.Value.Y + 1);
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

        private void treeButton_Click(object sender, EventArgs e)
        {
            selectedTile = 2;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Configure the dialog
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Title = "Save File As";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file path
                    string filePath = saveFileDialog.FileName;

                    try
                    {
                        //formatting for visibility
                        string formatedTiles = JsonConvert.SerializeObject(tiles)
                            .Replace("],", "]," + Environment.NewLine)
                            .Replace("[[", "[" + Environment.NewLine + "[")
                            .Replace("]]", "]" + Environment.NewLine + "]");

                        // Write the content to the file
                        File.WriteAllText(filePath, formatedTiles);

                        // Notify the user
                        DialogResult result = MessageBox.Show($"File successfully saved to:\n{filePath}", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (result == DialogResult.OK)
                        {
                            Application.Exit();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle errors
                        MessageBox.Show($"Error saving file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
