using QuickControlPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands
{
    public class CommandRegistry
    {
        public static CommandRegistry Instance = new CommandRegistry();

        public static char[] Flags = new[] { '^', '*', '?' };

        public List<Command> RegisteredCommands { get; } = new List<Command>();

        public ObservableCollection<CommandViewModel> FilterForInput(string input)
        {
            //if (input.Trim().Length == 0) return new ObservableCollection<CommandViewModel>();

            input = input.ToLower(); //make lower for matching cmd names
            input = input.Split(',')[0]; //get rid of users argument if present

            foreach (char flag in Flags) input = input.Replace(flag.ToString(), string.Empty); //get rid of all flags

            return new ObservableCollection<CommandViewModel>(RegisteredCommands.Where(c =>
                c.DisplayName.ToLower().Contains(input) || input.Contains(c.DisplayName.ToLower()) || //matches any command names?
                c.Aliases.Any(a => input.Contains(a) || a.Contains(input)) //matches any aliases?
                ).OrderByDescending(c => (c.DisplayName.ToLower().Contains(input) || input.Contains(c.DisplayName.ToLower())) ? 1 : 0) //todo better sorting, e.g. best match
                .Select(c => new CommandViewModel(c))); //convert command to vm command
        }

        private CommandRegistry()
        {
            try
            {
                var cmds = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith(".Commands.Implementations"));
                
                foreach (Type t in cmds)
                {
                    RegisterCommand((Command)Activator.CreateInstance(t));
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Error registering commands");
            }

            RegisterCommand(
                new StartProcessCommand("Windows Explorer", "Start Windows Explorer", @"C:\Windows\explorer.exe", true),
                new StartProcessCommand("Control Panel", "Start Control Panel", @"C:\Windows\System32\control.exe", true, "settings"),
                new StartProcessCommand("Settings", "Start Settings", @"ms-settings:", false, "control panel"),
                new StartProcessCommand("Command Prompt", "Start Command Prompt", @"C:\Windows\System32\cmd.exe", true, "cmd"),
                new StartProcessCommand("Powershell", "Start Powershell", @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", true, "cmd"),
                new StartProcessCommand("God Mode", "Open Control Panel with all options available", @"C:\Windows\explorer.exe", @"shell:::{ED7BA470-8E54-465E-825C-99712043E01C}", true, "control", "settings"),
                new StartProcessCommand("Clipboard Screenshot", "Create a screenshot and copy to your clipboard", @"C:\Windows\System32\SnippingTool.exe", "/clip", false, "snip"),
                new StartProcessCommand("Lock Computer", "Lock your computer", @"C:\Windows\System32\rundll32.exe", "user32.dll,LockWorkStation", false),
                new StartProcessCommand("Task Manager", "Start Task Manager", @"C:\Windows\System32\Taskmgr.exe", true, "taskmgr"),
                new StartProcessCommand("Calculator", "Open the Calculator", @"C:\Windows\System32\calc.exe", false)
                );

            RegisterCommand(
                new PowerStateCommand("Hibernate Computer", "Hibernate your computer", System.Windows.Forms.PowerState.Hibernate, "power"),
                new PowerStateCommand("Sleep Computer", "Put your computer to sleep mode", System.Windows.Forms.PowerState.Suspend, "standby", "suspend", "power")
                );

            //RegisterCommand(new TestCommand());
            //RegisterCommand(new LockCommand());
        }

        public void RegisterCommand(params Command[] command)
        {
            RegisteredCommands.AddRange(command);
        }

        public void ProcessCommand(Command command, string commandInput)
        {
            var split = commandInput.Split(new[] { ',' }, 2); //only get 1 split at , to filter user argument

            var flags = split[0].Where(c => Flags.Contains(c)).Distinct().ToArray(); //get all flags used before user's argument

            command.ExecuteCommand(flags, split.Length < 2 ? "" : split[1]);
        }
    }
}
