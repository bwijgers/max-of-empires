using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.GameStates
{
    class GameStateManager
    {
        private static GameState currentState;
        private static Dictionary<string, GameState> stateDict = new Dictionary<string, GameState>();
        private static Vector2 gridSize = new Vector2(0, 0);

        public static void AddState(string name, GameState state)
        {
            stateDict.Add(name, state);
        }

        public static void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            CurrentState?.Draw(time, gameObjectS, overlayS);
        }

        public static void HandleInput(InputHelper helper, KeyManager keys)
        {
            CurrentState?.HandleInput(helper, keys);
        }

        public static void SwitchState(string name)
        {
            if (stateDict.ContainsKey(name))
            {
                currentState = stateDict[name];
                CurrentState.Reset();
                return;
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        public static void Update(GameTime time)
        {
            CurrentState?.Update(time);
            if (CurrentState != null)
            {
                gridSize = CurrentState.GetCurrentGridSize();
            }
        }
        private static GameState CurrentState => currentState;
        public static Vector2 GridSize => gridSize;
    }
}
