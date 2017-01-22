using MaxOfEmpires.Buildings;
using MaxOfEmpires.GameObjects;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ebilkill.Gui;

namespace MaxOfEmpires
{
    class Tile : GameObjectDrawable
    {
        /// <summary>
        /// The current Building on this Tile. Can be null.
        /// </summary>
        private Building building;

        /// <summary>
        /// The Terrain of this Tile.
        /// </summary>
        private Terrain terrain;

        /// <summary>
        /// The current Unit on this Tile. Can be null.
        /// </summary>
        private Unit unit;

        /// <summary>
        /// Whether certain overlays should be drawn.
        /// </summary>
        private bool overlayAttack, overlayWalk;

        /// <summary>
        /// The x and y positions of this Tile in the containing Grid.
        /// </summary>
        private int x, y;

        public bool hills;

        public bool Mountain;

        private Texture2D terrainTexture;

        private Rectangle terrainSource; //TODO vervang door stuff

        /// <summary>
        /// Creates a new Tile at a specified position with a specified Terrain.
        /// </summary>
        /// <param name="terrain">The Terrain that this Tile should have.</param>
        /// <param name="x">The x-coord of the position that this Tile should be at.</param>
        /// <param name="y">The y-coord of the position that this Tile should be at.</param>
        public Tile(Terrain terrain, int x, int y)
        {
            this.terrain = terrain;
            this.x = x;
            this.y = y;
            position = new Vector2(x * 32, y * 32);
            overlayAttack = overlayWalk = false;
            terrainTexture = AssetManager.Instance.getAsset<Texture2D>("FE-Sprites/Terrain@5x4");
        }

        /// <summary>
        /// The movement cost for a specified Unit to move to this Tile.
        /// </summary>
        /// <param name="unit">The Unit for which you want to know the movement cost.</param>
        /// <returns>The movement cost on this Tile for the specified Unit.</returns>
        public int Cost(Unit unit)
        {
            if (!Passable(unit))
            {
                return int.MaxValue;
            }
            if (unit.id == "builder")
            {
                return 1;
            }
            int terrainCost = terrain.Cost;
            if (!hills)
                return terrainCost;
            return 1+terrainCost;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            DrawBackground(time, s);
            DrawForeground(time, s);
        }

        public void DrawBackground(GameTime time, SpriteBatch s)
        {
            //terrain.Draw(DrawPosition.ToPoint(), s);
            TerrainSpriteSelect();
            TerrainDraw(s);
            building?.Draw(time, s);

            // Draw a walking overlay if it should be drawn
            if (overlayWalk)
            {
                DrawingHelper.Instance.DrawRectangle(s, Bounds, new Color(0x00, 0x00, 0xFF, 0x88));
            }

            // Draw an attacking overlay if it should be drawn
            if (overlayAttack)
            {
                DrawingHelper.Instance.DrawRectangle(s, Bounds, new Color(0xFF, 0x00, 0x00, 0x88));
            }
        }

        public void DrawForeground(GameTime time, SpriteBatch s)
        {
            unit?.Draw(time, s);
        }
        
        private void TerrainSpriteSelect()
        {
            if(terrain == Terrain.Plains&&!hills)
            {
                SelectSprite(3, 1);
            }
            else if (terrain == Terrain.Plains && hills)
            {
                SelectSprite(2, 1);
            }
            else if (terrain == Terrain.Forest && hills)
            {
                SelectSprite(4, 1);
            }
            else if (terrain == Terrain.Forest && !hills)
            {
                SelectSprite(5, 1);
            }
            else if (terrain == Terrain.Desert && !hills)
            {
                SelectSprite(3, 3);
            }
            else if (terrain == Terrain.Desert && hills)
            {
                SelectSprite(2, 3);
            }
            else if (terrain == Terrain.Tundra && !hills)
            {
                SelectSprite(3, 2);
            }
            else if (terrain == Terrain.Tundra && hills)
            {
                SelectSprite(2, 2);
            }
            else if (terrain == Terrain.Swamp && !hills)
            {
                SelectSprite(5, 2);
            }
            else if (terrain == Terrain.Swamp && hills)
            {
                SelectSprite(4, 2);
            }
            else if (terrain == Terrain.Jungle && !hills)
            {
                SelectSprite(5, 3);
            }
            else if (terrain == Terrain.Jungle && hills)
            {
                SelectSprite(4, 3);
            }
            else if (terrain == Terrain.Lake)
            {
                SelectSprite(1, 4);
            }
            else if (terrain == Terrain.Mountain)
            {
                SelectSprite(1, 1);
            }
            else if (terrain == Terrain.TundraMountain)
            {
                SelectSprite(1, 2);
            }
            else if (terrain == Terrain.DesertMountain)
            {
                SelectSprite(1, 3);
            }

        }

