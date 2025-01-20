using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
                        // Create a Polygon for the Christmas tree
                        Polygon tree = new Polygon();

                        // Define the points for the tree's triangular layers (adjusted for better positioning)
                        tree.Points = new PointCollection
                        {
                            // Top layer of the tree
                            new Point(30, 0),          // Top of the tree (apex)
                            new Point(0, 50),          // Left bottom of the first layer
                            new Point(60, 50),         // Right bottom of the first layer

                            // Middle layer of the tree
                            new Point(15, 25),         // Left top of the second layer
                            new Point(45, 25),         // Right top of the second layer
                            new Point(30, 50),         // Bottom of the second layer

                            // Bottom layer of the tree
                            new Point(5, 40),          // Left bottom of the base layer
                            new Point(55, 40),         // Right bottom of the base layer
                            new Point(30, 70),         // Bottom of the base layer
                        };

                        // Set the tree's stroke and fill color
                        Brush treeColor = new SolidColorBrush(Colors.Green); // Green for tree
                        tree.Fill = treeColor;
                        tree.Stroke = Brushes.Black; // Optional: outline the tree

                        // Position the tree at the cursor, adjusting for better alignment
                        Canvas.SetTop(tree, e.GetPosition(this).Y - 70); // Position tree with respect to the bottom-most layer
                        Canvas.SetLeft(tree, e.GetPosition(this).X - 30 - mainWindow.Width / 5); // Adjust tree width

                        // Add the tree to the canvas
                        paintSurface.Children.Add(tree);

                        // Add a trunk below the tree
                        Rectangle trunk = new Rectangle();
                        trunk.Width = 20;
                        trunk.Height = 30;
                        Brush trunkColor = new SolidColorBrush(Colors.Brown);
                        trunk.Fill = trunkColor;

                        // Position the trunk just below the tree base (aligned with the tree's bottom center)
                        Canvas.SetTop(trunk, e.GetPosition(this).Y - 10);  // Adjust trunk's vertical position below the tree
                        Canvas.SetLeft(trunk, e.GetPosition(this).X - 10 - mainWindow.Width / 5); // Align trunk horizontally under tree

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
