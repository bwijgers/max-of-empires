using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    class BattleGrid
    {
        private int width;
        private int height;
        private Terrain[,] battleTerrain;

        public BattleGrid(int width, int height) // TODO: make this load from a file or something similar
        {
            // Initialize all variables
            this.width = width;
            this.height = height;
            battleTerrain = new Terrain[width, height];
        }

        public void InitField()
        {
            // Initialize the terrain
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    battleTerrain[x, y] = Terrain.Plains;
                }
            }
        }

        public void Draw(SpriteBatch s)
        {
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    battleTerrain[x, y].Draw(x, y, s);
                }
            }
        }

        public int Width => width;
        public int Height => height;
    }
}
