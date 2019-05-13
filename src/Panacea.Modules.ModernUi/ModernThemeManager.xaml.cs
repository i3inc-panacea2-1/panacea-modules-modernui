using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Panacea.Multilinguality;
using Panacea.Modules.ModernUi.Controls;
using ThemeManagers.Controls;
using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Core;
using Panacea.Modules.ModernUi.Models;

namespace Panacea.Modules.ModernUi
{
    /// <summary>
    /// Interaction logic for ModernThemeManager.xaml
    /// </summary>
    public partial class ModernThemeManager : NavigatorBase, IUiManager
    {

        private GridLength _originalNavigationBarSize;
        const double NavigationBarSize = .9;
        ToastWindow _toast;
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

        private MainPage _mainPage;
        IPopup _charmbar;

        public ModernThemeManager(PanaceaServices core)
        {
            InitializeComponent();
            _core = core;
            popup = new NotificationsWindow();
            _translator = new Translator("core");
        }

        private bool IsCharmsOpen()
        {
            return _charmsBarIsOpen;
        }

        public void ShowCharmsBar(bool animate = true)
        {
            var ch = new CharmsBar(_core) { };
            ch.Close += (oo, ee) =>
            {
                HidePopup(ch);
            };
            _charmbar = ShowPopup(ch);

        }

        public void HideCharmsBar(bool animate = true)
        {
                if(_charmbar != null)
            HidePopup(_charmbar);
        }
        public void EnableFullscreen()
        {
            if(leftBar.Height.Value>0)
                _originalNavigationBarSize = leftBar.Height;
            leftBar.Height = new GridLength(0, GridUnitType.Pixel);
        }

        public void DisableFullscreen()
        {
            leftBar.Height = _originalNavigationBarSize;

        }

        #region notification methods

        public FrameworkElement Notify(string message, Action del)
        {
            var c = popup.Add(message, del);
            if (IsNavigationDisabled) return c;
            ShowNotifications();

            return c;
        }

        public void Notify(FrameworkElement c)
        {
            if (IsNavigationDisabled) return;
            popup.Add(c, true);
            ShowNotifications();

        }

        public void Refrain(FrameworkElement c)
        {
            RemoveNotification(c);
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
            base.GoHome();
            ShowOrHideBackButton();
            ShowOrHideKeyboardButton();
        }

        public override void GoBack(int count = 1)
        {
            base.GoBack(count);
            ShowOrHideBackButton();
            ShowOrHideKeyboardButton();
        }

        public override void Navigate(FrameworkElement page, bool cache = true)
        {
            base.Navigate(page, cache);
            ShowOrHideBackButton();
            ShowOrHideKeyboardButton();
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

        private void ShowOrHideKeyboardButton()
        {
            //imgKeyboard.Visibility = (CurrentPage is IDoesNotAcceptKeyboard) ? Visibility.Hidden : Visibility.Visible;
        }

        #endregion navigation methods

        private void charmsbar_Close(object sender, EventArgs e)
        {
            HideCharmsBar(false);
        }


        private void UserFunctionsHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCharmsOpen())
            {
                ShowCharmsBar();
            }
            else
            {
                HideCharmsBar();
            }
            //ShowCharmsBar();
        }

        private void OnToggleKeyboardClicked(object sender, RoutedEventArgs e)
        {
            /*
            if(WindowStatus == PanaceaWindowStatus.Normal)
            {
                if (Resources["KeyboardBarHeight"].ToString() == "0")
                    ShowKeyboard();
                else HideKeyboard();
            }else
            {
                HostWindow.ShowOnScreenKeyboard();
            }
            */
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void ThemeManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            leftBar.Height =
                            new GridLength(NavigationBarSize, GridUnitType.Star);
            _mainPage = new MainPage();

            var window = Window.GetWindow(this);
            popup.Owner = window;

           
            if (Application.Current.Resources.Contains("NavigationBarSize"))
            {
                //_originalNavigationBarSize = leftBar.Width = new GridLength(NavigationBarSize, GridUnitType.Star);
            }

            HideCharmsBar(false);
            if (!_history.Any())
            {
                Navigate(_mainPage, true);
            }
            else
            {
                _history.RemoveAt(0);
                _history.Insert(0, _mainPage);
            }
        }

        private bool _keyboardOpen = false;
        public void ShowKeyboard(KeyboardTypes keyboardType = KeyboardTypes.Normal)
        {
            _keyboardOpen = true;
            if (keyboardType == KeyboardTypes.Normal)
            {
                normalKeyboardGrid.Visibility = Visibility.Visible;
                numericKeyboardGrid.Visibility = Visibility.Collapsed;
                dateKeyboardGrid.Visibility = Visibility.Collapsed;
            }
            else if (KeyboardTypes.Numeric == keyboardType || KeyboardTypes.Pin == keyboardType)
            {
                normalKeyboardGrid.Visibility = Visibility.Collapsed;
                numericKeyboardGrid.Visibility = Visibility.Visible;
                dateKeyboardGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                normalKeyboardGrid.Visibility = Visibility.Collapsed;
                numericKeyboardGrid.Visibility = Visibility.Collapsed;
                dateKeyboardGrid.Visibility = Visibility.Visible;

            }
            double percentage = .38;
            if (keyboardRow.Height.Value != 0.0) return;
            normalKeyboardGrid.Height = ActualHeight*percentage;
            //if (!(CurrentPage is IDoesNotAcceptKeyboard))
            {
                keyboardRow.Height = new GridLength(ActualHeight*percentage, GridUnitType.Pixel);
               
                Resources["KeyboardBarHeight"] = new GridLength(ActualHeight*percentage,
                    GridUnitType.Pixel);
            }
        }

