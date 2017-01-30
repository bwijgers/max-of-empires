using MaxOfEmpires.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace MaxOfEmpires.Units
{
    class Builder : Unit
    {
        public Builder(int x, int y, Player owner) : base(x, y, owner, "unit.builder")
        {
            DrawingTexture = AssetManager.Instance.getAsset<Spritesheet>(@"FE-Sprites\Units\Builder" + owner.ColorName + "@4x5");
            moveSpeed = 1;
        }
        
        public override bool Passable(Terrain terrain)
        {
            return true;
        }

        public static Builder LoadFromFile(BinaryReader reader, List<Player> players)
        {
            // Read position
            int x = reader.ReadInt16();
            int y = reader.ReadInt16();

            // Read Owner name
            string ownerName = reader.ReadString();

            // Get the actual owner
            Player owner = players.Find(p => p.Name.Equals(ownerName));

            // Create the builder
            Builder retVal = new Builder(x, y, owner);

            // Read moves left, id, and target
            retVal.movesLeft = reader.ReadByte();
            retVal.id = reader.ReadString();
            retVal.TargetPosition = new Point(reader.ReadInt16(), reader.ReadInt16());

            // Return the builder
            return retVal;
        }
    }
}
