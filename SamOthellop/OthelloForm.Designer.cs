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
            this.SuspendLayout();
            // 
            // InstructionBrowser
            // 
            this.InstructionBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstructionBrowser.Location = new System.Drawing.Point(517, 0);
            this.InstructionBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.InstructionBrowser.Name = "InstructionBrowser";
            this.InstructionBrowser.Size = new System.Drawing.Size(267, 475);
            this.InstructionBrowser.TabIndex = 0;
            // 
            // OthelloForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(784, 475);
            this.Controls.Add(this.InstructionBrowser);
            this.Name = "OthelloForm";
            this.Text = "Othello";
            this.Load += new System.EventHandler(this.OthelloForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser InstructionBrowser;
    }
}

