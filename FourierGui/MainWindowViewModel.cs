using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using ComputeFourier;
using Microsoft.Win32;
using Utility;
using WpfCommon.Input;

namespace FourierGui
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool canComputeFourier = false;

        private Stream audioStream;

        private ICommand openAudioFileCommand;

        private Utility.FileInfo fileInfo;

        private int firstSampleIndex;

        private int windowSize;

        private uint[] currentFourier;

        private uint[] currentSamples;

        public MainWindowViewModel(int windowSize)
        {
            this.windowSize = windowSize;
        }

        public int WindowSize
        {
            get
            {
                return windowSize;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler UpdateGraphs;

        public bool CanComputeFourier
        {
            get { return canComputeFourier; }
        }

        public int MaximumSampleIndex
        {
            get { return fileInfo.SampleCount <= windowSize ? 0 : fileInfo.SampleCount - windowSize; }
        }

        public ICommand OpenAudioFileCommand
        {
            get
            {
                if (openAudioFileCommand == null)
                {
                    openAudioFileCommand = new RelayCommand(param => OpenAudioFile());
                }

                return openAudioFileCommand;
            }

        }

        public Utility.FileInfo FileInfo
        {
            get { return fileInfo; }
        }

        private void OpenAudioFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                audioStream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                canComputeFourier = true;

                fileInfo = FileHeaderUtil.GetHeaderInfo(audioStream);
                audioStream.Seek(0, SeekOrigin.Begin);

                FirstSampleIndex = 0;
                OnPropertyChanged("MaximumSampleIndex");
                OnPropertyChanged("CanComputeFourier");

            }
        }

        public int FirstSampleIndex
        {
            get
            {
                return firstSampleIndex;
            }

            set
            {
                firstSampleIndex = value;

                currentSamples = Fourier.ReadSamples(audioStream, firstSampleIndex, windowSize, fileInfo);
                currentFourier = ComputeFourier(firstSampleIndex);

                OnPropertyChanged("FirstSampleIndex");
                OnUpdateGraphs();
            }
        }

        protected void OnUpdateGraphs()
        {
            var updateEvent = UpdateGraphs;
            if (updateEvent != null)
            {
                updateEvent(this, new EventArgs());
            }
        }

        public uint[] CurrentSamples
        {
            get { return currentSamples; }
        }

        public uint[] CurrentFourier
        {
            get { return currentFourier; }
        }

        public uint[] ComputeFourier(int start)
        {
            return Fourier.CalculateInt(currentSamples, windowSize);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var changedEvent = this.PropertyChanged;
            if (changedEvent != null)
            {
                changedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (audioStream != null)
                {
                    audioStream.Dispose();
                }
            }
        }
    }
}
