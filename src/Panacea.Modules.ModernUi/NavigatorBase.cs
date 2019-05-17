using Panacea.Mvvm;
using Panacea.Modularity.UiManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Panacea.Modules.ModernUi
{
    public class NavigatorBase : UserControl, INavigator
    {
        readonly List<string> _navigationHistory = new List<string>();

        public IReadOnlyList<string> NavigationHistory => _navigationHistory.AsReadOnly();

        public  ViewModelBase CurrentPage
        {
            get;set;
        }

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

        public IReadOnlyList<ViewModelBase> History => _history.AsReadOnly();

        protected List<ViewModelBase> _history = new List<ViewModelBase>();

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

        public virtual void Navigate(ViewModelBase page, bool cache = true)
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
