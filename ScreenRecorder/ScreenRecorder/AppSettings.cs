using Newtonsoft.Json;
using System.Collections.Generic;

namespace ScreenRecorder
{
    public partial class AppSettings
    {
        [JsonProperty("Recording-Location")]
        public string RecordingLocation { get; set; }

        [JsonProperty("MonitorCount")]
        public int MonitorCount { get; set; }

        [JsonProperty("Monitors")]
        public List<Monitor> Monitors { get; set; }

        [JsonProperty("FrameRate")]
        public int FrameRate { get; set; }

        [JsonProperty("Quality")]
        public int Quality { get; set; }
    }

    public partial class Monitor
    {
        [JsonProperty("Position")]
        public string Position { get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

        [JsonProperty("Selected")]
        public bool Selected { get; set; }

        [JsonProperty("ComboBoxIndex")]
        public int ComboBoxIndex { get; set; }

    }

}
