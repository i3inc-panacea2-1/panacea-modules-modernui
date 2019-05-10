using Panacea.Modules.ModernUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.ModernUi.Services
{
    public interface IThemeSettingsService
    {
        Task<GetThemesResponse> GetThemeSettingsAsync();
    }
}
