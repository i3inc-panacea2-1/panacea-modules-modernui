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
using System.Windows.Shapes;
using System.Windows.Threading;
using Panacea.Controls;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    /// Interaction logic for ToastWindow.xaml
    /// </summary>
    public partial class ToastWindow : NonFocusableWindow
    {
        int _timeout;
        public ToastWindow(int timeout)
        {
            InitializeComponent();
            
            DataContext = this;
            _timeout = timeout;
            Loaded += ToastWindow_Loaded;
        }

        private async void ToastWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ToastWindow_Loaded;
            await Task.Delay(_timeout);
            Close();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (ToastWindow), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var toast = dependencyObject as ToastWindow;
            if (toast == null)
            {
                return;
            }
            if (toast.Text == null)
            {
                toast.Opacity = 0;
                return;
            }
            toast.Opacity = 1;
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}