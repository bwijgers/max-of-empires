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
    partial class Grid : GameObjectGrid
    {
        private bool currentPlayer;

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
            Tile tile = (GameWorld as Grid)[newPos] as Tile;
            Tile oriTile = (GameWorld as Grid)[unit.PositionInGrid] as Tile;
            if (!tile.Occupied && unit.Move(newPos.X, newPos.Y))
            {
                tile.SetUnit(unit);
                oriTile.SetUnit(null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether a Unit can attack a Unit at the specified tile, and attacks it if it's possible.
        /// </summary>
        /// <param name="newPos">The position for the Unit to attack.</param>
        /// <param name="unit">The Unit which attacks.</param>
        /// <returns>True if the Unit attacked, false otherwise.</returns>
        public bool CheckAttackUnit(Point tileToAttack, Unit attackingUnit)
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
                if (t.Occupied && t.Unit.IsDead)
                {
                    t.SetUnit(null);
                }
            });

            return true;
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
                    t.Unit.GeneratePaths(new Point(x, y));

                    // Make a UnitTargetOverlay for this and add it to the list of overlays
                    TargetPositionOverlay uto = new TargetPositionOverlay(t.Unit);
                    unitTargets.Add(uto);
                }
            });
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);

            // Draw selected Unit overlay, Unit move overlay and Unit attacking overlay

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
        public void InitField()
        {
            // Initialize the terrain
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    this[x, y] = new Tile(Terrain.Plains, x, y);
                }
            }
            EconomyGenerate();
            //BattleGenerate(Terrain.Tundra,false, Terrain.Desert,true);

            // Place a swordsman for each player on the field.
            Unit u1 = UnitRegistry.GetUnit("swordsman", true);
            (this[4, 4] as Tile).SetUnit(u1);
            (this[3, 4] as Tile).SetUnit(UnitRegistry.GetUnit("archer", true));

            Unit u2 = UnitRegistry.GetUnit("swordsman", false);
            (this[10, 10] as Tile).SetUnit(u2);
            (this[11, 10] as Tile).SetUnit(UnitRegistry.GetUnit("archer", false));

            // Clear the target positions (because this method kinda sucks :/)
            ForEach((obj, x, y) => (obj as Tile).Unit?.ClearTargetPosition());
        }

        /// <summary>
        /// Executed when the player left-clicks on the grid.
        /// </summary>
        /// <param name="helper">The InputHelper used for mouse input.</param>
        private void OnLeftClick(InputHelper helper)
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
                Point movePos = SelectedTile.Unit.MoveTowardsTarget();

                if (CheckMoveUnit(movePos, SelectedTile.Unit) || CheckAttackUnit(clickedTile.GridPos, SelectedTile.Unit))
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
                    Point[] walkablePositions = clickedTile.Unit.ReachableTiles();
                    SetUnitWalkingOverlay(walkablePositions);
                }

                // This unit can be selected. Show the player it is selected too
                SelectTile(clickedTile.GridPos);

                // Add an overlay for enemy units that can be attacked
                if (!clickedTile.Unit.HasAttacked)
                {
                    SetUnitAttackingOverlay(clickedTile.Unit);
                }
            }
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
        /// Sets the attacking overlay on all tiles the parameter Unit can attack. Removes said overlay if the Unit == null.
        /// </summary>
        /// <param name="u">The Unit whose attacking overlay will be set.</param>
        private void SetUnitAttackingOverlay(Unit u)
        {
            // If no Unit is selected (anymore), unset the walking overlay everywhere
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
            int maxRange = u.Range.Max; // Hey Max :)

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

        private void SetUnitWalkingOverlay(Point[] overlay)
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
                        Point movePos = unit.MoveTowardsTarget();
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
        private Point InvalidTile => new Point(-1, -1);

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
