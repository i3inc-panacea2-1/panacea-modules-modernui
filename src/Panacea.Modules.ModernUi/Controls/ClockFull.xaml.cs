using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Panacea.Multilinguality;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    ///     Interaction logic for ClockFull.xaml
    /// </summary>
    public partial class ClockFull : UserControl
    {
        private DispatcherTimer t;
        public ClockFull()
        {
            InitializeComponent();
          
        }

	    private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
	    {
			Unloaded -= OnUnloaded;
			
			LanguageContext.Instance.LanguageChanged -= Instance_LanguageChanged;
            if (t != null)
            {
                t.Tick -= t_Elapsed;
                t.Stop();
                t = null;
            }
	    }

#if DEBUG
        ~ClockFull()
        {
            System.Diagnostics.Debug.WriteLine("~ClockFull");
        }
#endif

	    private void UpdateDate()
	    {
		    Text.Text = DateTime.Now.ToString("HH:mm");
		    date.Text = DateTime.Now.ToString("d MMMM, yyyy", LanguageContext.Instance.Culture);
		    day.Text = DateTime.Now.ToString("dddd", LanguageContext.Instance.Culture);
	    }


        void Instance_LanguageChanged(object sender, EventArgs e)
        {
            UpdateDate();
        }

        void t_Elapsed(object sender, EventArgs e)
        {
            UpdateDate();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            UpdateDate();
            LanguageContext.Instance.LanguageChanged += Instance_LanguageChanged;
            t.Tick += t_Elapsed;
            t.Start();
            Unloaded += OnUnloaded;
        }
    }
}