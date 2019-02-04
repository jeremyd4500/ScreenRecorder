using Newtonsoft.Json;

namespace ScreenRecorder
{
    public partial class AppSettings
    {
        [JsonProperty("Recording-Location")]
        public string RecordingLocation { get; set; }

        [JsonProperty("MonitorCount")]
        public int MonitorCount { get; set; }

        [JsonProperty("Monitors")]
        public Monitors[] Monitors { get; set; }

        [JsonProperty("FrameRate")]
        public int FrameRate { get; set; }

        [JsonProperty("Quality")]
        public int Quality { get; set; }
    }

    public partial class Monitors
    {
        [JsonProperty("Position")]
        public string Position { get; set; }

        [JsonProperty("Width")]
        public int Width { get; set; }

        [JsonProperty("Height")]
        public int Height { get; set; }

    }

}
