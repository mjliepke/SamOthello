using SamOthellop.Model;
using SamOthellop.Model.Agents;
using SamOthellop.Model.Genetic;
using SamOthellop.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamOthellop
{
    public partial class TestForm : Form
    {
        private OthelloGame _myGame;
        private PiecePanel[,] _boardPanels;
        private System.Timers.Timer _twoSecondTimer;
        private BoardNeuralNet _net;
        private IOthelloAgent _myAgent;
        private int _currentViewedMove;

        public TestForm()
        {
            InitializeComponent();
            _twoSecondTimer = new System.Timers.Timer(2000);
            _net = new BoardNeuralNet();
        }

        private void OthelloForm_Load(object sender, EventArgs e)
        {
            SetupBoard();
            SetupInstructions();
            SetupProgressBar();
        }

        private void SetupBoard()
        {
            _myGame = new OthelloGame();
            _myAgent = new MinMaxAgent();
            _boardPanels = new PiecePanel[OthelloGame.BOARD_SIZE, OthelloGame.BOARD_SIZE];
            _currentViewedMove = 0;
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
        }

        private void SetupInstructions()
        {
            string curDir = Directory.GetCurrentDirectory();
            InstructionBrowser.Navigate(new Uri(String.Format("file:///{0}/View/Instructions.html", curDir)));
        }

        private void SetupProgressBar()
        {
            GameCompletionProgressBar.Maximum = _myGame.MaxMoves;
            GameCompletionProgressBar.Value = _myGame.GetMovesMade();
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
                    _currentViewedMove = _myGame.GetMovesMade();
                }
            }

            if (_myGame.GameComplete)
            {
                GameOverLabel.Visible = true;
                _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(GameOver_VisibilityFalse);
                _twoSecondTimer.Enabled = true;
            }
        }

        private void RefreshControls(BoardStates[,] bstate)
        {
            for (int i = 0; i < bstate.GetLength(0); i++)
            {
                for (int j = 0; j < bstate.GetLength(1); j++)
                {
                    Color color;
                    BoardColorDictionary.BoardStateColors.TryGetValue(bstate[i, j], out color);
                    _boardPanels[i, j].ReColor(color);

                }
            }
        }

        private void OthelloPeice_Click(object sender, MouseEventArgs e)
        {
            BoardStates player = e.Button == MouseButtons.Left ? BoardStates.black : BoardStates.white;
            try
            {
                PiecePanel thisPanel = (PiecePanel)sender;
                if (player != _myGame.WhosTurn)
                {
                    if (player == BoardStates.white)
                    {
                        BlackMoveLabel.Visible = true;
                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(BlackMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                    else if (player == BoardStates.black)
                    {
                        WhiteMoveLabel.Visible = true;

                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(WhiteMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                }
                _myGame.MakeMove(player, new byte[] { (byte)thisPanel.location[0], (byte)thisPanel.location[1] });
                RefreshControls();
            }
            catch
            {
                throw new NotSupportedException("OthelloPeice_Click is not to be used with a control other than a PiecePanel");
            }
            GameCompletionProgressBar.Value = _myGame.GetMovesMade();
        }

        private void BlackMoveLabel_VisibilityFalse(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                BlackMoveLabel.Visible = false;
            }));
        }

        private void WhiteMoveLabel_VisibilityFalse(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                WhiteMoveLabel.Visible = false;
            }));
        }

        private void GameOver_VisibilityFalse(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                GameOverLabel.Visible = false;
            }));
        }

        private void GameCountTextBox_ColorControl(object sender, EventArgs e)
        {
            GameCountTextBox.BackColor = SystemColors.Control;
        }

        private void RandomMoveButton_Click(object sender, EventArgs e)
        {
            IOthelloAgent myAgent = new RandomAgent();
            _myGame.MakeMove(myAgent.MakeMove(_myGame, _myGame.WhosTurn));
            RefreshControls();
        }

        private void RandomGameButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {
                IOthelloAgent myAgent = new RandomAgent();
                while (!_myGame.GameComplete)
                {
                    _myGame.MakeMove(myAgent.MakeMove(_myGame, _myGame.WhosTurn));
                }
                Invoke(new Action(() =>
                {
                    RefreshControls();
                    EnableControlButtons();
                }));
            }).Start();
        }

        private void ClearBoardButton_Click(object sender, EventArgs e)
        {
            _myGame.ResetBoard();
            GameOverLabel.Visible = false;
            RefreshControls();
        }

        private void RunGamesButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GameCountTextBox.Text.Length; i++)
            {
                if (!char.IsNumber((char)(GameCountTextBox.Text[i])))
                {
                    GameCountTextBox.BackColor = Color.Red;
                    _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(GameCountTextBox_ColorControl);
                    _twoSecondTimer.Enabled = true;
                    return;
                }
            }

            int gameCount = Convert.ToInt32(GameCountTextBox.Text);
            EnableControlButtons(false);

            new Thread(() =>
            {
                IOthelloAgent myAgent = new RandomAgent();
                for (int i = 0; i < gameCount; i++)
                {
                    while (!_myGame.GameComplete)
                    {
                        _myGame.MakeMove(myAgent.MakeMove(_myGame, _myGame.WhosTurn));
                    }
                    Invoke(new Action(() =>
                    {
                        RefreshControls();
                    }));
                    _myGame.ResetBoard();
                    Thread.Sleep(5);
                }
                Invoke(new Action(() =>
                {
                    RefreshControls();
                    EnableControlButtons();
                }));
            }).Start();

        }

        private void EnableControlButtons(bool enable = true)
        {
            foreach (PiecePanel b in _boardPanels)
            {
                if (b.GetType() == typeof(Button))
                {
                    b.Enabled = enable;
                }
            }
            foreach (Control b in ControlGroupBox.Controls)
            {
                if (b.GetType() == typeof(Button))
                {
                    b.Enabled = enable;
                }
            }
        }

        private void RunNet_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                BoardNeuralNet net = new BoardNeuralNet();
                _net.StartTest();
            }).Start();
        }

        private void EducatedPlayButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {
                int unused = 0;
                _myAgent = new MinMaxAgent(new HeuristicAgent(), Convert.ToInt32(MoveDepth.Text));
                _myGame.MakeMove(_myGame.WhosTurn, _myAgent.MakeMove(_myGame, _myGame.WhosTurn));

                Invoke(new Action(() =>
                {
                    RefreshControls();
                    EnableControlButtons();
                }));
            }).Start();
        }

        private void FileLoadButton_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                string path = @"C:\Users\mjlie\source\repos\SamOthellop\SamOthellop\Database";

                Tests.TestThorFileReading(path);
            }).Start();
        }

        private void PreviousMoveButton_Click(object sender, EventArgs e)
        {
            _currentViewedMove -= 1;
            BoardStates[,] bstate = _myGame.GetBoardAtMove(_currentViewedMove);
            RefreshControls(bstate);
        }

        private void NextMoveButton_Click(object sender, EventArgs e)
        {
            if (_currentViewedMove == _myGame.GetMovesMade()) { return; }
            _currentViewedMove += 1;
            BoardStates[,] bstate = _myGame.GetBoardAtMove(_currentViewedMove);
            RefreshControls(bstate);
        }

        private void MiniMaxTestButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {

                Tests.TestMinMax(_myGame, Convert.ToInt32(MoveDepth.Text));
                Invoke(new Action(() =>
                {
                    EnableControlButtons();
                }));
            }).Start();
        }

        private void GetSafePeiceButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("White Safe Peices : " + _myGame.GetSafePeiceCountEstimation(BoardStates.white));
            Console.WriteLine("Black Safe Peices : " + _myGame.GetSafePeiceCountEstimation(BoardStates.black));

        }

        private void GAButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {
                Evolution.RunRecursiveGeneticAlgorithm();
                Invoke(new Action(() =>
                {
                    EnableControlButtons();
                }));
            }).Start();

        }

        private void EvaluateFitnessButton_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(@"E:\Source\SamOthellop\SamOthellop\Model\Agents\Genetic", "*.dat");

            foreach (string file in files)
            {
                Evolution.PrintChromosomeFitness(file, new RandomAgent());
                Evolution.PrintChromosomeFitness(file, new GreedyAgent());
                Evolution.PrintChromosomeFitness(file, new MinMaxAgent(new GreedyAgent(), 2));
            }
        }

        private void NormalizedGeneTestButton_Click(object sender, EventArgs e)
        {
            Tests.TestNormalizeGenes();
        }
    }
}


