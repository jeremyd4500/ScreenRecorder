using Newtonsoft.Json;

namespace ScreenRecorder
{
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
