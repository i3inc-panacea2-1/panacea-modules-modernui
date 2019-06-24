using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Controls;
using Panacea.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Panacea.Modules.ModernUi.ViewModels
{
    [View(typeof(CharmsBar))]
    public class CharmsBarViewModel: PopupViewModelBase<object>
    {
        public List<ControlContainer> Controls { get; set; }

        public CharmsBarViewModel(List<UIElement> controls)
        {
            Controls = controls.Select(c => new ControlContainer() { Control = c }).ToList();
        }
    }

    public class ControlContainer
    {
        public UIElement Control { get; set; }
    }
}
