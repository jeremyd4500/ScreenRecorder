using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace ScreenRecorder
{

    public partial class Main : Window
    {
        private AppMethods methods;
        private string contents;
        private string jsonPath = "../../Resources/AppSettings.json";

        public string filename;
        public bool isRecording = false;
        public Recorder rec;

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            contents = File.ReadAllText(jsonPath);
            Globals.settings = JsonConvert.DeserializeObject<AppSettings>(contents);

            methods = new AppMethods();
            methods.UpdateFields();
        }

        private void BtnIdentify_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Globals.monitorCount; i++)
            {
                // Identify identifier = new Identify(i);
                // identifier.Show();
            }
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!isRecording)
            {
                if (RecordingLocation.Text != "No Location Selected" && RecordingLocation.Text != "" && RecordingLocation.Text != null && Directory.Exists(RecordingLocation.Text))
                {
                    isRecording = true;
                    filename = "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".avi";
                    rec = new Recorder(new RecorderParams(RecordingLocation.Text + filename, Globals.frameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, Globals.quality, Globals.monitorCount));
                    methods.UpdateJsonFile();
                } else
                {
                    System.Windows.MessageBox.Show("That is not a valid recording path!");
                }
            } else
            {
                System.Windows.MessageBox.Show("You are already recording!");
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isRecording)
            {
                if (rec != null)
                {
                    rec.Dispose();
                }
                isRecording = false;
            } else
            {
                System.Windows.MessageBox.Show("You are not currently recording!");
            }
        }

        private void BtnRecordingLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderObj = new FolderBrowserDialog();
            folderObj.ShowDialog();
            if (folderObj.SelectedPath != null && folderObj.SelectedPath != "")
            {
                RecordingLocation.Text = folderObj.SelectedPath;
                Globals.settings.RecordingLocation = folderObj.SelectedPath;
            }
        }

        private void MonitorCount_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                monitorCount = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.settings.MonitorCount = monitorCount;
                methods.AddMonitors();
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
        }

        private void FrameRateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            { 
                frameRate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.settings.FrameRate = frameRate;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
}

        private void QualitySelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                quality = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.settings.Quality = quality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
