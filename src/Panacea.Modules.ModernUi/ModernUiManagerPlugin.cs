using Panacea.Modularity.UiManager;
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
        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
           
        }

        public Task EndInit()
        {
            return Task.CompletedTask;
        }

        public IUiManager GetUiManager()
        {
            throw new NotImplementedException();
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
