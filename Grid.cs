using MaxOfEmpires.GameObjects;
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
        /// <summary>
        /// The coords of the currently selected Tile within the grid.
        /// </summary>
        private Point selectedTile;

        private bool currentPlayer;

        public Grid(int width, int height, string id = "") : base(width, height, id)// TODO: make this load from a file or something similar
        {
            selectedTile = InvalidTile;
            currentPlayer = true;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            base.Draw(time, s);

            // Draws an overlay so the player knows which unit is selected.
            Ebilkill.Gui.DrawingHelper.Instance.DrawRectangle(s, new Rectangle(new Point(selectedTile.X * 32, selectedTile.Y * 32), new Point(32)), new Color(0x00, 0x00, 0xFF, 0x88));

            // TODO: add drawing to all positions the player can place the unit at.
        }

        public override void HandleInput(InputHelper helper, KeyManager keyManager)
        {
            // Check if the player clicked
            if (helper.MouseLeftButtonPressed)
            {
                // Get the current grid position the player clicked at
                Point gridPos = (helper.MousePosition / 32).ToPoint();

                // Just unselect this tile if the user clicks this again.
                if (gridPos.Equals(selectedTile))
                {
                    selectedTile = InvalidTile;
                    return;
                }

                // If the player had a tile selected and it contains a Unit...
                if (SelectedTile != null && SelectedTile.Occupied)
                {
                    // ... move the Unit there, if the square is not occupied and the unit is capable...
                    if (!(this[gridPos] as Tile).Occupied && SelectedTile.Unit.Move(gridPos.X, gridPos.Y))
                    {
                        (this[gridPos] as Tile).SetUnit(SelectedTile.Unit);
                        SelectedTile.SetUnit(null);
                    }

                    // ... And set the selected tile back to an invalid tile.
                    selectedTile = InvalidTile;
                    return;
                }

                // Check if the tile they clicked is valid;
                if (gridPos.X < 0 || gridPos.X >= Width || gridPos.Y < 0 || gridPos.Y > Height)
                {
                    // if it's not, reflect this in the selectedTile;
                    selectedTile = InvalidTile;
                }
                else if ((this[gridPos] as Tile).Occupied && (this[gridPos] as Tile).Unit.Owner == currentPlayer)
                {
                    // if it is, make sure the selected tile is the tile the player clicked, if there is a Unit here.
                    selectedTile = gridPos;
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

        public override void TurnUpdate(uint turn, bool player)
        {
            base.TurnUpdate(turn, player);

            // So the grid knows who is the current player. Useful for selecting units that are your own. 
            this.currentPlayer = player;
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
