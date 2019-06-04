using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Panacea.Modules.ModernUi
{
    class ModernWindow:Window
    {
        public ModernWindow()
        {
            InkInputHelper.DisableWPFTabletSupport();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Automation.AutomationElement asForm =
                System.Windows.Automation.AutomationElement.FromHandle(new WindowInteropHelper(this).Handle);


            InputPanelConfigurationLib.InputPanelConfiguration inputPanelConfig = new InputPanelConfigurationLib.InputPanelConfiguration();
            inputPanelConfig.EnableFocusTracking();
        }
    }
}
