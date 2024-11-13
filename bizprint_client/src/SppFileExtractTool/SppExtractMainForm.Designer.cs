namespace SppFileExtractTool
{
    partial class SppExtractMainForm
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
            this.lblInput = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(25, 27);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(63, 12);
            this.lblInput.TabIndex = 0;
            this.lblInput.Text = "入力ファイル";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(25, 68);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(52, 12);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "パスワード";
            // 
            // txtInput
            // 
            this.txtInput.AllowDrop = true;
            this.txtInput.Location = new System.Drawing.Point(100, 24);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(267, 19);
            this.txtInput.TabIndex = 2;
            this.txtInput.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtInput_DragDrop);
            this.txtInput.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtInput_DragEnter);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(99, 65);
            this.txtPassword.MaxLength = 64;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(268, 19);
            this.txtPassword.TabIndex = 3;
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(388, 20);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(55, 26);
            this.btnInput.TabIndex = 4;
            this.btnInput.Text = "参照";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnExtract.Location = new System.Drawing.Point(195, 108);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 39);
            this.btnExtract.TabIndex = 5;
            this.btnExtract.Text = "復号";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // SppExtractMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 168);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblInput);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SppExtractMainForm";
            this.ShowIcon = false;
            this.Text = "SppFileExtractTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnExtract;
    }
}

