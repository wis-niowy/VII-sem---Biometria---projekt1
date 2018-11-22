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
using System.Windows.Shapes;
using System.Drawing;
using System.ComponentModel;

namespace V_sem___GK___projekt3
{
    /// <summary>
    /// Interaction logic for HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window, INotifyPropertyChanged
    // with help: http://trompelecode.com/blog/2012/04/how-to-create-an-image-histogram-using-csharp-and-wpf/
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private System.Drawing.Color[,] PictureColors { get; set; }
        private PointCollection luminanceHistogramPoints = null;
        private PointCollection redColorHistogramPoints = null;
        private PointCollection greenColorHistogramPoints = null;
        private PointCollection blueColorHistogramPoints = null;

        public bool PerformHistogramSmoothing { get; set; }

        public PointCollection LuminanceHistogramPoints
        {
            get
            {
                return this.luminanceHistogramPoints;
            }
            set
            {
                if (this.luminanceHistogramPoints != value)
                {
                    this.luminanceHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LuminanceHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection RedColorHistogramPoints
        {
            get
            {
                return this.redColorHistogramPoints;
            }
            set
            {
                if (this.redColorHistogramPoints != value)
                {
                    this.redColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get
            {
                return this.greenColorHistogramPoints;
            }
            set
            {
                if (this.greenColorHistogramPoints != value)
                {
                    this.greenColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection BlueColorHistogramPoints
        {
            get
            {
                return this.blueColorHistogramPoints;
            }
            set
            {
                if (this.blueColorHistogramPoints != value)
                {
                    this.blueColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramPoints"));
                    }
                }
            }
        }


        public HistogramWindow(System.Drawing.Color[,] pictureColors)
        {
            InitializeComponent();
            this.PictureColors = pictureColors;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Luminance
            ImageData statistics = new ImageData(PictureColors);
            this.LuminanceHistogramPoints = ConvertToPointCollection(statistics.LuminanceValues);
            // RGB
            this.RedColorHistogramPoints = ConvertToPointCollection(statistics.RedValues);
            this.GreenColorHistogramPoints = ConvertToPointCollection(statistics.GreenValues);
            this.BlueColorHistogramPoints = ConvertToPointCollection(statistics.BlueValues);
        }

        private PointCollection ConvertToPointCollection(int[] values)
        {
            if (this.PerformHistogramSmoothing)
            {
                values = SmoothHistogram(values);
            }

            int max = values.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new System.Windows.Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new System.Windows.Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new System.Windows.Point(values.Length - 1, max));

            return points;
        }

        private int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues;
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
