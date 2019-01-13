using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Rectangles_and_elipses
{


    public class ViewModel:INotifyPropertyChanged
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource bitmap;
        private string rect;
        private ICommand _Generate;
        ObservableCollection<string> listSource = new ObservableCollection<string>();

        private delegate void GenerateBitmapDelegate();
        GenerateBitmapDelegate myDelegate;

        

        public ImageSource Bitmap
        {
            get { return this.bitmap; }
            set
            {
                this.bitmap = value;
                OnProperityChanged(nameof(this.Bitmap));
            }
        }

        public string Rect
        {
            get { return this.rect; }
            set
            {
                this.rect = value;
                listSource.Add(this.rect);
                OnProperityChanged(nameof(this.Rect));
            }
        }

        public ObservableCollection<string> ListSource
        {
            get { return this.listSource; }
            set
            {
                this.listSource = value;
                OnProperityChanged(nameof(this.ListSource));
            }
        }
        public ICommand Generate
        {
            get
            {
                if (this._Generate == null)
                {
                    myDelegate = new GenerateBitmapDelegate(GenerateBitmap);
                    _Generate = new RelayCommand(param => new Thread(new ThreadStart(GenerateBitmap)).Start(), param => true);
                }
                return _Generate;

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperityChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void GenerateBitmap()
        {
            int i = 0;
            while (true)
            {
                if (i == 20) break;
                Bitmap Btmap = new Bitmap(640, 480);
                string Dimensions = "X=";
                //Clear surface
                for (int y = 0; y < 480; y++)
                    for (int x = 0; x < 640; x++)
                    {
                        Btmap.SetPixel(x, y, System.Drawing.Color.White);
                    }
                using (Graphics g = Graphics.FromImage(Btmap))
                {
                    Random rng = new Random();
                    Rectangle rect = new Rectangle();
                    //Randomize rectangle position and size
                    rect.X = rng.Next(20, 600);
                    rect.Y = rng.Next(20, 440);
                    Dimensions += rect.X;
                    Dimensions += " Y=";
                    Dimensions += rect.Y;
                    rect.Width = rng.Next(20, 640 - rect.X);
                    rect.Height = rng.Next(20, 480 - rect.Y);
                    //Pen and brush
                    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 5);
                    System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
                    g.DrawRectangle(pen, rect);
                    g.FillRectangle(brush, rect);
                    //Randomize elipse position and size
                    rect.X = rng.Next(20, 600);
                    rect.Y = rng.Next(20, 440);
                    rect.Width = rng.Next(20, 640 - rect.X);
                    rect.Height = rng.Next(20, 480 - rect.Y);
                    pen = new System.Drawing.Pen(System.Drawing.Color.Blue, 5);
                    brush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue);
                    g.DrawEllipse(pen, rect);
                    g.FillEllipse(brush, rect);
                }
                var handle = Btmap.GetHbitmap();

                try
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            this.Rect = Dimensions;
                            this.Bitmap = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    }
                    else
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }
                finally
                {
                    DeleteObject(handle);
                    i++;
                }
            }

        }
    }
}
