using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Phoenix.ModuleLoader.Config;

namespace Phoenix.ModuleLoader.Forms
{

    public partial class MainForm : Form
    {
        static readonly Color ModuleActiveRowColor = Color.Green;
        //static readonly Color ModuleOfflineRowColor = Color.Red;

        //See MultiModuleLoader.cpp for error code reference.
        static void WriteLoaderResultCodeString(string moduleName, string spoofIp, int exitCode)
        {
            if (exitCode == -1)
            {
                Logger.WriteLine(LogType.Error, $"Loader error: Invalid arg count.");
                return;
            }

            if (exitCode == -2)
            {
                Logger.WriteLine(LogType.Error, $"Loader error: Failed to create suspended process for [{moduleName}].");
                return;
            }

            if (exitCode == -3)
            {
                Logger.WriteLine(LogType.Error, $"Loader error: Failed to inject library for [{moduleName}].");
                return;
            }

            if (exitCode > 0)
            {
                Logger.WriteLine(LogType.Normal, $"The module [{moduleName}] has been created with ip [{spoofIp}].");
                return;
            }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.AssignTextBox(textBoxLog);
            ConfigManager.AssignListView(listViewMain);

            ConfigManager.TryLoad("settings.json");

            foreach (ModuleConfigItem item in ConfigManager.ModuleConfigSet.ModuleConfigs)
            {
                var listViewItem = new ListViewItem(item.ModuleName);

                listViewItem.SubItems.Add(item.ModulePath);
                listViewItem.SubItems.Add(item.SpoofIP);
                listViewItem.SubItems.Add("<None>"); //PID
                listViewMain.Items.Add(listViewItem);
            }

 
        }
            
        private void listViewMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var listViewItem = listViewMain.GetItemAt(e.X, e.Y);

