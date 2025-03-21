﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ParagonPioneers
{
    public partial class Map : Form
    {
        private char[,] tiles;
        private Tile[,] tileGrid;
        private const int SPRITE_SIZE = 128;

        // Dragging and zooming the map
        private const float ZOOM_FACTOR = 0.0005f;
        private bool isDragging = false;
        private bool isPainting = false;
        private Point lastDragPoint;
        private Point? lastPaintedTile;
        private Point dragOffset;

        // Mountain placement
        private bool mountainMode = false;
        private List<Point> mountainRange;
        private List<List<Point>> allMountainRanges;
        private List<Point> allMountainTiles; // only needed for loading

        private char selectedTile = ' ';
        private bool showMountainTutorial = true;

        private Image mapErrorImage = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "MapError.png"));
        private Image tileSpritesheet = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "Background_Tiles.png"));
        private Image treeSpritesheet = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "World_Environment.png"));
        private Image toggleGridOn = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "ToggleGridOn.png"));
        private Image toggleGridOff = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "ToggleGridOff.png"));
        private Image mountainTutorialImage = Image.FromFile(Path.Combine(Application.StartupPath, "../../Images", "MountainTutorial.png"));

        // This dictionary contains the coordinates that a specific sprite has on the sprite sheet. It is sorted by the tile type.
        // A point of (-1, -1) indicates that the sprite is not available.
        private readonly Dictionary<Tile.Type, Point[]> spritesheetCoordinates = new Dictionary<Tile.Type, Point[]>()
        {
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
                new Point(-1, -1),    // ____
                new Point(-1, -1),    // T___
                new Point(-1, -1),    // _B__
                new Point(0, 3),  // TB__
                new Point(-1, -1),    // __L_
                new Point(2, 4),    // T_L_
                new Point(2, 2),    // _BL_
                new Point(-1, -1),  // TBL_
                new Point(-1, -1),    // ___R
                new Point(0, 4),    // T__R
                new Point(0, 2),    // _B_R
                new Point(-1, -1),  // TB_R
                new Point(1, 2),  // __LR
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

        private const string MOUNTAIN_TUTORIAL_MESSAGE = @"How to add mountains: 
First draw a complete ring. You can then decide if the ring should form a mountain or a valley.";

        public Map(char[,] tiles)
        {
            this.tiles = tiles;
            tileGrid = new Tile[tiles.GetLength(0), tiles.GetLength(1)];

            InitializeComponent();
            Init();
        }

        private void Init()
        {
            // Window header
            this.Text = "Tile Grid";

            InitToolTips();

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

            allMountainRanges = new List<List<Point>>();
            PopulateGrid();
            mapPanel.Initialize(tileSpritesheet, treeSpritesheet, SPRITE_SIZE, tileGrid, mapErrorImage);
        }

        private void PopulateGrid() 
        {
            int cols = tiles.GetLength(0);
            int rows = tiles.GetLength(1);

            // Set the correct Tile at each position
            for (int y = 0; y < rows; y++) 
            {                
                for (int x = 0; x < cols; x++) 
                {
                    tileGrid[x, y] = new Tile(tiles[x, y]);
                }
            }
            // Calculate the correct sprite at each position
            // (All tiles have to be set before this can happen)
            for (int y = 0; y < rows; y++) 
            {
                for (int x = 0; x < cols; x++)
                {
                    CalculateImageCoordinate(x, y);
                }
            }
            // Calculate all mountain ranges
            allMountainTiles = new List<Point>();
            for (int y = 0; y < rows; y++) 
            {
                for (int x = 0; x < cols; x++) 
                {
                    if (tileGrid[x, y].GetTileType() == Tile.Type.Mountain)
                    {
                        // Handle all mountain tiles only once
                        if (allMountainTiles.Contains(new Point(x, y))) {
                            continue;
                        }
                        // This tile was not handled yet which means that this is the first of
                        // a new mountain range
                        StartMountainMode();
                        FindNextMountainTile(x, y);
                    }
                }
            }
            Console.WriteLine(tileGrid[1, 2].GetTileType()); // TODO: Remove this line
            Console.WriteLine(tiles[1, 2]); // TODO: Remove this line
        }

        private void FindNextMountainTile(int x, int y) 
        {
            if (!IsInbounds(x, y)) return;
            if (tileGrid[x, y].GetTileType() != Tile.Type.Mountain) return;

            bool wasAdded = TryAddToMountainRange(x, y);
            if (!wasAdded) return;
            allMountainTiles.Add(new Point(x, y));

            // If the last mountain completed the range then the mountain mode will be set to false
            if (!mountainMode) return;

            // Only look at neighbours if the current tile was added
            FindNextMountainTile(x - 1, y);
            FindNextMountainTile(x, y - 1);
            FindNextMountainTile(x + 1, y);
            FindNextMountainTile(x, y + 1);
        }


        /// <summary>
        /// Checks wether the specified coordinate is in the bounds of the tile map.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>true if inbound, else false</returns>
        private bool IsInbounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            {
                return false;
            }
            else return true;
        }


        /// <summary>
        /// Checks wether the type of the tile at the specified coordinate is the same type as the specified type
        /// Out of bounds coordinates return true
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="type">the tile type to compare with</param>
        /// <returns></returns>
        private bool IsTypeAt(int x, int y, Tile.Type type)
        {
            // Check for out of bounds
            if (!IsInbounds(x, y)) return true;

            // Compare type
            return tileGrid[x, y].GetTileType() == type;
        }


        /// <summary>
        /// Calculates the image coordinate for the tile at the specified position. Neighbouring tiles are considered.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        private void CalculateImageCoordinate(int x, int y)
        {
            // Check for out of bounds
            if (!IsInbounds(x, y)) return;

            Tile.Type type = tileGrid[x, y].GetTileType();

            // At the moment there is only one available land sprite
            if (type == Tile.Type.Land)
            {
                tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][0]);
                return;
            }

            // Handling water tiles
            if (type == Tile.Type.Water)
            {
                tileGrid[x, y].SetSpritesheetCoordinate(spritesheetCoordinates[type][0]);
            }

            if (type == Tile.Type.Coast)
            {
                Point point = new Point(x, y);

                int coastDirection = GetNeighboursOfType(Tile.Type.Coast, point);
                int waterNeighbour = GetNeighboursOfType(Tile.Type.Water, point, true);
                int landNeighbour = GetNeighboursOfType(new[] { Tile.Type.Land, Tile.Type.Mountain }, point);
                int mountainNeighbour = GetNeighboursOfType(Tile.Type.Mountain, point);

                if (HasOneNeighbour(coastDirection) || coastDirection == 0)
                {
                    tileGrid[x, y].SetSpritesheetCoordinate(new Point(-1, -1));
                    tileGrid[x, y].SetBackgroundCoordinate(new Point(1, 2));
                    return;
                }

                if ((IsOposite(waterNeighbour, landNeighbour) || IsOposite(waterNeighbour, landNeighbour)) && (coastDirection == 3 || coastDirection == 12))
                {
                    tileGrid[x, y].SetSpritesheetCoordinate(GetStraightCoastPoint(waterNeighbour, true));
                    tileGrid[x, y].SetBackgroundCoordinate(GetStraightCoastPoint(waterNeighbour));
                }
                else if (waterNeighbour == 0)
                {
                    tileGrid[x, y].SetSpritesheetCoordinate(GetOuterCornerCoastPoint(coastDirection, true));
                    tileGrid[x, y].SetBackgroundCoordinate(GetOuterCornerCoastPoint(coastDirection));
                }
                else if (landNeighbour == 0 && mountainNeighbour == 0)
                {
                    tileGrid[x, y].SetSpritesheetCoordinate(GetInnerCornerCoastPoint(coastDirection, true));
                    tileGrid[x, y].SetBackgroundCoordinate(GetInnerCornerCoastPoint(coastDirection));
                }
            }

            if (type == Tile.Type.Mountain)
            {
                // Mountain sprites are only handled in the mountain mode
                return;
            }
        }

        private bool HasOneNeighbour(int direction)
        {
            return direction == 1 || direction == 2 || direction == 4 || direction == 8;
        }

        private Point GetInnerCornerCoastPoint(int coast, bool useWaterSprite = false)
        {
            if (!useWaterSprite)
            {
                if (coast == 5)
                    return new Point(4, 3);
                if (coast == 6)
                    return new Point(4, 2);
                if (coast == 9)
                    return new Point(3, 3);
                if (coast == 10)
                    return new Point(3, 2);
                return new Point(1, 2);
            } 
            else
            {
                if (coast == 5)
                    return new Point(5, 7);
                if (coast == 6)
                    return new Point(5, 6);
                if (coast == 9)
                    return new Point(4, 7);
                if (coast == 10)
                    return new Point(4, 6);
                return new Point(-1, -1);
            }
        }

        private Point GetOuterCornerCoastPoint(int coast, bool useWaterSprite = false)
        {
            if (!useWaterSprite)
            {
                if (coast == 5)
                    return new Point(2, 4);
                if (coast == 6)
                    return new Point(2, 2);
                if (coast == 9)
                    return new Point(0, 4);
                if (coast == 10)
                    return new Point(0, 2);
                return new Point(1, 2);
            }
            else
            {
                if (coast == 5)
                    return new Point(2, 7);
                if (coast == 6)
                    return new Point(2, 5);
                if (coast == 9)
                    return new Point(0, 7);
                if (coast == 10)
                    return new Point(0, 5);
                return new Point(-1, -1);
            }
        }

        private bool IsOposite(int a, int b)
        {
            return (a == 1 && b == 2) || (a == 2 && b == 1) || (a == 4 && b == 8) || (a == 8 && b == 4);
        }

        private Point GetStraightCoastPoint(int water, bool useWaterSprite = false)
        {
            if (!useWaterSprite)
            {
                if (water == 1)
                    return new Point(1, 4);
                if (water == 2)
                    return new Point(1, 2);
                if (water == 4)
                    return new Point(2, 3);
                if (water == 8)
                    return new Point(0, 3);
                return new Point(1, 2);
            } 
            else
            {
                if (water == 1)
                    return new Point(1, 7);
                if (water == 2)
                    return new Point(1, 5);
                if (water == 4)
                    return new Point(2, 6);
                if (water == 8)
                    return new Point(0, 6);
                return new Point(-1, -1);
            }
        }

        private int GetNeighboursOfType(Tile.Type type, Point pos, bool countNoneAsTile = false)
        {
            bool top, right, bottom, left;

            if (IsInbounds(pos.X, pos.Y - 1))
            {
                top = tileGrid[pos.X, pos.Y - 1].GetTileType() == type;
            }
            else
            {
                top = countNoneAsTile;
            }

            if (IsInbounds(pos.X + 1, pos.Y))
            {
                right = tileGrid[pos.X + 1, pos.Y].GetTileType() == type;
            }
            else
            {
                right = countNoneAsTile;
            }

            if (IsInbounds(pos.X, pos.Y + 1))
            {
                bottom = tileGrid[pos.X, pos.Y + 1].GetTileType() == type;
            }
            else
            {
                bottom = countNoneAsTile;
            }

            if (IsInbounds(pos.X - 1, pos.Y))
            {
                left = tileGrid[pos.X - 1, pos.Y].GetTileType() == type;
            }
            else
            {
                left = countNoneAsTile;
            }

            int index = 0;
            index += top ? 1 : 0;
            index += bottom ? 2 : 0;
            index += left ? 4 : 0;
            index += right ? 8 : 0;

            return index;
        }

        private int GetNeighboursOfType(Tile.Type[] types, Point pos, bool countNoneAsTile = false)
        {
            bool top, right, bottom, left;
            top = right = bottom = left = false;

            if (IsInbounds(pos.X, pos.Y - 1))
            {
                foreach (Tile.Type type in types)
                {
                    if (top)
                        break;

                    top = tileGrid[pos.X, pos.Y - 1].GetTileType() == type;
                }
            }
            else
            {
                top = countNoneAsTile;
            }

            if (IsInbounds(pos.X + 1, pos.Y))
            {
                foreach (Tile.Type type in types)
                {
                    if (right)
                        break;

                    right = tileGrid[pos.X + 1, pos.Y].GetTileType() == type;
                }
            }
            else
            {
                right = countNoneAsTile;
            }

            if (IsInbounds(pos.X, pos.Y + 1))
            {
                foreach (Tile.Type type in types)
                {
                    if (bottom)
                        break;

                    bottom = tileGrid[pos.X, pos.Y + 1].GetTileType() == type;
                }
            }
            else
            {
                bottom = countNoneAsTile;
            }

            if (IsInbounds(pos.X - 1, pos.Y))
            {
                foreach (Tile.Type type in types)
                {
                    if (left)
                        break;

                    left = tileGrid[pos.X - 1, pos.Y].GetTileType() == type;
                }
            }
            else
            {
                left = countNoneAsTile;
            }

            int index = 0;
            index += top ? 1 : 0;
            index += bottom ? 2 : 0;
            index += left ? 4 : 0;
            index += right ? 8 : 0;

            return index;
        }

        /// <summary>
        /// The tile at the given mouse position is set to the currently selected tile.
        /// </summary>
        /// <param name="mouseLocation"></param>
        private void SetTileAt(Point mouseLocation)
        {
            // Return if no tile is selected
            if (selectedTile == ' ')
            {
                return;
            }

            // The mouse position has to be transformed into a grid position
            Point? gridPos = mapPanel.MouseToGrid(mouseLocation);

            if (gridPos != null)
            {
                // Prevent the continuous painting from painting the same tile over and over
                if (lastPaintedTile == gridPos)
                {
                    return;
                }
                lastPaintedTile = (Point)gridPos;

                int x = gridPos.Value.X;
                int y = gridPos.Value.Y;

                // Trees work a little different than the other tiles
                if (selectedTile == '1')
                {
                    tileGrid[x, y].IncreaseTrees();
                    tiles[x, y] = tileGrid[x, y].GetTreeCount().ToString()[0];
                }
                else if (mountainMode)
                {
                    TryAddToMountainRange(x, y);
                    mapPanel.Invalidate();
                    return;
                }
                else
                {
                    if (tileGrid[x, y].IsTileType(Tile.Type.Mountain))
                    {
                        RemoveMountainTile(new Point(x, y));
                    }
                    tiles[x, y] = selectedTile;
                    tileGrid[x, y].SetTileType(Tile.CharToType(selectedTile));
                }
                CalculateImageCoordinate(x, y);

                // Update surrounding tiles
                // Orthogonal neighbours
                CalculateImageCoordinate(x - 1, y);
                CalculateImageCoordinate(x + 1, y);
                CalculateImageCoordinate(x, y - 1);
                CalculateImageCoordinate(x, y + 1);

                lastPaintedTile = gridPos;

                // The panel has to be drawn again to show the changes
                mapPanel.Invalidate();
            }
        }

        /// <summary>
        /// Starts dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDragging = true;
                lastDragPoint = e.Location;
            }
            else if (e.Button == MouseButtons.Left)
            {
                isPainting = true;
                SetTileAt(e.Location);
            }
        }

        /// <summary>
        /// Called while the user drags the map. This function updates the map position in the MapPanel.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = e.Location;
                dragOffset = new Point(currentPos.X - lastDragPoint.X, currentPos.Y - lastDragPoint.Y);
                mapPanel.MoveMap(dragOffset.X, dragOffset.Y);

                lastDragPoint = currentPos;
            }

            if (isPainting && lastPaintedTile != mapPanel.MouseToGrid(e.Location))
            {
                SetTileAt(e.Location);
            }
        }

        /// <summary>
        /// Ends dragging the map.
        /// </summary>
        /// <param name="sender">Either the map panel or one of the maps cells</param>
        /// <param name="e">The MouseEventArgs</param>
        private void Map_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isDragging = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                isPainting = false;
                lastPaintedTile = new Point(-1, -1);
            }
        }

        private void Map_MouseWheel(object sender, MouseEventArgs e)
        {
            // Custom behavior based on scroll direction
            if (e.Delta != 0)
            {
                mapPanel.Zoom(e.Delta * ZOOM_FACTOR, e.Location);
            }
        }

        private void waterButton_Click(object sender, EventArgs e)
        {
            EndMountainMode(true);
            selectedTile = 'W';
        }

        private void landButton_Click(object sender, EventArgs e)
        {
            EndMountainMode(true);
            selectedTile = '0';
        }

        private void treeButton_Click(object sender, EventArgs e)
        {
            EndMountainMode(true);
            selectedTile = '1';
        }

        private void coastButton_Click(object sender, EventArgs e)
        {
            EndMountainMode(true);
            selectedTile = 'K';
        }

        private void mountainButton_Click(object sender, EventArgs e)
        {
            StartMountainMode();
            // Show the mountain tutorial if it is the first time
            if (showMountainTutorial)
            {
                bool doNotShowAgain = MessageBoxWithCheckbox.Show(MOUNTAIN_TUTORIAL_MESSAGE, "Tutorial", mountainTutorialImage);
                if (doNotShowAgain)
                {
                    showMountainTutorial = false;
                }
            }

        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            // Before saving first check if the map is valid. Tell the user if it is not valid
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (tileGrid[x, y].GetSpritesheetCoordinate().X == -1)
                    {
                        // This tile is not valid
                        String message = "The map is currently not valid. Please correct the map layout before exporting.";
                        MessageBox.Show(message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

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
                        string stringTiles = CharArrayToString();

                        // Write the content to the file
                        File.WriteAllText(filePath, stringTiles);

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

        /// <summary>
        /// This method is used to transform the char array back into a single string to save it in a txt file.
        /// </summary>
        /// <returns>a single string containing all map info</returns>
        public string CharArrayToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    sb.Append(tiles[x, y]);
                }
                if (y < tiles.GetLength(0) - 1) // Add newline after each row, except the last one
                {
                    sb.Append(Environment.NewLine);
                }
            }

            // Convert StringBuilder to string
            return sb.ToString();
        }

        private void InitToolTips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(waterButton, "Water");
            toolTip.SetToolTip(landButton, "Land");
            toolTip.SetToolTip(treeButton, "Tree");
            toolTip.SetToolTip(exportButton, "Export");
            toolTip.SetToolTip(coastButton, "Coast");
        }

        private void GridOption_Toggle(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.BackgroundImage = toggleGridOn;
                mapPanel.ToggleGrid(true);
            }
            else
            {
                checkBox1.BackgroundImage = toggleGridOff;
                mapPanel.ToggleGrid(false);
            }
        }

        private void StartMountainMode()
        {
            mountainMode = true;
            mountainRange = new List<Point>();
            selectedTile = 'G';
        }

        private void EndMountainMode(bool revertChanges=false)
        {
            if (revertChanges && mountainMode) 
            {
                foreach (Point p in mountainRange) 
                {
                    tileGrid[p.X, p.Y].SetToPreviousTileType();
                }
                mapPanel.Invalidate();
            }
            mountainMode = false;
            selectedTile = ' ';
        }


        private void SetToMountain(int x, int y) 
        {
            if (tileGrid[x, y].IsTileType(Tile.Type.Mountain)) 
            {
                RemoveMountainTile(new Point(x, y));
            }
            tiles[x, y] = 'G';
            tileGrid[x, y].SetTileType(Tile.Type.Mountain);
            tileGrid[x, y].SetSpritesheetCoordinate(new Point(3, 0));
            mountainRange.Add(new Point(x, y));
        }

        private bool TryAddToMountainRange(int x, int y)
        {
            // The first tile can always be added
            if (mountainRange.Count == 0)
            {
                SetToMountain(x, y);
                return true;
            }

            // Every other tile has to be a neighbour of the last tile
            Point last = mountainRange[mountainRange.Count - 1];
            if (Math.Abs(last.X - x) + Math.Abs(last.Y - y) != 1)
            {
                return false;
            }

            // The tile is a neighbour but may already be in the list
            if (mountainRange.Contains(new Point(x, y)))
            {
                return false;
            }

            // The tile can be added
            SetToMountain(x, y);

            // If the newly added tile is a neighbour of the first tile (and the list is longer than 2 tiles),
            // then the mountain range is complete.
            Point first = mountainRange[0];
            if (mountainRange.Count > 2 && Math.Abs(first.X - x) + Math.Abs(first.Y - y) == 1)
            {
                // The mountain range is complete. Now, starting from the first tile, every tile needs the correct sprite
                int rotation = CalculateMountainRangeRotation();
                // 360° means the user drew the mountain range clockwise, -360° means counter-clockwise
                SetMountainRangeSprites(rotation);
                // Add the new mountain range to the list of all mountain ranges
                allMountainRanges.Add(mountainRange);
                EndMountainMode();
            }
            return true;
        }

        /// <summary>
        /// When the user draws a mountain range he either draws it clockwise or counter-clockwise. This rotation is needed
        /// to specify the correct sprites for each mountain tile.
        /// </summary>
        /// <returns></returns>
        private int CalculateMountainRangeRotation()
        {
            int rotation = 0;
            for (int i = 0; i < mountainRange.Count; i++)
            {
                Point current = mountainRange[i];
                Point former = i == 0 ? mountainRange[mountainRange.Count - 1] : mountainRange[i - 1];
                Point next = i == mountainRange.Count - 1 ? mountainRange[0] : mountainRange[i + 1];
                Point dif = new Point(next.X - former.X, next.Y - former.Y);

                // The rotation angle can only change on corners
                if (dif.X == -1)
                {
                    if (dif.Y == -1)
                    {
                        if (current.Y > next.Y)
                        {
                            // Left then up
                            rotation += 90;
                            Console.WriteLine("Left then Up");
                        }
                        else
                        {
                            // Up then left
                            rotation -= 90;
                            Console.WriteLine("Up then Left");
                        }
                    }
                    else
                    {
                        if (current.Y < next.Y)
                        {
                            // Left then down
                            rotation -= 90;
                            Console.WriteLine("Left then Down");
                        }
                        else
                        {
                            // Down then left
                            rotation += 90;
                            Console.WriteLine("Down then left");
                        }
                    }
                }
                else if (dif.X == 1)
                {
                    if (dif.Y == -1)
                    {
                        if (current.Y > next.Y)
                        {
                            // Right then up
                            rotation -= 90;
                            Console.WriteLine("Right then Up");
                        }
                        else
                        {
                            // Up then right
                            rotation += 90;
                            Console.WriteLine("Up then right");
                        }
                    }
                    else
                    {
                        if (current.Y < next.Y)
                        {
                            // Right then down
                            rotation += 90;
                            Console.WriteLine("Right then down");
                        }
                        else
                        {
                            // Down then right
                            rotation -= 90;
                            Console.WriteLine("down then right");
                        }
                    }
                }
            }
            Console.WriteLine($"The rotation of this mountain range is {rotation}.");
            return rotation;
        }

        /// <summary>
        /// Sets sprites for all tiles of the mountain range depending of whether the mountain range was drawn
        /// clockwise or counter-clockwise.
        /// </summary>
        /// <param name="rotation">360° means clockwise, -360° means counter-clockwise</param>
        /// <returns></returns>
        private void SetMountainRangeSprites(int rotation)
        {
            for (int i = 0; i < mountainRange.Count; i++)
            {
                Point current = mountainRange[i];
                Point former = i == 0 ? mountainRange[mountainRange.Count - 1] : mountainRange[i - 1];
                Point next = i == mountainRange.Count - 1 ? mountainRange[0] : mountainRange[i + 1];
                Point dif = new Point(next.X - former.X, next.Y - former.Y);

                if (dif.X == 0)
                {
                    // Vertical
                    if (dif.Y == -2)
                    {
                        // Vertical up
                        tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(0, 10) : new Point(2, 10));
                    }
                    else
                    {
                        // Vertical down
                        tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(2, 10) : new Point(0, 10));
                    }
                }
                else if (dif.Y == 0)
                {
                    // Horizontal
                    if (dif.X == -2)
                    {
                        // Horizontal left
                        tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(1, 11) : new Point(1, 9));
                    }
                    else
                    {
                        // Horizontal right
                        tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(1, 9) : new Point(1, 11));
                    }
                }
                // Corners
                else if (dif.X == -1)
                {
                    if (dif.Y == -1)
                    {
                        if (current.Y > next.Y)
                        {
                            // Left then up
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(0, 11) : new Point(3, 10));
                        }
                        else
                        {
                            // Up then left
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(4, 9) : new Point(2, 9));
                        }
                    }
                    else
                    {
                        if (current.Y < next.Y)
                        {
                            // Left then down
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(3, 9) : new Point(0, 9));
                        }
                        else
                        {
                            // Down then left
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(2, 11) : new Point(4, 10));
                        }
                    }
                }
                else
                {
                    // x = 1
                    if (dif.Y == -1)
                    {
                        if (current.Y > next.Y)
                        {
                            // Right then up
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(4, 10) : new Point(2, 11));
                        }
                        else
                        {
                            // Up then right
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(0, 9) : new Point(3, 9));
                        }
                    }
                    else
                    {
                        if (current.Y < next.Y)
                        {
                            // Right then down
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(2, 9) : new Point(4, 9));
                        }
                        else
                        {
                            // Down then right
                            tileGrid[current.X, current.Y].SetSpritesheetCoordinate(rotation == 360 ? new Point(3, 10) : new Point(0, 11));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unlike other tiles, a single mountain tile can not be overdrawn. Instead, the whole mountain range has to be removed.
        /// </summary>
        /// <param name="point">The mountain tile position that was overdrawn</param>
        private void RemoveMountainTile(Point point)
        {
            // Search through all mountain ranges and remove all mountain tiles of the range that contains this point
            foreach (List<Point> range in allMountainRanges)
            {
                if (range.Contains(point))
                {
                    foreach (Point p in range)
                    {
                        tileGrid[p.X, p.Y].SetTileType(Tile.Type.Land);
                        tileGrid[p.X, p.Y].SetSpritesheetCoordinate(new Point(1, 1));
                        tiles[p.X, p.Y] = '0';
                    }
                    allMountainRanges.Remove(range);
                    Console.WriteLine("Number of mountain ranges left: " + allMountainRanges.Count);
                    return;
                }
            }
        }
    }
}
