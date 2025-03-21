﻿using System;
using System.Drawing;

namespace ParagonPioneers
{
    internal class Tile
    {
        public enum Type
        {
            Land,
            Coast,
            Water,
            Mountain,
            Tree1,
            Tree2,
            Tree3,
        }
        private Type type, previousType;
        private int treeCount, previousTreeCount;
        private Point spriteCoord, previousSpriteCoord;
        private Point backgroundCoord, previousBackgroundCoord;

        public Tile(char c)
        {
            Setup(c);
        }

        public void SetTileType(Type type)
        {
            previousType = this.type;
            previousTreeCount = treeCount;
            this.type = type;
            if (type != Type.Land)
            {
                treeCount = 0;
            }
        }

        public void SetToPreviousTileType() {
            this.type = previousType;
            this.treeCount = previousTreeCount;
            this.spriteCoord = previousSpriteCoord;
            this.backgroundCoord = previousBackgroundCoord;
        }

        public Type GetTileType()
        {
            return type;
        }

        public bool IsTileType(Type type)
        {
            return this.type == type;
        }

        public Point GetSpritesheetCoordinate() 
        {
            return spriteCoord;
        }

        public void SetSpritesheetCoordinate(Point coord)
        {
            previousSpriteCoord = this.spriteCoord;
            this.spriteCoord = coord;
        }

        public Point GetBackgroundCoordinate()
        {
            return backgroundCoord;
        }

        public void SetBackgroundCoordinate(Point coord)
        {
            previousBackgroundCoord = this.backgroundCoord;
            this.backgroundCoord = coord;
        }

        public void IncreaseTrees()
        {
            treeCount++;
            if (treeCount > 3)
            {
                treeCount = 0;
            }
            type = Type.Land;
        }

        public int GetTreeCount()
        {
            return treeCount;
        }

        public static Tile.Type CharToType(char c)
        {
            switch (c)
            {
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

        public void Setup(char c)
        {
            switch (c)
            {
                case 'W':
                    type = Type.Water;
                    break;
                case 'K':
                    type = Type.Coast;
                    break;
                case 'G':
                    type = Type.Mountain;
                    break;
                case '0':
                    type = Type.Land;
                    break;
                case '1':
                    type = Type.Land;
                    treeCount = 1;
                    break;
                case '2':
                    type = Type.Land;
                    treeCount = 2;
                    break;
                case '3':
                    type = Type.Land;
                    treeCount = 3;
                    break;
                default: throw new ArgumentException("Invalid character");
            }
        }
    }
}
