using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands.Implementations
{
    public class TranslateToEnglishCommand : Command
    {
        public TranslateToEnglishCommand() : base("Google Translate to English", "Translate your clipboard or parameter to English", "translator")
        { }

        public override void ExecuteCommand(char[] flags, string param)
        {
            if (string.IsNullOrEmpty(param)) param = Clipboard.ContainsText() ? Clipboard.GetText() : "";
            Process.Start(@"https://translate.google.com/#auto/en/" + Uri.EscapeDataString(param));
        }
    }
}
