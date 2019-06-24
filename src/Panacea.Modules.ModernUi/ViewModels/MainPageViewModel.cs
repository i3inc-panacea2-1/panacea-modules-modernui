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
using System.Windows;
using Panacea.Modules.ModernUi.Views;

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

        public MainPageViewModel(PanaceaServices core, Theme theme)
        {
            _core = core;
            _theme = theme;
            _core.PluginLoader.PluginLoaded += PluginLoader_PluginLoaded;
            foreach (var plugin in core.PluginLoader.LoadedPlugins.Where(kv => kv.Value is ICallablePlugin).Select(kv => kv.Key))
            {
                try
                {
                    if (
                        _theme.Groups.Any(
                            g => g.AppearancePlugins.Any(p => p.Name == plugin))
                        && core.PluginLoader.LoadedPlugins.Any(p => p.Key == plugin && typeof(ILiveTilesPlugin).IsAssignableFrom(p.Value.GetType())))
                        _theme.Groups.SelectMany(g => g.AppearancePlugins)
                                .First(p => p.Name == plugin)
                                .Frames = (core.PluginLoader.LoadedPlugins[plugin] as ILiveTilesPlugin).Frames;
                            

                }
                catch (Exception)
                {
                    //_logger.Debug(null, String.Format("Failed to exceute '{0}': {1}", plugin.Name, ex.Message));
                }
            }

            MainImageButtonClickCommand = new RelayCommand((arg) =>
            {
                var ap = arg as AppearancePlugin;
                var name = ap.Name;
                if (name != null)
                {
                    if (_core.PluginLoader.LoadedPlugins.ContainsKey(name))
                    {
                        var callable = _core.PluginLoader.LoadedPlugins[name] as ICallablePlugin;
                        try
                        {
                            callable?.Call();
                        }
                        catch (Exception ex)
                        {
                            _core.Logger.Error(this, ex.Message);
                        }
                    }
                }
            });
        }

        public override void Activate()
        {
            Theme.Groups
                .SelectMany(g => g.AppearancePlugins)
                .ToList()
                .ForEach(ap => ap.Visibility = ap.ShowAlways || _core.PluginLoader.LoadedPlugins.ContainsKey(ap.Name ?? "") ? Visibility.Visible : Visibility.Collapsed);
        }

        private void PluginLoader_PluginLoaded(object sender, Modularity.IPlugin e)
        {
            Theme.Groups
                .SelectMany(g => g.AppearancePlugins)
                .ToList()
                .ForEach(ap => ap.Visibility = ap.ShowAlways || _core.PluginLoader.LoadedPlugins.ContainsKey(ap.Name ?? "") ? Visibility.Visible : Visibility.Collapsed);
        }

        public ICommand MainAccountButtonClickCommand { get; protected set; }
        public ICommand MainImageButtonClickCommand { get; protected set; }

    }
}