using Panacea.Multilinguality;
using System;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    ///     Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : UserControl
    {
        private DispatcherTimer t;

        public event RoutedEventHandler Click;

        public Clock()
        {
            InitializeComponent();
            UpdateClock();
        }
		
	    private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			this.Unloaded -= OnUnloaded;
			t.Tick -= t_Elapsed;
			LanguageContext.Instance.LanguageChanged -= Instance_LanguageChanged;

			t.Stop();
			t = null;
	    }

        private void UpdateClock()
        {

            Text.Text = DateTime.Now.ToString("HH:mm");
            Text2.Text = DateTime.Now.ToString("ddd MMM d", LanguageContext.Instance.Culture);

        }

        private void Clock_OnLoaded(object sender, RoutedEventArgs e)
        {
            t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            t.Tick += t_Elapsed;
            LanguageContext.Instance.LanguageChanged += Instance_LanguageChanged;
            t.Start();
            this.Unloaded += OnUnloaded;
        }

        void Instance_LanguageChanged(object sender, EventArgs e)
        {
            UpdateClock();
        }

        void t_Elapsed(object sender, EventArgs e)
        {
            UpdateClock();
        }

        protected void OnClick(RoutedEventArgs args)
        {
            Click?.Invoke(this, args);
        }

        private void TextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnClick(e);
        }
    }
}