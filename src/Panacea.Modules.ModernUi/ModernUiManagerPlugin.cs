using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Panacea.Modules.ModernUi
{
    public class ModernUiManagerPlugin : IUiManagerPlugin
    {
        ModernThemeManager _manager;
        private readonly PanaceaServices _core;
        IThemeSettingsService _themesService;

        public ModernUiManagerPlugin(PanaceaServices core)
        {
            _core = core;
            _themesService = new HttpThemeSettingsService(core.HttpClient);
        }

        public async Task BeginInit()
        {
            var settings = await _themesService.GetThemeSettingsAsync();
            _manager = new ModernThemeManager(_core, settings.Themes[0]);
        }

        public void Dispose()
        {
           
        }

        public Task EndInit()
        {
            var window = new Window();
            window.Content = _manager;
            window.Show();
            return Task.CompletedTask;
        }

        public IUiManager GetUiManager()
        {
            return _manager;
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
