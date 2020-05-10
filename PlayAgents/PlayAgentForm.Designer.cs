namespace PlayAgents
{
    partial class PlayAgentForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectAgentLabel = new System.Windows.Forms.Label();
            this.AgentSelectComboBox = new System.Windows.Forms.ComboBox();
            this.StartGameButton = new System.Windows.Forms.Button();
            this.PlayerSelectComboBox = new System.Windows.Forms.ComboBox();
            this.MinMaxAgentComboBox = new System.Windows.Forms.ComboBox();
            this.GameOverLabel = new System.Windows.Forms.Label();
            this.RestartButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SelectAgentLabel
            // 
            this.SelectAgentLabel.AutoSize = true;
            this.SelectAgentLabel.Location = new System.Drawing.Point(58, 9);
            this.SelectAgentLabel.Name = "SelectAgentLabel";
            this.SelectAgentLabel.Size = new System.Drawing.Size(111, 15);
            this.SelectAgentLabel.TabIndex = 1;
            this.SelectAgentLabel.Text = "Select an Opponent";
            // 
            // AgentSelectComboBox
            // 
            this.AgentSelectComboBox.FormattingEnabled = true;
            this.AgentSelectComboBox.Location = new System.Drawing.Point(41, 27);
            this.AgentSelectComboBox.Name = "AgentSelectComboBox";
            this.AgentSelectComboBox.Size = new System.Drawing.Size(143, 23);
            this.AgentSelectComboBox.TabIndex = 2;
            this.AgentSelectComboBox.SelectedIndexChanged += new System.EventHandler(this.AgentSelectComboBox_SelectedIndexChanged);
            // 
            // StartGameButton
            // 
            this.StartGameButton.Location = new System.Drawing.Point(74, 115);
            this.StartGameButton.Name = "StartGameButton";
            this.StartGameButton.Size = new System.Drawing.Size(75, 23);
            this.StartGameButton.TabIndex = 3;
            this.StartGameButton.Text = "Start Game";
            this.StartGameButton.UseVisualStyleBackColor = true;
            this.StartGameButton.Click += new System.EventHandler(this.StartGameButton_Click);
            // 
            // PlayerSelectComboBox
            // 
            this.PlayerSelectComboBox.FormattingEnabled = true;
            this.PlayerSelectComboBox.Location = new System.Drawing.Point(41, 56);
            this.PlayerSelectComboBox.Name = "PlayerSelectComboBox";
            this.PlayerSelectComboBox.Size = new System.Drawing.Size(143, 23);
            this.PlayerSelectComboBox.TabIndex = 2;
            // 
            // MinMaxAgentComboBox
            // 
            this.MinMaxAgentComboBox.FormattingEnabled = true;
            this.MinMaxAgentComboBox.Location = new System.Drawing.Point(41, 85);
            this.MinMaxAgentComboBox.Name = "MinMaxAgentComboBox";
            this.MinMaxAgentComboBox.Size = new System.Drawing.Size(143, 23);
            this.MinMaxAgentComboBox.TabIndex = 2;
            this.MinMaxAgentComboBox.Visible = false;
            // 
            // GameOverLabel
            // 
            this.GameOverLabel.AutoSize = true;
            this.GameOverLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.GameOverLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.GameOverLabel.Location = new System.Drawing.Point(203, 187);
            this.GameOverLabel.Name = "GameOverLabel";
            this.GameOverLabel.Size = new System.Drawing.Size(71, 30);
            this.GameOverLabel.TabIndex = 4;
            this.GameOverLabel.Text = "label1";
            // 
            // RestartButton
            // 
            this.RestartButton.Location = new System.Drawing.Point(179, 220);
            this.RestartButton.Name = "RestartButton";
            this.RestartButton.Size = new System.Drawing.Size(125, 23);
            this.RestartButton.TabIndex = 5;
            this.RestartButton.Text = "Start a New Game";
            this.RestartButton.UseVisualStyleBackColor = true;
            this.RestartButton.Click += new System.EventHandler(this.RestartButton_Click);
            // 
            // PlayAgentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.RestartButton);
            this.Controls.Add(this.GameOverLabel);
            this.Controls.Add(this.MinMaxAgentComboBox);
            this.Controls.Add(this.PlayerSelectComboBox);
            this.Controls.Add(this.StartGameButton);
            this.Controls.Add(this.AgentSelectComboBox);
            this.Controls.Add(this.SelectAgentLabel);
            this.Name = "PlayAgentForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label SelectAgentLabel;
        private System.Windows.Forms.ComboBox AgentSelectComboBox;
        private System.Windows.Forms.Button StartGameButton;
        private System.Windows.Forms.ComboBox PlayerSelectComboBox;
        private System.Windows.Forms.ComboBox MinMaxAgentComboBox;
        private System.Windows.Forms.Label GameOverLabel;
        private System.Windows.Forms.Button RestartButton;
    }
}

