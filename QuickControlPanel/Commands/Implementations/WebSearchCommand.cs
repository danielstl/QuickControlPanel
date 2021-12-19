using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickControlPanel.Utils;

namespace QuickControlPanel.Commands.Implementations
{
    public class WebSearchCommand : Command
    {
        public WebSearchCommand() : base("Google Search", "Search Google for your parameter", "lookup")
        { }

        public override void ExecuteCommand(char[] flags, string param)
        {
            CommandUtils.OpenUrl(@"https://www.google.com/search?q=" + Uri.EscapeDataString(param));
        }
    }
}
