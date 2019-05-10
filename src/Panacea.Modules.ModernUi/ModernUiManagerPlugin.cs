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
        IThemeSettingsService _themesService;

        public ModernUiManagerPlugin(PanaceaServices core)
        {
            _manager = new ModernThemeManager(core);
            _themesService = new HttpThemeSettingsService(core.HttpClient);
        }

        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
           
        }

        public async Task EndInit()
        {
            var settings = await _themesService.GetThemeSettingsAsync();
            var window = new Window();
            window.Content = _manager;
            window.Show();
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
