using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Phoenix.ModuleLoader.Forms
{
    public partial class EditModuleForm : Form
    {
        private readonly ListViewItem _lvm;

        public EditModuleForm(ListViewItem lvm)
        {
            _lvm = lvm;

            InitializeComponent();

            textBoxModuleName.Text = lvm.SubItems[0].Text;
            textBoxModulePath.Text = lvm.SubItems[1].Text;
            textBoxModuleIP.Text = lvm.SubItems[2].Text;

        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            if (textBoxModuleName.Text.Length == 0)
            {
                MessageBox.Show("Module name can not be empty.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                return;
            }

            if (textBoxModulePath.Text.Length == 0 || !File.Exists(textBoxModulePath.Text))
            {
                MessageBox.Show("Module path is empty, or invalid path.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
                return;
            }

            if (!IPAddress.TryParse(textBoxModuleIP.Text, out IPAddress _))
            {
                MessageBox.Show("Invalid module IP address specified.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var listViewItem = new ListViewItem(textBoxModuleName.Text);

            listViewItem.SubItems.Add(textBoxModulePath.Text);
            listViewItem.SubItems.Add(textBoxModuleIP.Text);

            _lvm.SubItems[0].Text = textBoxModuleName.Text;
            _lvm.SubItems[1].Text = textBoxModulePath.Text;
            _lvm.SubItems[2].Text = textBoxModuleIP.Text;


            ConfigManager.TrySave("settings.json");

            Close();

        }

        private void buttonBrowseModulePath_Click(object sender, System.EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse Module Path";
            openFileDialog.DefaultExt = "exe";
            openFileDialog.Filter = "Executable files (*.exe)|*.exe";
            openFileDialog.CheckFileExists = true;

            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBoxModulePath.Text = openFileDialog.FileName;
            }
        }

        private void EditModuleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
