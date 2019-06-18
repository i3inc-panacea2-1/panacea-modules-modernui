using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
        public ToastWindow()
        {
            InitializeComponent();
            Texts = new ObservableCollection<string>();
            var screen = Screen.PrimaryScreen;
            Left = screen.WorkingArea.Left;
            Top = screen.WorkingArea.Top;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
        }

        public ObservableCollection<string> Texts
        {
            get { return (ObservableCollection<string>)GetValue(TextsProperty); }
            set { SetValue(TextsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Texts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextsProperty =
            DependencyProperty.Register("Texts", typeof(ObservableCollection<string>), typeof(ToastWindow), new PropertyMetadata(null));


        public async void Add(string text, int timeout)
        {
            Texts.Add(text);
            Show();
            await Task.Delay(timeout);
            Texts.Remove(text);
            if (Texts.Count == 0)
                Hide();
        }
    }
}