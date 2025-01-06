using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParagonPioneers {
    internal class Tile {
        public enum Type {
            Land,
            Coast,
            Water,
            Mountain,
        }
        private Type type;
        private int treeCount;
        private Point spriteCoord;
        private Point backgroundCoord;

        public Tile(Type type) {
            this.type = type;
        }

        public void SetTileType(Type type) {
            this.type = type;
        }

        public Type GetTileType() {
            return type;
        }

        public Point GetSpritesheetCoordinate() {
            return spriteCoord;
        }

        public void SetSpritesheetCoordinate(Point coord) {
            this.spriteCoord = coord;
        }

        public Point GetBackgroundCoordinate() {
            return backgroundCoord;
        }

        public void SetBackgroundCoordinate(Point coord) {
            this.backgroundCoord = coord;
        }
    }
}
