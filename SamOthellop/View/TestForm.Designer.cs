namespace SamOthellop
{
    partial class TestForm
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
            this.ControlGroupBox = new System.Windows.Forms.GroupBox();
            this.GetSafePeiceButton = new System.Windows.Forms.Button();
            this.MiniMaxTextButton = new System.Windows.Forms.Button();
            this.NextMoveButton = new System.Windows.Forms.Button();
            this.PreviousMoveButton = new System.Windows.Forms.Button();
            this.FileLoadButton = new System.Windows.Forms.Button();
            this.MoveDepth = new System.Windows.Forms.TextBox();
            this.EducatedPlayButton = new System.Windows.Forms.Button();
            this.RunNetButton = new System.Windows.Forms.Button();
            this.GameCountTextBox = new System.Windows.Forms.TextBox();
            this.RunGamesButton = new System.Windows.Forms.Button();
            this.ClearBoardButton = new System.Windows.Forms.Button();
            this.RandomMoveButton = new System.Windows.Forms.Button();
            this.BlackMoveLabel = new System.Windows.Forms.Label();
            this.InvalidMoveLabel = new System.Windows.Forms.Label();
            this.WhiteMoveLabel = new System.Windows.Forms.Label();
            this.gameProgressLabel = new System.Windows.Forms.Label();
            this.GameCompletionProgressBar = new System.Windows.Forms.ProgressBar();
            this.GameOverLabel = new System.Windows.Forms.Label();
            this.GAButton = new System.Windows.Forms.Button();
            this.ControlGroupBox.SuspendLayout();
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
            // ControlGroupBox
            // 
            this.ControlGroupBox.Controls.Add(this.GAButton);
            this.ControlGroupBox.Controls.Add(this.GetSafePeiceButton);
            this.ControlGroupBox.Controls.Add(this.MiniMaxTextButton);
            this.ControlGroupBox.Controls.Add(this.NextMoveButton);
            this.ControlGroupBox.Controls.Add(this.PreviousMoveButton);
            this.ControlGroupBox.Controls.Add(this.FileLoadButton);
            this.ControlGroupBox.Controls.Add(this.MoveDepth);
            this.ControlGroupBox.Controls.Add(this.EducatedPlayButton);
            this.ControlGroupBox.Controls.Add(this.RunNetButton);
            this.ControlGroupBox.Controls.Add(this.GameCountTextBox);
            this.ControlGroupBox.Controls.Add(this.RunGamesButton);
            this.ControlGroupBox.Controls.Add(this.ClearBoardButton);
            this.ControlGroupBox.Controls.Add(this.RandomMoveButton);
            this.ControlGroupBox.Controls.Add(this.BlackMoveLabel);
            this.ControlGroupBox.Controls.Add(this.InvalidMoveLabel);
            this.ControlGroupBox.Controls.Add(this.WhiteMoveLabel);
            this.ControlGroupBox.Controls.Add(this.gameProgressLabel);
            this.ControlGroupBox.Controls.Add(this.GameCompletionProgressBar);
            this.ControlGroupBox.Controls.Add(this.InstructionBrowser);
            this.ControlGroupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.ControlGroupBox.Location = new System.Drawing.Point(469, 0);
            this.ControlGroupBox.Name = "ControlGroupBox";
            this.ControlGroupBox.Size = new System.Drawing.Size(307, 449);
            this.ControlGroupBox.TabIndex = 1;
            this.ControlGroupBox.TabStop = false;
            this.ControlGroupBox.Text = " ";
            // 
            // GetSafePeiceButton
            // 
            this.GetSafePeiceButton.Location = new System.Drawing.Point(18, 414);
            this.GetSafePeiceButton.Name = "GetSafePeiceButton";
            this.GetSafePeiceButton.Size = new System.Drawing.Size(103, 23);
            this.GetSafePeiceButton.TabIndex = 20;
            this.GetSafePeiceButton.Text = "Print Safe Peices";
            this.GetSafePeiceButton.UseVisualStyleBackColor = true;
            this.GetSafePeiceButton.Click += new System.EventHandler(this.GetSafePeiceButton_Click);
            // 
            // MiniMaxTextButton
            // 
            this.MiniMaxTextButton.Location = new System.Drawing.Point(170, 391);
            this.MiniMaxTextButton.Name = "MiniMaxTextButton";
            this.MiniMaxTextButton.Size = new System.Drawing.Size(103, 23);
            this.MiniMaxTextButton.TabIndex = 19;
            this.MiniMaxTextButton.Text = "Test MiniMax";
            this.MiniMaxTextButton.UseVisualStyleBackColor = true;
            this.MiniMaxTextButton.Click += new System.EventHandler(this.MiniMaxTestButton_Click);
            // 
            // NextMoveButton
            // 
            this.NextMoveButton.Location = new System.Drawing.Point(74, 389);
            this.NextMoveButton.Name = "NextMoveButton";
            this.NextMoveButton.Size = new System.Drawing.Size(47, 19);
            this.NextMoveButton.TabIndex = 18;
            this.NextMoveButton.Text = "Next";
            this.NextMoveButton.UseVisualStyleBackColor = true;
            this.NextMoveButton.Click += new System.EventHandler(this.NextMoveButton_Click);
            // 
            // PreviousMoveButton
            // 
            this.PreviousMoveButton.Location = new System.Drawing.Point(18, 389);
            this.PreviousMoveButton.Name = "PreviousMoveButton";
            this.PreviousMoveButton.Size = new System.Drawing.Size(47, 19);
            this.PreviousMoveButton.TabIndex = 17;
            this.PreviousMoveButton.Text = "Prev";
            this.PreviousMoveButton.UseVisualStyleBackColor = true;
            this.PreviousMoveButton.Click += new System.EventHandler(this.PreviousMoveButton_Click);
            // 
            // FileLoadButton
            // 
            this.FileLoadButton.Location = new System.Drawing.Point(170, 362);
            this.FileLoadButton.Name = "FileLoadButton";
            this.FileLoadButton.Size = new System.Drawing.Size(103, 23);
            this.FileLoadButton.TabIndex = 16;
            this.FileLoadButton.Text = "LoadGames";
            this.FileLoadButton.UseVisualStyleBackColor = true;
            this.FileLoadButton.Click += new System.EventHandler(this.FileLoadButton_Click);
            // 
            // MoveDepth
            // 
            this.MoveDepth.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.MoveDepth.Location = new System.Drawing.Point(157, 338);
            this.MoveDepth.MaxLength = 4;
            this.MoveDepth.Name = "MoveDepth";
            this.MoveDepth.Size = new System.Drawing.Size(35, 20);
            this.MoveDepth.TabIndex = 14;
            this.MoveDepth.Text = "1";
            // 
            // EducatedPlayButton
            // 
            this.EducatedPlayButton.Location = new System.Drawing.Point(198, 338);
            this.EducatedPlayButton.Name = "EducatedPlayButton";
            this.EducatedPlayButton.Size = new System.Drawing.Size(97, 20);
            this.EducatedPlayButton.TabIndex = 13;
            this.EducatedPlayButton.Text = "Make Educated Play";
            this.EducatedPlayButton.UseVisualStyleBackColor = true;
            this.EducatedPlayButton.Click += new System.EventHandler(this.EducatedPlayButton_Click);
            // 
            // RunNetButton
            // 
            this.RunNetButton.Location = new System.Drawing.Point(170, 312);
            this.RunNetButton.Name = "RunNetButton";
            this.RunNetButton.Size = new System.Drawing.Size(103, 20);
            this.RunNetButton.TabIndex = 12;
            this.RunNetButton.Text = "Run Neural Net";
            this.RunNetButton.UseVisualStyleBackColor = true;
            this.RunNetButton.Click += new System.EventHandler(this.RunNet_Click);
            // 
            // GameCountTextBox
            // 
            this.GameCountTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.GameCountTextBox.Location = new System.Drawing.Point(18, 363);
            this.GameCountTextBox.MaxLength = 4;
            this.GameCountTextBox.Name = "GameCountTextBox";
            this.GameCountTextBox.Size = new System.Drawing.Size(35, 20);
            this.GameCountTextBox.TabIndex = 11;
            this.GameCountTextBox.Text = "100";
            // 
            // RunGamesButton
            // 
            this.RunGamesButton.Location = new System.Drawing.Point(50, 363);
            this.RunGamesButton.Name = "RunGamesButton";
            this.RunGamesButton.Size = new System.Drawing.Size(71, 20);
            this.RunGamesButton.TabIndex = 10;
            this.RunGamesButton.Text = "Run Games";
            this.RunGamesButton.UseVisualStyleBackColor = true;
            this.RunGamesButton.Click += new System.EventHandler(this.RunGamesButton_Click);
            // 
            // ClearBoardButton
            // 
            this.ClearBoardButton.Location = new System.Drawing.Point(18, 338);
            this.ClearBoardButton.Name = "ClearBoardButton";
            this.ClearBoardButton.Size = new System.Drawing.Size(103, 20);
            this.ClearBoardButton.TabIndex = 9;
            this.ClearBoardButton.Text = "Clear Board";
            this.ClearBoardButton.UseVisualStyleBackColor = true;
            this.ClearBoardButton.Click += new System.EventHandler(this.ClearBoardButton_Click);
            // 
            // RandomMoveButton
            // 
            this.RandomMoveButton.Location = new System.Drawing.Point(18, 312);
            this.RandomMoveButton.Name = "RandomMoveButton";
            this.RandomMoveButton.Size = new System.Drawing.Size(103, 20);
            this.RandomMoveButton.TabIndex = 7;
            this.RandomMoveButton.Text = "Random Move";
            this.RandomMoveButton.UseVisualStyleBackColor = true;
            this.RandomMoveButton.Click += new System.EventHandler(this.RandomMoveButton_Click);
            // 
            // BlackMoveLabel
            // 
            this.BlackMoveLabel.AutoSize = true;
            this.BlackMoveLabel.BackColor = System.Drawing.Color.Yellow;
            this.BlackMoveLabel.Location = new System.Drawing.Point(201, 281);
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
            this.InvalidMoveLabel.Location = new System.Drawing.Point(127, 281);
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
            this.WhiteMoveLabel.Location = new System.Drawing.Point(30, 281);
            this.WhiteMoveLabel.Name = "WhiteMoveLabel";
            this.WhiteMoveLabel.Size = new System.Drawing.Size(91, 13);
            this.WhiteMoveLabel.TabIndex = 3;
            this.WhiteMoveLabel.Text = "It is White\'s Move";
            this.WhiteMoveLabel.Visible = false;
            // 
            // gameProgressLabel
            // 
            this.gameProgressLabel.AutoSize = true;
            this.gameProgressLabel.Location = new System.Drawing.Point(116, 268);
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
            // GameOverLabel
            // 
            this.GameOverLabel.AutoSize = true;
            this.GameOverLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GameOverLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this.GameOverLabel.Location = new System.Drawing.Point(205, 172);
            this.GameOverLabel.Name = "GameOverLabel";
            this.GameOverLabel.Size = new System.Drawing.Size(439, 76);
            this.GameOverLabel.TabIndex = 2;
            this.GameOverLabel.Text = "GAME OVER";
            this.GameOverLabel.Visible = false;
            // 
            // GAButton
            // 
            this.GAButton.Location = new System.Drawing.Point(170, 420);
            this.GAButton.Name = "GAButton";
            this.GAButton.Size = new System.Drawing.Size(103, 23);
            this.GAButton.TabIndex = 21;
            this.GAButton.Text = "Test GA";
            this.GAButton.UseVisualStyleBackColor = true;
            this.GAButton.Click += new System.EventHandler(this.GAButton_Click);
            // 
            // OthelloForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(776, 449);
            this.Controls.Add(this.ControlGroupBox);
            this.Controls.Add(this.GameOverLabel);
            this.Name = "OthelloForm";
            this.Text = "Othello";
            this.Load += new System.EventHandler(this.OthelloForm_Load);
            this.ControlGroupBox.ResumeLayout(false);
            this.ControlGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser InstructionBrowser;
        private System.Windows.Forms.GroupBox ControlGroupBox;
        private System.Windows.Forms.Label gameProgressLabel;
        private System.Windows.Forms.ProgressBar GameCompletionProgressBar;
        private System.Windows.Forms.Label BlackMoveLabel;
        private System.Windows.Forms.Label InvalidMoveLabel;
        private System.Windows.Forms.Label WhiteMoveLabel;
        private System.Windows.Forms.Button RandomMoveButton;
        private System.Windows.Forms.Label GameOverLabel;
        private System.Windows.Forms.Button ClearBoardButton;
        private System.Windows.Forms.TextBox GameCountTextBox;
        private System.Windows.Forms.Button RunGamesButton;
        private System.Windows.Forms.Button RunNetButton;
        private System.Windows.Forms.Button EducatedPlayButton;
        private System.Windows.Forms.TextBox MoveDepth;
        private System.Windows.Forms.Button FileLoadButton;
        private System.Windows.Forms.Button NextMoveButton;
        private System.Windows.Forms.Button PreviousMoveButton;
        private System.Windows.Forms.Button MiniMaxTextButton;
        private System.Windows.Forms.Button GetSafePeiceButton;
        private System.Windows.Forms.Button GAButton;
    }
}

