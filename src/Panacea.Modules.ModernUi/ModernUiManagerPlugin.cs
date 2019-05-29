using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Models;
using Panacea.Modules.ModernUi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Panacea.Modules.ModernUi
{
    public class ModernUiManagerPlugin : IUiManagerPlugin
    {
        ModernThemeManager _manager;
        private readonly PanaceaServices _core;
        IThemeSettingsService _themesService;
        GetThemesResponse _settings;
        public ModernUiManagerPlugin(PanaceaServices core)
        {
            _core = core;
            _themesService = new HttpThemeSettingsService(core.HttpClient);
            CacheImage.ImageUrlChanged += CacheImage_ImageUrlChanged;
        }

        public async Task BeginInit()
        {
            _settings = await _themesService.GetThemeSettingsAsync();
            _manager = new ModernThemeManager(_core, _settings.Themes[0]);
        }

        public void Dispose()
        {
           
        }

        public Task EndInit()
        {
           
            var window = new Window();
            window.Content = _manager;
            window.Show();
            window.Closed += Window_Closed;
            return Task.CompletedTask;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public IUiManager GetUiManager()
        {
            return _manager;
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }

        private async void CacheImage_ImageUrlChanged(object sender, string e)
        {
            var image = sender as CacheImage;
            if (!image.IsLoaded)
            {
                image.Loaded += Image_Loaded;
                return;
            }
            await CheckPath(image, e);
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = sender as CacheImage;
            image.Loaded -= Image_Loaded;
            await CheckPath(image, image.ImageUrl);
        }

        private async Task SetImagePath(CacheImage image, byte[] bytes)
        {
            try
            {
                BitmapImage img2 = null;
                await Task.Run(() =>
                {
                    img2 = new BitmapImage();
                    img2.BeginInit();
                    img2.CreateOptions |= BitmapCreateOptions.IgnoreColorProfile;
                    img2.CacheOption = BitmapCacheOption.OnLoad;
                    img2.StreamSource = new MemoryStream(bytes);
                    img2.EndInit();
                    img2.Freeze();
                });
                image.Source = img2;
            }
            catch (Exception e)
            {
                _core.Logger.Error(this, e.Message);
                //SetDefaultImage();
            }
        }

        void SetImage(CacheImage image, string url)
        {
            try
            {
                var img2 = new BitmapImage();
                img2.BeginInit();
                img2.CreateOptions |= BitmapCreateOptions.IgnoreColorProfile;
                img2.CacheOption = BitmapCacheOption.OnLoad;
                img2.UriSource = new Uri(url);
                img2.EndInit();
                //img2.Freeze();
                img2.DownloadCompleted += Img2_DownloadCompleted;

                image.Source = img2;
            }
            catch (Exception e)
            {
                _core.Logger.Error(this, e.Message);
                //SetDefaultImage();
            }
        }

        private void Img2_DownloadCompleted(object sender, EventArgs e)
        {
            BitmapImage bmp = (sender as BitmapImage);
            if (bmp.CanFreeze)
            {
                try
                {
                    bmp.Freeze();
                }
                catch (Exception ex)
                {
                    _core.Logger.Error(this, ex.Message);
                    //SetDefaultImage();
                }
            }

        }

        async Task CheckPath(CacheImage image, string url)
        {
            if (url == null) return;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri res))
            {
                SetImage(image, url);
            }
            else
            {
                var download = await _core.HttpClient.DownloadDataAsync(url);
                if (download != null)
                {
                    await SetImagePath(image, download);
                }
            }

        }
    }
}
