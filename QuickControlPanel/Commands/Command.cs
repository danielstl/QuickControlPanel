using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickControlPanel.Commands
{
    public abstract class Command
    {//todo command flag system (eg ^ to launch as elavated)
        public string DisplayName { get; }
        public string Description { get; }
        public string[] Aliases { get; }

        public Command(string displayName, string description, params string[] aliases)
        {
            DisplayName = displayName;
            Description = description;
            Aliases = aliases;
        }

        public abstract void ExecuteCommand(char[] flags, string param);
    }
}
