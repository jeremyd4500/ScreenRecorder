using Newtonsoft.Json;

namespace ScreenRecorder
{
    public partial class AppSettings
    {
        [JsonProperty("Recording-Location")]
        public string RecordingLocation { get; set; }

        [JsonProperty("ScreenWidth")]
        public int ScreenWidth { get; set; }

        [JsonProperty("ScreenHeight")]
        public int ScreenHeight { get; set; }

        [JsonProperty("FrameRate")]
        public int FrameRate { get; set; }

        [JsonProperty("Quality")]
        public int Quality { get; set; }
    }

}
