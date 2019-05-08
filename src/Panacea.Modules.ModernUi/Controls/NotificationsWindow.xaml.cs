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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Panacea.Modules.ModernUi.Controls
{
    /// <summary>
    /// Interaction logic for NotificationsWindow.xaml
    /// </summary>
    public partial class NotificationsWindow : Window
    {
        public NotificationsWindow()
        {
            InitializeComponent();
            IsVisibleChanged += NotificationsWindow_IsVisibleChanged;      
        }

        private void NotificationsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) return;
            Owner?.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            Owner?.Focus();
            Hide();
        }

        public Control Add(string caption, Action del, bool priority = true)
        {
            FadingPopupControlItem it = null;
            it = new FadingPopupControlItem(caption, () =>
            {
                Remove(it); //todo was Host.ThemeManager.Refrain(it);
                CheckNotificationsCount();
                del?.Invoke();
            });
            if (!priority)
                controls.Children.Add(it);
            else controls.Children.Insert(0, it);
            
            return it;
        }

        public new void Show()
        {
            if (Owner == null) return;
            base.Show();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ActualHeight + Top >= SystemParameters.WorkArea.Height)
                {
                    Top = SystemParameters.PrimaryScreenHeight - ActualHeight - 30;
                }
                if (Top <= 0)
                {
                    Top = 30;
                }
                if (ActualWidth + Left >= SystemParameters.WorkArea.Width)
                {
                   
                    Left = SystemParameters.PrimaryScreenWidth - ActualWidth - 30;
                }
                if (Left <= 0)
                {
                    Left = 30;
                }
            }), DispatcherPriority.Background);
            
            
        }

        public int NotificationCount()
        {
            return controls.Children.Count;
        }

        public void Remove(FrameworkElement c)
        {
            controls.Children.Remove(c);
        }

        public void Add(FrameworkElement c, bool priority = false)
        {
            if (priority)
                controls.Children.Insert(0, c);
            else controls.Children.Add(c);
            Show();
        }

        public List<FrameworkElement> Clear()
        {
            var lst = controls.Children.Cast<FrameworkElement>().ToList();
            controls.Children.Clear();
            return lst;
        }


        private void CheckNotificationsCount()
        {
            if (controls.Children.Count == 0)
            {
                Hide();
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        public new Window Owner
        {
            get { return base.Owner; }
            set
            {
                base.Owner = value;
                if (value.IsLoaded)
                {
                    if (NotificationCount() > 0)
                    {
                        Show();
                    }
                }
                else
                {
                    value.Loaded += Value_Loaded;
                }
                
            }
        }

        private void Value_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Window).Loaded -= Value_Loaded;
            if (NotificationCount() > 0)
            {
                Show();
            }
        }

        void InitialPlacement(Window window)
        {
            try
            {
                ContainerGrid.MaxHeight = window.ActualHeight - 40;
                Left = window.Left + window.ActualWidth - ActualWidth - 30;
                Top = window.Top + 30;
            }
            catch
            {
            }
        }

        private void NotificationsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
                InitialPlacement(Owner);
        }
    }
}
