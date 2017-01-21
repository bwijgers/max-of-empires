using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MaxOfEmpires.Units
{
    class Builder : Unit
    {
        public Builder(int x, int y, Player owner) : base(x, y, owner,"builder")
        {
            DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\Units\Builder" + owner.ColorName + "@4x5");
            moveSpeed = 1;
        }
        public override bool Move(int x, int y)
        {
            // If we already moved, we can't move anymore. Something like that
            if (HasMoved)
            {
                return false;
            }

            // Get the distance to the specified position.
            int distance = Math.Abs(PositionInGrid.X-x)+Math.Abs(PositionInGrid.Y-y);

            // Check if we can move to this position. Decrements moves left as well if we can.
            if (distance <= movesLeft)
            {
                movesLeft -= distance;
                return true;
            }
            return false;
        }
        public override bool Passable(Terrain terrain)
        {
            return true;
        }
    }
}
