using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParagonPioneers {
    internal class Map {
        public List<List<Cell>> Cells { get; }

        public Map() {
            Random r = new Random();
            Cells = new List<List<Cell>>();
            for (int x = 0; x < 10; x++) {
                List<Cell> column = new List<Cell>();
                for (int y = 0; y < 10; y++) {
                    column.Add(new Cell(r.Next(0, 6)));
                }
                Cells.Add(column);
            }
        }
    }
}
