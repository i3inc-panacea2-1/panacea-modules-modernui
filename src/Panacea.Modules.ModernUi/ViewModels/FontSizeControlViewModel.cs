using Panacea.Controls;
using Panacea.Modularity.UiManager;
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
    class FontSizeControlViewModel : SettingsControlViewModelBase
    {
        bool _popOpen;
        public bool PopupOpen
        {
            get => _popOpen;
            set
            {
                _popOpen = value;
                OnPropertyChanged();
            }
        }

        public FontSizeControlViewModel()
        {
            ClickCommand = new RelayCommand(args =>
            {
                PopupOpen = !PopupOpen;
            },
           args => !PopupOpen);
            IncreaseCommand = new RelayCommand(args =>
            {
                Ratio+=2;
            },
            args => Ratio < 140);

            DecreaseCommand = new RelayCommand(args =>
            {
                Ratio-=2;
            },
           args => Ratio > 80);
        }

        int _ratio = 100;
        public int Ratio
        {
            get => _ratio;
            set
            {
                _ratio = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand IncreaseCommand { get; }

        public RelayCommand DecreaseCommand { get; }

        public RelayCommand ClickCommand { get; }
    }
}
