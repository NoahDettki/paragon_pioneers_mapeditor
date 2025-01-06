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
        private Tile[,] tileGrid;
        private const int SPRITE_SIZE = 128;

        // Dragging and zooming the map
        private const float ZOOM_FACTOR = 0.0005f;
        private bool isDragging = false;
        private Point lastDragPoint;
        private Point dragOffset;

        private int selectedTile = 0;

        private Image mapErrorImage = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "MapError.jpg"));
        private Image tileSpritesheet = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Background_Tiles.png"));

        // This dictionary contains the coordinates that a specific sprite has on the sprite sheet. It is sorted by the tile type.
        // A point of (-1, -1) indicates that the sprite is not available.
        private readonly Dictionary<Tile.Type, Point[]> spritesheetCoordinates = new Dictionary<Tile.Type, Point[]>() {
            // Water sprites
            [Tile.Type.Water] = new[] {
                new Point(1, 6),    // ____
                new Point(1, 5),    // T___
                new Point(1, 7),    // _B__
                new Point(-1, -1),  // TB__
                new Point(0, 6),    // __L_
                new Point(0, 5),    // T_L_
                new Point(0, 7),    // _BL_
                new Point(-1, -1),  // TBL_
                new Point(2, 6),    // ___R
                new Point(2, 5),    // T__R
                new Point(2, 7),    // _B_R
                new Point(-1, -1),  // TB_R
                new Point(-1, -1),  // __LR
                new Point(-1, -1),  // T_LR
                new Point(-1, -1),  // _BLR
                new Point(-1, -1),  // TBLR
                new Point(5, 7),    // Diagonal top left
                new Point(4, 7),    // Diagonal top right
                new Point(5, 6),    // Diagonal bottom left
                new Point(4, 6),    // Diagonal bottom right
            },
            // Coast sprites
            [Tile.Type.Coast] = new[] {
                new Point(1, 3),    // ____
                new Point(1, 2),    // T___
                new Point(1, 4),    // _B__
                new Point(-1, -1),  // TB__
                new Point(0, 3),    // __L_
                new Point(0, 2),    // T_L_
                new Point(0, 4),    // _BL_
                new Point(-1, -1),  // TBL_
                new Point(2, 3),    // ___R
                new Point(2, 2),    // T__R
                new Point(2, 4),    // _B_R
                new Point(-1, -1),  // TB_R
                new Point(-1, -1),  // __LR
                new Point(-1, -1),  // T_LR
                new Point(-1, -1),  // _BLR
                new Point(-1, -1),  // TBLR
                new Point(4, 3),    // Diagonal top left
                new Point(3, 3),    // Diagonal top right
                new Point(4, 2),    // Diagonal bottom left
                new Point(3, 2),    // Diagonal bottom right
            },
            // Land sprites
            [Tile.Type.Land] = new[] {
                new Point(1, 1),
            },

        };

        public Map(int[,] tiles) {
            this.tiles = tiles;
            tileGrid = new Tile[tiles.GetLength(0), tiles.GetLength(1)];

            InitializeComponent();
            Init();
        }

        private void Init() {
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

            PopulateGrid();
            mapPanel.Initialize(tileSpritesheet, SPRITE_SIZE, tileGrid, mapErrorImage);
        }

        private void PopulateGrid() {
            int cols = tiles.GetLength(0);
            int rows = tiles.GetLength(1);

            for (int col = 0; col < cols; col++) {
                for (int row = 0; row < rows; row++) {
                    tileGrid[col, row] = new Tile(tiles[col, row] == 0 ? Tile.Type.Water : Tile.Type.Land);
                }
            }
            for (int col = 0; col < cols; col++) {
                for (int row = 0; row < rows; row++) {
                    CalculateImageCoordinate(col, row);
                }
            }
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
        private bool IsTypeAt(int x, int y, Tile.Type type) {
            // Check for out of bounds
            if (!IsInbounds(x, y)) return true;

            // Compare type
            return tileGrid[x, y].GetTileType() == type;
        }

        private void CalculateImageCoordinate(int x, int y) {
            // Check for out of bounds
            if (!IsInbounds(x, y)) return;

            Tile.Type type = tileGrid[x, y].GetTileType();

            // At the moment there is only one available land sprite
            if (type != Tile.Type.Water) {
                tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][0]);
                return;
            }

            // Handling water tiles
            if (type == Tile.Type.Water) {
                // Are the neighbouring tiles land tiles?
                bool top = IsInbounds(x, y - 1) && tileGrid[x, y - 1].GetTileType() != Tile.Type.Water;
                bool bottom = IsInbounds(x, y + 1) && tileGrid[x, y + 1].GetTileType() != Tile.Type.Water;
                bool left = IsInbounds(x - 1, y) && tileGrid[x - 1, y].GetTileType() != Tile.Type.Water;
                bool right = IsInbounds(x + 1, y) && tileGrid[x + 1, y].GetTileType() != Tile.Type.Water;

                bool topLeft = IsInbounds(x - 1, y - 1) && tileGrid[x - 1, y - 1].GetTileType() != Tile.Type.Water;
                bool topRight = IsInbounds(x + 1, y - 1) && tileGrid[x + 1, y - 1].GetTileType() != Tile.Type.Water;
                bool bottomLeft = IsInbounds(x - 1, y + 1) && tileGrid[x - 1, y + 1].GetTileType() != Tile.Type.Water;
                bool bottomRight = IsInbounds(x + 1, y + 1) && tileGrid[x + 1, y + 1].GetTileType() != Tile.Type.Water;

                // Return water sprite coordinate based on neighbouring tiles
                int index = 0;
                index += top ? 1 : 0;
                index += bottom ? 2 : 0;
                index += left ? 4 : 0;
                index += right ? 8 : 0;

                // There are four extra sprites for water tiles with diagonal neihbouring land tiles
                if (index == 0) {
                    Console.WriteLine("Water tile");
                    if (topLeft) {
                        tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][16]);
                        tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][16]);
                    } else if (topRight) {
                        tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][17]);
                        tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][17]);
                    } else if (bottomLeft) {
                        tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][18]);
                        tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][18]);
                    } else if (bottomRight) {
                        tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][19]);
                        tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][19]);
                    } else {
                        tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][0]);
                        tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][0]);
                    }
                } else {
                    tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][index]);
                    tileGrid[x, y].SetBackgroundCoordinate(spritesheetCoordinates[Tile.Type.Coast][index]);
                }
            }
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

                    tiles[x, y] = selectedTile;
                    tileGrid[x, y].SetTileType(selectedTile == 0 ? Tile.Type.Water : Tile.Type.Land);
                    CalculateImageCoordinate(x, y);

                    // Update surrounding tiles
                    // Orthogonal neighbours
                    CalculateImageCoordinate(x - 1, y);
                    CalculateImageCoordinate(x + 1, y);
                    CalculateImageCoordinate(x, y - 1);
                    CalculateImageCoordinate(x, y + 1);
                    // Diagonal neighbours
                    CalculateImageCoordinate(x - 1, y - 1);
                    CalculateImageCoordinate(x - 1, y + 1);
                    CalculateImageCoordinate(x + 1, y - 1);
                    CalculateImageCoordinate(x + 1, y + 1);

                    // The panel has to be drawn again to show the changes
                    mapPanel.Invalidate();
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
