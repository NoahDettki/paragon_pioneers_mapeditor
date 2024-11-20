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
        private const int IMAGE_SIZE = 16;
        private float zoom = 3;

        private readonly Dictionary<int, String> images = new Dictionary<int, String>()
        {
            [0] = Path.Combine(Application.StartupPath, "Images", "Water.png"),
            [1] = Path.Combine(Application.StartupPath, "Images", "Land.png"),
        };

        public Map(int[,] tiles)
        {
            this.tiles = tiles;

            InitializeComponent();

            Init();
        }

        private void Init()
        {
            // DataGridView settings
            grid.Dock = DockStyle.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.RowTemplate.Height = IMAGE_SIZE;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            grid.RowTemplate.Height = (int) (IMAGE_SIZE * zoom);

            this.Text = "Tile Grid";

            PopulateGrid();
        }

        private void PopulateGrid()
        {
            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                grid.Columns.Add(new DataGridViewImageColumn
                {
                    Name = "Col" + col,
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = (int)(IMAGE_SIZE * zoom)
                });
            }

            // Populate the grid with TileType values
            for (int row = 0; row < rows; row++)
            {
                grid.Rows.Add();

                for (int col = 0; col < cols; col++)
                {
                    int tile = tiles[row, col];
                    grid.Rows[row].Cells[col].Value = Image.FromFile(images[tile]);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public enum TileType
    {
        Water = 0,
        Land = 1,
    }
}
