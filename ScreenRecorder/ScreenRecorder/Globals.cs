namespace ScreenRecorder
{
    public static class Globals
    {
        public static AppSettings Settings { get; set; }
        public static Recorder Rec { get; set; }

        public static int StartX { get; set; }
        public static int StartY { get; set; }
        public static int Monitor1X { get; set; }
        public static int Monitor2X { get; set; }
        public static int Monitor3X { get; set; }

        public static bool isRecording = false;

        public static string jsonPath = "../../Resources/AppSettings.json";
        public static string Contents { get; set; }
        public static string FileName { get; set; }

        public static string[] positions1 = { "Center" };
        public static string[] positions2 = { "Left", "Right" };
        public static string[] positions3 = { "Left", "Center", "Right" };
    }
}
