using Shell32;

namespace QuickControlPanel.Commands.Implementations
{
    public class ToggleDesktopCommand : Command
    {
        public ToggleDesktopCommand() : base("Toggle desktop", "Show or hide your desktop")
        {
        }
                
        public override void ExecuteCommand(char[] flags, string param)
        {
            new Shell().ToggleDesktop();
        }
    }
}