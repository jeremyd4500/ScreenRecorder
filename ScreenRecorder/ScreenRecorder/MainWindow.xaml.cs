using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using ScreenRecorderLib;

namespace ScreenRecorder
{

    public partial class Main : Window
    {

        private AppSettings Settings { get; set; }
        private Recorder Rec { get; set; } // https://github.com/sskodje/ScreenRecorderLib
        private Stream OutStream { get; set; }

        private bool isRecording = false;

        // Any CPU
        // private string jsonPath = "../../Resources/AppSettings.json";
        private string jsonPath = "Resources/AppSettings.json"; // 64bit
        private string Contents { get; set; }
        private string FileName { get; set; }

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Default Json File
            // {"Recording-Location":"Select Recording Location","ScreenWidth":1920,"ScreenHeight":1080,"FrameRate":60,"Quality":100}
            Contents = File.ReadAllText(jsonPath);
            Settings = JsonConvert.DeserializeObject<AppSettings>(Contents);
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
            Identify identifier = new Identify(1, Screen.PrimaryScreen.Bounds.Left);
            identifier.Show();
            WaitTime(2);
            identifier.Close();
            EnableFields();
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(RecordingLocation.Text);
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(RecordingLocation.Text))
            {
                isRecording = true;
                FileName = RecordingLocation.Text + "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".mp4";
                Record(FileName);
                DisableFields(false);
                UpdateJsonFile();
            } else
            {
                System.Windows.MessageBox.Show("That is not a valid recording path!");
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isRecording)
            {
                EnableFields();
                Rec.Stop();
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
                Settings.RecordingLocation = folderObj.SelectedPath;
            }
        }

        private void DrpFrameRateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Settings.FrameRate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nFramerate selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
            }
        }

        private void DrpQualitySelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                Settings.Quality = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                Settings.Quality = Settings.Quality;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nQuality selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
            }
        }

        private void DrpScreenResolution_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string[] newRes = ((e.AddedItems[0] as ComboBoxItem).Content as string).Split(null);
                Settings.ScreenWidth = int.Parse(newRes[0]);
                Settings.ScreenHeight = int.Parse(newRes[2]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\nScreen Resolution selection was changed. Attempted to parse the selected value.\nError Message: " + ex.Message);
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

        private void DisableFields(bool all)
        {
            SelectFolder.IsEnabled = false;
            OpenFolder.IsEnabled = false;
            ScreenResolution.IsEnabled = false;
            FrameRateSelection.IsEnabled = false;
            QualitySelection.IsEnabled = false;
            StartRecording.IsEnabled = false;
            if (all)
            {
                StopRecording.IsEnabled = false;
            }
            IdentifyScreens.IsEnabled = false;
        }

        private void EnableFields()
        {
            SelectFolder.IsEnabled = true;
            OpenFolder.IsEnabled = true;
            ScreenResolution.IsEnabled = true;
            FrameRateSelection.IsEnabled = true;
            QualitySelection.IsEnabled = true;
            StartRecording.IsEnabled = true;
            StopRecording.IsEnabled = true;
            IdentifyScreens.IsEnabled = true;
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
            System.Windows.Forms.MessageBox.Show(error);
        }

        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }

        private void Record(string path)
        {
            RecorderOptions options = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,
                //If throttling is disabled, out of memory exceptions may eventually crash the program,
                //depending on encoder settings and system specifications.
                IsThrottlingDisabled = false,
                //Hardware encoding is enabled by default.
                IsHardwareEncodingEnabled = true,
                //Low latency mode provides faster encoding, but can reduce quality.
                IsLowLatencyEnabled = false,
                //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                IsMp4FastStartEnabled = false,
                AudioOptions = new AudioOptions
                {
                    Bitrate = AudioBitrate.bitrate_128kbps,
                    Channels = AudioChannels.Stereo,
                    IsAudioEnabled = true
                },
                VideoOptions = new VideoOptions
                {
                    BitrateMode = BitrateControlMode.UnconstrainedVBR,
                    Bitrate = 8000 * 1000,
                    Framerate = Settings.FrameRate,
                    IsMousePointerEnabled = true,
                    IsFixedFramerate = true,
                    EncoderProfile = H264Profile.Main
                }
            };
            Rec = Recorder.CreateRecorder(options);
            Rec.OnRecordingFailed += Rec_OnRecordingFailed;
            Rec.OnStatusChanged += Rec_OnStatusChanged;
            Rec.Record(path);
        }

        private void UpdateFields()
        {
            RecordingLocation.Text = Settings.RecordingLocation;

            string monitorRes = Screen.PrimaryScreen.Bounds.Width + " x " + Screen.PrimaryScreen.Bounds.Height;
            int index = 0;
            bool screenResFound = false;
            foreach (ComboBoxItem x in ScreenResolution.Items)
            {
                if (x.Content.ToString() == monitorRes)
                {
                    ScreenResolution.SelectedIndex = index;
                    screenResFound = true;
                    break;
                }
                else
                {
                    index++;
                }
            }

            if (!screenResFound)
            {
                ScreenResolution.Items.Add(monitorRes);
                ScreenResolution.SelectedIndex = index;
            }

            index = 0;
            foreach (ComboBoxItem x in FrameRateSelection.Items)
            {
                if (x.Content.ToString() == Settings.FrameRate.ToString())
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
                if (x.Content.ToString() == Settings.Quality.ToString())
                {
                    QualitySelection.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }
        }

        private void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(jsonPath, updatedFile);
        }

        private void WaitTime(int seconds)
        {
            if (seconds < 1) return;
            DateTime desired = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