                //The user is clicking on empty space.
                if (listViewItem == null)
                {
                    //Logger.WriteLine(LogType.Normal, "The user is clicking on empty space.");

                    var contextMenu = new ContextMenu();

                    var menuItems = new MenuItem[]
                    {
                        new MenuItem("Add new module", OnAddNewModuleClick),
                        new MenuItem("Kill all modules", OnKillAllModulesClick)
                    };

                    contextMenu.MenuItems.AddRange(menuItems);

                    contextMenu.Show(listViewMain, listViewMain.PointToClient(Cursor.Position));
                }
                else
                {
                    //Logger.WriteLine(LogType.Normal, "The user is clicking on a specific row.");

                    listViewItem.Selected = true;

                    var contextMenu = new ContextMenu();
                    var launchModuleItem = new MenuItem("Launch", OnLaunchModuleClick);
                    var editModuleItem = new MenuItem("Edit", OnEditModuleClick);
                    var removeModuleItem = new MenuItem("Remove", OnRemoveModuleClick);
                    var killModuleItem = new MenuItem("Kill", OnKillModuleClick);

                    contextMenu.MenuItems.AddRange(
                        new MenuItem[] { launchModuleItem, editModuleItem, removeModuleItem, killModuleItem }
                        );

                    contextMenu.Show(listViewMain, listViewMain.PointToClient(Cursor.Position));
                }
            }
        }

        private void OnAddNewModuleClick(object sender, EventArgs e)
        {
            //Logger.WriteLine(LogType.Normal, "Show the add new item form now.");
            var addModuleForm = new AddModuleForm(listViewMain);
            addModuleForm.ShowDialog();
        }

        private void OnKillAllModulesClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewMain.Items)
            {
                if (item.SubItems[3].Text == "<None>")
                    continue;

                var proc = Process.GetProcessById(int.Parse(item.SubItems[3].Text));
                if (proc == null)
                    continue;

                proc.Kill();
            }

            Logger.WriteLine(LogType.Normal, "Module processes killed.");
        }

        private void LaunchSelectedModule()
        {
            ListViewItem selectedItem = listViewMain.SelectedItems[0];
            listViewMain.SelectedIndices.Clear();

            string name = selectedItem.SubItems[0].Text;
            string procPath = selectedItem.SubItems[1].Text;
            string spoofIp = selectedItem.SubItems[2].Text;
            string pid = selectedItem.SubItems[3].Text;

            if (!File.Exists(procPath))
            {
                Logger.WriteLine(LogType.Error, $"Invalid module path [{procPath}].");
                return;
            }

            if (pid != "<None>")
            {
                Logger.WriteLine(LogType.Warning, $"The process [{name}] is already running.");
                return;
            }

            string workingDirectory = Environment.CurrentDirectory;
            string libFullPath = Path.Combine(workingDirectory, "MultiModuleLib.dll");
            string loaderFullPath = Path.Combine(workingDirectory, "MultiModuleLoader.exe");


            var startInfo = new ProcessStartInfo(
                loaderFullPath,
                $"\"{procPath}\" \"{libFullPath}\" {spoofIp}");

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = Path.GetDirectoryName(procPath);

            Logger.WriteLine(LogType.Normal, $"The module [{name}] will now start with ip [{spoofIp}].");
            Logger.WriteLine(LogType.Normal, $"Loader args: [{startInfo.Arguments}].");

            Process loaderProc = Process.Start(startInfo);
            //Wait for the loader to finish its job.
            loaderProc.WaitForExit();


            WriteLoaderResultCodeString(name, spoofIp, loaderProc.ExitCode);

            //Exit code is the return val from main() fn, which is PID on success.
            if (loaderProc.ExitCode > 0)
            {
                var moduleProc = Process.GetProcessById(loaderProc.ExitCode);
                moduleProc.EnableRaisingEvents = true;

                if (moduleProc != null)
                {
                    selectedItem.SubItems[3].Text = moduleProc.Id.ToString();
                    selectedItem.BackColor = ModuleActiveRowColor;
                    moduleProc.Exited += (s, e) => OnProcessExited(s, new ProcessExitedEventArgs(selectedItem));
                }
            }

        }

        private void OnProcessExited(object sender, ProcessExitedEventArgs e)
        {
            var proc = sender as Process;
            Logger.WriteLine(LogType.Warning, $"Process with PID {proc.Id} exited.");
            e.Row.SubItems[3].Text = "<None>";
            e.Row.BackColor = Color.Empty;
        }


        private void OnRemoveModuleClick(object sender, EventArgs e)
        {
            var selectedItem = listViewMain.SelectedItems[0];
            listViewMain.Items.Remove(selectedItem);

            ConfigManager.TrySave("settings.json");

            Logger.WriteLine(LogType.Normal, $"Module [{selectedItem.Text}] has been removed.");
        }


        private void OnEditModuleClick(object sender, EventArgs e)
        {
            var selectedItem = listViewMain.SelectedItems[0];
            new EditModuleForm(selectedItem).ShowDialog();
        }


        private void OnLaunchModuleClick(object sender, EventArgs e) =>
            LaunchSelectedModule();

        private void OnKillModuleClick(object sender, EventArgs e) => KillSelectedRowProcess();

        private void listViewMain_MouseDoubleClick(object sender, MouseEventArgs e) =>
            LaunchSelectedModule();


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => 
            new AboutForm().ShowDialog();


        private void listViewMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                KillSelectedRowProcess();
        }

        private void KillSelectedRowProcess()
        {
            if (listViewMain.SelectedItems.Count == 0)
                return;

            ListViewItem selectedItem = listViewMain.SelectedItems[0];

            if (selectedItem.SubItems[3].Text == "<None>")
            {
                Logger.WriteLine(LogType.Warning, "The process is not running.");
                return;
            }

            var proc = Process.GetProcessById(int.Parse(selectedItem.SubItems[3].Text));
            if (proc == null)
            {
                Logger.WriteLine(LogType.Warning, "Could not grab process by its id.");
                return;
            }

            proc.Kill();
            selectedItem.BackColor = Color.Empty;
        }
    }
}
