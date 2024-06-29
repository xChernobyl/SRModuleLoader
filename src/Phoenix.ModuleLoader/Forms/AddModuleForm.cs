using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Phoenix.ModuleLoader.Forms
{
    public partial class AddModuleForm : Form
    {
        private readonly ListView _listView;

        public AddModuleForm(ListView lv)
        {
            _listView = lv;
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
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
            listViewItem.SubItems.Add("<None>");

            _listView.Items.Add(listViewItem);

            //save up the data.
            ConfigManager.ModuleConfigSet.ModuleConfigs.Add(new
                Config.ModuleConfigItem(
                moduleName: textBoxModuleName.Text,
                modulePath: textBoxModulePath.Text,
                spoofIP: textBoxModuleIP.Text
                ));

            ConfigManager.TrySave("settings.json");

            Close();
        }

        private void buttonBrowseModulePath_Click(object sender, EventArgs e)
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

        private void AddModuleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
