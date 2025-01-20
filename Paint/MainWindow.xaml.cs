using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        // for line drawing
        private Line _currentLine;
        private Ellipse _startHandle, _endHandle;
        private bool _isDraggingStart, _isDraggingEnd;

        // for ellipse drawing
        private Ellipse currentEllipse;
        private Point startPoint;
        private bool isDrawing = false;

        // for rectangle drawing
        private Rectangle currentRectangle;

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

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 4;
        }

        private void btnRectangle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 5;
        }

        private void drawCircle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 6;
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

                    case 4:
                        if (isDrawing && currentEllipse != null)
                        {
                            Point currentPoint = e.GetPosition(paintSurface);

                            double width = Math.Abs(currentPoint.X - startPoint.X);
                            double height = Math.Abs(currentPoint.Y - startPoint.Y);

                            currentEllipse.Width = width;
                            currentEllipse.Height = height;

                            Canvas.SetLeft(currentEllipse, Math.Min(startPoint.X, currentPoint.X));
                            Canvas.SetTop(currentEllipse, Math.Min(startPoint.Y, currentPoint.Y));
                        }
                        break;

                    case 5:
                        if (isDrawing && currentRectangle != null)
                        {
                            Point currentPoint = e.GetPosition(paintSurface);

                            double width = Math.Abs(currentPoint.X - startPoint.X);
                            double height = Math.Abs(currentPoint.Y - startPoint.Y);

                            currentRectangle.Width = width;
                            currentRectangle.Height = height;

                            Canvas.SetLeft(currentRectangle, Math.Min(startPoint.X, currentPoint.X));
                            Canvas.SetTop(currentRectangle, Math.Min(startPoint.Y, currentPoint.Y));
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

                    case 4:
                        isDrawing = true;
                        startPoint = e.GetPosition(paintSurface);

                        currentEllipse = new Ellipse
                        {
                            Stroke = new SolidColorBrush(selectedColor),
                            Fill = Brushes.Transparent // Optional: Change if you want filled ellipses
                        };

                        Canvas.SetLeft(currentEllipse, startPoint.X);
                        Canvas.SetTop(currentEllipse, startPoint.Y);
                        paintSurface.Children.Add(currentEllipse);
                        break;

                    case 5:
                        isDrawing = true;
                        startPoint = e.GetPosition(paintSurface);

                        // Create a new Rectangle
                        currentRectangle = new Rectangle
                        {
                            Stroke = new SolidColorBrush(selectedColor),
                            Fill = Brushes.Transparent // Optional: Change for filled rectangles
                        };

                        Canvas.SetLeft(currentRectangle, startPoint.X);
                        Canvas.SetTop(currentRectangle, startPoint.Y);
                        paintSurface.Children.Add(currentRectangle);
                        break;

                    case 6:
                        Ellipse circle = new Ellipse();

                        circle.Width = 60;
                        circle.Height = 60;

                        Canvas.SetTop(circle, e.GetPosition(this).Y - circle.Height / 2);
                        Canvas.SetLeft(circle, e.GetPosition(this).X - circle.Width / 2 - mainWindow.Width / 5);

                        Brush brushColorCircle = new SolidColorBrush(selectedColor);
                        circle.Stroke = brushColorCircle;

                        paintSurface.Children.Add(circle);

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

                    case 9:
                        Polygon tree = new Polygon();
                        tree.Points = new PointCollection()
                        {
                            new Point(0, -60),
                            new Point(60, 60),
                            new Point(-60, 60)
                        };

                        tree.Fill = new SolidColorBrush(Colors.Green);

                        double treeWidth = 120;

                        double treeTopY = e.GetPosition(this).Y - 60 - 20;
                        Canvas.SetTop(tree, treeTopY);

                        Canvas.SetLeft(tree, e.GetPosition(this).X - treeWidth / 2 - mainWindow.Width / 5);

                        paintSurface.Children.Add(tree);

                        Rectangle trunk = new Rectangle();
                        trunk.Width = 20;
                        trunk.Height = 60;
                        trunk.Fill = new SolidColorBrush(Colors.Brown);

                        Canvas.SetTop(trunk, e.GetPosition(this).Y - 20);
                        Canvas.SetLeft(trunk, e.GetPosition(this).X - trunk.Width / 2 - mainWindow.Width / 5 - 60);

                        paintSurface.Children.Add(trunk);

                        break;
                }
            }
        }

        private void paintSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch(drawStyle)
            {
                case 3:
                    _isDraggingStart = false;
                    _isDraggingEnd = false;
                    break;

                case 4:
                    if (isDrawing)
                    {
                        isDrawing = false;
                        currentEllipse = null;
                    }
                    break;

                case 5:
                    if (isDrawing)
                    {
                        isDrawing = false;
                        currentRectangle = null;
                    }
                    break;
            }
        }

        private void drawRectangle_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 7;
        }

        private void drawPolygon_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 8;
        }

        private void drawTree_Click(object sender, RoutedEventArgs e)
        {
            drawStyle = 9;
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

        private void btnSaveToPng_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg",
                DefaultExt = "png",
                AddExtension = true
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;
                BitmapEncoder encoder;

                if (System.IO.Path.GetExtension(filename).ToLower() == ".jpg")
                {
                    encoder = new JpegBitmapEncoder();
                }
                else
                {
                    encoder = new PngBitmapEncoder();
                }

                paintSurface.Measure(new Size(paintSurface.ActualWidth, paintSurface.ActualHeight));
                paintSurface.Arrange(new Rect(new Size(paintSurface.ActualWidth, paintSurface.ActualHeight)));

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)paintSurface.ActualWidth, (int)paintSurface.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);

                renderBitmap.Render(paintSurface);

                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Obraz zapisany!", "Informacja o zapisie", MessageBoxButton.OK, MessageBoxImage.Information);
            }

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

        public void uploadImage(BitmapImage bitmapImage)
        {
            // Create a new Image control
            Image uploaded = new Image
            {
                Source = bitmapImage
            };

            // Resizing the image for canvas not to be cut off the screen
            double canvasWidth = paintSurface.ActualWidth;
            double canvasHeight = paintSurface.ActualHeight;

            double imageAspect = bitmapImage.Width / bitmapImage.Height;
            double canvasAspect = canvasWidth / canvasHeight;

            double imageWidth, imageHeight;
            if (imageAspect > canvasAspect)
            {
                imageWidth = canvasWidth;
                imageHeight = canvasWidth / imageAspect;
            }
            else
            {
                imageHeight = canvasHeight;
                imageWidth = canvasHeight * imageAspect;
            }

            uploaded.Width = imageWidth;
            uploaded.Height = imageHeight;

            Canvas.SetLeft(uploaded, (canvasWidth - imageWidth) / 2);
            Canvas.SetTop(uploaded, (canvasHeight - imageHeight) / 2);

            // Add the image container to the canvas
            paintSurface.Children.Add(uploaded);
        }
        private void DrawChristmasTree()
        {
            DrawTree(0, 0, 150, 300);
            DrawBauble(380, 300, 20, Brushes.Red);
            DrawBauble(420, 250, 20, Brushes.Blue);
            // Możesz dodać więcej bąbków według potrzeb
        }

        private void color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;

            if (rectangle != null)
            {
                switch (rectangle.Name)
                {
                    case "color1":
                        selectedColor = Color.FromRgb(0, 0, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color2":
                        selectedColor = Color.FromRgb(255, 0, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color3":
                        selectedColor = Color.FromRgb(0, 128, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color4":
                        selectedColor = Color.FromRgb(0, 0, 255);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color5":
                        selectedColor = Color.FromRgb(153, 153, 153);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color6":
                        selectedColor = Color.FromRgb(128, 0, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color7":
                        selectedColor = Color.FromRgb(0, 102, 102);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color8":
                        selectedColor = Color.FromRgb(0, 0, 128);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color9":
                        selectedColor = Color.FromRgb(204, 204, 204);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color10":
                        selectedColor = Color.FromRgb(128, 0, 128);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color11":
                        selectedColor = Color.FromRgb(0, 255, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color12":
                        selectedColor = Color.FromRgb(0, 255, 255);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color13":
                        selectedColor = Color.FromRgb(255, 255, 255);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color14":
                        selectedColor = Color.FromRgb(255, 0, 255);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color15":
                        selectedColor = Color.FromRgb(255, 255, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                    case "color16":
                        selectedColor = Color.FromRgb(128, 128, 0);
                        colorPicker.Fill = new SolidColorBrush(selectedColor);
                        break;

                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Ensure there are children in the canvas
            if (paintSurface.Children.Count > 0)
            {
                // Get the last child element
                var lastChild = paintSurface.Children[paintSurface.Children.Count - 1];

                // Remove the last child
                paintSurface.Children.Remove(lastChild);
            }
        }

        private void DrawTree(double x, double y, double width, double height)
        {
            var tree = new Polygon
            {
                Fill = Brushes.Green,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 2,
                Points = new PointCollection
                {
                    new Point(x, y),
                    new Point(x - width / 2, y + height),
                    new Point(x + width / 2, y + height)
                }
            };
            paintSurface.Children.Add(tree);
        }

        private void DrawBauble(double x, double y, double radius, Brush color)
        {
            var bauble = new Ellipse
            {
                Width = radius,
                Height = radius,
                Fill = color
            };
            Canvas.SetLeft(bauble, x - radius / 2);
            Canvas.SetTop(bauble, y - radius / 2);
            paintSurface.Children.Add(bauble);
        }
    }
}
