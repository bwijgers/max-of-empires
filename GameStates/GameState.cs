using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameStates
{
    abstract class GameState
    {
        public abstract void Draw(GameTime time, SpriteBatch s);

        public virtual void HandleInput(InputHelper helper, KeyManager manager)
        {
        }

        public virtual void Reset()
        {
        }

        public abstract void Update(GameTime time);
    }
}
