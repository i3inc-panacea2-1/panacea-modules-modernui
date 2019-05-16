using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Panacea.Controls;
using Panacea.Core;
using Panacea.Mvvm;
using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Models;
using Panacea.Multilinguality;

namespace Panacea.Modules.ModernUi.ViewModels
{
    [View(typeof(MainPage))]
    public class MainPageViewModel : ViewModelBase
    {

        private readonly Translator _translator = new Translator("core");
        private readonly PanaceaServices _core;
        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                if (value == _theme) return;
                _theme = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel(PanaceaServices core)
        {
            _core = core;
            foreach (var plugin in core.PluginLoader.LoadedPlugins.Where(kv => kv.Value is ICallablePlugin).Select(kv => kv.Key))
            {
                try
                {
                    //if (
                    //    _theme.Groups.Any(
                    //        g => g.AppearancePlugins.Any(p => p.Name == plugin))
                    //    && core.PluginLoader.LoadedPlugins.Any(p => p.Key == plugin))

                    //    (core.PluginLoader.LoadedPlugins[plugin] as ICallablePlugin).MainButton =
                    //        _theme.Groups.SelectMany(g => g.AppearancePlugins)
                    //            .First(p => p.Name == plugin);

                }
                catch (Exception)
                {
                    //_logger.Debug(null, String.Format("Failed to exceute '{0}': {1}", plugin.Name, ex.Message));
                }
            }

            MainImageButtonClickCommand = new RelayCommand((arg) =>
            {

            });
        }

        public ICommand MainAccountButtonClickCommand { get; protected set; }
        public ICommand MainImageButtonClickCommand { get; protected set; }

    }
}