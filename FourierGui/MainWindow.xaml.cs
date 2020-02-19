using System.Windows;

namespace FourierGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainWindowViewModel(256);
            viewModel.UpdateGraphs += viewModel_UpdateGraphs;

            this.DataContext = viewModel;
        }

        private void viewModel_UpdateGraphs(object sender, System.EventArgs e)
        {
            this.sampleGraph.MaximumSample = 1u << viewModel.FileInfo.SampleBits;
            this.fourierGraph.MaximumFrequency = (uint)(viewModel.FileInfo.Frequency / 2);
            this.sampleGraph.Points = viewModel.CurrentSamples;
            this.fourierGraph.Points = viewModel.CurrentFourier;
        }
    }
}
