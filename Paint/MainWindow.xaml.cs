using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        int drawStyle = 1;

        Color selectedColor = Color.FromRgb(0,0,0);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 1;
        }

        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 2;
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void paintSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (drawStyle)
                {
                    case 1:
                        Line line = new Line();
                        line.Stroke = new SolidColorBrush(selectedColor);
                        line.X1 = currentPoint.X - mainWindow.Width / 5;
                        line.Y1 = currentPoint.Y;
                        line.X2 = e.GetPosition(this).X - mainWindow.Width / 5;
                        line.Y2 = e.GetPosition(this).Y;

                        currentPoint = e.GetPosition(this);
                        paintSurface.Children.Add(line);

                        break;
                }
            }
        }

        private void paintSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch (drawStyle)
                {
                    case 2:
                        Ellipse ellipse = new Ellipse();
                        ellipse.Width = 6;
                        ellipse.Height = 6;

                        Canvas.SetTop(ellipse, e.GetPosition(this).Y - ellipse.Height / 2);
                        Canvas.SetLeft(ellipse, e.GetPosition(this).X - ellipse.Width / 2 - mainWindow.Width / 5);

                        ellipse.Fill = new SolidColorBrush(selectedColor);
                        paintSurface.Children.Add(ellipse);

                        break;
                    case 7:
                        Rectangle rect = new Rectangle();

                        rect.Width = 60;
                        rect.Height = 60;

                        Canvas.SetTop(rect, e.GetPosition(this).Y - rect.Height / 2);
                        Canvas.SetLeft(rect, e.GetPosition(this).X - rect.Width / 2 - mainWindow.Width / 5);

                        Brush brushColor = new SolidColorBrush(selectedColor);
                        rect.Stroke = brushColor;

                        paintSurface.Children.Add(rect);

                        break;
                    case 8:
                        Polygon poly = new Polygon();

                        double mouseX = e.GetPosition(this).X - mainWindow.Width / 5;
                        double mouseY = e.GetPosition(this).Y;

                        double polySize = 20;

                        Point point1 = new Point(mouseX - polySize, mouseY + 2 * polySize);
                        Point point2 = new Point(mouseX + polySize, mouseY + 2 * polySize);
                        Point point3 = new Point(mouseX + 2 * polySize, mouseY + 0);
                        Point point4 = new Point(mouseX + polySize, mouseY - 2 * polySize);
                        Point point5 = new Point(mouseX - polySize, mouseY - 2 * polySize);
                        Point point6 = new Point(mouseX - 2 * polySize, mouseY - 0);

                        poly.Points.Add(point1);
                        poly.Points.Add(point2);
                        poly.Points.Add(point3);
                        poly.Points.Add(point4);
                        poly.Points.Add(point5);
                        poly.Points.Add(point6);

                        brushColor = new SolidColorBrush(selectedColor);
                        poly.Stroke = brushColor;

                        paintSurface.Children.Add(poly);

                        break;
                }
            }
        }

        private void paintSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void drawSegment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editSegment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void drawRectangle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 7;
        }

        private void drawPolygon_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 8;
        }

        private void colorPicker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPicker colorPicker = new ColorPicker();
            colorPicker.Show();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "Image Files (*.jpg; *.jpeg; *.gif; *.bmp) | *.jpg; *.jpeg; *.gif; *.bmp";

            if (openDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openDialog.FileName);
                

            }
        }
    }
}
