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
        public Image loadedImage = new Image();
        Point currentPoint = new Point();
        int drawStyle = 1;

        public Color selectedColor = Color.FromRgb(0,0,0);

        private Line _currentLine;
        private Ellipse _startHandle, _endHandle;
        private bool _isDraggingStart, _isDraggingEnd;

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
            drawStyle = 3;
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

                    case 3:
                        if (_currentLine == null) return;

                        Point currentPosition = e.GetPosition(paintSurface);

                        if (_isDraggingStart)
                        {
                            UpdateHandlePosition(_startHandle, currentPosition);
                            _currentLine.X1 = currentPosition.X;
                            _currentLine.Y1 = currentPosition.Y;
                        }
                        else if (_isDraggingEnd)
                        {
                            UpdateHandlePosition(_endHandle, currentPosition);
                            _currentLine.X2 = currentPosition.X;
                            _currentLine.Y2 = currentPosition.Y;
                        }
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
                    case 3:
                        Point clickPosition = e.GetPosition(paintSurface);

                        if (_currentLine == null)
                        {
                            _currentLine = new Line
                            {
                                Stroke = new SolidColorBrush(selectedColor),
                                X1 = clickPosition.X,
                                Y1 = clickPosition.Y,
                                X2 = clickPosition.X,
                                Y2 = clickPosition.Y
                            };

                            paintSurface.Children.Add(_currentLine);

                            _startHandle = CreateHandle(clickPosition);
                            _endHandle = CreateHandle(clickPosition);

                            paintSurface.Children.Add(_startHandle);
                            paintSurface.Children.Add(_endHandle);
                        }
                        else
                        {
                            if (IsMouseOverHandle(_startHandle, clickPosition))
                            {
                                _isDraggingStart = true;
                            }
                            else if (IsMouseOverHandle(_endHandle, clickPosition))
                            {
                                _isDraggingEnd = true;
                            }
                            else
                            {
                                FinalizeLine();
                            }
                        }
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

        private void paintSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingStart = false;
            _isDraggingEnd = false;
        }

        private void paintSurface_MouseDown(object sender, MouseButtonEventArgs e)
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
                loadedImage.Source = new BitmapImage(fileUri);

                Canvas.SetTop(loadedImage, 200);
                Canvas.SetLeft(loadedImage, 200);

                ImageUpload imageUpload = new ImageUpload(new BitmapImage(fileUri));
                imageUpload.Show();
            }

        }

        // Line creation and edition logic

        private Ellipse CreateHandle(Point position)
        {
            var handle = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Opacity = 0.4,
            };
            UpdateHandlePosition(handle, position);
            return handle;
        }

        private void UpdateHandlePosition(Ellipse handle, Point position)
        {
            Canvas.SetLeft(handle, position.X - handle.Width / 2);
            Canvas.SetTop(handle, position.Y - handle.Height / 2);
        }

        private bool IsMouseOverHandle(Ellipse handle, Point mousePosition)
        {
            double left = Canvas.GetLeft(handle);
            double top = Canvas.GetTop(handle);

            return mousePosition.X >= left &&
                   mousePosition.X <= left + handle.Width &&
                   mousePosition.Y >= top &&
                   mousePosition.Y <= top + handle.Height;
        }

        private void FinalizeLine()
        {
            if (_startHandle != null)
            {
                paintSurface.Children.Remove(_startHandle);
                _startHandle = null;
            }

            if (_endHandle != null)
            {
                paintSurface.Children.Remove(_endHandle);
                _endHandle = null;
            }

            _currentLine = null;
        }

        public void uploadImage(BitmapImage bp)
        {
            Image uploaded = new Image();
            uploaded.Source = bp;

            paintSurface.Children.Add(uploaded);
        }
    }
}
