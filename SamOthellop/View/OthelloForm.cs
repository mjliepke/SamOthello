using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamOthellop
{
    public partial class OthelloForm : Form
    {
        private OthelloBoard _myBoard;
        private PiecePanel[,] _boardPanels;
        private System.Timers.Timer _twoSecondTimer;

        public OthelloForm()
        {
            InitializeComponent();
            _twoSecondTimer = new System.Timers.Timer(2000);
        }

        private void OthelloForm_Load(object sender, EventArgs e)
        {
            SetupBoard();
            SetupInstructions();
            SetupProgressBar();
        }
        private void SetupBoard()
        {
            _myBoard = new OthelloBoard();
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
                    if (OthelloBoard.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i, j], out panelcolor))
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
            GameCompletionProgressBar.Value = _myBoard.PlayesMade();
        }

        private void RefreshPanels()
        {
            for(int i = 0; i < _myBoard.BoardSize; i++)
            {
                for(int j=0; j<_myBoard.BoardSize; j++)
                {
                    Color color;
                    OthelloBoard.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i, j], out color);
                    _boardPanels[i, j].ReColor(color);

                }
            }
        }

        private void OthelloPeice_Click(object sender, MouseEventArgs e)
        {
            OthelloBoard.BoardStates player  = e.Button ==MouseButtons.Left ? OthelloBoard.BoardStates.black : OthelloBoard.BoardStates.white;
            try
            {
                PiecePanel thisPanel = (PiecePanel)sender;
                if (!player.Equals(_myBoard.WhosTurn))
                {
                    if (player.Equals(OthelloBoard.BoardStates.white))
                    {
                        BlackMoveLabel.Visible = true;
                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(BlackMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }else if (player.Equals(OthelloBoard.BoardStates.black))
                    {
                        WhiteMoveLabel.Visible = true;

                        _twoSecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(WhiteMoveLabel_VisibilityFalse);
                        _twoSecondTimer.Enabled = true;
                    }
                }
                _myBoard.MakeMove(player, new int[] { thisPanel.location[0], thisPanel.location[1] });
                RefreshPanels();
            }catch{
                throw new NotSupportedException("OthelloPeice_Click is not to be used with a control other than a PiecePanel");
            }
            GameCompletionProgressBar.Value = _myBoard.PlayesMade();
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

    }
}
