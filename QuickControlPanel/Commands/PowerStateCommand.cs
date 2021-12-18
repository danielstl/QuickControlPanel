using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickControlPanel.Commands
{
    public class PowerStateCommand : Command
    {
        public PowerState State { get; }

        public PowerStateCommand(string displayName, string description, PowerState state, params string[] aliases) : base(displayName, description, aliases)
        {
            State = state;
        }

        public override void ExecuteCommand(char[] flags, string param)
        {
            Application.SetSuspendState(State, flags.Contains('*'), true);
        }
    }
}
