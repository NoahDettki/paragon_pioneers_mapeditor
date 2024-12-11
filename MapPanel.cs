using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParagonPioneers {
    internal class MapPanel : Panel{
        private Image[,] mapImages;
        private readonly int TILE_SIZE = 16;
        private float zoom;
        private float currentTileSize;
        private PointF mapOffset;

        public MapPanel() {
            this.DoubleBuffered = true; // Prevents flickering
            Zoom(3, new Point(0, 0));
            mapOffset = new PointF(0, 0);
        }

        public Point? MouseToGrid(Point mousePos) {
            Point gridPoint = new Point(
                (int)((mousePos.X - mapOffset.X) / currentTileSize),
                (int)((mousePos.Y - mapOffset.Y) / currentTileSize)
            );
            if (gridPoint.X < 0 || gridPoint.Y < 0 || gridPoint.X >= mapImages.GetLength(0) || gridPoint.Y >= mapImages.GetLength(1)) {
                return null;
            } else return gridPoint;
        }

        public void SetMapImages(Image[,] images) {
            mapImages = images;
            this.Invalidate();
        }

        public void SetImageAt(Image image, int x, int y) {
            if (x < 0 || y < 0 || x >= mapImages.GetLength(0) || y >= mapImages.GetLength(1)) {
                Console.WriteLine($"ERROR: Can't SetImageAt({x}|{y}). The given coordinate is outside of mapImage's bounds!");
                return;
            }
            mapImages[x, y] = image;
            this.Invalidate();
        }

        public Image GetImageAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= mapImages.GetLength(0) || y >= mapImages.GetLength(1))
            {
                Console.WriteLine($"ERROR: Can't GetImageAt({x}|{y}). The given coordinate is outside of mapImage's bounds!");
                return null;
            }
            return mapImages[x, y];
        }

        public void MoveMap(float dx, float dy) {
            mapOffset.X += dx;
            mapOffset.Y += dy;
            this.Invalidate();
        }

        public void Zoom(float delta, Point mousePos) {
            float oldZoomLevel = zoom;
            zoom += delta;
            if (zoom > 10f) {
                zoom = 10f;
            }
            if (zoom < 0.5f) {
                zoom = 0.5f;
            }
            currentTileSize = TILE_SIZE * zoom;

            // The offset is calculated to keep the mouse position at the same point on the map
            mapOffset.X = (float)mousePos.X - zoom / oldZoomLevel* (float)(mousePos.X - mapOffset.X);
            mapOffset.Y = (float)mousePos.Y - zoom / oldZoomLevel * (float)(mousePos.Y - mapOffset.Y);

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            // This is important especially for the editor view, because the images array will be empty at that point
            if (mapImages == null) {
                return;
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;

            var attributes = new ImageAttributes();
            attributes.SetWrapMode(WrapMode.TileFlipXY);

            for (int col = 0; col < mapImages.GetLength(0); col++) {
                for (int row = 0; row < mapImages.GetLength(1); row++) {
                    //TODO maybe switch row and col
                    e.Graphics.DrawImage(
                        mapImages[col, row],
                        new Rectangle(
                            (int)Math.Floor(col * currentTileSize + mapOffset.X), 
                            (int)Math.Floor(row * currentTileSize + mapOffset.Y),
                            (int)Math.Ceiling(currentTileSize), 
                            (int)Math.Ceiling(currentTileSize)
                        ),
                        0,
                        0,
                        //(col * currentTileSize + mapOffset.X),
                        //(row * currentTileSize + mapOffset.Y),
                        //currentTileSize,
                        //currentTileSize,
                        mapImages[col, row].Width,
                        mapImages[col, row].Height,
                        GraphicsUnit.Pixel,
                        attributes
                    );
                }
            }
        }
    }
}
