using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires
{
    class Tile : GameObject
    {
        private int x, y;
        private Terrain terrain;
        private Unit unit;
        private Building building; 

        public Tile(Terrain terrain, int x, int y)
        {
            this.terrain = terrain;
            this.x = x;
            this.y = y;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            terrain.Draw(x, y, s);
        }

        public Building Building => building;
        public bool BuiltOn => building != null;
        public bool Occupied => Unit != null;
        public Terrain Terrain => terrain;
        public Unit Unit => unit;
    }
}
