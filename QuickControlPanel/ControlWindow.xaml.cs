using QuickControlPanel.Commands;
using QuickControlPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuickControlPanel
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window, INotifyPropertyChanged
    {
        #region win32 handles and stuff

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 1;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_F10 = 0x13; //pause key
            const uint MOD_CTRL = 0x0000; //no modifiers
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_F10))
            {
                MessageBox.Show("Error registering hotkey!");
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<Command, CommandViewModel> _commandMap =
            CommandRegistry.Instance.RegisteredCommands.ToDictionary(elem => elem, elem => new CommandViewModel(elem));

        private ObservableCollection<CommandViewModel> _matchingCommands = new ObservableCollection<CommandViewModel>();

        public ObservableCollection<CommandViewModel> MatchingCommands
        {
            get { return _matchingCommands; }
            set
            {
                if (!ReferenceEquals(_matchingCommands, value) && _matchingCommands.Count != value.Count) //TODO HACK
                {
                    _matchingCommands = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MatchingCommands)));
                }
            }
        }

        private string _commandInput;

        public string CommandInput
        {
            get => _commandInput;
            set
            {
                _commandInput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommandInput)));

                var matching = CommandRegistry.Instance.FilterForInput(value);
                var currentlySelected = MatchingCommands.FirstOrDefault(cmd => cmd.IsSelected)?.Command;

                MatchingCommands =
                    new ObservableCollection<CommandViewModel>(matching.Select(cmd => _commandMap[cmd]));

                var retainPrevSelection =
                    currentlySelected != null && matching.Any(cmd => cmd == currentlySelected);

                SelectedCommand = MatchingCommands.Count == 0 ? null :
                    retainPrevSelection ? _commandMap[currentlySelected] : MatchingCommands[0];
            }
        }

        public CommandViewModel SelectedCommand
        {
            get => _commandMap.FirstOrDefault(c => c.Value.IsSelected).Value;
            set
            {
                Debug.WriteLine(value == null ? "null" : value.Command.DisplayName);
                if (SelectedCommand == value)
                {
                    Debug.WriteLine("euqal");
                    return;
                }

                var cmd = SelectedCommand;
                if (cmd != null) cmd.IsSelected = false;

                if (value != null)
                {
                    value.IsSelected = true;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCommand)));
            }
        }

        private ICommand _executeCommandAction;

        public ICommand ExecuteCommandAction
        {
            get
            {
                return _executeCommandAction ??= new RelayCommand(param =>
                {
                    var input = CommandInput;
                    Hide();

                    CommandRegistry.Instance.ProcessCommand(param as Command, input);
                });
            }
        }

        public ControlWindow()
        {
            InitializeComponent();

            DataContext = this;

            MatchingCommands.CollectionChanged += (o, _) =>
            {
                PropertyChanged?.Invoke(o, new PropertyChangedEventArgs(nameof(MatchingCommands)));
            };

            var desktop = SystemParameters.WorkArea;
            Left = desktop.Right - Width;
            // Top = desktop.Bottom - Height;

            Height = desktop.Height;

            Hide();
        }

        private void OnHotKeyPressed()
        {
            CommandInput = "";

            var load = FindResource("LoadAnimation") as Storyboard;
            load.Begin();

            Show();
            Activate();

            tbInput.Focus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Top -= (ActualHeight - e.NewSize.Height); //todo use popup instead of this
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();

            CommandInput = "";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
            else if (e.Key == Key.Enter)
            {
                var cmd = SelectedCommand;
                if (cmd != null)
                {
                    var input = CommandInput;
                    Hide();
                    CommandRegistry.Instance.ProcessCommand(cmd.Command, input);
                }
            }
            else if (e.Key == Key.Up)
            {
                if (MatchingCommands.Count > 0)
                {
                    var cmd = SelectedCommand;
                    var ix = cmd == null ? 0 : MatchingCommands.IndexOf(SelectedCommand) - 1;

                    if (ix < 0) ix = MatchingCommands.Count - 1;
                    else if (ix >= MatchingCommands.Count) ix = 0;

                    SelectedCommand = MatchingCommands[ix];
                }
            }
            else if (e.Key == Key.Down)
            {
                if (MatchingCommands.Count > 0)
                {
                    var cmd = SelectedCommand;
                    var ix = cmd == null ? 0 : MatchingCommands.IndexOf(SelectedCommand) + 1;

                    if (ix < 0) ix = MatchingCommands.Count - 1;
                    else if (ix >= MatchingCommands.Count) ix = 0;

                    SelectedCommand = MatchingCommands[ix];
                }
            }
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
            }
        }
    }
}