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
    abstract class Grid : GameObjectGrid
    {
        protected bool currentPlayer;

        /// <summary>
        /// The coords of the currently selected Tile within the grid.
        /// </summary>
        private Point selectedTile;

        /// <summary>
        /// The current Unit targets that are displayed.
        /// </summary>
        private GameObjectList unitTargets;

        public Grid(int width, int height, string id = "") : base(width, height, id) // TODO: make this load from a file or something similar
        {
            selectedTile = InvalidTile;
            currentPlayer = true;
            unitTargets = new GameObjectList();
            unitTargets.Parent = this;
        }

        /// <summary>
        /// Checks whether a Unit can be moved to a position, and moves it if it's possible.
        /// </summary>
        /// <param name="newPos">The new position for the Unit to be moved.</param>
        /// <param name="unit">The Unit to be moved.</param>
        /// <returns>True if the Unit was moved, false otherwise.</returns>
        public bool CheckMoveUnit(Point newPos, Unit unit)
        {
            Tile targetTile = this[newPos] as Tile;
            Tile originTile = this[unit.PositionInGrid] as Tile;

            // If the Unit can move to its target, move it and tell the caller that we moved.
            if (targetTile != null && !targetTile.Occupied && unit.Move(newPos.X, newPos.Y))
            {
                targetTile.SetUnit(unit);
                originTile.SetUnit(null);
                return true;
            }

            // Otherwise, tell the caller that we didn't move.
            return false;
        }

        /// <summary>
        /// Checks whether a Unit can attack a Unit at the specified tile, and attacks it if it's possible.
        /// </summary>
        /// <param name="newPos">The position for the Unit to attack.</param>
        /// <param name="unit">The Unit which attacks.</param>
        /// <returns>True if the Unit attacked, false otherwise.</returns>
        public bool CheckAttackSoldier(Point tileToAttack, Soldier attackingUnit)
        {
            // Cannot attack more than once a turn. 
            if (attackingUnit.HasAttacked)
                return false;

            Tile toAttack = this[tileToAttack] as Tile;

            // Make sure the attack square is occupied by an enemy unit
            if(!toAttack.Occupied || toAttack.Unit.Owner == attackingUnit.Owner)
            {
                return false; // nothing to attack
            }

            // Make sure the attack square is in range of the attacking unit
            if (!attackingUnit.IsInRange(tileToAttack))
            {
                return false; // Enemy not in range
            }

            // We can actually attack this? Nice :D
            attackingUnit.Attack(tileToAttack);

            // After a battle, check if there are dead Units, and remove these if they are dead
            ForEach((obj, x, y) => {
                Tile t = obj as Tile;
                if (t.Occupied && (t.Unit as Soldier).IsDead)
                {
                    t.SetUnit(null);
                }
            });

            return true;
        }

        /// <summary>
        /// Clears the target positions of all Units on the grid.
        /// </summary>
        protected void ClearAllTargetPositions()
        {
            // Clear the target positions (because this method kinda sucks :/)
            ForEach((obj, x, y) =>
            {
                if ((obj as Tile).Occupied)
                    Pathfinding.ClearTargetPosition((obj as Tile).Unit);
            });
        }

        /// <summary>
        /// Creates Unit target overlays, showing players where Units are headed.
        /// </summary>
        private void CreateUnitTargetOverlays()
        {
            ForEach((obj, x, y) => {
                Tile t = obj as Tile;

                // If there is a Unit on this Tile and their target is not where they are
                if (t.Occupied && t.Unit.TargetPosition != t.Unit.PositionInGrid)
                {
                    // Recalculate the Unit's paths
                    Pathfinding.GeneratePaths(t.Unit, new Point(x, y));

                    // Make a UnitTargetOverlay for this and add it to the list of overlays
                    TargetPositionOverlay uto = new TargetPositionOverlay(t.Unit);
                    unitTargets.Add(uto);
                }
            });
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);

            // Draw the Unit target overlay, if it exists
            unitTargets.Draw(time, s);
        }

        /// <summary>
        /// Finds a Tile under the mouse within this Grid.
        /// </summary>
        /// <param name="helper">The InputHelper. Used for mouse position.</param>
        /// <param name="onClick">Whether this is called on click. Used to see if a Tile should be deselected.</param>
        /// <returns>The Tile under the mouse, or null if there is no such Tile.</returns>
        public Tile GetTileUnderMouse(InputHelper helper, bool onClick = false)
        {
            // Get the current grid position the player clicked at
            Vector2 mousePosRelativeToGrid = helper.MousePosition - DrawPosition;
            Point gridPos = (mousePosRelativeToGrid / 32).ToPoint();

            // Just unselect this tile if the user clicks this again.
            if (gridPos.Equals(selectedTile) && onClick)
            {
                SelectTile(InvalidTile);
                return null;
            }

            // If the tile doesn't exist, return null as well. This is checked implicitly.
            // Get the tile that is hovered over
            Tile clickedTile = this[gridPos] as Tile;
            return clickedTile;
        }

        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Check if the player clicked
            if (helper.MouseLeftButtonPressed)
            {
                OnLeftClick(helper);
            }

            // Check if the overlays should be rendered.
            if(keyManager.KeyPressed("unitTargetOverlay", helper))
            {
                CreateUnitTargetOverlays();
            }
        }

        /// <summary>
        /// Initializes the field.
        /// </summary>
        public abstract void InitField();

        /// <summary>
        /// Checks if two positions are adjacent. Points are not considered adjacent when they are diagonally adjacent.
        /// </summary>
        /// <returns>True when the positions are adjacent, false otherwise.</returns>
        protected bool IsAdjacent(Point a, Point b)
        {
            int xDiff = Math.Abs(a.X - b.X);
            int yDiff = Math.Abs(a.Y - b.Y);

            return (xDiff == 1 || yDiff == 1) && xDiff != yDiff;
        }

        /// <summary>
        /// Executed when the player left-clicks on the grid.
        /// </summary>
        /// <param name="helper">The InputHelper used for mouse input.</param>
        public virtual void OnLeftClick(InputHelper helper)
        {
        }

        /// <summary>
        /// Selects a Tile within this Grid.
        /// </summary>
        /// <param name="p">The position of the Tile to select.</param>
        public void SelectTile(Point p)
        {
            // Unselect the current tile if this should happen.
            Tile t = (this[p] as Tile);
            if (t != null)
            {
                t.OverlayWalk = false;
            }

            // Select the new tile
            selectedTile = p;

            // Unselecting a tile means the unit walking overlay will be non-existent
            if (selectedTile == InvalidTile)
            {
                SetUnitWalkingOverlay(null);
                SetUnitAttackingOverlay(null);
            }
            else
            {
                (this[p] as Tile).OverlayWalk = true;
            }
        }

        /// <summary>
        /// Shows an attacking overlay for armies.
        /// </summary>
        /// <param name="a">The army which needs an attacking overlay to show.</param>
        protected void SetArmyAttackingOverlay(Army a)
        {
            // Use a swordsman, as it has a range of 1. 
            Soldier swordsman = SoldierRegistry.GetSoldier("unit.swordsman", a.Owner);
            swordsman.PositionInGrid = a.PositionInGrid;
            SetUnitAttackingOverlay(swordsman);
        }

        /// <summary>
        /// Sets the attacking overlay on all tiles the parameter Unit can attack. Removes said overlay if the Unit == null.
        /// </summary>
        /// <param name="u">The Unit whose attacking overlay will be set.</param>
        protected void SetUnitAttackingOverlay(Soldier u)
        {
            // If no Unit is selected (anymore), unset the attacking overlay everywhere
            if (u == null)
            {
                ForEach((obj, x, y) => {
                    Tile t = obj as Tile;
                    if (t != null)
                        t.OverlayAttack = false;
                });

                return;
            }

            // Get the max range so we don't overshoot the search *too* much
            int maxRange = u.Range.Max + 1; // Hey Max :)

            // Search each tile within max range and check if we can attack there
            int startX = Math.Max(u.PositionInGrid.X - maxRange, 0); // Make sure we are in grid
            int startY = Math.Max(u.PositionInGrid.Y - maxRange, 0); // Same as above
            int endX = Math.Min(u.PositionInGrid.X + maxRange, Width); // Again, same as above
            int endY = Math.Min(u.PositionInGrid.Y + maxRange, Height); // I hate repeating myself...

            // Start searching already O_O
            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
                {
                    // Get the current Tile
                    Tile t = this[x, y] as Tile;

                    // If there is a Unit we can attack, set that overlay to true
                    if (t != null && t.Occupied && t.Unit.Owner != u.Owner)
                    {
                        // Check if the enemy is in range
                        if (u.IsInRange(new Point(x, y)))
                        {
                            t.OverlayAttack = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets a walking overlay to all Points in the <code>overlay</code> parameter. If <code>overlay</code> is null or empty, removes the overlay from all Tiles.
        /// </summary>
        /// <param name="overlay">The points to set the walking overlay on.</param>
        protected void SetUnitWalkingOverlay(Point[] overlay)
        {
            // Remove overlay from everything
            ForEach((obj, x, y) => (obj as Tile).OverlayWalk = false);

            // No overlay should be drawn
            if (overlay == null || overlay.Length == 0)
            {
                return;
            }

            // Draw the overlay
            foreach (Point p in overlay)
            {
                if (IsInGrid(p))
                {
                    (this[p] as Tile).OverlayWalk = true;
                }
            }
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            base.TurnUpdate(turn, player);

            // Make sure that anything that was selected no longer is selected.
            SelectTile(InvalidTile);

            // So the grid knows who is the current player. Useful for selecting units that are your own. 
            this.currentPlayer = player;

            // Makes the units go towards their target
            ForEach((obj, x, y) => {
                Tile tile = obj as Tile;
                if(tile.Occupied)
                {
                    Unit unit = tile.Unit;
                    if(unit.Owner != player) // End of turn for the player whose turn it is NOT right now.
                    {
                        Point movePos = Pathfinding.MoveTowardsTarget(unit);
                        CheckMoveUnit(movePos, unit);
                    }
                }
            });
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            // Updates the Unit target overlay
            unitTargets.Update(time);

            // Remove unitTargets that are done
            unitTargets.ForEach(obj => {
                TargetPositionOverlay uto = obj as TargetPositionOverlay;

                if (uto.Finished)
                {
                    unitTargets.RemoveChild(obj);
                }
            });
        }

        /// <summary>
        /// Property defining a position which is invalid. Unselects tiles.
        /// </summary>
        protected Point InvalidTile => new Point(-1, -1);

        /// <summary>
        /// Gets the selected tile.
        /// </summary>
        public Tile SelectedTile
        {
            get
            {
                // Check if the position actually is a tile, although it should be.
                // GameObject this[Point] checks whether the position is in bounds.
                if (this[selectedTile] is Tile)
                    return this[selectedTile] as Tile;

                // Return null if there is no selected tile, or the selected tile is out of bounds.
                return null;
            }
        }
    }
}
