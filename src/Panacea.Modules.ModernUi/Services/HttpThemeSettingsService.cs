using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panacea.Core;
using Panacea.Modules.ModernUi.Models;

namespace Panacea.Modules.ModernUi.Services
{
    public class HttpThemeSettingsService : HttpServiceBase, IThemeSettingsService
    {
        public HttpThemeSettingsService(IHttpClient client):base(client)
        {
            
        }
        public async Task<GetThemesResponse> GetThemeSettingsAsync()
        {
            return ThrowIfError(await _client.GetObjectAsync<GetThemesResponse>("get_themes/"));
        }
    }
}
