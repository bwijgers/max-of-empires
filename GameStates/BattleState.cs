﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Ebilkill.Gui.Elements;

namespace MaxOfEmpires.GameStates
{
    
    class BattleState : GameState
    {
        private Grid battleGrid;
        private Overlays.OverlayBattleState overlay;
        private uint turnNum;
        private bool currentPlayer; // false if player 2, true if player 1.
        private bool shouldTurnUpdate; // True if the next update should initiate a turn update.

        public BattleState()
        {
            // Initialize the battlefield
            battleGrid = new Grid(15, 15);
            battleGrid.InitField();

            // Initialize the overlay
            overlay = new Overlays.OverlayBattleState();
            InitOverlay();

            // Player 1 starts
            currentPlayer = true;

            // Turn number starts at 0
            turnNum = 0;
        }

        public override void Draw(GameTime time, SpriteBatch s)
        {
            battleGrid.Draw(time, s);
            overlay.draw(s);
        }

        public override void HandleInput(InputHelper helper, KeyManager manager)
        {
            battleGrid.HandleInput(helper, manager);
            overlay.update(helper);
        }

        private void InitOverlay()
        {
            GuiButton buttonEndTurn = GuiButton.createButtonWithLabel(new Point(500, 10), "End turn", null, "font");
            buttonEndTurn.ClickHandler = () => shouldTurnUpdate = true;
            overlay.addElement(buttonEndTurn);
        }

        public void TurnUpdate()
        {
            currentPlayer = !currentPlayer;
            battleGrid.TurnUpdate(turnNum, currentPlayer);
        }

        public override void Update(GameTime time)
        {
            battleGrid.Update(time);
            if (shouldTurnUpdate)
            {
                shouldTurnUpdate = false;
                TurnUpdate();
            }
        }

        public Grid BattleGrid => battleGrid;
    }
}
