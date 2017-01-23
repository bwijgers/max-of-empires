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
    abstract partial class Grid : GameObjectGrid
    {
        public struct WalkingUnit
        {
            private bool removeOnEnd;
            private Unit movingUnit;
            private Point targetPosition;

            public WalkingUnit(Unit movingUnit, Point targetPosition, bool removeOnEnd)
            {
                this.movingUnit = movingUnit;
                this.targetPosition = targetPosition;
                this.removeOnEnd = removeOnEnd;
            }

            public bool RemoveOnEnd => removeOnEnd;
            public Unit MovingUnit => movingUnit;
            public Point TargetPosition => targetPosition;
        }

        protected Player currentPlayer;
        public List<Player> players;
        protected Point[] walkablePositions;

        /// <summary>
        /// The coords of the currently selected Tile within the grid.
        /// </summary>
        private Point selectedTile;

        /// <summary>
        /// The current Unit targets that are displayed.
        /// </summary>
        private GameObjectList unitTargets;

        private Point mousePoint;

        private Point[] path;
        
        private List<WalkingUnit> walkingUnits;
        private bool removeWalkingUnit;

        private Point targetPosition;

        public Grid(int width, int height, List<Player> players, string id = "") : base(width, height, id)
        {
            selectedTile = InvalidTile;
            currentPlayer = null;
            this.players = players;
            unitTargets = new GameObjectList();
            unitTargets.Parent = this;
            removeWalkingUnit = false;
            walkingUnits = new List<WalkingUnit>();
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

            // If the Unit can move to its target, move it and tell the caller that we moved.
            if(unit is Army && !(unit as Army).AllUnitsSelected)
            {
                Army partial = (unit as Army).SplitArmy((unit as Army).SelectedUnits);
                if (partial == null)
                {
                    return false;
                }

                if (targetTile != null && !targetTile.Occupied && partial.Move(newPos.X, newPos.Y))
                {
                    OnUnitStartMoving(partial, newPos, false);
                    Pathfinding.ClearTargetPosition(unit);
                    Pathfinding.ClearTargetPosition(partial);
                    return true;
                }
                else
                {
                    (unit as Army).MergeArmy(partial);
                }
            }
            else if (targetTile != null && !targetTile.Occupied && unit.Move(newPos.X, newPos.Y))
            {
                OnUnitStartMoving(unit, newPos);
                return true;
            }

            // Otherwise, tell the caller that we didn't move.
            return false;
        }

        /// <summary>
        /// Clears the target positions of all Units on the grid.
        /// </summary>
        protected void ClearAllTargetPositions()
        {
            // Clear the target positions (because this method kinda sucks :/)
            ForEach(obj => {
                if ((obj as Tile).Occupied)
                    Pathfinding.ClearTargetPosition((obj as Tile).Unit);
            });
        }

        /// <summary>
        /// Creates Unit target overlays, showing players where Units are headed.
        /// </summary>
        private void CreateUnitTargetOverlays()
        {
            ForEach(obj => {
                Tile t = obj as Tile;

                // If there is a Unit on this Tile and their target is not where they are
                if (t.Occupied && t.Unit.TargetPosition != t.Unit.PositionInGrid)
                {
                    // Make a UnitTargetOverlay for this and add it to the list of overlays
                    TargetPositionOverlay uto = new TargetPositionOverlay(t.Unit);
                    unitTargets.Add(uto);
                }
            });
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            ForEach(obj => (obj as Tile).DrawBackground(time, s));
            ForEach(obj => (obj as Tile).DrawForeground(time, s));

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
            Vector2 mousePosRelativeToGrid = helper.GetMousePosition(true) - DrawPosition;
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
            // Check if the overlays should be rendered.
            if (keyManager.KeyPressed("unitTargetOverlay", helper))
            {
                CreateUnitTargetOverlays();
            }

            if (walkingUnits == null || walkingUnits.Count != 0)
            {
                return;
            }

            // Check if the player clicked
            if (helper.MouseLeftButtonPressed)
            {
                OnLeftClick(helper);
            }

            if (SelectedTile != null)
            {
                if (SelectedTile.Occupied)
                {
                    //foreach (Point p in walkablePositions)
                    ForEach(obj => {

                    //{
                            Point p = (obj as Tile).PositionInGrid;
                        if ((int)(helper.GetMousePosition(true).X+MaxOfEmpires.camera.Position.X) / 32 == p.X && (int)(helper.GetMousePosition(true).Y + MaxOfEmpires.camera.Position.Y) / 32 == p.Y && p != selectedTile&&(this[p] as Tile).Passable(SelectedTile.Unit))
                        {
                            if (mousePoint != p)
                            {
                                mousePoint = p;
                                Point[] pathWithoutOriginalTile = Pathfinding.GetPath(SelectedTile.Unit, p);
                                path = new Point[pathWithoutOriginalTile.Length + 1];
                                path[0] = selectedTile;
                                pathWithoutOriginalTile.CopyTo(path, 1);
                            }
                            DisplayPath(path);
                            return;
                        }
                        //}

                    });
                }
            }
        }

        private void DisplayPath(Point[] path)
        {

            (this[path[0]] as Tile).DrawArrow(path[1] - path[0], Point.Zero);

            for (int i = 1; i < path.Length - 1; i++)
            {
                (this[path[i]] as Tile).DrawArrow(path[i + 1] - path[i], path[i - 1] - path[i]);
            }
            (this[path[path.Length-1]] as Tile).DrawArrow(Point.Zero, path[path.Length-2]-path[path.Length-1]);
        }

        /// <summary>
        /// Initializes the field.
        /// </summary>
        public virtual void InitField()
        {
            // Initialize the terrain
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    this[x, y] = new Tile(Terrain.Plains, x, y);
                }
            }
            
            //BattleGenerate(Terrain.Tundra,false, Terrain.Desert,true);
        }

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

        public void OnUnitFinishMoving(WalkingUnit walkingUnit)
        {
            walkingUnit.MovingUnit.PositionFromParent = Vector2.Zero;
            walkingUnit.MovingUnit.DrawingTexture.SelectRow(0);

            // Get the tiles to move the Units from/to
            Tile targetTile = this[walkingUnit.TargetPosition] as Tile;
            Tile originTile = this[walkingUnit.MovingUnit.PositionInGrid] as Tile;

            // Move the Unit from its tile to the target tile
            targetTile.SetUnit(walkingUnit.MovingUnit);
            if (walkingUnit.RemoveOnEnd)
                originTile.SetUnit(null);
        }

        /// <summary>
        /// As soon as you press the button for moving to a target position, this method will be called
        /// </summary>
        /// <param name="u"></param>
        /// <param name="targetPos">The target position to move to in grid coordinates</param>
        public void OnUnitStartMoving(Unit u, Point targetPos, bool remove = true)
        {
            // Can make Unit movement animated by not calling this instantly (or from update or something, idk)
            // TODO: Start animating here
            //u.DrawPosition = new Vector2(-50, -50);
            targetPosition = targetPos;
            walkingUnits.Add(new WalkingUnit(u, targetPos, remove));

            //walkingUnit.Vectors.Add(new Vector2(targetPos.X,targetPos.Y));
            Point[] tempPath = Pathfinding.GetPath(u, targetPosition);
            foreach (Point p in tempPath)
            {
                u.Vectors.Add(new Vector2(p.X, p.Y));
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

            // Unselecting a tile means the unit walking overlay will be non-existent and the Unit will no longer be animated
            if (p == InvalidTile)
            {
                SetUnitWalkingOverlay(null);
                SetUnitAttackingOverlay(null);

                // Stop animating the Unit on this tile
                if (SelectedTile != null && SelectedTile.Occupied)
                    SelectedTile.Unit.ShouldAnimate = true;
            }
            else
            {
                t.OverlayWalk = true;

                // Start animating the Unit on this tile
                if (t.Occupied)
                    t.Unit.ShouldAnimate = false;
            }

            // Select the new tile
            selectedTile = p;
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
                ForEach(obj => {
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
            ForEach(obj => (obj as Tile).OverlayWalk = false);

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

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            // Make sure that anything that was selected no longer is selected.
            SelectTile(InvalidTile);

            base.TurnUpdate(turn, player,t);

            // So the grid knows who is the current player. Useful for selecting units that are your own. 
            currentPlayer = player;
            ForEach(obj => {
                Tile tile = obj as Tile;
                if (tile.Occupied)
                {
                    // Makes the units go towards their target
                    Unit unit = tile.Unit;
                    if (unit.Owner != player) // End of turn for the player whose turn it is NOT right now.
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

            //If we have a selected unit; if there is a path to follow, we start walking using Walk(), otherwise we move to target location
            if (walkingUnits.Count > 0)
            {
                for (int i = walkingUnits.Count - 1; i >= 0; --i)
                {
                    WalkingUnit walkingUnit = walkingUnits[i];
                    if (walkingUnit.MovingUnit.Vectors.Count > 0)
                    {
                        Walk(walkingUnit.MovingUnit, walkingUnit.MovingUnit.Vectors[0]);
                    }
                    else
                    {
                        OnUnitFinishMoving(walkingUnit);
                        walkingUnits.Remove(walkingUnit);
                    }
                }
            }
        }

        /// <summary>
        /// Check whether the X and Y of the unit are greater of smaller than those of the target position.
        /// If so, we give it a velocity, making it move towards the target position, if not,
        /// we conclude the unit has reached its destination so we can remove this destination from our path.
        /// </summary>
        /// <param name="tarPosCoor">Coordinates to walk to</param>
        public void Walk(Unit walkingUnit, Vector2 tarPosCoor)
        {
            Vector2 unitPos = walkingUnit.PositionFromParent;
            Vector2 tarPos = 32 * (new Vector2(tarPosCoor.X,tarPosCoor.Y) - walkingUnit.PositionInGrid.ToVector2());
            Vector2 direction = tarPos - unitPos;
            Vector2 velocity = Vector2.Normalize(direction);

            if (direction == Vector2.Zero)
            {
                walkingUnit.Vectors.RemoveAt(0);
                return;
            }

            int selectedRow = (int) ((velocity.Y == 0) ? (3 - velocity.X) : (2 + velocity.Y));
            walkingUnit.DrawingTexture.SelectRow(selectedRow);

            /*if (tarPos.X > unitPos.X)
            {
                velocity = new Vector2(1, 0);
                //PopSprites = PopSpritesFront;
            }
            else if (tarPos.X < unitPos.X)
            {
                velocity = new Vector2(-1, 0);
                //PopSprites = PopSpritesFront;
            }
            else if (tarPos.Y > unitPos.Y)
            {
                velocity = new Vector2(0, 1);
                //PopSprites = PopSpritesFront;
            }
            else if (tarPos.Y < unitPos.Y)
            {
                velocity = new Vector2(0, -1);
                //PopSprites = PopSpritesBack;
            }
            else
            {
                velocity = Vector2.Zero;
                walkingUnit.Vectors.RemoveAt(0);
                //PopSprites = PopSpritesFront;
            }*/

            // Relative position needs to be without the positioningrid
            walkingUnit.PositionFromParent = walkingUnit.PositionFromParent + velocity * 4;
        }

        public bool ContainsWalkingUnits => walkingUnits.Count > 0;

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