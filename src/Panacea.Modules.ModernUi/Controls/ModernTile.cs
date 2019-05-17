using Panacea.Modules.ModernUi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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
                    //var filename = Utils.Path() + @"resources\images\icons\" + System.IO.Path.GetFileName(plugin.Icon);
                    //Uri uriResult;
                    //if (!Uri.TryCreate(plugin.Icon, UriKind.Absolute, out uriResult) && File.Exists(filename))
                    //{
                    //    tile.BackgroundImage = filename;
                    //}
                }
                if (tile.IconImage == null)
                {
                    tile.BackgroundImage = plugin.BackgroundImage;
                }

            }
            if (plugin.Icon != null)
            {
                if (!plugin.Icon.StartsWith("/"))
                {
                    //var filename = Utils.Path() + @"resources\images\icons\" + System.IO.Path.GetFileName(plugin.Icon);
                    //Uri uriResult;
                    //if (!Uri.TryCreate(plugin.Icon, UriKind.Absolute, out uriResult) && File.Exists(filename))
                    //{
                    //    tile.IconImage = filename;
                    //}
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
                    tile.Decoration = "pack://application:,,,/PanaceaLib;component/resources/images/dollar103.png";
                    break;
                case "account":
                    tile.Decoration = "pack://application:,,,/PanaceaLib;component/resources/images/login_required.png";
                    break;
            }
        }






        public static readonly DependencyProperty FramesProperty =
            DependencyProperty.Register("Frames",
                typeof(List<UIElement>),
                typeof(ModernTile),
                new FrameworkPropertyMetadata(null));


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
        public List<UIElement> Frames { get; set; }

        readonly DispatcherTimer _dTimer = new DispatcherTimer();

        int frame = 0;
        public ModernTile()
        {
            Frames = new List<UIElement> { new ModernTileDefaultContent() };

            CurrentFrame = Frames[0];
            Interval = 4000;

            _dTimer.Tick += (oo, ee) =>
            {
                frame++;
                if (frame > Frames.Count - 1) frame = 0;
                try
                {
                    Dispatcher.Invoke(() => { CurrentFrame = Frames[frame]; });
                }
                catch
                {
                }
            };
            Loaded += (sernder, args) =>
            {
                if (Plugin != null)
                {
                    Plugin.Frames.CollectionChanged += Frames_CollectionChanged;
                    if (Plugin.Frames.Count > 0)
                    {
                        foreach (var el in Plugin.Frames) AddFrame(el);
                    }
                    frame = 0;
                    _dTimer.Interval = TimeSpan.FromMilliseconds(Plugin.Interval);

                    Plugin.IntervalChanged += Plugin_IntervalChanged;

                }
                if (Frames.Count > 1)
                {
                    _dTimer.Start();
                }
            };
            Unloaded += (sender, args) =>
            {
                _dTimer.Stop();
                if (Plugin != null)
                {
                    if (Plugin.Frames.Count > 0)
                    {
                        foreach (var el in Plugin.Frames) RemoveFrame(el);
                    }
                    Plugin.Frames.CollectionChanged -= Frames_CollectionChanged;
                    Plugin.IntervalChanged -= Plugin_IntervalChanged;
                }
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
        protected void Frames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs ee)
        {
            if (ee.Action == NotifyCollectionChangedAction.Add)
            {
                AddFrame((UIElement)ee.NewItems[0]);
            }
            else if (ee.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveFrame((UIElement)ee.OldItems[0]);
            }
            else if (ee.Action == NotifyCollectionChangedAction.Reset)
            {
                Frames.RemoveRange(1, Frames.Count - 1);
            }
        }
#if DEBUG
        ~ModernTile()
        {
            System.Diagnostics.Debug.WriteLine("~ModernTile v2");
        }
#endif

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
            if (Frames.Contains(element)) return;

            Frames.Add(element);
            CurrentFrame = Frames[0];

            _dTimer.Start();
        }

        public void RemoveFrame(UIElement element)
        {
            Frames.Remove(element);
            try
            {
                if (Frames.Count <= 1)
                {

                    CurrentFrame = Frames[0];

                }
                _dTimer.Stop();
            }
            catch
            {
            }
        }
    }

}
