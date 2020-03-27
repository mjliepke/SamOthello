namespace SamOthellop
{
    partial class OthelloForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InstructionBrowser = new System.Windows.Forms.WebBrowser();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BlackMoveLabel = new System.Windows.Forms.Label();
            this.InvalidMoveLabel = new System.Windows.Forms.Label();
            this.WhiteMoveLabel = new System.Windows.Forms.Label();
            this.gameProgressLabel = new System.Windows.Forms.Label();
            this.GameCompletionProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // InstructionBrowser
            // 
            this.InstructionBrowser.Dock = System.Windows.Forms.DockStyle.Top;
            this.InstructionBrowser.Location = new System.Drawing.Point(3, 16);
            this.InstructionBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.InstructionBrowser.Name = "InstructionBrowser";
            this.InstructionBrowser.ScrollBarsEnabled = false;
            this.InstructionBrowser.Size = new System.Drawing.Size(301, 232);
            this.InstructionBrowser.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BlackMoveLabel);
            this.groupBox1.Controls.Add(this.InvalidMoveLabel);
            this.groupBox1.Controls.Add(this.WhiteMoveLabel);
            this.groupBox1.Controls.Add(this.gameProgressLabel);
            this.groupBox1.Controls.Add(this.GameCompletionProgressBar);
            this.groupBox1.Controls.Add(this.InstructionBrowser);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(469, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 449);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " ";
            // 
            // BlackMoveLabel
            // 
            this.BlackMoveLabel.AutoSize = true;
            this.BlackMoveLabel.BackColor = System.Drawing.Color.Yellow;
            this.BlackMoveLabel.Location = new System.Drawing.Point(205, 331);
            this.BlackMoveLabel.Name = "BlackMoveLabel";
            this.BlackMoveLabel.Size = new System.Drawing.Size(90, 13);
            this.BlackMoveLabel.TabIndex = 6;
            this.BlackMoveLabel.Text = "It is Black\'s Move";
            this.BlackMoveLabel.Visible = false;
            // 
            // InvalidMoveLabel
            // 
            this.InvalidMoveLabel.AutoSize = true;
            this.InvalidMoveLabel.BackColor = System.Drawing.Color.Red;
            this.InvalidMoveLabel.Location = new System.Drawing.Point(127, 331);
            this.InvalidMoveLabel.Name = "InvalidMoveLabel";
            this.InvalidMoveLabel.Size = new System.Drawing.Size(68, 13);
            this.InvalidMoveLabel.TabIndex = 5;
            this.InvalidMoveLabel.Text = "Invalid Move";
            this.InvalidMoveLabel.Visible = false;
            // 
            // WhiteMoveLabel
            // 
            this.WhiteMoveLabel.AutoSize = true;
            this.WhiteMoveLabel.BackColor = System.Drawing.Color.Yellow;
            this.WhiteMoveLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.WhiteMoveLabel.Location = new System.Drawing.Point(19, 331);
            this.WhiteMoveLabel.Name = "WhiteMoveLabel";
            this.WhiteMoveLabel.Size = new System.Drawing.Size(91, 13);
            this.WhiteMoveLabel.TabIndex = 3;
            this.WhiteMoveLabel.Text = "It is White\'s Move";
            this.WhiteMoveLabel.Visible = false;
            // 
            // gameProgressLabel
            // 
            this.gameProgressLabel.AutoSize = true;
            this.gameProgressLabel.Location = new System.Drawing.Point(116, 281);
            this.gameProgressLabel.Name = "gameProgressLabel";
            this.gameProgressLabel.Size = new System.Drawing.Size(79, 13);
            this.gameProgressLabel.TabIndex = 2;
            this.gameProgressLabel.Text = "Game Progress";
            // 
            // GameCompletionProgressBar
            // 
            this.GameCompletionProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.GameCompletionProgressBar.Location = new System.Drawing.Point(0, 241);
            this.GameCompletionProgressBar.Name = "GameCompletionProgressBar";
            this.GameCompletionProgressBar.Size = new System.Drawing.Size(304, 24);
            this.GameCompletionProgressBar.TabIndex = 1;
            // 
            // OthelloForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(776, 449);
            this.Controls.Add(this.groupBox1);
            this.Name = "OthelloForm";
            this.Text = "Othello";
            this.Load += new System.EventHandler(this.OthelloForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser InstructionBrowser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label gameProgressLabel;
        private System.Windows.Forms.ProgressBar GameCompletionProgressBar;
        private System.Windows.Forms.Label BlackMoveLabel;
        private System.Windows.Forms.Label InvalidMoveLabel;
        private System.Windows.Forms.Label WhiteMoveLabel;
    }
}

