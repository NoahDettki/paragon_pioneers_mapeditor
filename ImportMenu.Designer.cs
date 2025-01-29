namespace ParagonPioneers
{
    partial class ImportMenu
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.submitImportButton = new System.Windows.Forms.Button();
            this.FileImportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Font = new System.Drawing.Font("Lucida Sans Typewriter", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(16, 76);
            this.textBox.Margin = new System.Windows.Forms.Padding(4);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(767, 198);
            this.textBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(297, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Paragon Pioneers - Map Editor";
            // 
            // submitImportButton
            // 
            this.submitImportButton.Location = new System.Drawing.Point(16, 369);
            this.submitImportButton.Margin = new System.Windows.Forms.Padding(4);
            this.submitImportButton.Name = "submitImportButton";
            this.submitImportButton.Size = new System.Drawing.Size(100, 28);
            this.submitImportButton.TabIndex = 2;
            this.submitImportButton.Text = "Importieren";
            this.submitImportButton.UseVisualStyleBackColor = true;
            this.submitImportButton.Click += new System.EventHandler(this.SubmitImportButton_Click);
            // 
            // FileImportButton
            // 
            this.FileImportButton.Location = new System.Drawing.Point(16, 282);
            this.FileImportButton.Margin = new System.Windows.Forms.Padding(4);
            this.FileImportButton.Name = "FileImportButton";
            this.FileImportButton.Size = new System.Drawing.Size(132, 28);
            this.FileImportButton.TabIndex = 4;
            this.FileImportButton.Text = "Datei importieren";
            this.FileImportButton.UseVisualStyleBackColor = true;
            this.FileImportButton.Click += new System.EventHandler(this.FileImportButton_Click);
            // 
            // ImportMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 412);
            this.Controls.Add(this.FileImportButton);
            this.Controls.Add(this.submitImportButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ImportMenu";
            this.Text = "Import Menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button submitImportButton;
        private System.Windows.Forms.Button FileImportButton;
    }
}

