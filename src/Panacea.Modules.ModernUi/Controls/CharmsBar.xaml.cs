using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
//using CoreAudioApi;
using System.Windows.Threading;
using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Multilinguality;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    ///     Interaction logic for CharmsBar.xaml
    /// </summary>
    public partial class CharmsBar : UserControl
    {
        public CharmsBar(PanaceaServices core)
        {
            _core = core;
            InitializeComponent();
        }

        private readonly PanaceaServices _core;

        public event EventHandler Close;

        protected void OnClose()
        {
            Close?.Invoke(this, null);
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            OnClose();
        }

        private void CharmsBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            //fontSize.Value = TextSizePercentage * 100;
            //if (!DesignerProperties.GetIsInDesignMode(this))
            //{

            //    LanguageSettings = (ILanguageSettings)PluginManager;


            //   //todo
            //   //if (!pluginManager.LoadedPlugins.ContainsKey("Favorites")) btnFavorites.Visibility = Visibility.Collapsed;
            //   //if (!pluginManager.LoadedPlugins.ContainsKey("Radio")) btnRadio.Visibility = Visibility.Collapsed;
            //   //if (!pluginManager.LoadedPlugins.ContainsKey("Television")) btnTelevision.Visibility = Visibility.Collapsed;
            //   //if (!pluginManager.LoadedIPlugins.ContainsKey("WebBrowser")) btnWeb.Visibility = Visibility.Collapsed;

            //    UserManager.UserLoggedIn += Host_UserLoggedIn;
            //    UserManager.UserLoggedOut += Host_UserLoggedOut;
            //    if (UserManager.User.ID == null)
            //    {
            //        Host_UserLoggedOut(null, UserManager.User);

            //    }
            //    else
            //    {
            //        Host_UserLoggedIn(null, UserManager.User);
            //    }
            //    if(userManager.User.ID!= null && PluginManager.LoadedPlugins.ContainsKey("Favorites"))
            //    {
            //        btnFavorites.Visibility = Visibility.Visible;
            //    }
            //}
        }

        private void CharmsBar_OnUnloaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void FontSize_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) return;
            //window.TextSizePercentage = fontSize.Value / 100;
        }

        private void DevelopersPageButton_Click(object sender, RoutedEventArgs e)
        {
            //window.RequestMagicPin();
            OnClose();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var res = base.MeasureOverride(constraint);
            if (constraint.Width != double.PositiveInfinity && constraint.Height != double.PositiveInfinity)
                return new Size(res.Width, constraint.Height);


            return res;
        }

    }

    public class MultiplyBy100Converter : IValueConverter
    {
        public object Convert(object value, Type t, object w, CultureInfo info)
        {
            return ((double)value * 100).ToString("#0");
        }

        public object ConvertBack(object value, Type t, object w, CultureInfo info)
        {
            throw new NotImplementedException();
        }

        
    }

    class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("0") + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}