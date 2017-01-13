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

        private static GameState GetState(string name)
        {
            if (stateDict.ContainsKey(name))
            {
                return stateDict[name];
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        /// <summary>
        /// Called when a player initiates a battle from the economy state.
        /// </summary>
        /// <param name="attackingArmy">The army that initiated the attack.</param>
        /// <param name="defendingArmy">The army that was attacked.</param>
        public static void OnInitiateBattle(Army attackingArmy, Army defendingArmy)
        {
            CurrentState.FadeOut = true;
            // Get the battle state
            BattleState state = (BattleState)GetState("battle");

            // Create the new battle grid
            state.OnInitiateBattle(attackingArmy, defendingArmy);

            // Switch to the battle state
            state.FadeIn = true;
            SwitchState("battle", true);
        }

        /// <summary>
        /// Called when a player wins a battle from the battle state.
        /// </summary>
        /// <param name="remainingArmy">The remaining Army on the battle field.</param>
        public static void OnPlayerWinBattle(Army remainingArmy)
        {
            CurrentState.FadeOut = true;
            EconomyState state = GetState("economy") as EconomyState;
            state.OnPlayerWinBattle(remainingArmy);
            state.FadeIn = true;
            SwitchState("economy", false);
        }

        /// <summary>
        /// Switches the current state for another state. Waits until the fade out is complete, if applicable.
        /// </summary>
        /// <param name="name">The name of the state to switch to.</param>
        /// <param name="reset">Whether the state should be reset on switch.</param>
        /// <param name="force">True when the fade out should be ignored.</param>
        public static void SwitchState(string name, bool reset, bool force = false)
        {
            if (CurrentState != null && CurrentState.FadeOut && !force)
            {
                // We need to wait until the fade-out is finished
                CurrentState.fadeoutEndCallback = () => 
                    SwitchState(name, reset, true);
                return;
            }

            // Check if the state to switch to exists
            if (stateDict.ContainsKey(name))
            {
                // Set the state and reset it if necessary
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
    }
}
