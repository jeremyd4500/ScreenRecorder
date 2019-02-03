using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

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
        public int monitorCount;
        public int frameRate;
        public int quality;
        public int monitorPosition;
        public bool isRecording = false;
        public Recorder rec;

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            contents = File.ReadAllText(jsonPath);
            settings = JsonConvert.DeserializeObject<RootData>(contents);

            monitorPosition = 0;
            UpdateFields(settings);
        }

        private void AddMonitors()
        {
            monitorPosition = 0;
            MonitorGrid.Children.Clear();

            RowDefinition row = new RowDefinition();
            MonitorGrid.RowDefinitions.Add(row);

            AddMonitorHeader("Monitor", 70, 0);
            AddMonitorHeader("Resolution", 180, 1);
            AddMonitorHeader("Position", 85, 2);
            AddMonitorHeader("Record", 60, 3);

            for (int i = 1; i <= monitorCount; i++)
            {
                row = new RowDefinition();
                System.Windows.Controls.Label name = new System.Windows.Controls.Label();

                row.Height = new GridLength(22);
                MonitorGrid.RowDefinitions.Add(row);

                AddMonitorName(i);
                AddMonitorResolution(i);
                AddMonitorPosition(i);
                AddMonitorRecord(i);
            }
        }

        private void AddMonitorHeader(string title, int width, int columnIndex)
        {
            ColumnDefinition column = new ColumnDefinition();
            System.Windows.Controls.Label name = new System.Windows.Controls.Label();
            
            column.Width = new GridLength(width);
            MonitorGrid.ColumnDefinitions.Add(column);

            name.Content = title;
            name.Padding = new Thickness(0);
            name.Margin = new Thickness(0);
            name.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            name.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            name.BorderBrush = Brushes.Black;
            name.BorderThickness = new Thickness(1);
            name.Background = Brushes.Gray;
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, columnIndex);
            MonitorGrid.Children.Add(name);
        }

        private void AddMonitorName(int rowIndex)
        {
            System.Windows.Controls.Label name = new System.Windows.Controls.Label();
            name.Content = rowIndex.ToString();
            name.Padding = new Thickness(0);
            name.Margin = new Thickness(0);
            name.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            name.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            name.BorderBrush = Brushes.Black;
            name.BorderThickness = new Thickness(.5);
            Grid.SetRow(name, rowIndex);
            Grid.SetColumn(name, 0);
            MonitorGrid.Children.Add(name);
        }

        private void AddMonitorPosition(int rowIndex)
        {
            System.Windows.Controls.ComboBox resolution = new System.Windows.Controls.ComboBox();
            Grid.SetRow(resolution, rowIndex);
            Grid.SetColumn(resolution, 2);
            if (monitorCount == 1)
            {
                resolution.Items.Add("Center");
                resolution.SelectedIndex = monitorPosition;
            } else if (monitorCount == 2)
            {
                resolution.Items.Add("Left");
                resolution.Items.Add("Right");
                resolution.SelectedIndex = monitorPosition;
            } else
            {
                resolution.Items.Add("Left");
                resolution.Items.Add("Center");
                resolution.Items.Add("Right");
                resolution.SelectedIndex = monitorPosition;
            }
            monitorPosition++;
            resolution.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            resolution.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            MonitorGrid.Children.Add(resolution);
        }

        private void AddMonitorRecord(int rowIndex)
        {
            System.Windows.Controls.RadioButton selected = new System.Windows.Controls.RadioButton();
            Grid.SetRow(selected, rowIndex);
            Grid.SetColumn(selected, 3);
            selected.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            selected.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            MonitorGrid.Children.Add(selected);
        }

        private void AddMonitorResolution(int rowIndex)
        {
            System.Windows.Controls.ComboBox resolution = new System.Windows.Controls.ComboBox();
            Grid.SetRow(resolution, rowIndex);
            Grid.SetColumn(resolution, 1);
            resolution.Items.Add("1280 x 720");
            resolution.Items.Add("1440 x 900");
            resolution.Items.Add("1600 x 900");
            resolution.Items.Add("1920 x 1080");
            resolution.Items.Add("2048 x 1080");
            resolution.Items.Add("2560 x 1440");
            resolution.Items.Add("3840 x 2160");
            resolution.SelectedIndex = 3;
            resolution.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            resolution.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            MonitorGrid.Children.Add(resolution);
        }

        private void BtnIdentify_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < monitorCount; i++)
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
                    rec = new Recorder(new RecorderParams(RecordingLocation.Text + filename, frameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, quality, monitorCount));
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

        private void MonitorCount_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                monitorCount = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                settings.AppSettings[0].MonitorCount = monitorCount;
                AddMonitors();
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
                settings.AppSettings[0].FrameRate = frameRate;
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
                settings.AppSettings[0].Quality = quality;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
}

        private void UpdateFields(RootData settings)
        {
            RecordingLocation.Text = settings.AppSettings[0].RecordingLocation;

            AddMonitors();
            monitorCount = settings.AppSettings[0].MonitorCount;
            MonitorCount.SelectedIndex = monitorCount;

            frameRate = settings.AppSettings[0].FrameRate;
            FrameRateSelection.SelectedItem = frameRate;

            quality = settings.AppSettings[0].Quality;
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

        [JsonProperty("MonitorCount")]
        public int MonitorCount { get; set; }

        [JsonProperty("FrameRate")]
        public int FrameRate { get; set; }

        [JsonProperty("Quality")]
        public int Quality { get; set; }
    }
}
