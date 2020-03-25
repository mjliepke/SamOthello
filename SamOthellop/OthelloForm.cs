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
                    var newPanel = new PiecePanel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * i, tileSize * j)
                    };

                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);

                    // add to our 2d array of panels for future use
                    _boardPanels[i, j] = newPanel;

                    //color the backgrounds
                    Color panelcolor = Color.Red;

                    if(_myBoard.BoardStateColors.TryGetValue(_myBoard.GetBoard()[i,j], out panelcolor))
                    {
                        _boardPanels[i, j].FillColor = panelcolor;
                    }
                    
                }
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                _boardPanels[0, 0].ReColor(Color.Red);
            }
            catch(Exception)
            {
            }
        }
    }
}
