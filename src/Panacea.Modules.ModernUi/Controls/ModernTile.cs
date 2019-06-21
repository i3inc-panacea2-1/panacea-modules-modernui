using Panacea.Modularity.UiManager;
using Panacea.Modules.ModernUi.Models;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Panacea.Modules.ModernUi.Controls
{
    public class ModernTile : Button
    {
        public static readonly DependencyProperty PluginProperty =
            DependencyProperty.Register("Plugin",
                typeof(AppearancePlugin),
                typeof(ModernTile),
                new FrameworkPropertyMetadata(null, OnPluginChanged));

        public AppearancePlugin Plugin
        {
            get { return (AppearancePlugin)GetValue(PluginProperty); }
            set { SetValue(PluginProperty, value); }
        }

        private string PathCombine(params string[] path)
        {
            var arr = new string[path.Length + 1];
            arr[0] = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            for (var i = 0; i < path.Length; i++)
            {
                arr[i + 1] = path[i];
            }
            return Path.Combine(arr);
        }

        private static void OnPluginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            var plugin = (AppearancePlugin)e.NewValue;
            if (plugin == null) return;
            var tile = (ModernTile)d;

            if (plugin.BackgroundImage != null)
            {
                if (!plugin.BackgroundImage.StartsWith("/"))
                {
                    var filename = tile.PathCombine(@"resources\mainpage\images\", Path.GetFileName(plugin.BackgroundImage));
                    Uri uriResult;
                    if (!Uri.TryCreate(plugin.Icon, UriKind.Absolute, out uriResult) && File.Exists(filename))
                    {
                        tile.BackgroundImage = filename;
                    }
                }
                if (tile.BackgroundImage == null)
                {
                    tile.BackgroundImage = plugin.BackgroundImage;
                }

            }
            if (plugin.Icon != null)
            {
                if (!plugin.Icon.StartsWith("/"))
                {
                    var filename = tile.PathCombine(@"resources\mainpage\images\", Path.GetFileName(plugin.Icon));
                    Uri uriResult;
                    if (!Uri.TryCreate(plugin.Icon, UriKind.Absolute, out uriResult) && File.Exists(filename))
                    {
                        tile.IconImage = filename;
                    }
                }
                if (tile.IconImage == null)
                {
                    tile.IconImage = plugin.Icon;
                }
            }
            switch (plugin.Decoration)
            {
                case "free":
                    tile.Decoration = "pack://application:,,,/Panacea.Modules.ModernUi;component/resources/images/free.png";
                    break;
                case "paid":
                    tile.Decoration = "pack://application:,,,/Panacea.Modules.ModernUi;component/resources/images/dollar103.png";
                    break;
                case "account":
                    tile.Decoration = "pack://application:,,,/Panacea.Modules.ModernUi;component/resources/images/login_required.png";
                    break;
            }
        }






        public static readonly DependencyProperty FramesProperty =
            DependencyProperty.Register("Frames",
                typeof(ObservableCollection<LiveTileFrame>),
                typeof(ModernTile),
                new FrameworkPropertyMetadata(null, OnFramesChanged));

        private static void OnFramesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ModernTile;
            if (d == null) return;
            var old = e.OldValue as ObservableCollection<LiveTileFrame>;

            var n = e.NewValue as ObservableCollection<LiveTileFrame>;
            if (n != null)
            {
                n.CollectionChanged += self.N_CollectionChanged;
                self.UpdateFrames();
            }
        }

        void UpdateFrames()
        {
            if (Plugin != null)
            {
                if (Plugin.Frames.Count > 0)
                {
                    foreach (var el in Plugin.Frames) AddFrame(el.Element.View);
                }
                frame = 0;
                _dTimer.Interval = TimeSpan.FromMilliseconds(Plugin.Interval);

                Plugin.IntervalChanged += Plugin_IntervalChanged;

            }
            if (InternalFrames.Count > 1)
            {
                _dTimer.Start();
            }
        }

        private void N_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AddFrame(((LiveTileFrame)e.NewItems[0]).Element.View);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveFrame(((LiveTileFrame)e.OldItems[0]).Element.View);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                InternalFrames.RemoveRange(1, InternalFrames.Count - 1);
            }
        }

        public static readonly DependencyProperty BackgroundImageProperty =
           DependencyProperty.Register("BackgroundImage",
               typeof(string),
               typeof(ModernTile),
               new FrameworkPropertyMetadata(null));

        public string BackgroundImage
        {
            get { return (string)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }


        public static readonly DependencyProperty CurrentFrameProperty =
          DependencyProperty.Register("CurrentFrame",
              typeof(UIElement),
              typeof(ModernTile),
              new FrameworkPropertyMetadata(null));

        public UIElement CurrentFrame
        {
            get { return (UIElement)GetValue(CurrentFrameProperty); }
            set { SetValue(CurrentFrameProperty, value); }
        }

        public static readonly DependencyProperty IconImageProperty =
           DependencyProperty.Register("IconImage",
               typeof(string),
               typeof(ModernTile),
               new FrameworkPropertyMetadata(null));

        public string IconImage
        {
            get { return (string)GetValue(IconImageProperty); }
            set { SetValue(IconImageProperty, value); }
        }

        public static readonly DependencyProperty DecorationProperty =
           DependencyProperty.Register("Decoration",
               typeof(string),
               typeof(ModernTile),
               new FrameworkPropertyMetadata(null));

        public string Decoration
        {
            get { return (string)GetValue(DecorationProperty); }
            set { SetValue(DecorationProperty, value); }
        }



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ModernTile), new PropertyMetadata(false));



        static ModernTile()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModernTile), new FrameworkPropertyMetadata(typeof(ModernTile)));
        }
        public ObservableCollection<LiveTileFrame> Frames
        {
            get => (ObservableCollection<LiveTileFrame>)GetValue(FramesProperty);
            set
            {
                SetValue(FramesProperty, value);
            }
        }

        readonly DispatcherTimer _dTimer = new DispatcherTimer();
        private List<UIElement> InternalFrames { get; set; }
        int frame = 0;
        public ModernTile()
        {
            InternalFrames = new List<UIElement> { new ModernTileDefaultContent() };

            CurrentFrame = InternalFrames[0];
            Interval = 4000;

            _dTimer.Tick += (oo, ee) =>
            {
                frame++;
                if (frame > InternalFrames.Count - 1) frame = 0;
                try
                {
                    Dispatcher.Invoke(() => { CurrentFrame = InternalFrames[frame]; });
                }
                catch
                {
                }
            };
            Loaded += (oo, ee) =>
            {
                if (InternalFrames.Count > 1)
                {
                    _dTimer.Start();
                }
            };
            Unloaded += (sender, args) =>
            {
                _dTimer.Stop();
                
            };
            _storyboard = new Storyboard();

            var scale = new ScaleTransform(1.0, 1.0);
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.RenderTransform = scale;

            var growAnimation = new DoubleAnimation();
            growAnimation.Duration = TimeSpan.FromMilliseconds(80);
            growAnimation.From = 1;
            growAnimation.To = .95;
            growAnimation.AutoReverse = true;
            Storyboard.SetTargetProperty(growAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimation, this);
            _storyboard.Children.Add(growAnimation);

            growAnimation = new DoubleAnimation();
            growAnimation.Duration = TimeSpan.FromMilliseconds(80);
            growAnimation.From = 1;
            growAnimation.To = .95;
            growAnimation.AutoReverse = true;
            Storyboard.SetTargetProperty(growAnimation, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growAnimation, this);
            _storyboard.Children.Add(growAnimation);


            _storyboard.Completed += (oo, ee) => { base.OnClick(); };
        }

        private readonly Storyboard _storyboard;

        void Plugin_IntervalChanged(object sender, IntervalChangedEventArgs ee)
        {
            _dTimer.Interval = TimeSpan.FromMilliseconds(ee.Interval);
        }


        protected override void OnClick()
        {

            _storyboard.Begin();
        }
        public int Interval
        {
            get { return (int)_dTimer.Interval.TotalMilliseconds; }
            set { _dTimer.Interval = TimeSpan.FromMilliseconds(value); }
        }

        public void AddFrame(UIElement element)
        {
            if (InternalFrames.Contains(element)) return;

            InternalFrames.Add(element);
            CurrentFrame = InternalFrames[0];

            _dTimer.Start();
        }

        public void RemoveFrame(UIElement element)
        {
            InternalFrames.Remove(element);
            try
            {
                if (InternalFrames.Count <= 1)
                {

                    CurrentFrame = InternalFrames[0];

                }
                _dTimer.Stop();
            }
            catch
            {
            }
        }
    }

}
