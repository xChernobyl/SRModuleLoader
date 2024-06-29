
namespace Phoenix.ModuleLoader.Forms
{
    partial class AddModuleForm
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
            this.groupBoxMain = new System.Windows.Forms.GroupBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxModuleIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonBrowseModulePath = new System.Windows.Forms.Button();
            this.textBoxModulePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxModuleName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMain
            // 
            this.groupBoxMain.Controls.Add(this.buttonAdd);
            this.groupBoxMain.Controls.Add(this.textBoxModuleIP);
            this.groupBoxMain.Controls.Add(this.label3);
            this.groupBoxMain.Controls.Add(this.buttonBrowseModulePath);
            this.groupBoxMain.Controls.Add(this.textBoxModulePath);
            this.groupBoxMain.Controls.Add(this.label2);
            this.groupBoxMain.Controls.Add(this.textBoxModuleName);
            this.groupBoxMain.Controls.Add(this.label1);
            this.groupBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMain.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMain.Name = "groupBoxMain";
            this.groupBoxMain.Size = new System.Drawing.Size(493, 143);
            this.groupBoxMain.TabIndex = 0;
            this.groupBoxMain.TabStop = false;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(53, 108);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(373, 23);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxModuleIP
            // 
            this.textBoxModuleIP.Location = new System.Drawing.Point(53, 73);
            this.textBoxModuleIP.Name = "textBoxModuleIP";
            this.textBoxModuleIP.Size = new System.Drawing.Size(95, 20);
            this.textBoxModuleIP.TabIndex = 6;
            this.textBoxModuleIP.Text = "192.168.0.130";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "IP";
            // 
            // buttonBrowseModulePath
            // 
            this.buttonBrowseModulePath.Location = new System.Drawing.Point(432, 42);
            this.buttonBrowseModulePath.Name = "buttonBrowseModulePath";
            this.buttonBrowseModulePath.Size = new System.Drawing.Size(43, 23);
            this.buttonBrowseModulePath.TabIndex = 4;
            this.buttonBrowseModulePath.Text = "...";
            this.buttonBrowseModulePath.UseVisualStyleBackColor = true;
            this.buttonBrowseModulePath.Click += new System.EventHandler(this.buttonBrowseModulePath_Click);
            // 
            // textBoxModulePath
            // 
            this.textBoxModulePath.Location = new System.Drawing.Point(53, 44);
            this.textBoxModulePath.Name = "textBoxModulePath";
            this.textBoxModulePath.ReadOnly = true;
            this.textBoxModulePath.Size = new System.Drawing.Size(373, 20);
            this.textBoxModulePath.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path";
            // 
            // textBoxModuleName
            // 
            this.textBoxModuleName.Location = new System.Drawing.Point(53, 13);
            this.textBoxModuleName.Name = "textBoxModuleName";
            this.textBoxModuleName.Size = new System.Drawing.Size(373, 20);
            this.textBoxModuleName.TabIndex = 1;
            this.textBoxModuleName.Text = "Machine Manager #1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // AddModuleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 143);
            this.Controls.Add(this.groupBoxMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "AddModuleForm";
            this.Text = "Phoenix.ModuleLoader - Add new module";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddModuleForm_KeyDown);
            this.groupBoxMain.ResumeLayout(false);
            this.groupBoxMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMain;
        private System.Windows.Forms.TextBox textBoxModuleIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonBrowseModulePath;
        private System.Windows.Forms.TextBox textBoxModulePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxModuleName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAdd;
    }
}