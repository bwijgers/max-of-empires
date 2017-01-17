using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;
using Ebilkill.Gui.Elements;
using MaxOfEmpires.GameStates.Overlays;
using MaxOfEmpires.Units;
using System.Text;
using MaxOfEmpires.Files;

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

        protected void AddRecruitingButton(GuiList buildingActions, string unitToRecruit)
        {
            StringBuilder buttonText = new StringBuilder();
            buttonText.Append(Translations.GetTranslation(unitToRecruit)).Append(" ("); // Soldier (
            buttonText.Append(SoldierRegistry.GetSoldierCost(unitToRecruit)).Append("G): "); // Soldier ('cost'G)
            buildingActions.addElement(ElementBuildButton.CreateBuildButton(buildingActions.Bounds.Location, buttonText.ToString(), () => TrySpawnUnit(unitToRecruit)));
        }

        protected void TrySpawnUnit(string soldierType)
        {
            // Check if the player can afford this soldier
            int cost = SoldierRegistry.GetSoldierCost(soldierType);
            if (!owner.CanAfford(cost))
            {
                return;
            }

            // Set this soldier in the world if possible
            Tile currentTile = ((GameWorld as Grid)[positionInGrid] as Tile);
            if (!currentTile.Occupied)
            {
                // Nothing here, just place it in this square
                Army army = new Army(positionInGrid.X, positionInGrid.Y, owner);
                army.AddSoldier(SoldierRegistry.GetSoldier(soldierType, owner));
                currentTile.SetUnit(army);
            }
            else if (currentTile.Unit.Owner == owner && currentTile.Unit is Army)
            {
                // Our own army is here, just place it in there :)
                Army a = currentTile.Unit as Army;
                a.AddSoldier(SoldierRegistry.GetSoldier(soldierType, owner));
            }
            else
            {
                // We can't place it, just stop this whole function
                return;
            }

            // Buy the soldier, as we placed it.
            owner.Buy(cost);
        }

        public Player Owner => owner;
    }
}
