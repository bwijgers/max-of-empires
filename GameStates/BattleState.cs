using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaxOfEmpires.GameStates
{
    class BattleState : GameState
    {

        private Grid battleGrid;
        public BattleState()
        {
            battleGrid = new Grid(15, 15);
            battleGrid.InitField();
        }
        public override void Draw(GameTime time, SpriteBatch s)
        {
            battleGrid.Draw(time, s);
        }

        public override void Update(GameTime time)
        {
        }
    }
}
