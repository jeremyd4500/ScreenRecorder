using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace ScreenRecorder
{

    public partial class Main : Window
    {

        private RootData settings;
        private string contents;
        private string jsonPath = "../../Resources/AppSettings.json";

        public Main()
        {
            InitializeComponent();
            Title = "Screen Recorder";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            contents = File.ReadAllText(jsonPath);
            settings = JsonConvert.DeserializeObject<RootData>(contents);

            UpdateFields(settings);
        }

        private void BtnRecordingLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderObj = new FolderBrowserDialog();
            folderObj.ShowDialog();
            if (folderObj.SelectedPath != null && folderObj.SelectedPath != "")
            {
                settings.AppSettings[0].RecordingLocation = folderObj.SelectedPath;
                UpdateFields(settings);
                UpdateJsonFile(settings);
            }
        }

        private void BtnTemporaryLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderObj = new FolderBrowserDialog();
            folderObj.ShowDialog();
            if (folderObj.SelectedPath != null && folderObj.SelectedPath != "")
            {
                settings.AppSettings[0].TemporaryLocation = folderObj.SelectedPath;
                UpdateFields(settings);
                UpdateJsonFile(settings);
            }
        }

        private void UpdateFields(RootData settings)
        {
            RecordingLocation.Text = settings.AppSettings[0].RecordingLocation;
            TemporaryLocation.Text = settings.AppSettings[0].TemporaryLocation;
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

        [JsonProperty("Temporary-Location")]
        public string TemporaryLocation { get; set; }
    }
}
