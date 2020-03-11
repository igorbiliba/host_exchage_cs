namespace host_exchage_cs.Info
{
    partial class InfoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoForm));
            this.label1 = new System.Windows.Forms.Label();
            this.balancesTextBox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btcAddressesTextBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.emailCheckerTextBox = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.clientsInRunTextBox = new System.Windows.Forms.TextBox();
            this.clientCntUsedsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Balances";
            // 
            // balancesTextBox
            // 
            this.balancesTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.balancesTextBox.Location = new System.Drawing.Point(12, 30);
            this.balancesTextBox.Multiline = true;
            this.balancesTextBox.Name = "balancesTextBox";
            this.balancesTextBox.ReadOnly = true;
            this.balancesTextBox.Size = new System.Drawing.Size(192, 110);
            this.balancesTextBox.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(211, 9);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(799, 541);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btcAddressesTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(791, 515);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "BTC Addresses";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btcAddressesTextBox
            // 
            this.btcAddressesTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.btcAddressesTextBox.Location = new System.Drawing.Point(6, 6);
            this.btcAddressesTextBox.Multiline = true;
            this.btcAddressesTextBox.Name = "btcAddressesTextBox";
            this.btcAddressesTextBox.ReadOnly = true;
            this.btcAddressesTextBox.Size = new System.Drawing.Size(779, 503);
            this.btcAddressesTextBox.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.emailCheckerTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(791, 515);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Email Checker";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // emailCheckerTextBox
            // 
            this.emailCheckerTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.emailCheckerTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.emailCheckerTextBox.Location = new System.Drawing.Point(6, 6);
            this.emailCheckerTextBox.Multiline = true;
            this.emailCheckerTextBox.Name = "emailCheckerTextBox";
            this.emailCheckerTextBox.ReadOnly = true;
            this.emailCheckerTextBox.Size = new System.Drawing.Size(779, 503);
            this.emailCheckerTextBox.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.clientsInRunTextBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(791, 515);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Clients in run";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // clientsInRunTextBox
            // 
            this.clientsInRunTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.clientsInRunTextBox.Location = new System.Drawing.Point(6, 6);
            this.clientsInRunTextBox.Multiline = true;
            this.clientsInRunTextBox.Name = "clientsInRunTextBox";
            this.clientsInRunTextBox.ReadOnly = true;
            this.clientsInRunTextBox.Size = new System.Drawing.Size(779, 503);
            this.clientsInRunTextBox.TabIndex = 0;
            // 
            // clientCntUsedsTextBox
            // 
            this.clientCntUsedsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.clientCntUsedsTextBox.Location = new System.Drawing.Point(12, 183);
            this.clientCntUsedsTextBox.Multiline = true;
            this.clientCntUsedsTextBox.Name = "clientCntUsedsTextBox";
            this.clientCntUsedsTextBox.ReadOnly = true;
            this.clientCntUsedsTextBox.Size = new System.Drawing.Size(192, 110);
            this.clientCntUsedsTextBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Client cnt useds";
            // 
            // InfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 557);
            this.Controls.Add(this.clientCntUsedsTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.balancesTextBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InfoForm";
            this.Text = "Homer Info";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox balancesTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox btcAddressesTextBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox emailCheckerTextBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox clientsInRunTextBox;
        private System.Windows.Forms.TextBox clientCntUsedsTextBox;
        private System.Windows.Forms.Label label2;
    }
}