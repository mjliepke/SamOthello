using SamOthellop.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private OthelloGame _myBoard;
        private PiecePanel[,] _boardPanels;
        private System.Timers.Timer _twoSecondTimer;
        private BoardNeuralNet _net;

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
            _myBoard = new OthelloGame();
            _boardPanels = new PiecePanel[_myBoard.BoardSize, _myBoard.BoardSize];
            int tileSize = ((Size.Width > Size.Height) ? Size.Height - 45 : Size.Width - 45) / _myBoard.BoardSize;

            for (int i = 0; i < _myBoard.BoardSize; i++)
            {
                for (int j = 0; j < _myBoard.BoardSize; j++)
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
                    if (OthelloGame.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i, j], out panelcolor))
                    {
                        _boardPanels[i, j].ReColor(panelcolor);
                    }
                }
            }
        }

        private void SetupInstructions()
        {
            string curDir = Directory.GetCurrentDirectory();
            InstructionBrowser.Navigate(new Uri(String.Format("file:///{0}/Instructions.html", curDir)));

        }

        private void SetupProgressBar()
        {
            GameCompletionProgressBar.Maximum = _myBoard.MaxMoves;
            GameCompletionProgressBar.Value = _myBoard.MovesMade();
        }

        private void RefreshControls()
        {
            for (int i = 0; i < _myBoard.BoardSize; i++)
            {
                for (int j = 0; j < _myBoard.BoardSize; j++)
                {
                    Color color;
                    OthelloGame.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i, j], out color);
                    _boardPanels[i, j].ReColor(color);

                }
            }

            if (_myBoard.GameOver())
            {
                GameOverLabel.Visible = true;
                _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(GameOver_VisibilityFalse);
                _twoSecondTimer.Enabled = true;
            }
        }

        private void OthelloPeice_Click(object sender, MouseEventArgs e)
        {
            OthelloGame.BoardStates player = e.Button == MouseButtons.Left ? OthelloGame.BoardStates.black : OthelloGame.BoardStates.white;
            try
            {
                PiecePanel thisPanel = (PiecePanel)sender;
                if (!player.Equals(_myBoard.WhosTurn))
                {
                    if (player.Equals(OthelloGame.BoardStates.white))
                    {
                        BlackMoveLabel.Visible = true;
                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(BlackMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                    else if (player.Equals(OthelloGame.BoardStates.black))
                    {
                        WhiteMoveLabel.Visible = true;

                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(WhiteMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                }
                _myBoard.MakeMove(player, new int[] { thisPanel.location[0], thisPanel.location[1] });
                RefreshControls();
            }
            catch
            {
                throw new NotSupportedException("OthelloPeice_Click is not to be used with a control other than a PiecePanel");
            }
            GameCompletionProgressBar.Value = _myBoard.MovesMade();
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
            _myBoard.MakeRandomMove();
            RefreshControls();
        }

        private void RandomGameButton_Click(object sender, EventArgs e)
        {
            EnableControlButtons(false);
            new Thread(() =>
            {
                while (!_myBoard.GameOver())
                {
                    _myBoard.MakeRandomMove();
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
            _myBoard.ResetBoard();
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
                    while (!_myBoard.GameOver())
                    {
                        _myBoard.MakeRandomMove();
                    }
                    Invoke(new Action(() =>
                    {
                        RefreshControls();
                        EnableControlButtons();
                    }));
                    _myBoard.ResetBoard();
                    Thread.Sleep(5);
                }
            }).Start();



        }
        private void EnableControlButtons(bool enable = true)
        {
            RandomGameButton.Enabled = enable;
            RunGamesButton.Enabled = enable;
            RandomMoveButton.Enabled = enable;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _net.StartTest();
        }

        private void EducatedPlayButton_Click(object sender, EventArgs e)
        {
            int unused = 0;
            _myBoard.MakeMove(_myBoard.WhosTurn, _net.PredictBestMove(Convert.ToInt32(MoveDepth.Text), _myBoard, _myBoard.WhosTurn, ref unused));
            RefreshControls();
        }
    }
}