        public void HideKeyboard()
        {
            _keyboardOpen = false;
            if (keyboardRow.Height.Value == 0) return;
            Task.Run(async () =>
            {
                await Task.Delay(150);
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_keyboardOpen) return;
                    keyboardRow.Height = new GridLength(0);
                    if (Application.Current != null)
                        Resources["KeyboardBarHeight"] = new GridLength(0);
                    normalKeyboardGrid.Visibility = Visibility.Collapsed;
                    numericKeyboardGrid.Visibility = Visibility.Collapsed;
                    dateKeyboardGrid.Visibility = Visibility.Collapsed;
                }));
            });
        }

        private void ThemeManager_OnUnloaded(object sender, RoutedEventArgs e)
        {
            //todo ServerCommunicator.TaskStarted -= ServerCommunicatorOnTaskStarted;
            //todo ServerCommunicator.TaskCompleted -= ServerCommunicatorOnTaskCompleted; 
            Navigate(null);
        }

        private Dictionary<object, object> popedElements = new Dictionary<object, object>();

        public IPopup ShowPopup(
            FrameworkElement element,
            string title = null,
            PopupType popupType = PopupType.None,
            bool closable = true,
            bool trasnparent = true)
        {

            if (element.Parent != null)
            {
                ((Border) element.Parent).Child = null; //.Clear();
            }
            /*
            var modal = trasnparent ? new ModalPopup(HostWindow as Window) : new ModalPopup(HostWindow as Window, Utils.CaptureScreen(this));
            modal.Closed += Popup_ClickedOutside;
 
            modal.PopupContent = element;
            modal.PopupType = popupType;
            modal.Title = title ?? "";
           
            modal.Closable = closable;
            
            if (!popedElements.ContainsKey(element))
                popedElements.Add(element, modal);
            modal.Show();
            return modal;
            */
                return null;
        }

      
        public void HidePopup(IPopup element)
        {
            int i = popedElements.Count - 1;
            while (i >= 0)
            {
                if (popedElements.ElementAt(i).Value == element) popedElements.Remove(popedElements.ElementAt(i).Key);
                i--;
            }
            //element.Close();
            element.Close(); //element.PopupContent = null;

           
        }

        public void HidePopup(object element)
        {
            if (popedElements.ContainsKey(element))
            {
                HidePopup((IPopup)popedElements[element]);
            }
        }

        public void HideAllPopups()
        {
            var keys = popedElements.Keys.ToList();
            foreach (var key in keys)
            {
                HidePopup((IPopup)popedElements[key]);
            }
        }

        public double TextSizePercentage { get ; set ; }

        public bool IsPaused { get; private set; }

        private void charmsbar_ShowNotificationsClick(object sender, EventArgs e)
        {
            HideCharmsBar(false);
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

        public void Toast(string text, int timeout = 5000)
        {
            _toast = new ToastWindow(timeout);
            _toast.Text = text;
            _toast.Show();
        }
        
        public void RequestMagicPin()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadDataAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task DoWhileBusy(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> DoWhileBusy<TResult>(Func<Task<TResult>> action)
        {
            throw new NotImplementedException();
        }

        public void Restart(string message, Exception exception = null)
        {
            throw new NotImplementedException();
        }

        float _pauseVolume;
        public void Pause()
        {
            if (IsPaused) return;

            //if (_audioManager != null)
            //{
            //    _pauseVolume = _audioManager.SpeakersVolume;
            //    _audioManager.SpeakersVolume = 0;
            //}
            IsPaused = true;
            Paused?.Invoke(this, EventArgs.Empty);
            popup.WindowState = WindowState.Minimized;
            
        }

        public void Resume()
        {
            if (!IsPaused) return;
            //if (_audioManager != null)
            //{
            //    _audioManager.SpeakersVolume = _pauseVolume;
            //}
            IsPaused = false;
            Resumed?.Invoke(this, EventArgs.Empty);
            popup.WindowState = WindowState.Normal;
        }

        public void RemoveToolButton(object button)
        {
            
        }

        public void RemoveNavigationBarControl(FrameworkElement c)
        {
            Buttons.Children.Remove(c);
        }

        public void AddMainPageControl(FrameworkElement c)
        {
            throw new NotImplementedException();
        }

        public void RemoveMainPageControl(FrameworkElement c)
        {
            throw new NotImplementedException();
        }

        public void AddNavigationBarControl(FrameworkElement c)
        {
            var list = new List<FrameworkElement>();
            foreach (FrameworkElement control in Buttons.Children)
            {
                list.Add(control);
            }
            Buttons.Children.Clear();
            list.Add(c);
            list = list.OrderBy(e => { return e.GetValue(DockPanel.ZIndexProperty); }).ToList();
            foreach (var item in list)
            {
                Buttons.Children.Add(item);
            }
        }

        public void AddSettingsControl(FrameworkElement c)
        {
            throw new NotImplementedException();
        }

        public void RemoveSettingsControl(FrameworkElement c)
        {
            throw new NotImplementedException();
        }
    }
}
