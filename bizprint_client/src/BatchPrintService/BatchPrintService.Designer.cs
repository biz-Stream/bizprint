namespace BatchPrintServiceMain
{
    partial class BatchPrintSeviceMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchPrintSeviceMain));
            this.contextMenuBatchPrint = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.batchPrintServiceの終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BatchPrintServiceNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuBatchPrint.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuBatchPrint
            // 
            this.contextMenuBatchPrint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchPrintServiceの終了ToolStripMenuItem});
            this.contextMenuBatchPrint.Name = "contextMenuBatchPrint";
            this.contextMenuBatchPrint.Size = new System.Drawing.Size(201, 26);
            this.contextMenuBatchPrint.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuBatchPrint_Opening);
            // 
            // batchPrintServiceの終了ToolStripMenuItem
            // 
            this.batchPrintServiceの終了ToolStripMenuItem.Name = "batchPrintServiceの終了ToolStripMenuItem";
            this.batchPrintServiceの終了ToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.batchPrintServiceの終了ToolStripMenuItem.Text = "BatchPrintServiceの終了";
            this.batchPrintServiceの終了ToolStripMenuItem.Visible = false;
            this.batchPrintServiceの終了ToolStripMenuItem.Click += new System.EventHandler(this.BatchPrintServiceCloseToolStripMenuItem_Click);
            // 
            // BatchPrintServiceNotifyIcon
            // 
            this.BatchPrintServiceNotifyIcon.ContextMenuStrip = this.contextMenuBatchPrint;
            this.BatchPrintServiceNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("BatchPrintServiceNotifyIcon.Icon")));
            this.BatchPrintServiceNotifyIcon.Text = "BatchPrintService";
            this.BatchPrintServiceNotifyIcon.Visible = true;
            // 
            // BatchPrintSeviceMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 0);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchPrintSeviceMain";
            this.ShowInTaskbar = false;
            this.Text = "BatchPrintApplication";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Activated += new System.EventHandler(this.BatchPrintSeviceMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BatchPrintSeviceMain_FormClosing);
            this.contextMenuBatchPrint.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuBatchPrint;
        private System.Windows.Forms.ToolStripMenuItem batchPrintServiceの終了ToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon BatchPrintServiceNotifyIcon;
    }
}

