using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panacea.Controls;
using Panacea.Modularity.UiManager;


namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    /// Interaction logic for ModalPopup.xaml
    /// </summary>
    public partial class ModalPopup : OverlayWindow
    {
        public ModalPopup(Window window):base(window)
        {
            Transparency = true;
            InitializeComponent();
            DataContext = this;
        }

        public ModalPopup(Window window, ImageSource source):this(window)
        {
            if(source.CanFreeze)
                source.Freeze();
            Transparency = false;
            AllowsTransparency = false;
            Img.Source = source;
        }

        private bool _closing = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            PopupContent = null;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            var res = Closed?.GetInvocationList();
            if (res != null)
            {
                foreach (Delegate d in Closed?.GetInvocationList())
                {
                    Closed -= (EventHandler) d;
                }
            }
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnClickedOutside();
        }

        public static readonly DependencyProperty TransparencyProperty = DependencyProperty.Register(
            "Transparency", typeof (bool), typeof (ModalPopup), new PropertyMetadata(default(bool)));

        public bool Transparency
        {
            get { return (bool) GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        public new event EventHandler Closed;

        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.Register("PopupContent", typeof(FrameworkElement),
                typeof(ModalPopup),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        // .NET Property wrapper
        public FrameworkElement PopupContent
        {
            get { return (FrameworkElement)GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }

        public static readonly DependencyProperty PopupTypeProperty =
            DependencyProperty.Register("PopupType", typeof(PopupType),
                typeof(ModalPopup),
                new FrameworkPropertyMetadata(PopupType.Empty, OnPopupTypeChanged));


        public static readonly DependencyProperty PopupColorProperty =
            DependencyProperty.Register("PopupColor", typeof(Brush),
                typeof(ModalPopup),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(150, 0, 0, 0))));

        public Brush PopupColor
        {
            get { return (Brush)GetValue(PopupColorProperty); }
            set { SetValue(PopupColorProperty, value); }
        }


        private static void OnPopupTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mp = (ModalPopup)d;

            switch ((PopupType)e.NewValue)
            {
                case PopupType.Error:
                    mp.PopupColor = (SolidColorBrush)Application.Current.Resources["ColorError"];
                    break;
                case PopupType.None:
                    mp.PopupColor = (SolidColorBrush)Application.Current.Resources["ColorNone"];
                    break;
                case PopupType.Warning:
                    mp.PopupColor = (SolidColorBrush)Application.Current.Resources["ColorWarning"];
                    break;
                case PopupType.Information:
                    mp.PopupColor = (SolidColorBrush)Application.Current.Resources["ColorInformation"];
                    break;
                case PopupType.Success:
                    mp.PopupColor = (SolidColorBrush)Application.Current.Resources["ColorSuccess"];
                    break;
            }
        }

        // .NET Property wrapper
        public PopupType PopupType
        {
            get { return (PopupType)GetValue(PopupTypeProperty); }
            set { SetValue(PopupTypeProperty, value); }
        }
        
        protected void OnClickedOutside()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void ModalPopup_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!AllowsTransparency)
                return;

            contentArea.RenderTransform = new ScaleTransform(1, 1);
            contentArea.Opacity = 1;
            return;

        }

        private void ModalPopup_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key.ToString());
            if (e.Key == Key.System)
            {
                e.Handled = true;
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class ShadowDepthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (double) value/10.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class NotNullAndFalseToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is bool && (values[1] is string || values[1] == null))
                return !(bool) values[0] && String.IsNullOrEmpty((string) values[1])
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
           throw new NotImplementedException();
        }
    }
}
