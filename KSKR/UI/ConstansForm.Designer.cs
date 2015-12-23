namespace UI
{
    partial class ConstansForm
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
            this.TetaLabel = new System.Windows.Forms.Label();
            this.VilsonInputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NumarkInputTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.NumarkGroupBox = new System.Windows.Forms.GroupBox();
            this.VilsonGroupBox = new System.Windows.Forms.GroupBox();
            this.NumarkGroupBox.SuspendLayout();
            this.VilsonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // TetaLabel
            // 
            this.TetaLabel.AutoSize = true;
            this.TetaLabel.Location = new System.Drawing.Point(11, 16);
            this.TetaLabel.Name = "TetaLabel";
            this.TetaLabel.Size = new System.Drawing.Size(189, 26);
            this.TetaLabel.TabIndex = 0;
            this.TetaLabel.Text = "Введите значение θ (θ≥1).\r\nОбычно θ принимается равным 1.4.";
            // 
            // VilsonInputTextBox
            // 
            this.VilsonInputTextBox.Location = new System.Drawing.Point(12, 46);
            this.VilsonInputTextBox.Name = "VilsonInputTextBox";
            this.VilsonInputTextBox.Size = new System.Drawing.Size(121, 20);
            this.VilsonInputTextBox.TabIndex = 1;
            this.VilsonInputTextBox.TextChanged += new System.EventHandler(this.VilsonInputTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "α и δ - параметры, определяющие \r\nтрчнорсть и устойчивость интегрирования.\r\nδ ≥ 0" +
    ".50; α≥0.25(0.5+δ^2)";
            // 
            // NumarkInputTextBox
            // 
            this.NumarkInputTextBox.Location = new System.Drawing.Point(9, 81);
            this.NumarkInputTextBox.Name = "NumarkInputTextBox";
            this.NumarkInputTextBox.Size = new System.Drawing.Size(108, 20);
            this.NumarkInputTextBox.TabIndex = 3;
            this.NumarkInputTextBox.TextChanged += new System.EventHandler(this.NumarkInputTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Введите значение δ:";
            // 
            // NumarkGroupBox
            // 
            this.NumarkGroupBox.Controls.Add(this.label1);
            this.NumarkGroupBox.Controls.Add(this.label2);
            this.NumarkGroupBox.Controls.Add(this.NumarkInputTextBox);
            this.NumarkGroupBox.Location = new System.Drawing.Point(15, 12);
            this.NumarkGroupBox.Name = "NumarkGroupBox";
            this.NumarkGroupBox.Size = new System.Drawing.Size(235, 110);
            this.NumarkGroupBox.TabIndex = 5;
            this.NumarkGroupBox.TabStop = false;
            this.NumarkGroupBox.Text = "Метод Ньюмарка";
            // 
            // VilsonGroupBox
            // 
            this.VilsonGroupBox.Controls.Add(this.TetaLabel);
            this.VilsonGroupBox.Controls.Add(this.VilsonInputTextBox);
            this.VilsonGroupBox.Location = new System.Drawing.Point(15, 12);
            this.VilsonGroupBox.Name = "VilsonGroupBox";
            this.VilsonGroupBox.Size = new System.Drawing.Size(235, 110);
            this.VilsonGroupBox.TabIndex = 6;
            this.VilsonGroupBox.TabStop = false;
            this.VilsonGroupBox.Text = "Метод Вилсона";
            // 
            // ConstansForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 136);
            this.Controls.Add(this.VilsonGroupBox);
            this.Controls.Add(this.NumarkGroupBox);
            this.Name = "ConstansForm";
            this.Text = "ConstansForm";
            this.NumarkGroupBox.ResumeLayout(false);
            this.NumarkGroupBox.PerformLayout();
            this.VilsonGroupBox.ResumeLayout(false);
            this.VilsonGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TetaLabel;
        private System.Windows.Forms.TextBox VilsonInputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NumarkInputTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox NumarkGroupBox;
        private System.Windows.Forms.GroupBox VilsonGroupBox;
    }
}