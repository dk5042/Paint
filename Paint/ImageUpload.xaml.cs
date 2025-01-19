using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Paint
{
    /// <summary>
    /// Logika interakcji dla klasy ImageUpload.xaml
    /// </summary>
    public partial class ImageUpload : Window
    {
        BitmapImage bitmapImage;
        public ImageUpload(BitmapImage bitmapImage)
        {
            InitializeComponent();
            this.bitmapImage = bitmapImage;
            ImageSpace.Source = bitmapImage;
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).uploadImage(bitmapImage);
            Close();
        }

        private void MatrixFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float[,] filter = new float[3, 3];

                filter[0, 0] = float.Parse(Matrix11.Text);
                filter[0, 1] = float.Parse(Matrix12.Text);
                filter[0, 2] = float.Parse(Matrix13.Text);

                filter[1, 0] = float.Parse(Matrix21.Text);
                filter[1, 1] = float.Parse(Matrix22.Text);
                filter[1, 2] = float.Parse(Matrix23.Text);

                filter[2, 0] = float.Parse(Matrix31.Text);
                filter[2, 1] = float.Parse(Matrix32.Text);
                filter[2, 2] = float.Parse(Matrix33.Text);

                Bitmap converted = BitmapImage2Bitmap(bitmapImage);

                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);
                BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, converted.PixelFormat);
                Image<Bgr, Byte> prefiltered = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0);

                ConvolutionKernelF kernel = new ConvolutionKernelF(filter);
                Image<Bgr, float> filtered = prefiltered.Convolution(kernel);
                Bitmap temp = filtered.ToBitmap();
                bitmapImage = Bitmap2BitmapImage(temp);
                ImageSpace.Source = bitmapImage;
            }
            catch
            {
                MessageBox.Show("Wprowadzono nieprawidłowe dane.", "Wystąpił błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap converted = BitmapImage2Bitmap(bitmapImage);

            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);
            BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, converted.PixelFormat);

            if (FilterSelection.SelectedIndex == 0)
            {
                Image<Bgr, Byte> sobelBip = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0);
                Image<Bgr, float> sobel = sobelBip.Sobel(0, 1, 3);

                Bitmap temp = sobel.ToBitmap();
                bitmapImage = Bitmap2BitmapImage(temp);
                ImageSpace.Source = bitmapImage;
            }
        }
    }
}
