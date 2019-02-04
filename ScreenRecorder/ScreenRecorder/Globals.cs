namespace ScreenRecorder
{
    public static class Globals
    {
        public static AppSettings Settings { get; set; }
        public static Recorder Rec { get; set; }

        public static int StartX { get; set; }
        public static int StartY { get; set; }

        public static bool isRecording = false;

        public static string jsonPath = "../../Resources/AppSettings.json";
        public static string contents;
        public static string filename;

        public static string[] positions1 = { "Center" };
        public static string[] positions2 = { "Left", "Right" };
        public static string[] positions3 = { "Left", "Center", "Right" };
    }
}
