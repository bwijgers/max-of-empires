using MaxOfEmpires.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.Units
{
    abstract class Unit : GameObject
    {
        protected bool hasAttacked;
        protected int movesLeft;
        protected int moveSpeed;
        private Texture2D texture;
        private int x;
        private int y;
        private bool owner; // 2 is false, 1 is true
        private Stats stats;

        protected Unit(int x, int y, bool owner, string resName)
        {
            this.x = x;
            this.y = y;
            this.owner = owner;
            texture = AssetManager.Instance.getAsset<Texture2D>(@"FE-sprites/" + resName);
        }

        private int DistanceTo(int x, int y)
        {
            int xDist = Math.Abs(x - this.x);
            int yDist = Math.Abs(y - this.y);
            return xDist + yDist;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            s.Draw(texture, DrawPos, Color.White);
        }

        public bool Move(int x, int y)
        {
            // If we already moved, we can't move anymore. Something like that
            if (HasMoved)
            {
                return false;
            }

            // Get the distance to the specified position.
            int distance = DistanceTo(x, y);

            // Check if we can move to this position before actually just moving there. CanMoveTo decrements MovesLeft as well, if it is possible to move to the position.
            if(distance <= movesLeft)
            {
                this.x = x;
                this.y = y;
                movesLeft -= distance;
                return true;
            }
            return false;
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            this.movesLeft = moveSpeed;
        }

        public Vector2 DrawPos => new Vector2(x * texture.Width, y * texture.Height);

        public Point GridPos
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                this.x = value.X;
                this.y = value.Y;
            }
        }

        public bool HasAttacked => hasAttacked;
        public bool HasMoved => movesLeft <= 0;
        public bool Owner => owner;
        public Stats Stats => stats;
    }
}
