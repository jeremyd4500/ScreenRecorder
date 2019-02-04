using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

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
            DisableFields();
            IdentifyMonitors();
            EnableFields();
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.isRecording)
            {
                if (RecordingLocation.Text != "No Location Selected" && RecordingLocation.Text != "" && RecordingLocation.Text != null && Directory.Exists(RecordingLocation.Text))
                {
                    Globals.isRecording = true;
                    // Globals.filename = "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".avi";
                    // Globals.Rec = new Recorder(new RecorderParams(RecordingLocation.Text + Globals.filename, Globals.FrameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, Globals.Quality, Globals.Settings.MonitorCount));
                    UpdateJsonMonitors();
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
                Globals.Settings.MonitorCount = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                AddMonitors();
            } catch(Exception ex)
            {
                Debug.WriteLine("\nMonitor Count selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
            }
        }

        private void FrameRateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                Globals.Settings.FrameRate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.Settings.FrameRate = Globals.Settings.FrameRate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nFramerate selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
            }
        }

        private void QualitySelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                Globals.Settings.Quality = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Globals.Settings.Quality = Globals.Settings.Quality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nQuality selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
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

            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                row = new RowDefinition();
                row.Height = new GridLength(22);
                MonitorGrid.RowDefinitions.Add(row);
                
                try { UnregisterName("MonitorResolution" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }
                try { UnregisterName("MonitorPosition" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }
                try { UnregisterName("MonitorRecord" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }

                AddMonitorName(i);
                AddMonitorResolution(i);
                AddMonitorRecord(i);
            }

            if (Globals.Settings.MonitorCount == 1)
            {
                AddMonitorPosition(Globals.positions1);
            }
            else if (Globals.Settings.MonitorCount == 2)
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
            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                System.Windows.Controls.Label position = new System.Windows.Controls.Label
                {
                    Content = positions[i - 1],
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(.5),
                    
                };
                Grid.SetRow(position, i);
                Grid.SetColumn(position, 2);
                RegisterName("MonitorPosition" + i, position);
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
            RegisterName("MonitorRecord" + rowIndex, selected);
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
            RegisterName("MonitorResolution" + rowIndex, resolution);
            MonitorGrid.Children.Add(resolution);
        }

        public void AddMonitorToJson(string position, int width, int height, bool selected, int comboboxindex)
        {
            Monitor newMonitor = new Monitor() {
                Position = position,
                Width = width,
                Height = height,
                Selected = selected,
                ComboBoxIndex = comboboxindex
            };
            Globals.Settings.Monitors.Add(newMonitor);
        }

        public void IdentifyMonitors()
        {
            if (Globals.Settings.MonitorCount == 1)
            {
                Identify identify1 = new Identify(1, 0);
                identify1.Show();
                WaitTime(2);
                identify1.Close();

            } else if (Globals.Settings.MonitorCount == 2)
            {
                Identify identify1 = new Identify(1, 0);
                identify1.Show();
                Identify identify2 = new Identify(2, Globals.Settings.Monitors[0].Width);
                identify2.Show();
                WaitTime(2);
                identify1.Close();
                identify2.Close();
            } else
            {
                Identify identify1 = new Identify(1, (Globals.Settings.Monitors[0].Width * -1));
                identify1.Show();
                Identify identify2 = new Identify(2, 0);
                identify2.Show();
                Identify identify3 = new Identify(3, Globals.Settings.Monitors[1].Width);
                identify3.Show();
                WaitTime(2);
                identify1.Close();
                identify2.Close();
                identify3.Close();
            }
        }

        public void DisableFields()
        {
            SelectFolder.IsEnabled = false;
            MonitorCount.IsEnabled = false;
            FrameRateSelection.IsEnabled = false;
            QualitySelection.IsEnabled = false;
            StartRecording.IsEnabled = false;
            StopRecording.IsEnabled = false;
            IdentifyScreens.IsEnabled = false;

            foreach(UIElement element in MonitorGrid.Children)
            {
                element.IsEnabled = false;
            }

        }

        public void EnableFields()
        {
            SelectFolder.IsEnabled = true;
            MonitorCount.IsEnabled = true;
            FrameRateSelection.IsEnabled = true;
            QualitySelection.IsEnabled = true;
            StartRecording.IsEnabled = true;
            StopRecording.IsEnabled = true;
            IdentifyScreens.IsEnabled = true;

            foreach (UIElement element in MonitorGrid.Children)
            {
                element.IsEnabled = true;
            }
        }

        public string GetMonitorPosition(int index)
        {
            try
            {
                System.Windows.Controls.Label position = (System.Windows.Controls.Label)FindName("MonitorPosition" + index);
                return position.Content.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a Label that did not exist.\nError Message: " + ex.Message);
            }
            return null;
        }

        public bool GetMonitorRecord(int index)
        {
            try
            {
                System.Windows.Controls.RadioButton selected = (System.Windows.Controls.RadioButton)FindName("MonitorRecord" + index);
                return selected.IsChecked.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a ComboBox that did not exist.\nError Message: " + ex.Message);
            }
            return false;
        }

        public string[] GetMonitorResolution(int index)
        {
            try
            {
                System.Windows.Controls.ComboBox resBox = (System.Windows.Controls.ComboBox)FindName("MonitorResolution"+index);
                return resBox.Text.Split(null);
            } catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a ComboBox that did not exist.\nError Message: " + ex.Message);
            }
            return null;
        }

        public int GetMonitorSelectedIndex(int index)
        {
            try
            {
                System.Windows.Controls.ComboBox comboIndex = (System.Windows.Controls.ComboBox)FindName("MonitorResolution" + index);
                return comboIndex.SelectedIndex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a ComboBox that did not exist.\nError Message: " + ex.Message);
            }
            return -1;
        }

        public void UpdateFields()
        {
            RecordingLocation.Text = Globals.Settings.RecordingLocation;
            MonitorCount.SelectedIndex = Globals.Settings.MonitorCount - 1;

            int index = 0;
            foreach (ComboBoxItem x in FrameRateSelection.Items)
            {
                if (x.Content.ToString() == Globals.Settings.FrameRate.ToString())
                {
                    FrameRateSelection.SelectedIndex = index;
                    break;
                } else
                {
                    index++;
                }
            }

            index = 0;
            foreach (ComboBoxItem x in QualitySelection.Items)
            {
                if (x.Content.ToString() == Globals.Settings.Quality.ToString())
                {
                    QualitySelection.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }

            AddMonitors();
            UpdateMonitorFields();
        }

        public void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Globals.Settings);
            File.WriteAllText(Globals.jsonPath, updatedFile);
        }

        public void UpdateJsonMonitors()
        {
            Globals.Settings.Monitors.Clear();
            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                string position = GetMonitorPosition(i);

                int width, height;
                string[] resXY = GetMonitorResolution(i);
                if (resXY == null)
                {
                    width = 1920;
                    height = 1080;
                } else
                {
                    width = int.Parse(resXY[0]);
                    height = int.Parse(resXY[2]);
                }

                bool selected = GetMonitorRecord(i);

                int index = GetMonitorSelectedIndex(i);
                if (position == null || index == -1)
                {
                    index = 3; // Set to 1920x1080
                    width = 1920;
                    height = 1080;
                }

                AddMonitorToJson(position, width, height, selected, index);
            }

        }

        public void UpdateMonitorFields()
        {
            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                try
                {
                    System.Windows.Controls.ComboBox resBox = (System.Windows.Controls.ComboBox)FindName("MonitorResolution" + i);
                    System.Windows.Controls.RadioButton selected = (System.Windows.Controls.RadioButton)FindName("MonitorRecord" + i);

                    resBox.SelectedIndex = Globals.Settings.Monitors[i - 1].ComboBoxIndex;
                    selected.IsChecked = Globals.Settings.Monitors[i - 1].Selected;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("\nAttempted to find a ComboBox that did not exist.\nError Message: " + ex.Message);
                }

            }
        }

        private void WaitTime(int seconds)
        {
            if (seconds < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < _desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
