using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        public OthelloForm()
        {
            InitializeComponent();
        }

        private void OthelloForm_Load(object sender, EventArgs e)
        { 
            _myBoard = new OthelloBoard();
            _boardPanels = new PiecePanel[_myBoard.BoardSize, _myBoard.BoardSize];
            int tileSize = ((Size.Width > Size.Height) ? Size.Height -2 : Size.Width -2) / _myBoard.BoardSize;

            for(int i = 0; i < _myBoard.BoardSize; i++)
            {
                for(int j = 0; j < _myBoard.BoardSize; j++)
                {
                    var newPanel = new PiecePanel()
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * i, tileSize * j)
                    };

                    Controls.Add(newPanel);
                    _boardPanels[i, j] = newPanel;
                    Color panelcolor = Color.Red;

                    if(OthelloBoard.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i,j], out panelcolor))
                    {
                        _boardPanels[i, j].ReColor(panelcolor);
                    }
                }
            }

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

        private void OthelloPeice_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

            _myBoard.MakeMove(OthelloBoard.BoardStates.black, new int[] { 2,3});
            RefreshPanels();
        }
    }
}
