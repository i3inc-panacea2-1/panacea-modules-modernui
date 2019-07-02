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
        List<ResourceDictionary> _resources = new List<ResourceDictionary>();
        public ModernUiManagerPlugin(PanaceaServices core)
        {
            _core = core;
            _themesService = new HttpThemeSettingsService(core.HttpClient);
            CacheImage.ImageUrlChanged += CacheImage_ImageUrlChanged;
            _resources.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Panacea.Controls;component/Styles/Colors/Default.xaml", UriKind.Absolute)
            });
            _resources.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Panacea.Controls;component/Styles/Default.xaml", UriKind.Absolute)
            });
            _resources.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Panacea.Modules.ModernUi;component/Styles/Shared.xaml", UriKind.Absolute)
            });
            foreach (var res in _resources)
            {
                Application.Current.Resources.MergedDictionaries.Add(res);
            }

            //
        }

        public async Task BeginInit()
        {
            _settings = await _themesService.GetThemeSettingsAsync();
            _manager = new ModernThemeManager(_core, _settings.Themes[0]);
            _manager.Paused += _manager_Paused;
            _manager.Resumed += _manager_Resumed;
        }

        private void _manager_Resumed(object sender, EventArgs e)
        {
            if(_window!= null)
            {
                _window.WindowState = _prevState;
            }
        }
        WindowState _prevState;
        private void _manager_Paused(object sender, EventArgs e)
        {
            if (_window != null)
            {
                _prevState = _window.WindowState;
                _window.WindowState = WindowState.Minimized;
            }
        }

        public void Dispose()
        {

        }

        public Task EndInit()
        {
            _core.PluginLoader.LoadFinished += PluginLoader_LoadFinished;
            return Task.CompletedTask;
        }
        ModernWindow _window;
        private void PluginLoader_LoadFinished(object sender, EventArgs e)
        {
            _core.PluginLoader.LoadFinished -= PluginLoader_LoadFinished;
            _window = new ModernWindow();
            _window.Content = _manager;
            _window.Show();
            _window.Closed += Window_Closed;
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
            foreach (var res in _resources)
            {
                Application.Current.Resources.MergedDictionaries.Remove(res);
            }
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
                try
                {
                    var download = await _core.HttpClient.DownloadDataAsync(url);
                    if (download != null)
                    {
                        await SetImagePath(image, download);
                    }
                }
                catch
                {
                    //set default
                }
            }

        }
    }
}
