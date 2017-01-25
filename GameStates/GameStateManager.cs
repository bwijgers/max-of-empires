using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MaxOfEmpires.Units;
using System;

namespace MaxOfEmpires.GameStates
{
    class GameStateManager
    {
        private static GameState currentState;
        private static Vector2 gridSize = new Vector2(0, 0);
        private static Dictionary<string, GameState> stateDict = new Dictionary<string, GameState>();

        public static void AddState(string name, GameState state)
        {
            stateDict.Add(name, state);
        }

        public static void Draw(GameTime time, SpriteBatch gameObjectS, SpriteBatch overlayS)
        {
            CurrentState?.Draw(time, gameObjectS, overlayS);
        }

        private static GameState GetState(string name)
        {
            if (stateDict.ContainsKey(name))
            {
                return stateDict[name];
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        public static void HandleInput(InputHelper helper, KeyManager keys)
        {
            CurrentState?.HandleInput(helper, keys);
        }

        /// <summary>
        /// Called when a player initiates a battle from the economy state.
        /// </summary>
        /// <param name="attackingArmy">The army that initiated the attack.</param>
        /// <param name="defendingArmy">The army that was attacked.</param>
        public static void OnInitiateBattle(Army attackingArmy, Army defendingArmy, Tile attackingTile, Tile defendingTile)
        {
            AssetManager.Instance.PlayMusic("Music/Legend of the Dark Lord");

            CurrentState.FadeOut = true;
            // Get the battle state
            BattleState state = (BattleState)GetState("battle");

            // Create the new battle grid
            state.OnInitiateBattle(attackingArmy, defendingArmy, attackingTile, defendingTile);

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

            AssetManager.Instance.PlayMusic("Music/Village of Emerdale");
        }

        public static void OnPlayerWinGame(Player owner)
        {
            CurrentState.FadeOut = true;

            // Get the win state
            PlayerWinState state = (PlayerWinState)GetState("playerWin");
            state.OnPlayerWinGame(owner);

            // Switch to the win state
            state.FadeIn = true;
            SwitchState("playerWin", false);

            AssetManager.Instance.PlayMusic("Music/FEvictory", false);

        }

        public static void OnRequestStatistics()
        {
            CurrentState.FadeOut = true;

            GetState("mainMenu").FadeIn = true;
            SwitchState("mainMenu", false);

            AssetManager.Instance.PlayMusic("Music/Village of Emerdale");
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

                // Get the grid size, if it exists
                if (CurrentState != null)
                {
                    if (CurrentState is EconomyState)
                    {
                        gridSize = (CurrentState as EconomyState).EconomyGrid.Size.ToVector2();
                    }
                    else if (CurrentState is BattleState)
                    {
                        gridSize = (CurrentState as BattleState).BattleGrid.Size.ToVector2();
                    }
                }
                return;
            }
            throw new KeyNotFoundException("GameState with name '" + name + "' does not exist.");
        }

        public static void Update(GameTime time)
        {
            CurrentState?.Update(time);
        }

        public static void UpdateResolution()
        {
            foreach (GameState state in stateDict.Values)
            {
                state.ResetOverlay();
            }
        }

        private static GameState CurrentState => currentState;
        public static Vector2 GridSize => gridSize;
    }
}
