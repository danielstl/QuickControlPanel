using Shell32;

namespace QuickControlPanel.Commands.Implementations
{
    public class MinimiseWindowsCommand : Command
    {
        public MinimiseWindowsCommand() : base("Minimise windows", "Minimise all open windows")
        {
        }

        public override void ExecuteCommand(char[] flags, string param)
        {
            new Shell().MinimizeAll();
        }
    }
}
