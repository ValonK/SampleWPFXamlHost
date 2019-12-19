using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;

namespace SampleWPFXamlHost
{
	public class MainViewModel : PropertyChangedAware
	{
        public MainViewModel()
        {
            GetUwpCaptureElement();
        }

        private MediaCapture _mediaCapture;

        public MediaCapture MediaCapture {
            get {
                if (_mediaCapture == null)
                    _mediaCapture = new MediaCapture();
                return _mediaCapture;
            }
            set {
                _mediaCapture = value;
                OnPropertyChanged(nameof(MediaCapture));
            }
        }


        public CaptureElement CaptureElement { get; set; }

        public WindowsXamlHost XamlHostCaptureElement { get; set; }

        /// <summary>
        /// Create / Host UWP CaptureElement
        /// </summary>
        private void GetUwpCaptureElement()
        {
            XamlHostCaptureElement = new WindowsXamlHost
            {
                InitialTypeName = "Windows.UI.Xaml.Controls.CaptureElement"
            };
            XamlHostCaptureElement.ChildChanged += XamlHost_ChildChangedAsync;
        }

        private async void XamlHost_ChildChangedAsync(object sender, EventArgs e)
        {
            var windowsXamlHost = (WindowsXamlHost)sender;

            var captureElement = (CaptureElement)windowsXamlHost.Child;
            CaptureElement = captureElement;
            CaptureElement.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;

            try
            {
                await StartPreviewAsync();
            }
            catch (Exception)
            {

            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                await MediaCapture.InitializeAsync();
            }
            catch (UnauthorizedAccessException ex)
            {
                //_logger.Info($"The app was denied access to the camera \n {ex}");
                return;
            }

            try
            {
                CaptureElement.Source = MediaCapture;
                await MediaCapture.StartPreviewAsync();
            }
            catch (System.IO.FileLoadException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

        }
    }

    public class PropertyChangedAware : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
