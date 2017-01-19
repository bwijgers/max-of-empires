using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.Units
{
    class Builder : Unit
    {
        public Builder(int x, int y, Player owner) : base(x, y, owner)
        {
            DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\swordsman_blue");
            moveSpeed = 2;
        }
    }
}
