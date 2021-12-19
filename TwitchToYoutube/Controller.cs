using System.Text.Json;

namespace TwitchToYoutube
{
    public class Controller
    {
        public class Settings
        {
            public string OutputFolder { get; set; }
            public string GameURL { get; set; }
            public string Intro { get; set; }
            public string Intermission { get; set; }
            public string End { get; set; }
        }
        public class ClipInfo
        {
            public string ClipName { get; set; }
            public string Streamer { get; set; }
            public string URL { get; set; }
            public int ClipNum { get; set; }
            public string SaveLocation { get; set; }
        }
        static void Main(string[] args)
        {
            // Loads Settings from Json
            Settings settings = new Settings();
            if (args.Length == 1 && File.Exists(args[0]))
                settings = LoadSettings(args[0]);
            else
            {
                Console.WriteLine("Settings file not found.");
                return;
            }
            
            // Gets list of clips
            List<ClipInfo> Clips = TwitchDownload.GetClips(settings.GameURL);

            // Downloads clips
            Clips = TwitchDownload.DownloadClips(Clips, settings.OutputFolder);

            // Play and Audit Clips
            Clips = CreateVideo.AuditClips(Clips);

            // TODO: stitch clips together into video
            string Video = CreateVideo.StitchClips(Clips, settings);

            // TODO: upload video to youtube
        }
        static Settings LoadSettings(string fileLocation)
        {
            string jsonString = File.ReadAllText(fileLocation);
            try
            {
                Settings settings = JsonSerializer.Deserialize<Settings>(jsonString);
                return settings;
            }
            catch (Exception)
            {
                Console.WriteLine("Error Loading Settings.");
                throw;
            }
        }
    }
}



