using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ThemeManagers
{
    public class ActualSizePropertyProxy : FrameworkElement
    {
      
        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        public double ActualWidthValue
        {
            get { return (double) GetValue(ActualWidthValueProperty); }
            set { SetValue(ActualWidthValueProperty, value); }
        }

        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(ActualSizePropertyProxy),
                                        new PropertyMetadata(null, OnElementPropertyChanged));

        public static readonly DependencyProperty ActualWidthValueProperty =
            DependencyProperty.Register("ActualWidthValue", typeof(double), typeof(ActualSizePropertyProxy),
                                        new FrameworkPropertyMetadata(0d));

        private static void OnElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ActualSizePropertyProxy)d).OnElementChanged(e);
        }

        private void OnElementChanged(DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement oldElement = (FrameworkElement)e.OldValue;
            FrameworkElement newElement = (FrameworkElement)e.NewValue;

            newElement.SizeChanged += new SizeChangedEventHandler(Element_SizeChanged);
            ActualWidthValue = Element.ActualWidth;
            if (oldElement != null)
            {
                oldElement.SizeChanged -= new SizeChangedEventHandler(Element_SizeChanged);
            }
        }

        private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ActualWidthValue = Element.ActualWidth;
        }

        
    }
}
