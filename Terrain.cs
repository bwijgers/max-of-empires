using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    class Terrain
    {
        private static Terrain plains = new Terrain(TerrainType.PLAINS);
        public static Terrain Plains => plains;

        private enum TerrainType
        {
            PLAINS
        }

        private TerrainType terrainType;
        private Texture2D tex;

        private Terrain(TerrainType terrainType)
        {
            this.terrainType = terrainType;
            string texName = terrainType.ToString().ToLower();
            tex = AssetManager.Instance.getAsset<Texture2D>(@"FE-Sprites\" + texName);
        }

        public void Draw(int x, int y, SpriteBatch s)
        {
            s.Draw(tex, new Rectangle(x * tex.Width, y * tex.Height, tex.Width, tex.Height), Color.White);
        }
    }
}
