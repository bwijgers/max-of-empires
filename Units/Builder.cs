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
        
        public override bool Passable(Terrain terrain)
        {
            return true;
        }
    }
}
