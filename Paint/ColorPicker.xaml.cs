using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Paint
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab_rgb.IsSelected)
            {
                try
                {
                    double h = double.Parse(tbHValue.Text);
                    double s = double.Parse(tbSValue.Text) / 100;
                    double v = double.Parse(tbVValue.Text) / 100;

                    (byte r, byte g, byte b) = ConvertHsvToRgb(h, s, v);

                    tbRValue.Text = r.ToString();
                    tbGValue.Text = g.ToString();
                    tbBValue.Text = b.ToString();
                }
                catch
                {
                    tbRValue.Text = "0";
                    tbGValue.Text = "0";
                    tbBValue.Text = "0";
                }
            }

            else if (tab_hsv.IsSelected)
            {
                try
                {
                    byte r = byte.Parse(tbRValue.Text);
                    byte g = byte.Parse(tbGValue.Text);
                    byte b = byte.Parse(tbBValue.Text);

                    (double h, double s, double v) = ConvertRgbToHsv(r, g, b);

                    tbHValue.Text = h.ToString();
                    tbSValue.Text = s.ToString();
                    tbVValue.Text = v.ToString();
                }
                catch
                {
                    tbHValue.Text = "0";
                    tbSValue.Text = "0";
                    tbVValue.Text = "0";
                }
            }
        }

        private void OnRGBColorValueChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    byte r = byte.Parse(tbRValue.Text);
                    byte g = byte.Parse(tbGValue.Text);
                    byte b = byte.Parse(tbBValue.Text);

                    rectColorView.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnHSVColorValueChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    double h = double.Parse(tbHValue.Text);
                    double s = double.Parse(tbSValue.Text) / 100;
                    double v = double.Parse(tbVValue.Text) / 100;

                    (byte r, byte g, byte b) = ConvertHsvToRgb(h, s, v);

                    rectColorView.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUpdateColor_Click(object sender, RoutedEventArgs e)
        {
            if (tab_rgb.IsSelected)
            {
                try
                {
                    byte r = byte.Parse(tbRValue.Text);
                    byte g = byte.Parse(tbGValue.Text);
                    byte b = byte.Parse(tbBValue.Text);

                    rectColorView.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else if (tab_hsv.IsSelected)
            {
                try
                {
                    double h = double.Parse(tbHValue.Text);
                    double s = double.Parse(tbSValue.Text) / 100;
                    double v = double.Parse(tbVValue.Text) / 100;

                    (byte r, byte g, byte b) = ConvertHsvToRgb(h, s, v);

                    rectColorView.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectAllText(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void btnPickColor_Click(object sender, RoutedEventArgs e)
        {
            if (tab_rgb.IsSelected)
            {
                try
                {
                    byte r = byte.Parse(tbRValue.Text);
                    byte g = byte.Parse(tbGValue.Text);
                    byte b = byte.Parse(tbBValue.Text);

                    var window = ((MainWindow)Application.Current.MainWindow);
                    window.colorPicker.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                    window.selectedColor = Color.FromRgb(r, g, b);
                    Close();
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else if (tab_hsv.IsSelected)
            {
                try
                {
                    double h = double.Parse(tbHValue.Text);
                    double s = double.Parse(tbSValue.Text);
                    double v = double.Parse(tbVValue.Text);

                    (byte r, byte g, byte b) = ConvertHsvToRgb(h, s, v);

                    var window = ((MainWindow)Application.Current.MainWindow);
                    window.colorPicker.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                    window.selectedColor = Color.FromRgb(r, g, b);
                    Close();
                }
                catch
                {
                    MessageBox.Show("Podano złe liczby!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private (byte r, byte g, byte b) ConvertHsvToRgb(double h, double s, double v)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double rPrime = 0, gPrime = 0, bPrime = 0;

            if (h >= 0 && h < 60) { rPrime = c; gPrime = x; bPrime = 0; }
            else if (h >= 60 && h < 120) { rPrime = x; gPrime = c; bPrime = 0; }
            else if (h >= 120 && h < 180) { rPrime = 0; gPrime = c; bPrime = x; }
            else if (h >= 180 && h < 240) { rPrime = 0; gPrime = x; bPrime = c; }
            else if (h >= 240 && h < 300) { rPrime = x; gPrime = 0; bPrime = c; }
            else if (h >= 300 && h < 360) { rPrime = c; gPrime = 0; bPrime = x; }

            byte r = (byte)((rPrime + m) * 255);
            byte g = (byte)((gPrime + m) * 255);
            byte b = (byte)((bPrime + m) * 255);

            return (r, g, b);
        }

        private (double H, double S, double V) ConvertRgbToHsv(byte r, byte g, byte b)
        {
            double rNorm = r / 255.0;
            double gNorm = g / 255.0;
            double bNorm = b / 255.0;

            double max = Math.Max(rNorm, Math.Max(gNorm, bNorm));
            double min = Math.Min(rNorm, Math.Min(gNorm, bNorm));
            double delta = max - min;

            double h = 0;
            if (delta != 0)
            {
                if (max == rNorm) { h = (gNorm - bNorm) / delta + (gNorm < bNorm ? 6 : 0); }
                else if (max == gNorm) { h = (bNorm - rNorm) / delta + 2; }
                else if (max == bNorm) { h = (rNorm - gNorm) / delta + 4; }
                h *= 60;
            }

            double s = max == 0 ? 0 : delta / max;

            double v = max;

            return (h, s * 100, v * 100); // Scale S and V to [0, 100]
        }
    }
}
