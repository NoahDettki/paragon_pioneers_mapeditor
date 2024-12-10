namespace ParagonPioneers
{
    partial class Map
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
            this.mapPanel = new ParagonPioneers.MapPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.landButton = new System.Windows.Forms.Button();
            this.waterButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapPanel
            // 
            this.mapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel.Location = new System.Drawing.Point(0, 0);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(800, 450);
            this.mapPanel.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.exportButton);
            this.panel1.Controls.Add(this.landButton);
            this.panel1.Controls.Add(this.waterButton);
            this.panel1.Location = new System.Drawing.Point(728, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 426);
            this.panel1.TabIndex = 1;
            // 
            // landButton
            // 
            this.landButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.landButton.ForeColor = System.Drawing.Color.Black;
            this.landButton.Location = new System.Drawing.Point(5, 59);
            this.landButton.Name = "landButton";
            this.landButton.Size = new System.Drawing.Size(50, 50);
            this.landButton.TabIndex = 1;
            this.landButton.Text = "Land";
            this.landButton.UseVisualStyleBackColor = false;
            this.landButton.Click += new System.EventHandler(this.landButton_Click);
            // 
            // waterButton
            // 
            this.waterButton.BackColor = System.Drawing.Color.Blue;
            this.waterButton.ForeColor = System.Drawing.Color.White;
            this.waterButton.Location = new System.Drawing.Point(4, 3);
            this.waterButton.Name = "waterButton";
            this.waterButton.Size = new System.Drawing.Size(50, 50);
            this.waterButton.TabIndex = 0;
            this.waterButton.Text = "Water";
            this.waterButton.UseVisualStyleBackColor = false;
            this.waterButton.Click += new System.EventHandler(this.waterButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.BackColor = System.Drawing.Color.Red;
            this.exportButton.ForeColor = System.Drawing.Color.White;
            this.exportButton.Location = new System.Drawing.Point(4, 371);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(50, 50);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mapPanel);
            this.Name = "Map";
            this.Text = "Map";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MapPanel mapPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button waterButton;
        private System.Windows.Forms.Button landButton;
        private System.Windows.Forms.Button exportButton;
    }
}