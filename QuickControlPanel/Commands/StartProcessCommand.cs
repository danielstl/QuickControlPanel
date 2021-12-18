using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands
{
    public class StartProcessCommand : Command
    {
        public string ProcessName { get; }
        public bool SupportsElevation { get; }
        public string Parameter { get; }

        public StartProcessCommand(string displayName, string description, string processName, bool supportsElevation = false, params string[] aliases) : this(displayName, description, processName, null, supportsElevation, aliases)
        {
        }

        public StartProcessCommand(string displayName, string description, string processName, string parameter, bool supportsElevation = false, params string[] aliases) : base(displayName, description, aliases)
        {
            ProcessName = processName;
            SupportsElevation = supportsElevation;
            Parameter = parameter;
        }

        public override void ExecuteCommand(char[] flags, string param)
        {
            var proc = new ProcessStartInfo
            {
                FileName = ProcessName
            };

            if (SupportsElevation && flags.Contains('^'))
            {
                proc.Verb = "runas"; //start as admin
            }

            if (Parameter != null)
            {
                proc.Arguments = Parameter;
            }

            try
            {
                Process.Start(proc);
            } catch (Exception e)
            {
                if (e is Win32Exception)
                {
                    var ex = e as Win32Exception;
                    //todo stop elevation cancelling showing error
                    //if (ex.HResult ==)
                }
                MessageBox.Show("Error launching " + ProcessName + ", " + e.Message + ", " + e.HResult);
            }
        }
    }
}
