using System;
using System.Windows.Forms;

namespace Phoenix.ModuleLoader
{
    public enum LogType
    {
        Normal,
        Warning,
        Error
    }

    public class Logger
    {
        private static TextBox LoggerTextBox;

        public static void AssignTextBox(TextBox tb)
        {
            Logger.LoggerTextBox = tb;
        }

        public static void WriteLine(LogType type, string format, params object[] args)
        {
            string msg = string.Format(format, args);
            string final = $"[{DateTime.Now}] [{type}] \t-> {msg} {Environment.NewLine}";

            LoggerTextBox.AppendText(final);

            LoggerTextBox.SelectionStart = LoggerTextBox.TextLength;
            LoggerTextBox.ScrollToCaret();
        }
    }
}
