using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace From_WritableBitmap_to_Image
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial  class MainWindow : Window
    {
        WriteableBitmap wb;

        public MainWindow()
        {
            InitializeComponent();
                    
        }

        //BmpBitmapEncoder method
        private System.Drawing.Image BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
            swatch.Start(); 

            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
                swatch.Stop();
                externalTime.Content = swatch.Elapsed.Seconds + " sec " + swatch.Elapsed.TotalMilliseconds + " msec";

                return bmp;
            }

        }

        //CopyPixels method
        private Bitmap GetBitmap(BitmapSource source)
        {
            System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
            swatch.Start();
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);

            swatch.Stop();
            externalTime1.Content = swatch.Elapsed.Seconds +" sec " + swatch.Elapsed.TotalMilliseconds + " msec";
            return bmp;
        }



        //Base method
        public System.Drawing.Image Image(WriteableBitmap writeBmp)
        {          
                System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch(); 
                swatch.Start(); 
                
                if (wb == null)
                    {
                        return null;
                    }

                    int width = wb.PixelWidth; 
                    int height = wb.PixelHeight; 
                    int[] pixels = new int[width * height]; 
                    Int32Rect rect = new Int32Rect(0, 0, width, height);
                    int stride = (rect.Width * wb.Format.BitsPerPixel) / 8; 
                    wb.CopyPixels(rect, pixels, stride, 0); 

                    System.Drawing.Bitmap bm = new System.Drawing.Bitmap(width, height); 

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            bm.SetPixel(x, y, System.Drawing.Color.FromArgb(pixels[x + y * width])); 
                        }
                    }
                swatch.Stop(); 
                baseTime.Content = swatch.Elapsed.Seconds + " sec " + swatch.Elapsed.TotalMilliseconds + " msec";

            return (System.Drawing.Bitmap)bm;                
            
        }

        //Load writeableBitmap
        private void openFile_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;
            Nullable<bool> result = openFileDialog1.ShowDialog();

            if (result == true)
            {
                BitmapSource bmp = BitmapFrame.Create(
            new Uri(openFileDialog1.FileName, UriKind.Relative), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                if (bmp.Format != PixelFormats.Bgra32)
                    bmp = new FormatConvertedBitmap(bmp, PixelFormats.Bgra32, null, 1);
                // Just ignore the last parameter

                wb = new WriteableBitmap(bmp.PixelWidth, bmp.PixelHeight,
                    bmp.DpiX, bmp.DpiY, bmp.Format, bmp.Palette);

                Int32Rect r = new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight);
                wb.Lock();
                bmp.CopyPixels(r, wb.BackBuffer, wb.BackBufferStride * wb.PixelHeight,
                    wb.BackBufferStride);

                wb.AddDirtyRect(r);
                wb.Unlock();


                status.Content = "Success";
            }
            else
            {
                status.Content = "Something gone wrong";
            }
        }
  
        private void baseMethod_Click(object sender, RoutedEventArgs e)
        {
            var tmp = Image(wb);          
        }

        private void externalMethod_Click(object sender, RoutedEventArgs e)
        {             
            var tmp = BitmapFromWriteableBitmap(wb);
        }

        private void externalMethod2_Click(object sender, RoutedEventArgs e)
        {
            var tmp = GetBitmap(wb);
        }               
    }
}
