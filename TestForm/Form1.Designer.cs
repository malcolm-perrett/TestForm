namespace TestForm
{
    partial class Form1
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
            this.KeyCombo = new System.Windows.Forms.ComboBox();
            this.NoteLable = new System.Windows.Forms.Label();
            this.StafPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // KeyCombo
            // 
            this.KeyCombo.FormattingEnabled = true;
            this.KeyCombo.Location = new System.Drawing.Point(12, 12);
            this.KeyCombo.Name = "KeyCombo";
            this.KeyCombo.Size = new System.Drawing.Size(121, 21);
            this.KeyCombo.TabIndex = 0;
            this.KeyCombo.SelectedIndexChanged += new System.EventHandler(this.KeyCombo_SelectedIndexChanged);
            // 
            // NoteLable
            // 
            this.NoteLable.AutoSize = true;
            this.NoteLable.Location = new System.Drawing.Point(12, 216);
            this.NoteLable.Name = "NoteLable";
            this.NoteLable.Size = new System.Drawing.Size(35, 13);
            this.NoteLable.TabIndex = 1;
            this.NoteLable.Text = "label1";
            // 
            // StafPanel
            // 
            this.StafPanel.BackColor = System.Drawing.Color.White;
            this.StafPanel.Location = new System.Drawing.Point(12, 39);
            this.StafPanel.Name = "StafPanel";
            this.StafPanel.Size = new System.Drawing.Size(188, 174);
            this.StafPanel.TabIndex = 2;
            this.StafPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.StafPanel_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 234);
            this.Controls.Add(this.StafPanel);
            this.Controls.Add(this.NoteLable);
            this.Controls.Add(this.KeyCombo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox KeyCombo;
        private System.Windows.Forms.Label NoteLable;
        private System.Windows.Forms.Panel StafPanel;
    }
}

