using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;

namespace MaxOfEmpires.Buildings
{
    class Building : GameObject
    {
        private Player owner;
        private Point positionInGrid;

        public Building(Point positionInGrid, Player owner)
        {
            this.positionInGrid = positionInGrid;
            this.owner = owner;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            DrawingHelper.Instance.DrawRectangle(s, new Rectangle(Parent.DrawPosition.ToPoint(), new Point(32)), new Color(0, 0, 0, 0.4F));
        }

        public Player Owner => owner;
    }
}
