using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScreenRecorder
{

    public partial class Main : Window
    {

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Globals.contents = File.ReadAllText(Globals.jsonPath);
            Globals.Settings = JsonConvert.DeserializeObject<AppSettings>(Globals.contents);
            UpdateFields();
        }

        // ################################################################################################################
        // ################################################################################################################
        // ################################################################################################################
        //
        // Event Handling Methods 
        //
        // ################################################################################################################
        // ################################################################################################################
        // ################################################################################################################

        private void BtnIdentify_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= Globals.MonitorCount; i++)
            {
                GetMonitorResolution(i);
                // Identify identifier = new Identify(i);
                // identifier.Show();
            }
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.isRecording)
            {
                if (RecordingLocation.Text != "No Location Selected" && RecordingLocation.Text != "" && RecordingLocation.Text != null && Directory.Exists(RecordingLocation.Text))
                {
                    Globals.isRecording = true;
                    Globals.filename = "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".avi";
                    Globals.Rec = new Recorder(new RecorderParams(RecordingLocation.Text + Globals.filename, Globals.FrameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, Globals.Quality, Globals.MonitorCount));
                    UpdateJsonFile();
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
            if (Globals.isRecording)
            {
                if (Globals.Rec != null)
                {
                    Globals.Rec.Dispose();
                }
                Globals.isRecording = false;
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
                Globals.Settings.RecordingLocation = folderObj.SelectedPath;
            }
        }

        private void MonitorCount_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                Globals.MonitorCount = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.Settings.MonitorCount = Globals.MonitorCount;
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
                Globals.FrameRate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.Settings.FrameRate = Globals.FrameRate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void QualitySelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                Globals.Quality = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.Settings.Quality = Globals.Quality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // ################################################################################################################
        // ################################################################################################################
        // ################################################################################################################
        //
        // Application Methods 
        //
        // ################################################################################################################
        // ################################################################################################################
        // ################################################################################################################

        public void AddMonitors()
        {

            MonitorGrid.Children.Clear();

            RowDefinition row = new RowDefinition();
            MonitorGrid.RowDefinitions.Add(row);

            AddMonitorHeader("Monitor", 70, 0);
            AddMonitorHeader("Resolution", 180, 1);
            AddMonitorHeader("Position", 85, 2);
            AddMonitorHeader("Capture", 60, 3);

            for (int i = 1; i <= Globals.MonitorCount; i++)
            {
                row = new RowDefinition();
                System.Windows.Controls.Label name = new System.Windows.Controls.Label();

                row.Height = new GridLength(22);
                MonitorGrid.RowDefinitions.Add(row);

                AddMonitorName(i);
                AddMonitorResolution(i);
                AddMonitorRecord(i);
            }

            if (Globals.MonitorCount == 1)
            {
                AddMonitorPosition(Globals.positions1);
            }
            else if (Globals.MonitorCount == 2)
            {
                AddMonitorPosition(Globals.positions2);
            }
            else
            {
                AddMonitorPosition(Globals.positions3);
            }

        }

        public void AddMonitorHeader(string title, int width, int columnIndex)
        {
            ColumnDefinition column = new ColumnDefinition();
            System.Windows.Controls.Label name = new System.Windows.Controls.Label();

            column.Width = new GridLength(width);
            MonitorGrid.ColumnDefinitions.Add(column);

            name.Content = title;
            name.Padding = new Thickness(0);
            name.Margin = new Thickness(0);
            name.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            name.VerticalContentAlignment = VerticalAlignment.Center;
            name.BorderBrush = Brushes.Black;
            name.BorderThickness = new Thickness(1);
            name.Background = Brushes.Gray;
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, columnIndex);
            MonitorGrid.Children.Add(name);
        }

        public void AddMonitorName(int rowIndex)
        {
            System.Windows.Controls.Label name = new System.Windows.Controls.Label
            {
                Content = rowIndex.ToString(),
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(.5)
            };
            Grid.SetRow(name, rowIndex);
            Grid.SetColumn(name, 0);
            MonitorGrid.Children.Add(name);
        }

        public void AddMonitorPosition(string[] positions)
        {
            for (int i = 1; i <= Globals.MonitorCount; i++)
            {
                System.Windows.Controls.Label position = new System.Windows.Controls.Label
                {
                    Content = positions[i - 1],
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(.5)
                };
                Grid.SetRow(position, i);
                Grid.SetColumn(position, 2);
                MonitorGrid.Children.Add(position);
            }
        }

        public void AddMonitorRecord(int rowIndex)
        {
            System.Windows.Controls.RadioButton selected = new System.Windows.Controls.RadioButton
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(selected, rowIndex);
            Grid.SetColumn(selected, 3);
            
            MonitorGrid.Children.Add(selected);
        }

        public void AddMonitorResolution(int rowIndex)
        {
            System.Windows.Controls.ComboBox resolution = new System.Windows.Controls.ComboBox
            {
                SelectedIndex = 3,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            resolution.Items.Add("1280 x 720");
            resolution.Items.Add("1440 x 900");
            resolution.Items.Add("1600 x 900");
            resolution.Items.Add("1920 x 1080");
            resolution.Items.Add("2048 x 1080");
            resolution.Items.Add("2560 x 1440");
            resolution.Items.Add("3840 x 2160");
            Grid.SetRow(resolution, rowIndex);
            Grid.SetColumn(resolution, 1);
            MonitorGrid.Children.Add(resolution);
        }

        public UIElement GetGridElement(Grid g, int r, int c)
        {
            foreach (UIElement e in MonitorGrid.Children)
            {
                if (e.GetType() == typeof(System.Windows.Controls.ComboBox) && Grid.GetRow(e) == r && Grid.GetColumn(e) == c)
                {
                    return e;
                }   
            }
            return null;
        }

        public void GetMonitorResolution(int index)
        {
            UIElement combo = GetGridElement(MonitorGrid, index, 1);
            Debug.WriteLine(combo.GetType());
        }

        public void UpdateFields()
        {
            Globals.MonitorCount = Globals.Settings.MonitorCount;
            Globals.FrameRate = Globals.Settings.FrameRate;
            Globals.Quality = Globals.Settings.Quality;

            RecordingLocation.Text = Globals.Settings.RecordingLocation;
            MonitorCount.SelectedIndex = Globals.MonitorCount - 1;
            FrameRateSelection.SelectedItem = Globals.FrameRate;
            QualitySelection.SelectedItem = Globals.Quality;

            AddMonitors();
        }

        public void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Globals.Settings);
            File.WriteAllText(Globals.jsonPath, updatedFile);
        }
    }
}
