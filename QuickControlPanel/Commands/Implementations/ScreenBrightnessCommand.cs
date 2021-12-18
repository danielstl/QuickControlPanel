using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickControlPanel.Commands.Implementations
{
    public class ScreenBrightnessCommand : Command
    {
        /*[DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);
        private static Int32 hdc;
        private static int a;*/

        public ScreenBrightnessCommand() : base("Adjust Screen Brightness", "Adjust the brightness of your screen to your parameter", "display", "light")
        {
        }

        public override void ExecuteCommand(char[] flags, string param)
        {
            byte percentage;
            if (!byte.TryParse(param, out percentage))
            {
                MessageBox.Show("Invalid parameter! (0-255 needed)");
                return;
            }

            var scope = new ManagementScope("root\\WMI");
            var query = new SelectQuery("WmiMonitorBrightnessMethods");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                using (var coll = searcher.Get())
                {
                    foreach (ManagementObject obj in coll)
                    {
                        obj.InvokeMethod("WmiSetBrightness", new Object [] { UInt32.MaxValue, percentage });
                        break;
                    }
                }
            }
        }
    }
}
