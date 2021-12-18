using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands.Implementations
{
    public class RunCommand : Command
    {
        public RunCommand() : base("Run", "Runs the specified parameter", "start")
        { }

        public override void ExecuteCommand(char[] flags, string param)
        {
            if (param.Trim().Length == 0)
            {
                //todo some sort of gray out to force a parameter without this msg (forceParam command variable?)
                MessageBox.Show("Parameter required");
                return;
            }

            var split = param.Split(new[] { ' ' }, 2);

            var proc = new ProcessStartInfo
            {
                FileName = split[0]
            };

            if (split.Length > 1)
            {
                proc.Arguments = split[1];
            }

            if (flags.Contains('^'))
            {
                proc.Verb = "runas";
            }

            try
            {
                Process.Start(proc);
            } catch (Exception)
            {
                MessageBox.Show("Couldn't find that file");
                //todo some sort of other error system, like in the ui itself
            }
        }
    }
}
