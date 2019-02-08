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

        [JsonProperty("VideoBitrate")]
        public int VideoBitrate { get; set; }

        [JsonProperty("HardwareEncoding")]
        public bool HardwareEncoding { get; set; }

        [JsonProperty("LowLatency")]
        public bool LowLatency { get; set; }

        [JsonProperty("RecordMouse")]
        public bool RecordMouse { get; set; }

        [JsonProperty("EnableAudio")]
        public bool EnableAudio { get; set; }

        [JsonProperty("AudioBitrate")]
        public int AudioBitrate { get; set; }

        [JsonProperty("AudioChannels")]
        public string AudioChannels { get; set; }
    }

}
