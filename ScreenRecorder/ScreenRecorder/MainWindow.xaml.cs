using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace ScreenRecorder
{

    public static class Globals
    {
        public static int startX { get; set; }
        public static int startY { get; set; }
    }

    public partial class Main : Window
    {

        private RootData settings;
        private string contents;
        private string jsonPath = "../../Resources/AppSettings.json";

        public string filename;
        public int monitorIndex;
        public int frameRate;
        public int quality;
        public bool isRecording = false;
        public Recorder rec;

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            contents = File.ReadAllText(jsonPath);
            settings = JsonConvert.DeserializeObject<RootData>(contents);

            UpdateFields(settings);
        }

        private void AddMonitors()
        {
            List<string> screens = new List<string>();
            for(int i = 0; i < Screen.AllScreens.Length; i++)
            {
                screens.Add("Display " + i + ": {Width=" + Screen.AllScreens[i].Bounds.Width + 
                    ", Height=" + Screen.AllScreens[i].Bounds.Height + ", Primary Screen=" + Screen.AllScreens[i].Primary + "}");
            }
            MonitorSelection.ItemsSource = screens;
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!isRecording)
            {
                if (RecordingLocation.Text != "No Location Selected" && RecordingLocation.Text != "" && RecordingLocation.Text != null)
                {
                    isRecording = true;
                    filename = "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".avi";
                    rec = new Recorder(new RecorderParams(RecordingLocation.Text + filename, frameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, quality, monitorIndex));
                    UpdateJsonFile(settings);
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
                settings.AppSettings[0].RecordingLocation = folderObj.SelectedPath;
            }
        }

        private void MonitorSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                monitorIndex = MonitorSelection.SelectedIndex;
                settings.AppSettings[0].SelectedMonitor = monitorIndex;
                Globals.startX = Screen.AllScreens[monitorIndex].Bounds.Left;
                Globals.startY = Screen.AllScreens[monitorIndex].Bounds.Top;
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
                settings.AppSettings[0].SelectedFrameRate = frameRate;
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
                settings.AppSettings[0].SelectedQuality = quality;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
}

        private void UpdateFields(RootData settings)
        {
            RecordingLocation.Text = settings.AppSettings[0].RecordingLocation;

            AddMonitors();
            monitorIndex = settings.AppSettings[0].SelectedMonitor;
            MonitorSelection.SelectedIndex = monitorIndex;
            Globals.startX = Screen.AllScreens[monitorIndex].Bounds.Left;
            Globals.startY = Screen.AllScreens[monitorIndex].Bounds.Top;

            frameRate = settings.AppSettings[0].SelectedFrameRate;
            FrameRateSelection.SelectedItem = frameRate;

            quality = settings.AppSettings[0].SelectedQuality;
            QualitySelection.SelectedItem = quality;
        }

        private void UpdateJsonFile(RootData settings)
        {
            string updatedFile = JsonConvert.SerializeObject(settings);
            File.WriteAllText(jsonPath, updatedFile);
        }
        
    }

    public partial class RootData
    {
        [JsonProperty("AppSettings")]
        public AppSettings[] AppSettings { get; set; }
    }

    public partial class AppSettings
    {
        [JsonProperty("Recording-Location")]
        public string RecordingLocation { get; set; }

        [JsonProperty("Selected-Monitor")]
        public int SelectedMonitor { get; set; }

        [JsonProperty("Selected-FrameRate")]
        public int SelectedFrameRate { get; set; }

        [JsonProperty("Selected-Quality")]
        public int SelectedQuality { get; set; }
    }
}
