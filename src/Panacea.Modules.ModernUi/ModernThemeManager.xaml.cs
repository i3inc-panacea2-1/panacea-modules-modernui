using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Panacea.Multilinguality;
using Panacea.Modules.ModernUi.Controls;
using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Core;
using Panacea.Modules.ModernUi.Models;
using Panacea.Mvvm;
using Panacea.Modules.ModernUi.ViewModels;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace Panacea.Modules.ModernUi
{
    /// <summary>
    /// Interaction logic for ModernThemeManager.xaml
    /// </summary>
    public partial class ModernThemeManager : NavigatorBase, IUiManager
    {

        public new event KeyEventHandler PreviewKeyDown;
        public new event KeyEventHandler PreviewKeyUp;

        public FrameworkElement CurrentView
        {
            get { return (FrameworkElement)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register("CurrentView", typeof(FrameworkElement), typeof(ModernThemeManager), new PropertyMetadata(null));


        private GridLength _originalNavigationBarSize;
        const double NavigationBarSize = .9;
        private NotificationsWindow popup;
        private readonly PanaceaServices _core;
        //private readonly AudioManager _audioManager;
        private bool _charmsBarIsOpen = false;

        private Translator _translator;
        private Theme _theme;

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    //_mainPage.Theme = Theme;
                }
            }
        }

        private MainPageViewModel _mainPage;

        public ModernThemeManager(PanaceaServices core, Theme theme)
        {
            _core = core;
            _mainPage = new MainPageViewModel(_core, theme);
            InitializeComponent();

            popup = new NotificationsWindow();
            _translator = new Translator("core");

        }

        private bool IsCharmsOpen()
        {
            return _charmsBarIsOpen;
        }


        public void EnableFullscreen()
        {
            if (leftBar.Height.Value > 0)
                _originalNavigationBarSize = leftBar.Height;
            leftBar.Height = new GridLength(0, GridUnitType.Pixel);
        }

        public void DisableFullscreen()
        {
            leftBar.Height = _originalNavigationBarSize;

        }

        #region notification methods

        public void Notify(string message, Action del)
        {
            var c = popup.Add(message, del);
            if (IsNavigationDisabled) return;
            ShowNotifications();
        }

        public void Notify(ViewModelBase c)
        {
            if (IsNavigationDisabled) return;
            popup.Add(c.View, true);
            ShowNotifications();

        }

        public void Refrain(ViewModelBase c)
        {
            RemoveNotification(c.View);
        }

        private void AddNotification(Control c)
        {
            popup.Add(c);
            ShowNotifications();
        }

        private void RemoveNotification(FrameworkElement c)
        {
            popup.Remove(c);
            if (popup.NotificationCount() == 0) popup.Hide();

        }

        public void ShowNotifications()
        {
            if (popup.controls.Children.Count == 0) return;
            popup.Show();
        }

        public void HideNotifications()
        {
            popup.Hide();
        }

        public void ClearNotifications()
        {
            try
            {
                var children = popup.controls.Children.Cast<Control>().ToList();
                foreach (var pop in children)
                {
                    RemoveNotification(pop);
                }
            }
            catch
            {
            }
        }

        #endregion

        public override void GoHome()
        {
            if (IsNavigationDisabled) return;
            base.GoHome();
            ShowOrHideBackButton();
        }

        public override void GoBack(int count = 1)
        {
            if (IsNavigationDisabled) return;
            base.GoBack(count);
            ShowOrHideBackButton();
        }

        public override void Navigate(ViewModelBase page, bool cache = true)
        {
            if (IsNavigationDisabled) return;
            if (page == null)
            {
                base.Navigate(null, false);
                return;
            }
            var view = page.View;
            CurrentView = view;
            
            base.Navigate(page, cache);
            ShowOrHideBackButton();
        }

        #region navigation methods

        public void SetNavigationBarControl(FrameworkElement c)
        {

        }

        private void ShowOrHideBackButton()
        {
            if (CurrentPage != _mainPage)
            {
                backButton.IsEnabled = homeButton.IsEnabled = true;
            }
            else
            {
                backButton.IsEnabled = homeButton.IsEnabled = false;
            }
        }

        #endregion navigation methods

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private static string[] _fontSizes = new string[13]
        {
            "FontSize-Xxx-Huge",
            "FontSize-Xx-Huge",
            "FontSize-X-Huge",
            "FontSize-Huge",
            "FontSize-Xxx-Large",
            "FontSize-Xx-Large",
            "FontSize-X-Large",
            "FontSize-Large",
            "FontSize-Normal",
            "FontSize-Small",
            "FontSize-X-Small",
            "FontSize-Xx-Small",
            "FontSize-Xxx-Small"
        };

        float _ratio = 1f;

        void ResizeFonts()
        {
            var scr = System.Windows.Forms.Screen.PrimaryScreen;
            var ratio = ((double)scr.Bounds.Height* (double)scr.Bounds.Width) / (2000 * 1300.0) * _ratio;
            foreach (var name in _fontSizes)
            {
                Application.Current.Resources[name] = (double)Application.Current.Resources["Original" + name] * ratio;
            }
        }

        FontSizeControlViewModel _fontSizeControl;
        bool _loaded = false;
        private void ThemeManager_OnLoaded(object sender, RoutedEventArgs e)
        {

            if (_loaded) return;
            _loaded = true;
            ResizeFonts();
            var window = Window.GetWindow(this);
            window.PreviewKeyDown += Window_PreviewKeyDown;
            window.PreviewKeyUp += Window_PreviewKeyUp;
            popup.Owner = window;
            _doingWork = new UiBlockWindow(window);
            _doingWork.IsVisibleChanged += _doingWork_IsVisibleChanged;
            if (!_history.Any())
            {
                Navigate(_mainPage, true);
            }
            else
            {
                _history.RemoveAt(0);
                _history.Insert(0, _mainPage);
            }
            _fontSizeControl = new FontSizeControlViewModel();
            _fontSizeControl.PropertyChanged += _fontSizeControl_PropertyChanged;
            AddNavigationBarControl(_fontSizeControl);
        }

        private void _doingWork_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == false)
            {
                var w = Window.GetWindow(this);
                if(w!= null)
                {
                    w.Activate();
                }
            }
        }

        private void _fontSizeControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FontSizeControlViewModel.Ratio))
            {
                _ratio = _fontSizeControl.Ratio/ 100f;
                ResizeFonts();
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            PreviewKeyUp?.Invoke(this, e);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PreviewKeyDown?.Invoke(this, e);
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _ratio = _fontSizeControl.Ratio / 100f;
            ResizeFonts();
        }

        private bool _keyboardOpen = false;
        public void ShowKeyboard(KeyboardTypes keyboardType = KeyboardTypes.Normal)
        {

        }

        public void HideKeyboard()
        {

        }

        private void ThemeManager_OnUnloaded(object sender, RoutedEventArgs e)
        {
           
            //todo ServerCommunicator.TaskStarted -= ServerCommunicatorOnTaskStarted;
            //todo ServerCommunicator.TaskCompleted -= ServerCommunicatorOnTaskCompleted; 
            Navigate(null);
        }

        private Dictionary<ViewModelBase, ModalPopup> _popedElements = new Dictionary<ViewModelBase, ModalPopup>();


        public static BitmapSource CaptureScreen(FrameworkElement target)
        {
            if (target == null)
            {
                return null;
            }
            var bounds = target.RenderTransform.TransformBounds(new Rect(target.RenderSize));
            if (bounds.Width == 0.0 || bounds.Height == 0.0)
            {
                bounds = VisualTreeHelper.GetDescendantBounds(target);
            }

            var rtb = new RenderTargetBitmap((int)(bounds.Width * 96.0 / 96.0),
                                                            (int)(bounds.Height * 96.0 / 96.0),
                                                            96.0,
                                                            96.0,
                                                            PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target) { Stretch = Stretch.None };
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            rtb.Render(dv);
            rtb.Freeze();
            return rtb;
        }

        public Task<TResult> ShowPopup<TResult>(
            PopupViewModelBase<TResult> element,
            string title = null,
            PopupType popupType = PopupType.None,
            bool closable = true,
            bool trasnparent = true)
        {
            return Application.Current.Dispatcher.Invoke(async () =>
            {
                if (element.View.Parent != null)
                {
                    ((Border)element.View.Parent).Child = null; //.Clear();
                }
                var modal = trasnparent ? new ModalPopup(Window.GetWindow(this)) : new ModalPopup(Window.GetWindow(this), CaptureScreen(this));
                modal.Closed += (oo, ee) => element.Close();
                modal.PopupContent = element.View;
                element.Closable = closable;
                modal.DataContext = element;
                modal.PopupType = popupType;
                modal.Title = title ?? "";

                if (!_popedElements.ContainsKey(element))
                    _popedElements.Add(element, modal);
                modal.Show();
                var res = await element.GetTask();
                HidePopup(element);
                return res;
            });
            //return modal;

        }


        public void HidePopup(ViewModelBase element)
        {
            if (_popedElements.ContainsKey(element))
            {
                _popedElements[element].Close();
                _popedElements.Remove(element);
            }
        }

        public void HideAllPopups()
        {
            var keys = _popedElements.Keys.ToList();
            foreach (var key in keys)
            {
                //HidePopup((IPopup)popedElements[key]);
            }
        }

        public double TextSizePercentage { get; set; }

        public bool IsPaused { get; private set; }

        private void charmsbar_ShowNotificationsClick(object sender, EventArgs e)
        {
            ShowNotifications();
        }



        public object AddToolButton(string text, string namesp, string iconUrl, Action action)
        {
            var but = new ImageButton();

            but.Image = iconUrl;
            but.Click += (oo, ee) => action();
            but.TextVisibility = Visibility.Visible;
            but.Tag = this;
            var trans = new Translator(namesp);
            trans.CreateBinding(ImageButton.TextProperty, but, text);
            Buttons.Children.Insert(_customButtonIndex, but);
            //_customButtonIndex++;
            return but;
        }

        private int _customButtonIndex = 4;

        public event EventHandler Paused;
        public event EventHandler Resumed;
        ToastWindow _toast;
        public void Toast(string text, int timeout = 5000)
        {
            Dispatcher.Invoke(() =>
            {
                if (_toast == null)
                {
                    _toast = new ToastWindow();
                    _toast.Owner = Window.GetWindow(this);
                }
                _toast.Add(text, timeout);
            });
        }

        public void RequestMagicPin()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadDataAsync(string url)
        {
            throw new NotImplementedException();
        }

        private UiBlockWindow _doingWork;

        private int _runningTasks;
        public async Task DoWhileBusy(Func<Task> action)
        {
            _runningTasks++;
            SetTaskLockVisibility();
            try
            {
                await action();
            }
            finally
            {
                _runningTasks--;
                SetTaskLockVisibility();
            }
        }

        public async Task<TResult> DoWhileBusy<TResult>(Func<Task<TResult>> action)
        {
            _runningTasks++;
            SetTaskLockVisibility();
            try
            {
                return await action();
            }
            finally
            {
                _runningTasks--;
                SetTaskLockVisibility();
            }
        }

        private void SetTaskLockVisibility()
        {
            if (_doingWork == null) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (this)
                {
                    if (_runningTasks == 0)
                    {
                        _doingWork.Hide();
                    }
                    else
                    {
                        _doingWork.Show();

                    }
                }
            });
        }

        public void Restart(string message, Exception exception = null)
        {
            _core.Logger.Info(this, "Restart signal received: " + message);
            Application.Current.Shutdown();
        }

        float _pauseVolume;
        public void Pause()
        {
            if (IsPaused) return;
            IsPaused = true;
            Paused?.Invoke(this, EventArgs.Empty);
            popup.WindowState = WindowState.Minimized;

        }

        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            Resumed?.Invoke(this, EventArgs.Empty);
            popup.WindowState = WindowState.Maximized;
        }

        public void RemoveToolButton(object button)
        {

        }

        public void RemoveNavigationBarControl(ViewModelBase c)
        {
            Buttons.Children.Remove(c.View);
        }

        public void AddMainPageControl(ViewModelBase c)
        {
            _mainPage.TopBarControls.Add(c);
        }

        public void RemoveMainPageControl(ViewModelBase c)
        {
            _mainPage.TopBarControls.Remove(c);
        }

        public void AddNavigationBarControl(ViewModelBase c)
        {
            var list = new List<FrameworkElement>();
            foreach (FrameworkElement control in Buttons.Children)
            {
                list.Add(control);
            }
            Buttons.Children.Clear();
            list.Add(c.View);
            list = list.OrderByDescending(e => { return e.GetValue(Panel.ZIndexProperty); }).ToList();
            foreach (var item in list)
            {
                Buttons.Children.Add(item);
            }
        }

        public void AddSettingsControl(SettingsControlViewModelBase c)
        {
            c.Close += C_Close;
            _settingsControls.Add(c.View);
        }

        private void C_Close(object sender, EventArgs e)
        {
            HidePopup(_ch);
        }

        public void RemoveSettingsControl(SettingsControlViewModelBase c)
        {
            c.Close -= C_Close;
            _settingsControls.Remove(c.View);
        }

        List<UIElement> _settingsControls = new List<UIElement>();
        CharmsBarViewModel _ch;
        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _ch = new CharmsBarViewModel(_settingsControls);
            await ShowPopup(_ch);
        }

        
    }
}
