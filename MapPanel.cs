using System;
using System.Collections.Generic;
using System.Drawing;
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
            Zoom(3);
            mapOffset = new PointF(0, 0);
        }

        public void SetMapImages(Image[,] images) {
            mapImages = images;
            this.Invalidate(); // Redraws the panel
        }

        public void MoveMap(float dx, float dy) {
            mapOffset.X += dx;
            mapOffset.Y += dy;

            this.Invalidate(); // Redraw the panel
        }

        public void Zoom(float factor) {
            zoom = factor;
            currentTileSize = TILE_SIZE * zoom;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            // This is important especially for the editor view, because the images array will be empty at that point
            if (mapImages == null) {
                return;
            }

            for (int col = 0; col < mapImages.GetLength(0); col++) {
                for (int row = 0; row < mapImages.GetLength(1); row++) {
                    //TODO maybe switch row and col
                    e.Graphics.DrawImage(
                        mapImages[row, col],
                        col * currentTileSize + mapOffset.X,
                        row * currentTileSize + mapOffset.Y,
                        currentTileSize,
                        currentTileSize
                    );
                }
            }
        }
    }
}
