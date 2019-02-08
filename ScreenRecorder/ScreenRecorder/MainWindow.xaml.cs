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
        private AudioBitrate A_bitrate { get; set; }
        private AudioChannels A_Channels { get; set; }

        private bool isRecording = false;

        // Any CPU
        // private string jsonPath = "../../Resources/AppSettings.json";
        private string jsonPath = "Resources/AppSettings.json"; // 64bit
        private string logPath = "Resources/Log.txt"; 
        private string Contents { get; set; }
        private string FileName { get; set; }

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            UpdateLogFile("Main window content loaded.");

            // Default Json File
            // {"Recording-Location":"Select Recording Location","ScreenWidth":1920,"ScreenHeight":1080,"FrameRate":60,"Quality":100}
            Contents = File.ReadAllText(jsonPath);
            UpdateLogFile("Read in file: " + jsonPath);
            Settings = JsonConvert.DeserializeObject<AppSettings>(Contents);
            UpdateLogFile("Deserialized AppSettings.json into an Object.");
            UpdateFields();
            UpdateLogFile("Updated data fields in the app window.");
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
            UpdateLogFile("Identify button was clicked.");
            DisableFields(true);
            Identify identifier = new Identify();
            identifier.Show();
            UpdateLogFile("Created an Identify object and displayed it.");
            WaitTime(2);
            identifier.Close();
            UpdateLogFile("Closed the Identify object");
            EnableFields();
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(RecordingLocation.Text);
            UpdateLogFile("Open Folder button was clicked.");
        }

        private void BtnOpenLog_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.Windows.Forms.Application.StartupPath + "\\" + logPath);
            UpdateLogFile("Open Log File button was clicked.");
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            UpdateLogFile("Start Recording button was clicked.");
            if (Directory.Exists(RecordingLocation.Text))
            {
                isRecording = true;
                FileName = RecordingLocation.Text + "\\" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".mp4";
                UpdateLogFile("New recording file: " + FileName);
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
                UpdateLogFile("Recording stopped.");
                isRecording = false;
            } else
            {
                System.Windows.MessageBox.Show("You are not currently recording!");
            }
        }

        private void BtnRecordingLocation_Click(object sender, RoutedEventArgs e)
        {
            UpdateLogFile("Select Folder button was clicked.");
            FolderBrowserDialog folderObj = new FolderBrowserDialog();
            folderObj.ShowDialog();
            if (folderObj.SelectedPath != null && folderObj.SelectedPath != "")
            {
                RecordingLocation.Text = folderObj.SelectedPath;
                Settings.RecordingLocation = folderObj.SelectedPath;
                UpdateLogFile("Recording Location has been changed. New value: " + Settings.RecordingLocation);
            } else
            {
                UpdateLogFile("Recording Location was not changed.");
            }
        }

        private void CbEnableAudio_Click(object sender, RoutedEventArgs e)
        {
            Settings.EnableAudio = EnableAudio.IsChecked.Value;
            UpdateLogFile("Enable Audio checkbox was clicked. New value: " + Settings.EnableAudio);
        }

        private void CbHardwareEncoding_Click(object sender, RoutedEventArgs e)
        {
            Settings.HardwareEncoding = HardwareEncoding.IsChecked.Value;
            UpdateLogFile("Hardware Encoding checkbox was clicked. New value: " + Settings.HardwareEncoding);
        }

        private void CbLowLatency_Click(object sender, RoutedEventArgs e)
        {
            Settings.LowLatency = LowLatency.IsChecked.Value;
            UpdateLogFile("Low Latency checkbox was clicked. New value: " + Settings.LowLatency);
        }

        private void CbRecordMouse_Click(object sender, RoutedEventArgs e)
        {
            Settings.RecordMouse = RecordMouse.IsChecked.Value;
            UpdateLogFile("Record Mouse checkbox was clicked. New value: " + Settings.RecordMouse);
        }

        private void DrpA_BitrateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                Settings.AudioBitrate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                UpdateLogFile("Audio Bitrate selection has been changed. New value: " + Settings.AudioBitrate);
            }
            catch (Exception ex)
            {
                UpdateLogFile("Audio Bitrate selection was changed. Attempted to parse the selected value. Error Message: " + ex.Message);
            }
        }

        private void DrpAudioChannels_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Settings.AudioChannels = (e.AddedItems[0] as ComboBoxItem).Content as string;
                UpdateLogFile("Audio Channels selection has been changed. New value: " + Settings.AudioChannels);
            }
            catch (Exception ex)
            {
                UpdateLogFile("Audio Channels selection was changed. Attempted to parse the selected value. Error Message: " + ex.Message);
            }
        }

        private void DrpFrameRateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Settings.FrameRate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                UpdateLogFile("Framerate selection has been changed. New value: " + Settings.FrameRate);
            }
            catch (Exception ex)
            {
                UpdateLogFile("Framerate selection was changed. Attempted to parse the selected value. Error Message: " + ex.Message);
            }
        }

        private void DrpScreenResolution_DataChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string[] newRes = ((e.AddedItems[0] as ComboBoxItem).Content as string).Split(null);
                Settings.ScreenWidth = int.Parse(newRes[0]);
                Settings.ScreenHeight = int.Parse(newRes[2]);
                UpdateLogFile("Screen Resolution selection has been changed. New value: " + (e.AddedItems[0] as ComboBoxItem).Content as string);
            }
            catch (Exception ex)
            {
                UpdateLogFile("Screen Resolution selection was changed. Attempted to parse the selected value. Error Message: " + ex.Message);
            }
        }

        private void DrpV_BitrateSelection_DataChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                Settings.VideoBitrate = int.Parse((e.AddedItems[0] as ComboBoxItem).Content as string);
                UpdateLogFile("Video Bitrate selection has been changed. New value: " + Settings.VideoBitrate);
            }
            catch (Exception ex)
            {
                UpdateLogFile("Video bitrate selection was changed. Attempted to parse the selected value. Error Message: " + ex.Message);
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
            V_BitrateSelection.IsEnabled = false;
            HardwareEncoding.IsEnabled = false;
            LowLatency.IsEnabled = false;
            RecordMouse.IsEnabled = false;
            StartRecording.IsEnabled = false;
            if (all)
            {
                StopRecording.IsEnabled = false;
            }
            IdentifyScreens.IsEnabled = false;
            UpdateLogFile("Main window fields have been disabled.");
        }

        private void EnableFields()
        {
            SelectFolder.IsEnabled = true;
            OpenFolder.IsEnabled = true;
            ScreenResolution.IsEnabled = true;
            FrameRateSelection.IsEnabled = true;
            V_BitrateSelection.IsEnabled = true;
            HardwareEncoding.IsEnabled = true;
            LowLatency.IsEnabled = true;
            RecordMouse.IsEnabled = true;
            StartRecording.IsEnabled = true;
            StopRecording.IsEnabled = true;
            IdentifyScreens.IsEnabled = true;
            UpdateLogFile("Main window fields have been enabled.");
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
            UpdateLogFile("Recorder through an error. Error message: " + error);
        }

        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
            UpdateLogFile("Recorder status has changed. New status: " + e.Status.ToString());
        }

        private void Record(string path)
        {

            if (Settings.AudioBitrate == 96) A_bitrate = AudioBitrate.bitrate_96kbps;
            else if (Settings.AudioBitrate == 128) A_bitrate = AudioBitrate.bitrate_128kbps;
            else if (Settings.AudioBitrate == 160) A_bitrate = AudioBitrate.bitrate_160kbps;
            else A_bitrate = AudioBitrate.bitrate_192kbps;

            if (Settings.AudioChannels == "5.1") A_Channels = AudioChannels.FivePointOne;
            else if (Settings.AudioChannels == "Mono") A_Channels = AudioChannels.Mono;
            else A_Channels = AudioChannels.Stereo;

            UpdateLogFile("Creating recording options.");
            RecorderOptions options = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,
                //If throttling is disabled, out of memory exceptions may eventually crash the program,
                //depending on encoder settings and system specifications.
                IsThrottlingDisabled = false,
                //Hardware encoding is enabled by default.
                IsHardwareEncodingEnabled = Settings.HardwareEncoding,
                //Low latency mode provides faster encoding, but can reduce quality.
                IsLowLatencyEnabled = Settings.LowLatency,
                //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                IsMp4FastStartEnabled = false,
                AudioOptions = new AudioOptions
                {
                    Bitrate = A_bitrate,
                    Channels = A_Channels,
                    IsAudioEnabled = Settings.EnableAudio
                },
                VideoOptions = new VideoOptions
                {
                    BitrateMode = BitrateControlMode.UnconstrainedVBR,
                    Bitrate = Settings.VideoBitrate * 1000000,
                    Framerate = Settings.FrameRate,
                    IsMousePointerEnabled = Settings.RecordMouse,
                    IsFixedFramerate = true,
                    EncoderProfile = H264Profile.Main
                }
            };
            UpdateLogFile("Creating Recorder object.");
            Rec = Recorder.CreateRecorder(options);
            Rec.OnRecordingFailed += Rec_OnRecordingFailed;
            Rec.OnStatusChanged += Rec_OnStatusChanged;
            UpdateLogFile("Recording started.");
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
                    UpdateLogFile("Actual screen resolution among the options. Selected value: " + monitorRes);
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
                UpdateLogFile("Actual screen resolution was not found but was added to the options. New value: " + monitorRes);
            }

            index = 0;
            foreach (ComboBoxItem x in FrameRateSelection.Items)
            {
                if (x.Content.ToString() == Settings.FrameRate.ToString())
                {
                    UpdateLogFile("Framerate selection was changed. Current selected index: " + index);
                    FrameRateSelection.SelectedIndex = index;
                    break;
                } else
                {
                    index++;
                }
            }

            index = 0;
            foreach (ComboBoxItem x in V_BitrateSelection.Items)
            {
                if (x.Content.ToString() == Settings.VideoBitrate.ToString())
                {
                    UpdateLogFile("Bitrate selction was changed. Current selected index: " + index);
                    V_BitrateSelection.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }

            HardwareEncoding.IsChecked = Settings.HardwareEncoding;
            UpdateLogFile("Hardware Encoding checkbox was updated. New value: " + Settings.HardwareEncoding);
            LowLatency.IsChecked = Settings.LowLatency;
            UpdateLogFile("Low Latency checkbox was updated. New value: " + Settings.LowLatency);
            RecordMouse.IsChecked = Settings.RecordMouse;
            UpdateLogFile("Record Mouse checkbox was updated. New value: " + Settings.RecordMouse);
        }

        private void UpdateJsonFile()
        {
            string updatedFile = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(jsonPath, updatedFile);
            UpdateLogFile("AppSettings.json has been updated with the current Json object.");
        }

        private void UpdateLogFile(string content)
        {
            using (StreamWriter sw = File.AppendText(logPath))
            {
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " -- " + content);
            }
        }

        private void WaitTime(int seconds)
        {
            if (seconds < 1) return;
            DateTime desired = DateTime.Now.AddSeconds(seconds);
            while (DateTime.Now < desired)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            UpdateLogFile("App waited " + seconds + "seconds asynchronously.");
        }
    }
}
