using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.GameStates.Overlays;
using System.Collections.Generic;
using MaxOfEmpires.Units;

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

        public virtual void PopulateBuildingActions(GuiList buildingActions)
        {
            
        }

        protected void TrySpawnUnit(string soldierType)
        {
            // Check if the player can afford this soldier
            int cost = SoldierRegistry.GetSoldierCost(soldierType);
            if (!owner.CanAfford(cost))
            {
                return;
            }

            // Buy the soldier
            owner.Buy(cost);

            // Set this soldier in the world if possible
            Tile currentTile = ((GameWorld as Grid)[positionInGrid] as Tile);
            if (!currentTile.Occupied)
            {
                Army army = new Army(positionInGrid.X, positionInGrid.Y, owner);
                army.AddSoldier(SoldierRegistry.GetSoldier(soldierType, owner));
                currentTile.SetUnit(army);
            }
            else if (currentTile.Unit.Owner == owner && currentTile.Unit is Army)
            {
                Army a = currentTile.Unit as Army;
                a.AddSoldier(SoldierRegistry.GetSoldier(soldierType, owner));
            }
        }

        public Player Owner => owner;
    }
}
