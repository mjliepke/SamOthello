﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamOthellop
{
    public class PiecePanel : Panel
    {
        private Color FillColor = Color.Black;
        public int[] location{
            get;private set;
        }

        public PiecePanel(Color fillColor) : base()
        {
            FillColor = fillColor;
        }

        public PiecePanel() : base()
        {

        }
        public PiecePanel(int[] location) : base()
        {
            this.location = new int[] { location[0], location[1] };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillEllipse(new SolidBrush(FillColor), 5, 5, this.Width - 6, this.Height - 6);
            base.OnPaint(e);
        }
        protected override void OnResize(EventArgs e)
        {
            this.Width = this.Height;
            base.OnResize(e);
        }
        public void ReColor(Color newColor)
        {
            FillColor = newColor;
            this.Invalidate();
        }
    }

}
