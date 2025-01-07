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
            Tree1,
            Tree2,
            Tree3,
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

        public static Tile.Type CharToType(char c) {
            switch (c) {
                case 'W': return Type.Water;
                case 'K': return Type.Coast;
                case 'G': return Type.Mountain;
                case '0': return Type.Land;
                case '1': return Type.Tree1;
                case '2': return Type.Tree2;
                case '3': return Type.Tree3;
                default: throw new ArgumentException("Invalid character");
            }
        }
    }
}
