using Panacea.Modularity.UiManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Panacea.Modules.ModernUi
{
    public abstract class NavigatorBase : UserControl, INavigator
    {
        readonly List<string> _navigationHistory = new List<string>();

        public IReadOnlyList<string> NavigationHistory => _navigationHistory.AsReadOnly();

        public virtual FrameworkElement CurrentPage
        {
            get { return (FrameworkElement)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(FrameworkElement), typeof(NavigatorBase), new FrameworkPropertyMetadata(null, OnCurrentPageChanged));

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public event EventHandler<BeforeNavigateEventArgs> Back;

        protected void OnBack(BeforeNavigateEventArgs args)
        {
            Back?.Invoke(this, args);
        }

        public event EventHandler AfterNavigate;
        public event EventHandler<BeforeNavigateEventArgs> BeforeNavigate;

        public IReadOnlyList<FrameworkElement> History => _history.AsReadOnly();

        protected List<FrameworkElement> _history = new List<FrameworkElement>();

        public virtual bool IsNavigationDisabled { get; set; }

        public bool IsHomeTheCurrentPage => CurrentPage == _history.FirstOrDefault();

        public virtual void Dispose()
        {
            _history.Clear();
        }

        public virtual void GoBack(int count = 1)
        {
            var args = new BeforeNavigateEventArgs();
            OnBack(args);
            if (args.Cancel) return;
            if (_history.Last() != CurrentPage)
            {
                count--;
            }
            if (_history.Count() - count > 1) _history.RemoveRange(_history.Count() - 1, count);
            else _history.RemoveRange(1, _history.Count() - 1);

            Navigate(_history.Last(), false);
        }

        public virtual void Navigate(FrameworkElement page, bool cache = true)
        {
            if (page == null) return;
            var args = new BeforeNavigateEventArgs();
            BeforeNavigate?.Invoke(this, args);
            if (args.Cancel) return;
            if (cache) _history.Add(page);
            CurrentPage = page;
            if (_navigationHistory.Count > 200)
            {
                _navigationHistory.RemoveAt(0);
            }
            _navigationHistory.Add(page.GetType().FullName);
            AfterNavigate?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GoHome()
        {
            if (_history.Count() > 1)
            {
                _history.RemoveRange(1, _history.Count() - 1);
            }
            Navigate(_history.Last(), false);
        }
    }
}
