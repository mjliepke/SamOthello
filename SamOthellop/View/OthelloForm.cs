using SamOthellop.Model;
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
    public partial class OthelloForm : Form
    {
        private OthelloGame _myGame;
        private PiecePanel[,] _boardPanels;
        private System.Timers.Timer _twoSecondTimer;
        private BoardNeuralNet _net;
        private int _currentViewedMove;

        public OthelloForm()
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
            GameCompletionProgressBar.Value = _myGame.MovesMade();
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
                    _currentViewedMove = _myGame.MovesMade();
                }
            }

            if (_myGame.GameOver())
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
                if (!player.Equals(_myGame.WhosTurn))
                {
                    if (player.Equals(BoardStates.white))
                    {
                        BlackMoveLabel.Visible = true;
                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(BlackMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                    else if (player.Equals(BoardStates.black))
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
            GameCompletionProgressBar.Value = _myGame.MovesMade();
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
            _myGame.MakeRandomMove();
            RefreshControls();
        }

        private void RandomGameButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {
                while (!_myGame.GameOver())
                {
                    _myGame.MakeRandomMove();
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
                for (int i = 0; i < gameCount; i++)
                {
                    while (!_myGame.GameOver())
                    {
                        _myGame.MakeRandomMove();
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
                _myGame.MakeMove(_myGame.WhosTurn, BoardNeuralNet.PredictBestMove(Convert.ToInt32(MoveDepth.Text), _myGame, _myGame.WhosTurn));

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

                var totalstopwatch1 = Stopwatch.StartNew();
                List<OthelloGame> method1Games = FileIO.ReadAllGames(path);
                totalstopwatch1.Stop();
                Console.WriteLine("Elapsed time for Method1 : {0}", totalstopwatch1.Elapsed);
                var method1Count = method1Games.Count();

                var totalstopwatch2 = Stopwatch.StartNew();
                List<OthelloGame> method2Games = FileIO.ReadAllGames(path);
                totalstopwatch2.Stop();
                Console.WriteLine("Elapsed time for Method2 : {0}", totalstopwatch2.Elapsed);
                var method2Count = method2Games.Count();

                var totalstopwatch3 = Stopwatch.StartNew();
                List<OthelloGame> method3Games = FileIO.ReadAllGames(path);
                totalstopwatch3.Stop();
                Console.WriteLine("Elapsed time for Method3 : {0}", totalstopwatch3.Elapsed);
                var method3Count = method3Games.Count();

                var totalstopwatch4 = Stopwatch.StartNew();
                List<OthelloGame> method4Games = FileIO.ReadAllGames(path);
                totalstopwatch4.Stop();
                Console.WriteLine("Elapsed time for Method4 : {0}", totalstopwatch4.Elapsed);
                var method4Count = method4Games.Count();

                Console.WriteLine("Method1: " + method1Count + " games");
                Console.WriteLine("Method2: " + method2Count + " games");
                Console.WriteLine("Method3: " + method3Count + " games");
                Console.WriteLine("Method4: " + method4Count + " games");

                Console.WriteLine("Elapsed time for Method1 : {0}", totalstopwatch1.Elapsed);
                Console.WriteLine("Elapsed time for Method2 : {0}", totalstopwatch2.Elapsed);
                Console.WriteLine("Elapsed time for Method3 : {0}", totalstopwatch3.Elapsed);
                Console.WriteLine("Elapsed time for Method4 : {0}", totalstopwatch4.Elapsed);

                Console.WriteLine("Total should be about :" + 1010000);
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
            if (_currentViewedMove == _myGame.MovesMade()) { return; }
            _currentViewedMove += 1;
            BoardStates[,] bstate = _myGame.GetBoardAtMove(_currentViewedMove);
            RefreshControls(bstate);
        }

        private void MiniMaxTestButton_Click(object sender, EventArgs e)
        {

            new Thread(() =>
            {
                const BoardStates myPlayer = BoardStates.black;
                const int testCount = 25;
                const int minimaxDepth = 3;
                object wonGamesLock = new object();
                int wonGames = 0;

                //Parallel.For(0, testCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 }, index =>
                //{
                for (int index = 0; index < testCount; index++)
                {
                    OthelloGame testGame = new OthelloGame();
                    _myGame = testGame;
                    while (!testGame.GameOver())
                    {
                        if (testGame.WhosTurn == OthelloGame.OpposingPlayer(myPlayer))
                        {
                            testGame.MakeRandomMove();
                            Invoke(new Action(() =>
                            {
                                RefreshControls();
                            }));
                        }
                        else
                        {
                            byte[] move = BoardNeuralNet.PredictBestMove(minimaxDepth, testGame, myPlayer);
                            testGame.MakeMove(move);
                            Invoke(new Action(() =>
                            {
                                RefreshControls();
                            }));
                        }
                    }
                    if (testGame.GameOver())//just gotta check
                    {
                        if (testGame.FinalWinner == myPlayer)
                        {
                            lock (wonGamesLock) { wonGames++; }
                        }
                        Console.WriteLine("Finished Game " + index + ", " + testGame.FinalWinner.ToString() + " won");
                    }
                    else
                    {
                        throw new Exception("MiniMax Testing didn't complete a game");
                    }
                }
                //});

                Console.WriteLine("Won " + wonGames + " / " + testCount + " games, " + ((double)wonGames / testCount) * 100 + " %");
            }).Start();
        }
    }
}


