namespace PFEProject
{
    partial class DSFview
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
            this.dsf = new Kitware.VTK.RenderWindowControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // dsf
            // 
            this.dsf.AddTestActors = false;
            this.dsf.BackColor = System.Drawing.Color.LightGray;
            this.dsf.Location = new System.Drawing.Point(12, 12);
            this.dsf.Name = "dsf";
            this.dsf.Size = new System.Drawing.Size(1264, 706);
            this.dsf.TabIndex = 0;
            this.dsf.TestText = null;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.dowork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.complete);
            // 
            // DSFview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1285, 748);
            this.Controls.Add(this.dsf);
            this.Name = "DSFview";
            this.Text = "DSFview";
            this.Load += new System.EventHandler(this.onload);
            this.ResumeLayout(false);

        }

        #endregion

        private Kitware.VTK.RenderWindowControl dsf;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}