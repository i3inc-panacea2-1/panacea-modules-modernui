using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Controls;
using Panacea.Mvvm;
using System.Collections.Generic;
using System.Windows;

namespace Panacea.Modules.ModernUi.ViewModels
{
    [View(typeof(CharmsBar))]
    public class CharmsBarViewModel: PopupViewModelBase<object>
    {
        public List<UIElement> Controls { get; set; }

        public CharmsBarViewModel(List<UIElement> controls)
        {
            Controls = controls;
        }
    }
}
