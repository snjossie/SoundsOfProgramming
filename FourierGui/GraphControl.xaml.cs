using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FourierGui
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl, INotifyPropertyChanged
    {
        private WriteableBitmap source;

        private const int HeightInPixels = 512;

        private const int WidthInPixels = 1024;

        private uint[] points;

        private GraphType graphType;

        private uint maximumFrequency;

        public GraphControl()
        {
            InitializeComponent();
            source = BitmapFactory.New(WidthInPixels, HeightInPixels);
            this.graphImg.Source = source;
            this.DataContext = this;
        }

        public GraphType GraphType
        {
            get
            {
                return graphType;
            }

            set
            {
                graphType = value;
                SetAxes();
            }
        }

        public string Maximum { get; set; }

        public string Center { get; set; }

        public string Minimum { get; set; }

        public uint[] Points
        {
            get
            {
                return points;
            }

            set
            {
                points = value;
                Draw(points);
            }
        }

        public uint MaximumSample { get; set; }

        public uint MaximumFrequency
        {
            get
            {
                return maximumFrequency;
            }

            set
            {
                maximumFrequency = value;
                SetAxes();
            }
        }

        public bool IsCenterLineVisible { get; set; }

        public bool IsVerticalLineVisible { get; set; }

        public string LowFrequency { get; set; }

        public string HighFrequency { get; set; }

        public void SetAxes()
        {
            if (GraphType == FourierGui.GraphType.Sample)
            {
                Maximum = "1.0";
                Center = "0";
                Minimum = "-1.0";
                IsCenterLineVisible = true;

                IsVerticalLineVisible = false;
                LowFrequency = string.Empty;
                HighFrequency = string.Empty;
            }
            else if (GraphType == FourierGui.GraphType.Fourier)
            {
                Maximum = string.Empty;
                Center = string.Empty;
                Minimum = string.Empty;
                IsCenterLineVisible = false;

                LowFrequency = "0 Hz";
                HighFrequency = MaximumFrequency + " Hz";
                IsVerticalLineVisible = true;
            }

            OnPropertyChanged("IsCenterLineVisible");
            OnPropertyChanged("IsVerticalLineVisible");
            OnPropertyChanged("Maximum");
            OnPropertyChanged("Center");
            OnPropertyChanged("Minimum");
            OnPropertyChanged("LowFrequency");
            OnPropertyChanged("HighFrequency");
        }

        public unsafe void Draw(uint[] energyLevels)
        {
            source.Lock();
            source.Clear();

            if (energyLevels != null)
            {
                double widthScaleFactor = (double)WidthInPixels / energyLevels.Length;

                double heightScaleFactor = 1;

                if (GraphType == FourierGui.GraphType.Sample)
                {
                    heightScaleFactor = (double)HeightInPixels / MaximumSample;
                }
                else if (GraphType == FourierGui.GraphType.Fourier)
                {
                    heightScaleFactor = (double)HeightInPixels / energyLevels.Skip(1).Max();
                }

                for (int i = 0; i < energyLevels.Length - 1; i++)
                {
                    source.DrawLine((int)(i * widthScaleFactor),
                                    (int)(HeightInPixels - energyLevels[i] * heightScaleFactor),
                                    (int)((i + 1) * widthScaleFactor),
                                    (int)(HeightInPixels - energyLevels[i + 1] * heightScaleFactor),
                                    Colors.Black);
                }
            }

            source.AddDirtyRect(new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight));
            source.Unlock();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changedEvent = this.PropertyChanged;
            if (changedEvent != null)
            {
                changedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
