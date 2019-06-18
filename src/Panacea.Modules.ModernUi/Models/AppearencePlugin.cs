using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Panacea.Modules.ModernUi.Models
{

    public class IntervalChangedEventArgs : EventArgs
    {
        public int Interval { get; set; }

    }


    [DataContract]
    public class AppearancePlugin : Translatable
    {
        public event EventHandler<IntervalChangedEventArgs> IntervalChanged;

        public AppearancePlugin()
            : base()
        {
            this.Size = new MenuTileSize() { X = 1, Y = 1 };
            Frames = new ObservableCollection<UIElement>();
            interval = 4000;
        }


        public bool HideText
        {
            get { return !InvisibleTileText; }
        }


        [DataMember(Name = "invisibleTileText")]
        public bool InvisibleTileText { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [IsTranslatable]
        [DataMember(Name = "text")]
        public string Text
        {
            get => GetTranslation();
            set => SetTranslation(value);
        }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "size")]
        public MenuTileSize Size { get; set; }

        [DataMember(Name = "text_fill")]
        public string TextFill { get; set; }

        public Brush TextFillBrush
        {
            get
            {
                if (TextFill == null) return Brushes.White;
                try
                {
                    return ((SolidColorBrush)(new BrushConverter().ConvertFrom(TextFill)));
                }
                catch
                {
                    return Brushes.White;
                }
            }
        }

        [DataMember(Name = "background_color")]
        public TileBackground TileBackground { get; set; }

        [DataMember(Name = "background_image")]
        public string BackgroundImage { get; set; }

        [DataMember(Name = "decoration")]
        public string Decoration { get; set; }

        [DefaultValue(false)]
        [DataMember(Name = "showAlways")]
        public bool ShowAlways { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "liteCompatible")]
        public bool LiteCompatible { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public Brush TitleBackground
        {
            get
            {
                if (BackgroundImage != null) return this._backgroundGradient;
                return Brushes.Transparent;
            }
        }

        private Visibility _visibility;

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public Border GridContent { get; set; }

        private int interval;

        public int Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                IntervalChanged?.Invoke(this, new IntervalChangedEventArgs() { Interval = value });
            }
        }

        public ObservableCollection<UIElement> Frames { get; set; }

        private LinearGradientBrush _backgroundGradient;
        public LinearGradientBrush BackgroundGradient
        {
            get
            {
                if (TileBackground == null) return new LinearGradientBrush(Colors.Transparent, Colors.Transparent, 0);
                if (_backgroundGradient != null) return _backgroundGradient;

                _backgroundGradient = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };

                // Create and add Gradient stops
                var blueGs = new GradientStop();
                blueGs.Color =
                    ((SolidColorBrush)(new BrushConverter().ConvertFrom(TileBackground.From)))
                        .Color;
                blueGs.Offset = 0.0;
                _backgroundGradient.GradientStops.Add(blueGs);
                try
                {
                    var orangeGS = new GradientStop();
                    orangeGS.Color =
                        ((SolidColorBrush)(new BrushConverter().ConvertFrom(TileBackground.To)))
                            .Color;
                    orangeGS.Offset = 1;
                    _backgroundGradient.GradientStops.Add(orangeGS);
                    _backgroundGradient.Freeze();
                    return _backgroundGradient;
                }
                catch
                {
                    return null;
                }
            }
        }

        public Brush BackgroundBrush
        {
            get
            {
                try
                {
                    return ((SolidColorBrush)(new BrushConverter().ConvertFrom(TileBackground.From)));
                }
                catch
                {
                    return Brushes.Transparent;
                }
            }
        }
    }


    [DataContract]
    public class MenuTileSize
    {
        [DefaultValue(1)]
        [DataMember(Name = "x")]
        public int X { get; set; }

        [DefaultValue(1)]
        [DataMember(Name = "y")]
        public int Y { get; set; }

    }

    [DataContract]
    public class TileBackground
    {
        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

    }


    [DataContract]
    public class GetThemesResponse
    {
        [DataMember(Name = "theme")]
        public List<Theme> Themes { get; set; }
    }



    [DataContract]
    public class Theme
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "layout_style")]
        public string LayoutStyle { get; set; }

        [DataMember(Name = "keys")]
        public List<ThemeKey> Keys { get; set; }

        [DataMember(Name = "groups")]
        public List<TileGroup> Groups { get; set; }
    }

    [DataContract]
    public class ThemeKey
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }


    }

    [DataContract]
    public class TileGroup : Translatable
    {
        [IsTranslatable]
        [DataMember(Name = "name")]
        public string Name
        {
            get => GetTranslation();
            set => SetTranslation(value);
        }

        [DataMember(Name = "showText")]
        public bool ShowText { get; set; }

        public Visibility TextVisibility
        {
            get
            {
                return ShowText ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private ObservableCollection<AppearancePlugin> _appearancePlugins;

        [DataMember(Name = "appearance_plugins")]
        public ObservableCollection<AppearancePlugin> AppearancePlugins
        {
            get { return _appearancePlugins; }
            set
            {
                if (value != _appearancePlugins)
                {
                    _appearancePlugins = value;
                    OnPropertyChanged("Visibility");
                    foreach (var ap in _appearancePlugins)
                    {
                        ap.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == "Visibility")
                            {
                                OnPropertyChanged("Visibility");
                            }
                        };


                    }
                    _appearancePlugins.CollectionChanged += (oo, ee) => OnPropertyChanged("Visibility");
                }
            }
        }

        public Visibility Visibility
        {
            get
            {
                return AppearancePlugins.Any(ap =>
                    ap.Visibility == Visibility.Visible || ap.ShowAlways)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
