using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands.Implementations
{
    public class CloseCommand : Command
    {
        public CloseCommand() : base("Exit", "Fully close the quick control panel", "close")
        { }
        
        public override void ExecuteCommand(char[] flags, string param)
        {
            Application.Current.Shutdown();
        }
    }
}
