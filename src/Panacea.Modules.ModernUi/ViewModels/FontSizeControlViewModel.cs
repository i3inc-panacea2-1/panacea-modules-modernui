using Panacea.Modules.ModernUi.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.ModernUi.ViewModels
{
    [View(typeof(FontSizeSettingsControl))]
    class FontSizeControlViewModel:ViewModelBase
    {
        int _ratio;
        public int Ratio
        {
            get => _ratio;
            set
            {
                _ratio = value;
                OnPropertyChanged();
            }
        }
    }
}
