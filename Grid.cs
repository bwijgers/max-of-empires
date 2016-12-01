using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    class Grid : GameObjectGrid
    {
        private Point selectedTile;

        public Grid(int width, int height, string id = "") : base(width, height, id)// TODO: make this load from a file or something similar
        {
            selectedTile = InvalidTile;
        }

        public void InitField()
        {
            // Initialize the terrain
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    grid[x, y] = new Tile(Terrain.Plains, x, y);
                }
            }
        }

        private Point InvalidTile => new Point(-1, -1);
        public Tile SelectedTile
        {
            get
            {
                if (grid[selectedTile.X, selectedTile.Y] is Tile)
                    return grid[selectedTile.X, selectedTile.Y] as Tile;
                return null;
            }
        }
    }
}
