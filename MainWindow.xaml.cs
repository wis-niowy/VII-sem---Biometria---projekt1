using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace V_sem___GK___projekt3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static int bitmapWidth;
        private static int bitmapHeight;
        private bool isRedCanalTicked;
        private bool isGreenCanalTicked;
        private bool isBlueCanalTicked;
        public System.Drawing.Color[,] pictureColors;
        public bool[,] alreadyFilteredPixels;
        public bool IsBrushInUse { get; set; }
        public int brushRadius;
        public int textboxInputValue;
        public String CurrentPicturePath { get; set; }
        public ModeType Mode { get; set; }
        private FilterType filterType;
        private Bitmap currentBitmap;
        public Bitmap CurrentBitmap
        {
            get
            {
                return currentBitmap;
            }
            set
            {
                currentBitmap = value;
                NotifyPropertyChanged("CurrentBitmap");
            }
        }
        public int BrushRadius
        {
            get
            {
                return brushRadius;
            }
            set
            {
                brushRadius = value;
            }
        }
        public int TextboxInputValue
        {
            get
            {
                return textboxInputValue;
            }
            set
            {
                textboxInputValue = value;
            }
        }
        public bool IsRedCanalTicked
        {
            get
            {
                return isRedCanalTicked;
            }
            set
            {
                isRedCanalTicked = value;
                NotifyPropertyChanged("IsRedCanalTicked");
            }
        }
        public bool IsGreenCanalTicked
        {
            get
            {
                return isGreenCanalTicked;
            }
            set
            {
                isGreenCanalTicked = value;
                NotifyPropertyChanged("IsGreenCanalTicked");
            }
        }
        public bool IsBlueCanalTicked
        {
            get
            {
                return isBlueCanalTicked;
            }
            set
            {
                isBlueCanalTicked = value;
                NotifyPropertyChanged("IsBlueCanalTicked");
            }
        }
        public FilterType FilterType
        {
            get
            {
                return filterType;
            }
            set
            {
                filterType = value;
                NotifyPropertyChanged("FilterType");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void CanvasWithImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point clickCoords = new System.Drawing.Point((int)e.GetPosition(BottomImage).X, (int)e.GetPosition(BottomImage).Y);
            if (Mode == ModeType.Brush)
            {
                alreadyFilteredPixels = new bool[bitmapWidth, bitmapHeight]; // default is false
            }
        }

        private void CanvasWithImage_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Drawing.Point clickCoords = new System.Drawing.Point((int)e.GetPosition(BottomImage).X, (int)e.GetPosition(BottomImage).Y);
            FilterCanalArgs args = new FilterCanalArgs(IsRedCanalTicked, IsGreenCanalTicked, IsBlueCanalTicked);
            if (Mode == ModeType.Brush && e.LeftButton == MouseButtonState.Pressed)
            {
                switch (FilterType)
                {
                    case FilterType.None:
                        break;
                    case FilterType.Negative:
                        Filter f = new NegativeFilter(pictureColors);
                        currentBitmap = f.ApplyFilter(currentBitmap, args, clickCoords, BrushRadius, alreadyFilteredPixels);
                        break;
                    case FilterType.Brightness:
                        f = new BrightnessFilter(pictureColors, TextboxInputValue);
                        currentBitmap = f.ApplyFilter(currentBitmap, args, clickCoords, BrushRadius, alreadyFilteredPixels);
                        break;
                    case FilterType.Contrast:
                        f = new ContrastFilter(pictureColors, TextboxInputValue);
                        currentBitmap = f.ApplyFilter(currentBitmap, args, clickCoords, BrushRadius, alreadyFilteredPixels);
                        break;
                    case FilterType.Gamma:
                        f = new GammaFilter(pictureColors);
                        currentBitmap = f.ApplyFilter(currentBitmap, args, clickCoords, BrushRadius, alreadyFilteredPixels);
                        break;
                    case FilterType.GreyScale:
                        f = new GreyFilter(pictureColors);
                        currentBitmap = f.ApplyFilter(currentBitmap, args, clickCoords, BrushRadius, alreadyFilteredPixels);
                        break;
                    case FilterType.Own:
                        break;
                    default:
                        break;
                }
                BottomImage.Source = loadBitmap(CurrentBitmap);
            }
        }

        private void CanvasWithImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateImage();
        }

        private void CanvasWithImage_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CanvasWithImage_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CanvasWithImage_Loaded(object sender, RoutedEventArgs e)
        {
            bitmapWidth = (int)CanvasWithImage.ActualWidth;
            bitmapHeight = (int)CanvasWithImage.ActualHeight;
            this.CurrentBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            this.pictureColors = new System.Drawing.Color[bitmapWidth, bitmapHeight];
            IsRedCanalTicked = IsGreenCanalTicked = IsBlueCanalTicked = true;
        }

        private void LoadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter += "Image files (*.jpg, *.bmp, *.png) | *.jpg; *.bmp; *.png | All files (*.*) | *.*";
            while (true)
            {
                bool? dr = ofd.ShowDialog();
                if (dr.HasValue && dr.Value)
                {
                    string filename = ofd.FileName;
                    string ext = System.IO.Path.GetExtension(filename);
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png")
                    {
                        System.Windows.MessageBox.Show("Wrong file extension!\n You need to upload graphics.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    SetNewPicture(filename);
                    CurrentPicturePath = filename;

                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void ReloadPictureButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPicturePath != null)
                SetNewPicture(CurrentPicturePath);
        }

        private void ShowHistogramButton_Click(object sender, RoutedEventArgs e)
        {
            HistogramWindow hw = new HistogramWindow(pictureColors);
            hw.Show();
        }


        private void PerformButton_Click(object sender, RoutedEventArgs e)
        {
            Filter f;
            FilterCanalArgs args = new FilterCanalArgs(IsRedCanalTicked, IsGreenCanalTicked, IsBlueCanalTicked);
            switch (FilterType)
            {
                case FilterType.None:
                    break;
                case FilterType.Negative:
                    f = new NegativeFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Brightness:
                    f = new BrightnessFilter(pictureColors, TextboxInputValue);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Contrast:
                    f = new ContrastFilter(pictureColors, TextboxInputValue);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Gamma:
                    f = new GammaFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.GreyScale:
                    f = new GreyFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Threshold:
                    f = new ThresholdFilter(pictureColors, TextboxInputValue);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.NormalizeHistogram:
                    f = new NormalizeHistogramFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.EqualizeHistogram:
                    f = new EqualizeHistogramFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Averaging:
                    f = new AveragingFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Gaussian:
                    f = new GaussianFilter(pictureColors, TextboxInputValue);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Laplacian:
                    f = new LaplacianFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.LaplacianDiag:
                    f = new LaplacianDerivativesOnDiagFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.LaplacianThreeParallelLines:
                    f = new LaplacianThreeParallelLinesFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.RobertsCross:
                    f = new RobertsCrossFilter(pictureColors);
                    currentBitmap = f.ApplyFilter(currentBitmap, args);
                    break;
                case FilterType.Own:
                    break;
                default:
                    break;
                
            }
            UpdateImage();
            //BottomImage.Source = loadBitmap(CurrentBitmap);
        }

        private void SetNewPicture(string filename)
        {
            CurrentBitmap = new Bitmap(new Bitmap(filename), new System.Drawing.Size(bitmapWidth, bitmapHeight));
            BottomImage.Source = loadBitmap(CurrentBitmap);
            pictureColors = WriteBitmapToArray(CurrentBitmap);
        }

        private void UpdateImage()
        {
            BottomImage.Source = loadBitmap(CurrentBitmap);
            pictureColors = WriteBitmapToArray(CurrentBitmap);
        }

        private System.Drawing.Color[,] WriteBitmapToArray(Bitmap bitmap)
        {
            System.Drawing.Color[,] returnArray = new System.Drawing.Color[bitmap.Width, bitmap.Height];
            for (int i = 0; i < bitmap.Width; ++i)
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    returnArray[i, j] = bitmap.GetPixel(i, j);
                }
            return returnArray;
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //CheckIsNumeric(e);
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("^-?[0-9]+$"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return ((double)value).ToString();
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return double.Parse((string)value);
        }
    }

    public class FilterTypeToLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                String returnString = "";
                switch((FilterType)value)
                {
                    case FilterType.Brightness:
                        returnString = "Delta:";
                        break;
                    case FilterType.Contrast:
                        returnString = "Contrast:";
                        break;
                    case FilterType.Threshold:
                        returnString = "Threshold:";
                        break;
                    case FilterType.Gaussian:
                        returnString = "Sigma:";
                        break;
                }
                return returnString;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class FilterCanalArgs
    {
        public bool Red;
        public bool Green;
        public bool Blue;

        public FilterCanalArgs(bool r, bool g, bool b)
        {
            Red = r; Green = g; Blue = b;
        }
    }

    public enum ModeType
    {
        WholePicture,
        Brush
    }

    public enum FilterType
    {
        None,
        Negative,
        Brightness,
        Contrast,
        Gamma,
        GreyScale,
        Threshold,
        NormalizeHistogram,
        EqualizeHistogram,
        Averaging,
        Gaussian,
        Laplacian,
        LaplacianDiag,
        LaplacianThreeParallelLines,
        RobertsCross,
        Own
    }
}
