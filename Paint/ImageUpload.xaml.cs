using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
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
                float[,] filter = new float[3, 3]
                {
                    { float.Parse(Matrix11.Text), float.Parse(Matrix12.Text), float.Parse(Matrix13.Text) },
                    { float.Parse(Matrix21.Text), float.Parse(Matrix22.Text), float.Parse(Matrix23.Text) },
                    { float.Parse(Matrix31.Text), float.Parse(Matrix32.Text), float.Parse(Matrix33.Text) }
                };

                Bitmap converted = BitmapImage2Bitmap(bitmapImage);

                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, converted.Width, converted.Height);
                BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                try
                {
                    using (Image<Bgr, Byte> prefiltered = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0))
                    {
                        ConvolutionKernelF kernel = new ConvolutionKernelF(filter);
                        using (Image<Bgr, float> filtered = prefiltered.Convolution(kernel))
                        {
                            using (Image<Bgr, Byte> normalized = filtered.Convert<Bgr, Byte>())
                            {
                                Bitmap temp = normalized.ToBitmap();
                                bitmapImage = Bitmap2BitmapImage(temp);
                            }
                        }
                    }
                }
                finally
                {
                    converted.UnlockBits(bmpData);
                }

                ImageSpace.Source = bitmapImage;
                ImageSpace.Stretch = Stretch.Uniform;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie wpisano poprawnie liczb", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
            BitmapData bmpData = converted.LockBits(rectangle, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            try
            {
                if (FilterSelection.SelectedIndex == 0) // Sobel Filter
                {
                    using (Image<Bgr, Byte> sobelBip = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0))
                    {
                        using (Image<Bgr, float> sobel = sobelBip.Sobel(0, 1, 3))
                        {
                            Bitmap temp = sobel.ToBitmap();
                            bitmapImage = Bitmap2BitmapImage(temp);
                        }
                    }
                }
                
                else if (FilterSelection.SelectedIndex == 1) // Laplace Filter
                {
                    using (Image<Bgr, Byte> laplaceBip = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0))
                    {
                        using (Image<Gray, Byte> gray = laplaceBip.Convert<Gray, Byte>())
                        {

                            using (Image<Gray, float> laplace = gray.Laplace(3))
                            {
                                Bitmap temp = laplace.ToBitmap();
                                bitmapImage = Bitmap2BitmapImage(temp);
                            }
                        }
                    }
                }

                else if (FilterSelection.SelectedIndex == 2) // Gaussian Blur
                {
                    using (Image<Bgr, Byte> gaussBip = new Image<Bgr, Byte>(converted.Width, converted.Height, bmpData.Stride, bmpData.Scan0))
                    {
                        using (Image<Bgr, Byte> gauss = gaussBip.SmoothGaussian(5))
                        {
                            Bitmap temp = gauss.ToBitmap();
                            bitmapImage = Bitmap2BitmapImage(temp);
                        }
                    }
                }
            }
            finally
            {
                converted.UnlockBits(bmpData);
            }

            ImageSpace.Source = bitmapImage;
            ImageSpace.Stretch = Stretch.Uniform;


        }
    }
}
