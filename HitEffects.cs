using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    class HitEffects : GameObjectAnimated
    {
        public HitEffects(string damageType) : base(true, 0.10D)
        {
            LoadTexture(damageType);
            ShouldAnimate = true;
        }

        private void LoadTexture(string damageType)
        {
            switch (damageType)
            {
                case "unit.healer":
                    DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\HitEffects\Heal@4x1");
                    break;
                case "unit.mage":
                    DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\HitEffects\Fire@4x1");
                    break;
                default:
                    DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\HitEffects\Slash@4x1"); ;
                    break;
            }
        }
        public void DeterminePosition(Point p)
        {
            DrawPosition = new Vector2(p.X * 32, p.Y * 32);
        }
    }
}
