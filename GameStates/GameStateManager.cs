using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxOfEmpires.Units;

namespace MaxOfEmpires.GameStates
{
    class GameStateManager
    {
        private static GameState currentState;
        private static Dictionary<string, GameState> stateDict = new Dictionary<string, GameState>();

        public static void AddState(string name, GameState state)
        {
            stateDict.Add(name, state);
        }

        public static void Draw(GameTime time, SpriteBatch s)
        {
            CurrentState?.Draw(time, s);
        }

        public static void HandleInput(InputHelper helper, KeyManager keys)
        {
            CurrentState?.HandleInput(helper, keys);
        }

        public static GameState GetState(string name)
        {
            if (stateDict.ContainsKey(name))
            {
                return stateDict[name];
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        public static void SwitchState(string name, bool reset)
        {
            if (stateDict.ContainsKey(name))
            {
                currentState = stateDict[name];
                if (reset)
                {
                    CurrentState.Reset();
                }
                return;
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        public static void Update(GameTime time)
        {
            CurrentState?.Update(time);
        }

        private static GameState CurrentState => currentState;

        public static void OnInitiateBattle(Army attackingArmy, Army defendingArmy)
        {
            // Get the battle state
            BattleState state = (BattleState)GetState("battle");

            // Create the new battle grid
            state.OnInitiateBattle(attackingArmy, defendingArmy);

            // Switch to the battle state
            SwitchState("battle", true);
        }

        public static void OnPlayerWinBattle(Army remainingArmy)
        {
            EconomyState state = GetState("economy") as EconomyState;
            state.OnPlayerWinBattle(remainingArmy);
            SwitchState("economy", false);
        }
    }
}
