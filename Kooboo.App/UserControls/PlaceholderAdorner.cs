//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Kooboo.App.UserControls
{
    public class PlaceholderAdorner1 : Adorner
    {
        string _placeholder;

        public PlaceholderAdorner1(UIElement ele, string placeholder)
            : base(ele)
        {
            _placeholder = placeholder;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            TextBox txt = this.AdornedElement as TextBox;
            if (txt == null || !txt.IsVisible || string.IsNullOrEmpty(_placeholder)) return;

            this.IsHitTestVisible = false;

            drawingContext.DrawText(
                new FormattedText
                (
                    _placeholder,
                    CultureInfo.CurrentCulture,
                    txt.FlowDirection,
                    new Typeface(txt.FontFamily, FontStyles.Italic, txt.FontWeight, txt.FontStretch),
                    txt.FontSize,
                    Brushes.Gray
                ),
                new Point(4, 2));
        }
    }

    public class PlaceholderAdorner : Adorner
    {
        private VisualCollection _visCollec;
        private TextBlock _tb;
        private TextBox _txt;

        public PlaceholderAdorner(UIElement ele, string placeholder)
            : base(ele)
        {
            _txt = ele as TextBox;
            if (_txt == null) return;

            Binding bd = new Binding("IsVisible");
            bd.Source = _txt;
            bd.Mode = BindingMode.OneWay;
            bd.Converter = new BoolToVisibilityConverter();
            this.SetBinding(TextBox.VisibilityProperty, bd);
            _visCollec = new VisualCollection(this);
            _tb = new TextBlock();
            _tb.Style = null;
            _tb.FontSize = _txt.FontSize;
            _tb.FontFamily = _txt.FontFamily;
            _tb.FontWeight = _txt.FontWeight;
            _tb.FontStretch = _txt.FontStretch;
            _tb.FontStyle = FontStyles.Italic;
            _tb.Foreground = Brushes.Gray;
            _tb.Text = placeholder;
            _tb.IsHitTestVisible = false;
            _visCollec.Add(_tb);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visCollec.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _tb.Arrange(new Rect(new Point(4, 2), finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visCollec[index];
        }
    }

    public partial class TextBoxHelper
    {
        #region Placeholder2

        public static string GetPlaceholder2(DependencyObject obj)
        {
            return (string)obj.GetValue(Placeholder2Property);
        }

        public static void SetPlaceholder2(DependencyObject obj, string value)
        {
            obj.SetValue(Placeholder2Property, value);
        }

        public static readonly DependencyProperty Placeholder2Property =
            DependencyProperty.RegisterAttached("Placeholder2", typeof(string), typeof(TextBoxHelper),
                new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnPlaceholder2Changed)));

        public static void OnPlaceholder2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox txt = d as TextBox;
            if (txt == null || e.NewValue?.ToString().Trim().Length == 0) return;

            if (DesignerProperties.GetIsInDesignMode(txt))
            {
                txt.Text = e.NewValue?.ToString();
                txt.FontStyle = FontStyles.Italic;
                txt.Foreground = Brushes.Gray;
            }

            RoutedEventHandler loadHandler = null;
            string text = null;
            if (e.NewValue?.ToString() != null)
                text = e.NewValue?.ToString();  // Data.Language.Hardcoded.GetValuexxxx(e.NewValue?.ToString());

            loadHandler = (s1, e1) =>
            {
                txt.Loaded -= loadHandler;

                if (txt.Text.Length == 0)
                {
                    txt.Text = text;
                    txt.FontStyle = FontStyles.Italic;
                    txt.Foreground = Brushes.Gray;
                }
            };

            txt.Loaded += loadHandler;

            txt.GotFocus += (s1, e1) =>
            {
                if (txt.Text == text
                 && txt.FontStyle == FontStyles.Italic
                 && txt.Foreground == Brushes.Gray)
                {
                    txt.Clear();
                    txt.FontStyle = FontStyles.Normal;
                    txt.Foreground = SystemColors.WindowTextBrush;
                }
            };

            txt.LostFocus += (s1, e1) =>
            {
                if (txt.Text.Length == 0)
                {
                    txt.Text = text;
                    txt.FontStyle = FontStyles.Italic;
                    txt.Foreground = Brushes.Gray;
                }
            };
        }
        #endregion
    }

    public partial class TextBoxHelper
    {
        #region MyRegion
        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder",
                typeof(string),
                typeof(TextBoxHelper),
                new UIPropertyMetadata(string.Empty,
                    new PropertyChangedCallback(OnPlaceholderChanged)
                    ));

        public static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox txt = d as TextBox;
            if (txt == null || e.NewValue.ToString().Trim().Length == 0) return;

            RoutedEventHandler loadHandler = null;
            loadHandler = (s1, e1) =>
            {
                txt.Loaded -= loadHandler;

                var lay = AdornerLayer.GetAdornerLayer(txt);
                if (lay == null)
                {
                    return;
                }

                Adorner[] ar = lay.GetAdorners(txt);
                if (ar != null)
                {
                    for (int i = 0; i < ar.Length; i++)
                    {
                        if (ar[i] is PlaceholderAdorner)
                        {
                            lay.Remove(ar[i]);
                        }
                    }
                }

                if (txt.Text.Length == 0)
                {
                    lay.Add(new PlaceholderAdorner(txt, e.NewValue.ToString()));
                }
            };
            txt.Loaded += loadHandler;
            txt.TextChanged += (s1, e1) =>
            {
                bool isShow = txt.Text.Length == 0;

                var lay = AdornerLayer.GetAdornerLayer(txt);
                if (lay == null)
                {
                    return;
                }

                if (isShow)
                {
                    lay.Add(new PlaceholderAdorner(txt, e.NewValue.ToString()));
                }
                else
                {
                    Adorner[] ar = lay.GetAdorners(txt);
                    if (ar != null)
                    {
                        for (int i = 0; i < ar.Length; i++)
                        {
                            if (ar[i] is PlaceholderAdorner)
                            {
                                lay.Remove(ar[i]);
                            }
                        }
                    }
                }
            };
        } 
        #endregion
    }
}
