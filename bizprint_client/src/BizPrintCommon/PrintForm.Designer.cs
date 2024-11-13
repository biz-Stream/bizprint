namespace BizPrintCommon
{
    partial class PrintForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintForm));
            this.pdfOcx = new AxAcroPDFLib.AxAcroPDF();
            this.TimerPrtForm = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pdfOcx)).BeginInit();
            this.SuspendLayout();
            // 
            // pdfOcx
            // 
            this.pdfOcx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pdfOcx.Enabled = true;
            this.pdfOcx.Location = new System.Drawing.Point(0, 0);
            this.pdfOcx.Name = "pdfOcx";
            this.pdfOcx.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("pdfOcx.OcxState")));
            this.pdfOcx.Size = new System.Drawing.Size(392, 528);
            this.pdfOcx.TabIndex = 1;
            // 
            // TimerPrtForm
            // 
            this.TimerPrtForm.Enabled = false;
            this.TimerPrtForm.Interval = SettingMng.PrintFormTimerInterval;
            this.TimerPrtForm.Tick += new System.EventHandler(this.TimerPrtForm_Tick);
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 0);
            this.Controls.Add(this.pdfOcx);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PrintForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Activated += new System.EventHandler(this.PrintForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrintForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pdfOcx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAcroPDFLib.AxAcroPDF pdfOcx;
        private System.Windows.Forms.Timer TimerPrtForm;
    }
}