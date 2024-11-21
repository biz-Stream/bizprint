// Copyright 2024 BrainSellers.com Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace DirectPrintService
{
    partial class DirectPrintServiceMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectPrintServiceMain));
            this.DirectPrintService_Icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripDirect = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.directPrintServiceの終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChkRecieveTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStripDirect.SuspendLayout();
            this.SuspendLayout();
            // 
            // DirectPrintService_Icon
            // 
            this.DirectPrintService_Icon.ContextMenuStrip = this.contextMenuStripDirect;
            this.DirectPrintService_Icon.Icon = ((System.Drawing.Icon)(resources.GetObject("DirectPrintService_Icon.Icon")));
            this.DirectPrintService_Icon.Text = "DirectPrintService";
            this.DirectPrintService_Icon.Visible = true;
            // 
            // contextMenuStripDirect
            // 
            this.contextMenuStripDirect.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.directPrintServiceの終了ToolStripMenuItem});
            this.contextMenuStripDirect.Name = "contextMenuStripDirect";
            this.contextMenuStripDirect.Size = new System.Drawing.Size(202, 26);
            this.contextMenuStripDirect.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDirect_Opening);
            // 
            // directPrintServiceの終了ToolStripMenuItem
            // 
            this.directPrintServiceの終了ToolStripMenuItem.Name = "directPrintServiceの終了ToolStripMenuItem";
            this.directPrintServiceの終了ToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.directPrintServiceの終了ToolStripMenuItem.Text = "DirectPrintServiceの終了";
            this.directPrintServiceの終了ToolStripMenuItem.Visible = false;
            this.directPrintServiceの終了ToolStripMenuItem.Click += new System.EventHandler(this.directPrintServiceCloseToolStripMenuItem_Click);
            // 
            // ChkRecieveTimer
            // 
            this.ChkRecieveTimer.Interval = 1000;
            this.ChkRecieveTimer.Tick += new System.EventHandler(this.ChkRecieveTimer_Tick);
            // 
            // DirectPrintServiceMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 0);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DirectPrintServiceMain";
            this.ShowInTaskbar = false;
            this.Text = "DirectPrintService";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Activated += new System.EventHandler(this.DirectPrintServiceMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DirectPrintServiceMain_FormClosing);
            this.contextMenuStripDirect.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon DirectPrintService_Icon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDirect;
        private System.Windows.Forms.ToolStripMenuItem directPrintServiceの終了ToolStripMenuItem;
        private System.Windows.Forms.Timer ChkRecieveTimer;
    }
}

