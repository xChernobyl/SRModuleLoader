using System;
using System.Windows.Forms;

namespace Phoenix.ModuleLoader
{
    class ProcessExitedEventArgs : EventArgs
    {
        public ListViewItem Row;

        public ProcessExitedEventArgs(ListViewItem row)
        {
            Row = row;
        }
    }
}
