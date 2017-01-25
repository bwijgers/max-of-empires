using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    class HitEffects : GameObjectAnimated
    {
        public HitEffects(string damageType) : base(false, false, 0.25D)
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
                case "unit.mage.1":
                case "unit.mage.2":
                case "unit.mage.3":
                    AssetManager.Instance.PlaySound("Music/Fireball");
                    DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\HitEffects\Fire@4x1");
                    break;
                default:
                    AssetManager.Instance.PlaySound("Music/Slash");
                    DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\HitEffects\Slash@4x1");
                    break;
            }
        }

        public void DeterminePosition(Point p)
        {
            PositionFromParent = new Vector2(p.X * 32, p.Y * 32);
        }
    }
}
