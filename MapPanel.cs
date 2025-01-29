using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ParagonPioneers
{
    internal class MapPanel : Panel
    {
        private Image spriteSheet;
        private Image treeSheet;
        private Image mapErrorImage;
        private int tileSize;
        private Tile[,] tileGrid;
        private float zoom;
        private float currentTileSize;
        private PointF mapOffset;
        private bool isInitialized = false;
        private Rectangle treeRect;
        private bool drawGrid;

        public MapPanel()
        {
            this.DoubleBuffered = true; // Prevents flickering
            treeRect = new Rectangle(0, 0, 82, 128);
            drawGrid = true;
        }

        public void Initialize(Image spriteSheet, Image treeSheet, int tileSize, Tile[,] tileGrid, Image mapError)
        {
            this.spriteSheet = spriteSheet;
            this.treeSheet = treeSheet;
            this.tileSize = tileSize;
            this.tileGrid = tileGrid;
            this.mapErrorImage = mapError;
            zoom = 0.1f;
            currentTileSize = tileSize * zoom;
            mapOffset = new PointF(0, 0);
            isInitialized = true;
        }

        public Point? MouseToGrid(Point mousePos)
        {
            Point gridPoint = new Point(
                (int)((mousePos.X - mapOffset.X) / currentTileSize),
                (int)((mousePos.Y - mapOffset.Y) / currentTileSize)
            );
            if (gridPoint.X < 0 || gridPoint.Y < 0 || gridPoint.X >= tileGrid.GetLength(0) || gridPoint.Y >= tileGrid.GetLength(1))
            {
                return null;
            }
            else return gridPoint;
        }

        public void SetTileGrid(Tile[,] grid)
        {
            tileGrid = grid;
            Invalidate();
        }

        public void SetSpriteSheet(Image spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            Invalidate();
        }

        public void SetTreeSheet(Image treeSheet)
        {
            this.treeSheet = treeSheet;
            Invalidate();
        }

        public void SetTileSize(int size)
        {
            tileSize = size;
            Invalidate();
        }

        public void ToggleGrid(bool toggle)
        {
            this.drawGrid = toggle;
            Invalidate();
        }

        public void MoveMap(float dx, float dy)
        {
            float mapImageWidth = tileGrid.GetLength(0) * tileSize * zoom;
            float mapImageHeight = tileGrid.GetLength(1) * tileSize * zoom;
            //determines how much map at least has to be visible
            float gap = tileSize * zoom;

            // Clamps X offset
            mapOffset.X = Math.Max(Math.Min(mapOffset.X + dx, this.Width - gap), 0 - mapImageWidth + gap);

            // Clamp Y offset
            mapOffset.Y = Math.Max(Math.Min(mapOffset.Y + dy, this.Height - gap), 0 - mapImageHeight + gap);

            Invalidate();
        }

        public void Zoom(float delta, Point mousePos)
        {
            float oldZoomLevel = zoom;
            zoom += delta;
            if (zoom > 5f)
            {
                zoom = 5f;
            }
            if (zoom < 0.1f)
            {
                zoom = 0.1f;
            }
            currentTileSize = tileSize * zoom;

            // The offset is calculated to keep the mouse position at the same point on the map
            mapOffset.X = (float)mousePos.X - zoom / oldZoomLevel * (float)(mousePos.X - mapOffset.X);
            mapOffset.Y = (float)mousePos.Y - zoom / oldZoomLevel * (float)(mousePos.Y - mapOffset.Y);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // This is important especially for the [Design] view, because the images array will be empty at that point
            if (isInitialized == false)
            {
                return;
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;

            var attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            // Draw every tile
            Point p;
            for (int col = 0; col < tileGrid.GetLength(0); col++)
            {
                for (int row = 0; row < tileGrid.GetLength(1); row++)
                {
                    //TODO maybe switch row and col

                    if (tileGrid[col, row].GetSpritesheetCoordinate().X == -1)
                    {
                        e.Graphics.DrawImage(
                            mapErrorImage,
                            new Rectangle(
                                (int)Math.Floor(col * currentTileSize + mapOffset.X),
                                (int)Math.Floor(row * currentTileSize + mapOffset.Y),
                                (int)Math.Ceiling(currentTileSize),
                                (int)Math.Ceiling(currentTileSize)
                            ),
                            0,
                            0,
                            mapErrorImage.Width,
                            mapErrorImage.Height,
                            GraphicsUnit.Pixel,
                            attributes
                        );
                        continue;
                    }

                    // Water tiles have an additional background layer
                    if (tileGrid[col, row].GetTileType() == Tile.Type.Coast)
                    {
                        p = tileGrid[col, row].GetBackgroundCoordinate();

                        e.Graphics.DrawImage(
                            spriteSheet,
                            new Rectangle(
                                (int)Math.Floor(col * currentTileSize + mapOffset.X),
                                (int)Math.Floor(row * currentTileSize + mapOffset.Y),
                                (int)Math.Ceiling(currentTileSize),
                                (int)Math.Ceiling(currentTileSize)
                            ),
                            p.X * tileSize,
                            p.Y * tileSize,
                            tileSize,
                            tileSize,
                            GraphicsUnit.Pixel,
                            attributes
                        );
                    }

                    // Draw the base layer of the tile
                    p = tileGrid[col, row].GetSpritesheetCoordinate();
                    if (p == null)
                    {
                        p.X = 8;
                        p.Y = 8;
                    }

                    e.Graphics.DrawImage(
                        spriteSheet,
                        new Rectangle(
                            (int)Math.Floor(col * currentTileSize + mapOffset.X),
                            (int)Math.Floor(row * currentTileSize + mapOffset.Y),
                            (int)Math.Ceiling(currentTileSize),
                            (int)Math.Ceiling(currentTileSize)
                        ),
                        p.X * tileSize,
                        p.Y * tileSize,
                        tileSize,
                        tileSize,
                        GraphicsUnit.Pixel,
                        attributes
                    );

                    // Draw the tree layer
                    for (int i = 0; i < tileGrid[col, row].GetTreeCount(); i++)
                    {
                        e.Graphics.DrawImage(
                            treeSheet,
                            new Rectangle(
                                (int)Math.Floor((col + i * 0.3f) * currentTileSize + mapOffset.X),
                                (int)Math.Floor((row + i * 0.2f) * currentTileSize + mapOffset.Y),
                                (int)Math.Ceiling(treeRect.Width * zoom * 0.5f),
                                (int)Math.Ceiling(treeRect.Height * zoom * 0.5f)
                            ),
                            treeRect.X,
                            treeRect.Y,
                            treeRect.Width,
                            treeRect.Height,
                            GraphicsUnit.Pixel,
                            attributes
                        );
                    }
                }
            }

            // Draw grid lines
            if (drawGrid)
            {
                Pen pen = new Pen(Color.DimGray, 1);
                PointF start;
                PointF end;
                // Vertical lines
                for (int col = 1; col < tileGrid.GetLength(0); col++)
                {
                    start = new PointF(mapOffset.X + col * currentTileSize, mapOffset.Y);
                    end = new PointF(mapOffset.X + col * currentTileSize, mapOffset.Y + tileGrid.GetLength(1) * currentTileSize);
                    e.Graphics.DrawLine(pen, start, end);
                }
                // Horrizontal lines
                for (int row = 1; row < tileGrid.GetLength(1); row++)
                {
                    start = new PointF(mapOffset.X, mapOffset.Y + row * currentTileSize);
                    end = new PointF(mapOffset.X + tileGrid.GetLength(0) * currentTileSize, mapOffset.Y + row * currentTileSize);
                    e.Graphics.DrawLine(pen, start, end);
                }
            }
        }
    }
}
