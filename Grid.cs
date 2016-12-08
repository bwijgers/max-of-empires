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
    class Grid : GameObjectGrid
    {
        private bool currentPlayer;

        /// <summary>
        /// The coords of the currently selected Tile within the grid.
        /// </summary>
        private Point selectedTile;

        private Point[] walkablePositions;

        public Grid(int width, int height, string id = "") : base(width, height, id)// TODO: make this load from a file or something similar
        {
            selectedTile = InvalidTile;
            currentPlayer = true;
        }

        public bool CheckMoveUnit(Point newPos, Unit unit)
        {
            Tile tile = (GameWorld as Grid)[newPos] as Tile;
            Tile oriTile = (GameWorld as Grid)[unit.GridPos] as Tile;
            if (!tile.Occupied && unit.Move(newPos.X, newPos.Y))
            {
                tile.SetUnit(unit);
                oriTile.SetUnit(null);
                return true;
            }

            return false;
        }

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
                return false;
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

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);

            // Draws an overlay so the player knows which unit is selected.
            Ebilkill.Gui.DrawingHelper.Instance.DrawRectangle(s, new Rectangle(new Point(selectedTile.X * 32, selectedTile.Y * 32), new Point(32)), new Color(0x00, 0x00, 0xFF, 0x88));

            // Add an overlay to all positions the selected Unit can walk to, if there are such positions.
            if (walkablePositions != null)
            {
                foreach (Point p in walkablePositions)
                    Ebilkill.Gui.DrawingHelper.Instance.DrawRectangle(s, new Rectangle(new Point(p.X * 32, p.Y * 32), new Point(32)), new Color(0x00, 0x00, 0xFF, 0x88));
            }
        }

        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Check if the player clicked
            if (helper.MouseLeftButtonPressed)
            {
                // Get the current grid position the player clicked at
                Point gridPos = (helper.MousePosition / 32).ToPoint();

                // Just unselect this tile if the user clicks this again.
                if (gridPos.Equals(selectedTile) || !IsInGrid(gridPos))
                {
                    SelectTile(InvalidTile);
                    return;
                }

                // Get the tile that was clicked
                Tile clickedTile = this[gridPos] as Tile;

                // If the player had a tile selected and it contains a Unit...
                if (SelectedTile != null && SelectedTile.Occupied)
                {
                    // ... move the Unit there, if the square is not occupied and the unit is capable, then unselect the tile.
                    SelectedTile.Unit.setTarget = gridPos;
                    Point movePos = SelectedTile.Unit.MoveTowardsTarget();

                    if(CheckMoveUnit(movePos, SelectedTile.Unit) || CheckAttackUnit(gridPos, SelectedTile.Unit))
                    {
                        SelectTile(InvalidTile);
                        return;
                    }
                }

                // Check if the player clicked a tile with a Unit on it, and select it if it's there. 
                else if (clickedTile.Occupied && clickedTile.Unit.Owner == currentPlayer && clickedTile.Unit.HasAction)
                {
                    SelectTile(gridPos);
                    walkablePositions = clickedTile.Unit.ReachableTiles();
                }
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

            // Place a swordsman for each player on the field.
            (this[4, 4] as Tile).SetUnit(new Units.Swordsman(4, 4, true));
            (this[10, 10] as Tile).SetUnit(new Units.Swordsman(10, 10, false));
        }

        /// <summary>
        /// Selects a Tile within this Grid.
        /// </summary>
        /// <param name="p">The position of the Tile to select.</param>
        public void SelectTile(Point p)
        {
            selectedTile = p;
            if (selectedTile == InvalidTile)
            {
                this.walkablePositions = null;
            }
        }

        public override void TurnUpdate(uint turn, bool player)
        {
            base.TurnUpdate(turn, player);

            // So the grid knows who is the current player. Useful for selecting units that are your own. 
            this.currentPlayer = player;

            //makes the units go towards their target
            ForEach((obj, x, y) => {
                Tile tile = obj as Tile;
                if(tile.Occupied)
                {
                    Units.Unit unit = tile.Unit;
                    if(unit.Owner != player)
                    {
                        Point movePos = unit.MoveTowardsTarget();
                        CheckMoveUnit(movePos, unit);
                    }
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
