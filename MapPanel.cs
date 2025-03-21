﻿using System;
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
        private bool isGridVisible = true;

        public MapPanel()
        {
            this.DoubleBuffered = true; // Prevents flickering
            treeRect = new Rectangle(0, 0, 82, 128);
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

        public void ToggleGrid(bool toggle) {
            this.isGridVisible = toggle;
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
            if (!isInitialized) {
                return;
            }

            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;

            DrawGridTiles(e);

            if (isGridVisible) {
                DrawGrid(e);
            }
        }

        private void DrawGridTiles(PaintEventArgs e)
        {
            for (int y = 0; y < tileGrid.GetLength(1); y++)
            {
                for (int x = 0; x < tileGrid.GetLength(0); x++)
                {
                    //TODO maybe switch row and col

                    Tile tile = tileGrid[x, y];

                    // Draw additional background layer for coast
                    if (tile.IsTileType(Tile.Type.Coast))
                    {
                        DrawImage(e, x, y, spriteSheet, tile.GetBackgroundCoordinate());
                    }

                    // Draw the base layer of the tile
                    DrawImage(e, x, y, spriteSheet, tile.GetSpritesheetCoordinate());

                    // Draw the tree layer
                    for (int i = 0; i < tile.GetTreeCount(); i++)
                    {
                        DrawImage(e, x + i * 0.3f, y + i * 0.2f, treeSheet, treeRect);
                    }

                    // Draw Error Image
                    if (tile.GetSpritesheetCoordinate().X == -1)
                    {
                        DrawImage(e, x, y, mapErrorImage);
                    }
                }
            }
        }

        private void DrawGrid(PaintEventArgs e)
        {
            Pen pen = new Pen(Color.DimGray, 1);
            PointF start;
            PointF end;
            // Vertical lines
            for (int x = 1; x < tileGrid.GetLength(0); x++)
            {
                start = new PointF(mapOffset.X + x * currentTileSize, mapOffset.Y);
                end = new PointF(mapOffset.X + x * currentTileSize, mapOffset.Y + tileGrid.GetLength(1) * currentTileSize);
                e.Graphics.DrawLine(pen, start, end);
            }
            // Horrizontal lines
            for (int y = 1; y < tileGrid.GetLength(1); y++)
            {
                start = new PointF(mapOffset.X, mapOffset.Y + y * currentTileSize);
                end = new PointF(mapOffset.X + tileGrid.GetLength(0) * currentTileSize, mapOffset.Y + y * currentTileSize);
                e.Graphics.DrawLine(pen, start, end);
            }
        }

        private void DrawImage(PaintEventArgs e, float x, float y, Image img)
        {
            e.Graphics.DrawImage(
                img,
                new Rectangle(
                    (int)Math.Floor(x * currentTileSize + mapOffset.X),
                    (int)Math.Floor(y * currentTileSize + mapOffset.Y),
                    (int)Math.Ceiling(currentTileSize),
                    (int)Math.Ceiling(currentTileSize)
                ),
                0,
                0,
                img.Width,
                img.Height,
                GraphicsUnit.Pixel,
                GetAttributesForDrawing()
            );
        }

        private void DrawImage(PaintEventArgs e, float x, float y, Image img, Point spriteSheetPosition)
        {
            e.Graphics.DrawImage(
                img,
                new Rectangle(
                    (int)Math.Floor(x * currentTileSize + mapOffset.X),
                    (int)Math.Floor(y * currentTileSize + mapOffset.Y),
                    (int)Math.Ceiling(currentTileSize),
                    (int)Math.Ceiling(currentTileSize)
                ),
                spriteSheetPosition.X * tileSize,
                spriteSheetPosition.Y * tileSize,
                tileSize,
                tileSize,
                GraphicsUnit.Pixel,
                GetAttributesForDrawing()
            );
        }

        private void DrawImage(PaintEventArgs e, float x, float y, Image img, Rectangle srcRect)
        {
            e.Graphics.DrawImage(
                img,
                new Rectangle(
                    (int)Math.Floor(x * currentTileSize + mapOffset.X),
                    (int)Math.Floor(y * currentTileSize + mapOffset.Y),
                    (int)Math.Ceiling(srcRect.Width * zoom * 0.5f),
                    (int)Math.Ceiling(srcRect.Height * zoom * 0.5f)
                ),
                srcRect.X,
                srcRect.Y,
                srcRect.Width,
                srcRect.Height,
                GraphicsUnit.Pixel,
                GetAttributesForDrawing()
            );
        }

        private ImageAttributes GetAttributesForDrawing()
        {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            return attributes;
        }
    }
}
