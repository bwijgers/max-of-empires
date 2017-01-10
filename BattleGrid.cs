using MaxOfEmpires.GameObjects;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    class BattleGrid : Grid
    {
        public BattleGrid(int width, int height, string id = "") : base(width, height, id) // TODO: make this load from procedural generation.
        {
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);
        }

        /// <summary>
        /// Initializes the field.
        /// </summary>
        public override void InitField()
        {
            // Initialize the terrain
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    this[x, y] = new Tile(Terrain.Plains, x, y);
                }
            }
        }

        /// <summary>
        /// Populates the field with the armies of both attacker and defender.
        /// </summary>
        /// <param name="attacker">The attacking Army.</param>
        /// <param name="defender">The defending Army.</param>
        public void PopulateField(Army attacker, Army defender)
        {
            // Initialize the attacker's field
            int currentX = 0;
            int currentY = 0;

            // Iterate over every type of Soldier in the attacking Army
            foreach (Soldier s in attacker.UnitsAndCounts.Keys)
            {
                // Get the amount of this kind of Soldier
                int soldierCount = attacker.UnitsAndCounts[s];

                // Place them in a position based on how many soldiers we have passed so far.
                for (; soldierCount > 0; --soldierCount)
                {
                    (this[currentX, currentY] as Tile).SetUnit(s.Copy(attacker.Owner));
                    ++currentX;

                    // Make sure we don't create a line of soldiers longer than the field.
                    if (currentX >= Width)
                    {
                        currentX = 0;
                        ++currentY;
                    }
                }
            }

            // Do the same for the defending Army, except start bottom right and go left->up for each Soldier.
            currentX = Width - 1;
            currentY = Height - 1;

            foreach (Soldier s in defender.UnitsAndCounts.Keys)
            {
                int soldierCount = defender.UnitsAndCounts[s];

                for (; soldierCount > 0; --soldierCount)
                {
                    (this[currentX, currentY] as Tile).SetUnit(s.Copy(defender.Owner));
                    --currentX;

                    if (currentX < 0)
                    {
                        currentX = Width - 1;
                        --currentY;
                    }
                }
            }

            // And clear all target positions after we populated the field.
            ClearAllTargetPositions();
        }

        /// <summary>
        /// Executed when the player left-clicks on the grid.
        /// </summary>
        /// <param name="helper">The InputHelper used for mouse input.</param>
        public override void OnLeftClick(InputHelper helper)
        {
            // Get the current Tile under the mouse
            Tile clickedTile = GetTileUnderMouse(helper, true);

            // Do nothing if there is no clicked tile.
            if (clickedTile == null)
                return;

            // If the player had a tile selected and it contains a Unit...
            if (SelectedTile != null && SelectedTile.Occupied)
            {
                // ... move the Unit there, if the square is not occupied and the unit is capable, then unselect the tile.
                SelectedTile.Unit.TargetPosition = clickedTile.GridPos;
                Point movePos = Pathfinding.MoveTowardsTarget(SelectedTile.Unit);

                if (CheckMoveUnit(movePos, SelectedTile.Unit) || CheckAttackSoldier(clickedTile.GridPos, (Soldier) SelectedTile.Unit))
                {
                    SelectTile(InvalidTile);
                    return;
                }
            }

            // Check if the player clicked a tile with a Unit on it, and select it if it's there. 
            else if (clickedTile.Occupied && clickedTile.Unit.Owner == currentPlayer && clickedTile.Unit.HasAction)
            {
                // If the Unit can walk, show where it is allowed to walk. 
                if (!clickedTile.Unit.HasMoved)
                {
                    Point[] walkablePositions = Pathfinding.ReachableTiles(clickedTile.Unit);
                    SetUnitWalkingOverlay(walkablePositions);
                }

                // This unit can be selected. Show the player it is selected too
                SelectTile(clickedTile.GridPos);

                // Add an overlay for enemy units that can be attacked
                if (!(clickedTile.Unit as Soldier).HasAttacked)
                {
                    SetUnitAttackingOverlay((Soldier) clickedTile.Unit);
                }
            }
        }
    }
}
