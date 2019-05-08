using System;
using System.Windows.Controls;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    ///     Interaction logic for FadingPopupControlItem.xaml
    /// </summary>
    public partial class FadingPopupControlItem : UserControl
    {
        public event EventHandler Dismiss;
        public FadingPopupControlItem()
        {
            InitializeComponent();
        }

        public FadingPopupControlItem(string caption, Action d)
        {
            InitializeComponent();
            captiontxt.Text = caption;
            timetxt.Text = DateTime.Now.ToShortTimeString();
            this.d = d;
        }

        private void Button_Click(object o, object e)
        {
            d();
        }

        protected Action d;

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            d();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Dismiss?.Invoke(this, null);
        }
    }
}