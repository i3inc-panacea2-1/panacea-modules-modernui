// using InputPanelConfigurationLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Panacea.Modules.ModernUi
{
    class ModernWindow:Window
    {
        public ModernWindow()
        {
            //InkInputHelper.DisableWPFTabletSupport();
            //SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            this.Loaded += MainWindow_Loaded;
            if (!Debugger.IsAttached)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }
        }
        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //System.Windows.Automation.AutomationElement asForm =
                //System.Windows.Automation.AutomationElement.FromHandle(new WindowInteropHelper(this).Handle);


          //var inputPanelConfig = new InputPanelConfiguration();
          //  inputPanelConfig.EnableFocusTracking();
            
        }
    }

}
