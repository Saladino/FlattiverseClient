namespace FlattiverseClient
{
    partial class View
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
            this.textBoxMessages = new System.Windows.Forms.RichTextBox();
            this.radarScreen = new System.Windows.Forms.Panel();
            this.energyBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMessages.Location = new System.Drawing.Point(365, 436);
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ReadOnly = true;
            this.textBoxMessages.Size = new System.Drawing.Size(512, 121);
            this.textBoxMessages.TabIndex = 0;
            this.textBoxMessages.Text = "";
            this.textBoxMessages.WordWrap = false;
            // 
            // radarScreen
            // 
            this.radarScreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radarScreen.BackColor = System.Drawing.Color.Black;
            this.radarScreen.Location = new System.Drawing.Point(365, 2);
            this.radarScreen.Name = "radarScreen";
            this.radarScreen.Size = new System.Drawing.Size(512, 428);
            this.radarScreen.TabIndex = 1;
            // 
            // energyBar
            // 
            this.energyBar.Location = new System.Drawing.Point(12, 534);
            this.energyBar.Name = "energyBar";
            this.energyBar.Size = new System.Drawing.Size(347, 23);
            this.energyBar.TabIndex = 0;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 556);
            this.Controls.Add(this.energyBar);
            this.Controls.Add(this.radarScreen);
            this.Controls.Add(this.textBoxMessages);
            this.KeyPreview = true;
            this.Name = "View";
            this.Text = "Flattiverse Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEventHandler);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBoxMessages;
        private System.Windows.Forms.Panel radarScreen;
        private System.Windows.Forms.ProgressBar energyBar;
    }
}