        private void SelectSprite(int x,int y)
        {
            terrainSource = new Rectangle((x-1) * 32, (y-1) * 32, 32, 32);
        }

        private void TerrainDraw(SpriteBatch s)
        {
            s.Draw(terrainTexture, DrawPosition, terrainSource, Color.White);
        }

        /// <summary>
        /// Whether or not this Tile is passable for a certain unit. 
        /// </summary>
        /// <param name="unit">The Unit for which you want to know if this Tile is passable.</param>
        /// <returns>True if the Unit can pass through this Tile, false otherwise.</returns>
        public bool Passable(Unit unit)
        {
            // Enemy unit here
            if (Occupied && this.unit.Owner != unit.Owner)
            {
                if (this.unit is Soldier)
                {
                    // Enemy soldier here
                    if (!(this.unit as Soldier).IsDead)
                    {
                        // Alive? Can't pass
                        return false;
                    }
                    // Dead? Depends on terrain
                }
                else
                {
                    // Enemy unit that is not soldier, definitely alive
                    return false;
                }
            }
            return unit.Passable(terrain);
        }

        /// <summary>
        /// Sets a Unit on this tile. Also updates that Unit's GridPos to this Tile's position.
        /// </summary>
        /// <param name="u">The Unit to set on this Tile.</param>
        public void SetUnit(Unit u)
        {
            // Check to make sure the unit is not overriding another unit.
            if (Occupied && u != null)
                return;

            // Set the unit 
            unit = u;

            // Set the unit's position and parent, if it is not null
            if(unit != null)
            {
                unit.PositionInGrid = new Point(x, y);
                unit.Parent = this;
            }
        }

        public override void TurnUpdate(uint turn, Player player)
        {
            // Update the Unit at this position if it exists.
            if (Occupied)
                Unit.TurnUpdate(turn, player);

            // Update the Building at this position if it exists.
            if (BuiltOn)
                Building.TurnUpdate(turn, player);
        }

        public override void Update(GameTime time)
        {
            building?.Update(time);
            unit?.Update(time);
        }

        /// <summary>
        /// The Building built on this Tile.
        /// </summary>
        public Building Building
        {
            get
            {
                return building;
            }
            set
            {
                building = value;
                value.Parent = this;
            }
        }

        /// <summary>
        /// Whether there is a Building on this Tile.
        /// </summary>
        public bool BuiltOn => building != null;

        /// <summary>
        /// The position in the Grid of this Tile.
        /// </summary>
        public Point PositionInGrid => new Point(x, y);

        /// <summary>
        /// Whether there is a Unit on this Tile.
        /// </summary>
        public bool Occupied => Unit != null;

        /// <summary>
        /// True when the attacking overlay on this tile should be shown, false otherwise.
        /// </summary>
        public bool OverlayAttack
        {
            get
            {
                return overlayAttack;
            }
            set
            {
                overlayAttack = value;
            }
        }

        /// <summary>
        /// True when the walking overlay on this tile should be shown, false otherwise.
        /// </summary>
        public bool OverlayWalk
        {
            get
            {
                return overlayWalk;
            }
            set
            {
                overlayWalk = value;
            }
        }

        public override Vector2 Size
        {
            get
            {
                return new Vector2(32, 32);
            }
        }

        /// <summary>
        /// The Terrain of this Tile.
        /// </summary>
        public Terrain Terrain {
            get
            {
                return terrain;
            }
            set
            {
                terrain = value;
            }
        }

        /// <summary>
        /// The Unit occupying this Tile.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return unit;
            }
        }
    }
}
