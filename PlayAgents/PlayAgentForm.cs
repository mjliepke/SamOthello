using SamOthellop;
using SamOthellop.Model;
using SamOthellop.Model.Agents;
using SamOthellop.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayAgents
{
    public partial class PlayAgentForm : Form
    {
        private IOthelloAgent _myAgent;
        private PiecePanel[,] _boardPanels;
        private OthelloGame _myGame;
        private BoardStates _myPlayer;

        public PlayAgentForm()
        {
            InitializeComponent();
            _myGame = new OthelloGame();
            InitializeAgentSelectionComboBox();
            InitializePlayerSelectionComboBox();
            InitializeMinMaxSelectionComboBox();
            SetGameCompleteVisuals(false);

        }

        private void InitializePlayerSelectionComboBox()
        {
            PlayerSelectComboBox.Items.Add(BoardStates.black.ToString());
            PlayerSelectComboBox.Items.Add(BoardStates.white.ToString());
            PlayerSelectComboBox.SelectedIndex = 1;
        }

        private void InitializeAgentSelectionComboBox()
        {
            AgentDict dict = new AgentDict();
            AgentSelectComboBox.Items.AddRange(dict.AgentDictionary.Keys.ToArray());
            AgentSelectComboBox.SelectedIndex = 0;
        }

        private void InitializeMinMaxSelectionComboBox()
        {
            AgentDict dict = new AgentDict();
            MinMaxAgentComboBox.Items.AddRange(dict.AgentDictionary.Keys.ToArray());
            MinMaxAgentComboBox.Items.Remove(typeof(MinMaxAgent).Name);
            MinMaxAgentComboBox.Items.Remove(typeof(RandomAgent).Name);
            MinMaxAgentComboBox.SelectedIndex = 0;
        }

        private void EnableStartGameItems(bool enable = true)
        {
            SelectAgentLabel.Visible = enable;
            SelectAgentLabel.Enabled = enable;
            AgentSelectComboBox.Visible = enable;
            AgentSelectComboBox.Enabled = enable;
            MinMaxAgentComboBox.Visible = enable;
            MinMaxAgentComboBox.Enabled = enable;
            PlayerSelectComboBox.Visible = enable;
            PlayerSelectComboBox.Enabled = enable;
            StartGameButton.Visible = enable;
            StartGameButton.Enabled = enable;
        }

        private void SetupGame()
        {
            _myGame.ResetBoard();
            _myAgent = GetAgent();
            _myPlayer = PlayerSelectComboBox.SelectedItem.ToString() == BoardStates.white.ToString() ? BoardStates.black : BoardStates.white;
            _boardPanels = new PiecePanel[OthelloGame.BOARD_SIZE, OthelloGame.BOARD_SIZE];
            int tileSize = ((Size.Width > Size.Height) ? Size.Height - 45 : Size.Width - 45) / OthelloGame.BOARD_SIZE;

            for (int i = 0; i < OthelloGame.BOARD_SIZE; i++)
            {
                for (int j = 0; j < OthelloGame.BOARD_SIZE; j++)
                {
                    var newPanel = new PiecePanel(new int[] { i, j })
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * i, tileSize * j)
                    };

                    newPanel.MouseClick += new MouseEventHandler(OthelloPeice_Click);
                    Controls.Add(newPanel);
                    _boardPanels[i, j] = newPanel;

                    Color panelcolor = Color.Red;
                    if (BoardColorDictionary.BoardStateColors.TryGetValue(_myGame.GetBoard()[i, j], out panelcolor))
                    {
                        _boardPanels[i, j].ReColor(panelcolor);
                    }
                }
            }
            if (_myGame.WhosTurn != _myPlayer)
            {
                _myGame.MakeMove(_myAgent.MakeMove(_myGame, ~_myPlayer));
            }
        }

        private void SetGameCompleteVisuals(bool gameover = true)
        {
            if (_myGame.GameComplete)
            {
                bool won = _myGame.FinalWinner == _myPlayer;
                string wonStr = won ? "WON" : "LOST";
                string scoreStr = " " + _myGame.FinalBlackTally.ToString() + " - " + _myGame.FinalWhiteTally.ToString() + " ";
                GameOverLabel.Text = "The Game is Over, You " + wonStr + scoreStr;
            }
            GameOverLabel.Visible = gameover;
            RestartButton.Visible = gameover;
            RestartButton.Enabled = gameover;
        }

        private void RefreshControls()
        {
            for (int i = 0; i < OthelloGame.BOARD_SIZE; i++)
            {
                for (int j = 0; j < OthelloGame.BOARD_SIZE; j++)
                {
                    Color color;
                    BoardColorDictionary.BoardStateColors.TryGetValue(_myGame.GetBoard()[i, j], out color);
                    _boardPanels[i, j].ReColor(color);
                }
            }

            if (_myGame.GameComplete)
            {

            }
        }

        private IOthelloAgent GetAgent()
        {
            AgentDict dict = new AgentDict();
            Type agentType;
            dict.AgentDictionary.TryGetValue(AgentSelectComboBox.SelectedItem.ToString(), out agentType);
            IOthelloAgent agent;
            if (agentType == typeof(MinMaxAgent))
            {
                Type subAgentType;
                dict.AgentDictionary.TryGetValue(MinMaxAgentComboBox.SelectedItem.ToString(), out subAgentType);
                agent = new MinMaxAgent((IEvaluationAgent)Activator.CreateInstance(subAgentType));
            }
            else
            {
                agent = (IOthelloAgent)Activator.CreateInstance(agentType);
            }
            return agent;
        }

        private void AgentSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AgentSelectComboBox.SelectedItem.ToString().Equals(typeof(MinMaxAgent).Name.ToString()))
            {
                MinMaxAgentComboBox.Enabled = true;
                MinMaxAgentComboBox.Visible = true;
            }
            else
            {
                MinMaxAgentComboBox.Enabled = false;
                MinMaxAgentComboBox.Visible = false;
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            SetGameCompleteVisuals(false);
            EnableStartGameItems(true);
        }

        private void OthelloPeice_Click(object sender, MouseEventArgs e)
        {
            try
            {
                PiecePanel thisPanel = (PiecePanel)sender;
                if (_myGame.WhosTurn == _myPlayer && _myGame.ValidMove(_myPlayer, new byte[] { (byte)thisPanel.location[0], (byte)thisPanel.location[1] })) 
                {
                    _myGame.MakeMove(_myPlayer, new byte[] { (byte)thisPanel.location[0], (byte)thisPanel.location[1] });
                    _myGame.MakeMove(_myAgent.MakeMove(_myGame, ~_myPlayer));
                }
                else
                {
                    _myGame.MakeMove(~_myPlayer, _myAgent.MakeMove(_myGame, ~_myPlayer));
                }
                RefreshControls();
            }
            catch
            {
                throw new NotSupportedException("OthelloPeice_Click is not to be used with a control other than a PiecePanel");
            }
            if (_myGame.GameComplete)
            {
                SetGameCompleteVisuals();
            }
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            EnableStartGameItems(false);
            SetupGame();
        }

    }
}
