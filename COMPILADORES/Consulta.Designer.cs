namespace COMPILADORES
{
    partial class Consulta
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
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.txtWhere = new System.Windows.Forms.TextBox();
            this.txtOrderBy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtConsulta = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtOriginal
            // 
            this.txtOriginal.Location = new System.Drawing.Point(12, 25);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.Size = new System.Drawing.Size(377, 86);
            this.txtOriginal.TabIndex = 0;
            // 
            // txtWhere
            // 
            this.txtWhere.Location = new System.Drawing.Point(12, 147);
            this.txtWhere.Multiline = true;
            this.txtWhere.Name = "txtWhere";
            this.txtWhere.Size = new System.Drawing.Size(377, 37);
            this.txtWhere.TabIndex = 1;
            // 
            // txtOrderBy
            // 
            this.txtOrderBy.Location = new System.Drawing.Point(12, 220);
            this.txtOrderBy.Multiline = true;
            this.txtOrderBy.Name = "txtOrderBy";
            this.txtOrderBy.Size = new System.Drawing.Size(377, 58);
            this.txtOrderBy.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Consulta original:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Clausula Where";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Clausula Order By";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 414);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Generar consulta";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 292);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Consulta SQL:";
            // 
            // txtConsulta
            // 
            this.txtConsulta.Location = new System.Drawing.Point(12, 308);
            this.txtConsulta.Multiline = true;
            this.txtConsulta.Name = "txtConsulta";
            this.txtConsulta.Size = new System.Drawing.Size(377, 100);
            this.txtConsulta.TabIndex = 7;
            // 
            // Consulta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 452);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtConsulta);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOrderBy);
            this.Controls.Add(this.txtWhere);
            this.Controls.Add(this.txtOriginal);
            this.Name = "Consulta";
            this.Text = "Consulta";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOriginal;
        private System.Windows.Forms.TextBox txtWhere;
        private System.Windows.Forms.TextBox txtOrderBy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtConsulta;
    }
}