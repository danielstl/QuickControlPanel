using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands.Implementations
{
    public class TestCommand : Command
    {
        public TestCommand() : base("Test command", "Test description", "alias", "ppp")
        { }            

        public override void ExecuteCommand(char[] flags, string param)
        {
            MessageBox.Show($"Flags given: { string.Join(", ", flags) }\nParameter given: { param }", "Test command executed!");
        }
    }
}
