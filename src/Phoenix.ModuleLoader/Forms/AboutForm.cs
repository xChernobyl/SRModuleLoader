using System;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.ModuleLoader.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Phoenix.ModuleLoader");
            sb.AppendFormat("Version: 1.0.5");
            sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Chernobyl, 2021 - 2022");
            sb.AppendLine("Discord: Chernobyl#1465");
            sb.AppendLine("Email: cherno0x2f@gmail.com");

            textBoxContent.Text = sb.ToString();
            textBoxContent.SelectionStart = textBoxContent.Text.Length;
        }

        private void AboutForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
