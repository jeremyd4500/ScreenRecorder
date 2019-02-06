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

            //foreach (Screen screen in Screen.AllScreens)
            //{
            //    Debug.WriteLine(screen);
            //}

            // Default Json File
            // {"Recording-Location":"Select A Folder","MonitorCount":1,"Monitors":[],"FrameRate":60,"Quality":100}
            Globals.Contents = File.ReadAllText(Globals.jsonPath);
            Globals.Settings = JsonConvert.DeserializeObject<AppSettings>(Globals.Contents);
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
            DisableFields(true);
            IdentifyMonitors();
            EnableFields();
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Globals.isRecording)
            {
                if (Directory.Exists(RecordingLocation.Text))
                {
                    UpdateJsonMonitors();
                    UpdateMonitorPositions();
                    if (VerifySingleMonitorSelected())
                    {
                        Globals.isRecording = true;
                        Globals.FileName = "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".avi";
                        for (int i = 0; i < Globals.Settings.MonitorCount; i++)
                        {
                            if (Globals.Settings.Monitors[i].Selected)
                            {
                                DisableFields(false);
                                Globals.StartX = GetSelectedMonitorStartPosition(i);
                                Globals.SizeW = Globals.Settings.Monitors[i].Width;
                                Globals.SizeH = Globals.Settings.Monitors[i].Height;
                                Debug.WriteLine(Globals.StartX);
                                Debug.WriteLine(Globals.SizeW);
                                Debug.WriteLine(Globals.SizeH);
                                Globals.Rec = new Recorder(new RecorderParams(RecordingLocation.Text + Globals.FileName, Globals.Settings.FrameRate, SharpAvi.KnownFourCCs.Codecs.MotionJpeg, Globals.Settings.Quality));
                                UpdateJsonFile();
                                break;
                            } 
                        }
                    } else
                    {
                        System.Windows.MessageBox.Show("You must have no more and no less than one monitor selected!");
                    }
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
                EnableFields();
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

        private void AddMonitors()
        {

            MonitorGrid.Children.Clear();

            RowDefinition row = new RowDefinition
            {
                Height = new GridLength(22)
            };
            MonitorGrid.RowDefinitions.Add(row);

            AddMonitorHeader("Monitor", 70, 0);
            AddMonitorHeader("Resolution", 140, 1);
            AddMonitorHeader("Position", 75, 2);
            AddMonitorHeader("Primary", 55, 3);
            AddMonitorHeader("Capture", 55, 4);

            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                row = new RowDefinition
                {
                    Height = new GridLength(22)
                };
                MonitorGrid.RowDefinitions.Add(row);
                
                try { UnregisterName("MonitorResolution" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }
                try { UnregisterName("MonitorPosition" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }
                try { UnregisterName("MonitorPrimary" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }
                try { UnregisterName("MonitorRecord" + i); } catch (Exception ex) { Debug.WriteLine("\nAttempted to unregister a resource name that did not exist.\nError Message: " + ex.Message); }

                AddMonitorName(i);
                AddMonitorResolution(i);
                AddMonitorPrimary(i);
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

        private void AddMonitorHeader(string title, int width, int columnIndex)
        {
            ColumnDefinition column = new ColumnDefinition
            {
                Width = new GridLength(width)
            };
            MonitorGrid.ColumnDefinitions.Add(column);

            System.Windows.Controls.Label name = new System.Windows.Controls.Label
            {
                Content = title,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.Gray
            };
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, columnIndex);
            MonitorGrid.Children.Add(name);
        }

        private void AddMonitorName(int rowIndex)
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

        private void AddMonitorPosition(string[] positions)
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

        private void AddMonitorPrimary(int rowIndex)
        {
            System.Windows.Controls.CheckBox primary = new System.Windows.Controls.CheckBox
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsEnabled = false
            };
            Grid.SetRow(primary, rowIndex);
            Grid.SetColumn(primary, 3);
            RegisterName("MonitorPrimary" + rowIndex, primary);
            MonitorGrid.Children.Add(primary);
        }

        private void AddMonitorRecord(int rowIndex)
        {
            System.Windows.Controls.CheckBox selected = new System.Windows.Controls.CheckBox
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(selected, rowIndex);
            Grid.SetColumn(selected, 4);
            RegisterName("MonitorRecord" + rowIndex, selected);
            MonitorGrid.Children.Add(selected);
        }

        private void AddMonitorResolution(int rowIndex)
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

        private void AddMonitorToJson(string position, int width, int height, bool primary, bool selected, int comboboxindex)
        {
            Monitor newMonitor = new Monitor() {
                Position = position,
                Width = width,
                Height = height,
                Primary = primary,
                Selected = selected,
                ComboBoxIndex = comboboxindex
            };
            Globals.Settings.Monitors.Add(newMonitor);
        }

        private void DisableFields(bool all)
        {
            SelectFolder.IsEnabled = false;
            FrameRateSelection.IsEnabled = false;
            QualitySelection.IsEnabled = false;
            StartRecording.IsEnabled = false;
            if (all)
            {
                StopRecording.IsEnabled = false;
            }
            IdentifyScreens.IsEnabled = false;

            foreach(UIElement element in MonitorGrid.Children)
            {
                element.IsEnabled = false;
            }
        }

        private void EnableFields()
        {
            SelectFolder.IsEnabled = true;
            FrameRateSelection.IsEnabled = true;
            QualitySelection.IsEnabled = true;
            StartRecording.IsEnabled = true;
            StopRecording.IsEnabled = true;
            IdentifyScreens.IsEnabled = true;

            foreach (UIElement element in MonitorGrid.Children)
            {
                if (Grid.GetColumn(element) != 3 || Grid.GetRow(element) == 0)
                {
                    element.IsEnabled = true;
                }
            }
        }

        private string GetMonitorPosition(int index)
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

        private bool GetMonitorPrimary(int index)
        {
            try
            {
                System.Windows.Controls.CheckBox selected = (System.Windows.Controls.CheckBox)FindName("MonitorPrimary" + index);
                return selected.IsChecked.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a CheckBox that did not exist.\nError Message: " + ex.Message);
            }
            return false;
        }

        private bool GetMonitorRecord(int index)
        {
            try
            {
                System.Windows.Controls.CheckBox selected = (System.Windows.Controls.CheckBox)FindName("MonitorRecord" + index);
                return selected.IsChecked.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nAttempted to find a CheckBox that did not exist.\nError Message: " + ex.Message);
            }
            return false;
        }

        private string[] GetMonitorResolution(int index)
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

        private int GetMonitorSelectedIndex(int index)
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

        private int GetSelectedMonitorStartPosition(int index)
        {
            if (index == 0)
            {
                return Globals.Monitor1X;
            } else if (index == 1)
            {
                return Globals.Monitor2X;
            } else
            {
                return Globals.Monitor3X;
            }
        }

        private void IdentifyMonitors()
        {
            if (Globals.Settings.MonitorCount == 1)
            {
                Identify identify1 = new Identify(1, 0);
                identify1.Show();
                WaitTime(2);
                identify1.Close();

            } else if (Globals.Settings.MonitorCount == 2)
            {
                if (Screen.AllScreens[0].Primary)
                {
                    Globals.Monitor1X = 0;
                    Globals.Monitor2X = Globals.Settings.Monitors[0].Width;
                } else
                {
                    Globals.Monitor1X = Globals.Settings.Monitors[0].Width * -1;
                    Globals.Monitor2X = 0;
                }
                Identify identify1 = new Identify(1, Globals.Monitor1X);
                identify1.Show();
                Identify identify2 = new Identify(2, Globals.Monitor2X);
                identify2.Show();
                WaitTime(2);
                identify1.Close();
                identify2.Close();
            } else
            {
                if (Screen.AllScreens[0].Primary)
                {
                    Globals.Monitor1X = 0;
                    Globals.Monitor2X = Globals.Settings.Monitors[0].Width;
                    Globals.Monitor3X = Globals.Settings.Monitors[0].Width + Globals.Settings.Monitors[1].Width;
                }
                else if (Screen.AllScreens[1].Primary)
                {
                    Globals.Monitor1X = Globals.Settings.Monitors[0].Width * -1;
                    Globals.Monitor2X = 0;
                    Globals.Monitor3X = Globals.Settings.Monitors[0].Width;
                } else
                {
                    Globals.Monitor1X = (Globals.Settings.Monitors[0].Width + Globals.Settings.Monitors[1].Width) * -1;
                    Globals.Monitor2X = Globals.Settings.Monitors[1].Width * -1;
                    Globals.Monitor3X = 0;
                }
                Identify identify1 = new Identify(1, Globals.Monitor1X);
                identify1.Show();
                Identify identify2 = new Identify(2, Globals.Monitor2X);
                identify2.Show();
                Identify identify3 = new Identify(3, Globals.Monitor3X);
                identify3.Show();
                WaitTime(2);
                identify1.Close();
                identify2.Close();
                identify3.Close();
            }
        }

        private void UpdateFields()
        {
            RecordingLocation.Text = Globals.Settings.RecordingLocation;

            if (Screen.AllScreens.Length > 3)
            {
                System.Windows.MessageBox.Show("This application currently does not support systems with more than 3 monitors. I'm sorry for the inconvenience.");
                System.Windows.Application.Current.Shutdown();
            }

            Globals.Settings.MonitorCount = Screen.AllScreens.Length;
            MonitorCount.Content = Globals.Settings.MonitorCount;

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
            UpdateJsonMonitors();
            UpdateMonitorPositions();
        }

        private void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Globals.Settings);
            File.WriteAllText(Globals.jsonPath, updatedFile);
        }

        private void UpdateJsonMonitors()
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

                bool primary = GetMonitorPrimary(i);
                bool selected = GetMonitorRecord(i);

                int index = GetMonitorSelectedIndex(i);
                if (position == null || index == -1)
                {
                    index = 3; // Set to 1920x1080
                    width = 1920;
                    height = 1080;
                }

                AddMonitorToJson(position, width, height, primary, selected, index);
            }

        }

        private void UpdateMonitorFields()
        {
            for (int i = 1; i <= Globals.Settings.MonitorCount; i++)
            {
                try
                {
                    System.Windows.Controls.ComboBox resBox = (System.Windows.Controls.ComboBox)FindName("MonitorResolution" + i);
                    System.Windows.Controls.CheckBox primary = (System.Windows.Controls.CheckBox)FindName("MonitorPrimary" + i);
                    System.Windows.Controls.CheckBox selected = (System.Windows.Controls.CheckBox)FindName("MonitorRecord" + i);

                    if (Globals.Settings.Monitors.Count == 0)
                    {
                        resBox.SelectedIndex = 3;
                    } else
                    {
                        resBox.SelectedIndex = Globals.Settings.Monitors[i - 1].ComboBoxIndex;
                    }

                    if (Screen.AllScreens[i - 1].Primary){
                        primary.IsChecked = true;
                    } else
                    {
                        primary.IsChecked = false;
                    }

                    selected.IsChecked = Globals.Settings.Monitors[i - 1].Selected;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("\nAttempted to find UI elements that did not exist.\nError Message: " + ex.Message);
                }
            }
        }

        private void UpdateMonitorPositions()
        {
            if (Globals.Settings.MonitorCount == 1)
            {
                Globals.Monitor1X = 0;
                Globals.Monitor2X = -1;
                Globals.Monitor2X = -1;
            }
            else if (Globals.Settings.MonitorCount == 2)
            {
                if (Screen.AllScreens[0].Primary)
                {
                    Globals.Monitor1X = 0;
                    Globals.Monitor2X = Globals.Settings.Monitors[0].Width;
                    Globals.Monitor3X = -1;
                }
                else
                {
                    Globals.Monitor1X = Globals.Settings.Monitors[0].Width * -1;
                    Globals.Monitor2X = 0;
                    Globals.Monitor3X = -1;
                }
            }
            else
            {
                if (Screen.AllScreens[0].Primary)
                {
                    Globals.Monitor1X = 0;
                    Globals.Monitor2X = Globals.Settings.Monitors[0].Width;
                    Globals.Monitor3X = Globals.Settings.Monitors[0].Width + Globals.Settings.Monitors[1].Width;
                }
                else if (Screen.AllScreens[1].Primary)
                {
                    Globals.Monitor1X = Globals.Settings.Monitors[0].Width * -1;
                    Globals.Monitor2X = 0;
                    Globals.Monitor3X = Globals.Settings.Monitors[0].Width;
                }
                else
                {
                    Globals.Monitor1X = (Globals.Settings.Monitors[0].Width + Globals.Settings.Monitors[1].Width) * -1;
                    Globals.Monitor2X = Globals.Settings.Monitors[1].Width * -1;
                    Globals.Monitor3X = 0;
                }
            }
        }

        private bool VerifySingleMonitorSelected()
        {
            int selected = 0;
            for (int i = 0; i < Globals.Settings.MonitorCount; i++)
            {
                if (Globals.Settings.Monitors[i].Selected)
                {
                    selected++;
                }
            }
            return selected == 1;
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
