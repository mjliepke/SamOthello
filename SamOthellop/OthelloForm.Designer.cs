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
            this.components = new System.ComponentModel.Container();
            this.othelloBoardBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.testButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.othelloBoardBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // othelloBoardBindingSource
            // 
            this.othelloBoardBindingSource.DataSource = typeof(SamOthellop.OthelloBoard);
            // 
            // testButton
            // 
            this.testButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.testButton.Location = new System.Drawing.Point(663, 0);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(21, 461);
            this.testButton.TabIndex = 0;
            this.testButton.Text = "HIT ME";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.Button1_Click);
            // 
            // OthelloForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.testButton);
            this.Name = "OthelloForm";
            this.Text = "Othello";
            this.Load += new System.EventHandler(this.OthelloForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.othelloBoardBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource othelloBoardBindingSource;
        private System.Windows.Forms.Button testButton;
    }
}

