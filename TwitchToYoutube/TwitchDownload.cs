using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;

namespace TwitchToYoutube
{
    public static class TwitchDownload
    {
        public static List<Controller.ClipInfo> GetClips(string GameURL)
        {
            List<Controller.ClipInfo> Clips  = new List<Controller.ClipInfo>();

            // Setups Headless Selenium Chrome
            var options = new ChromeOptions()
            {
                BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            };
            options.AddArguments(new List<string>() { "headless", "disable-gpu", "--log-level=3" });
            var browser = new ChromeDriver(options);

            // Goes to GameURL page
            browser.Navigate().GoToUrl(GameURL);

            // Wait for webpage to load, TODO: Automate this instead of 2 second guess?
            Thread.Sleep(2000);

            // Finds list of clip elements
            var ClipElements = browser.FindElements(By.XPath(@"//a[@lines=""1""]"));

            // Pulls information from each clip element
            int i = 0;
            foreach(var ClipElement in ClipElements)
            {
                i++;
                Controller.ClipInfo ClipToAdd = new Controller.ClipInfo();
                ClipToAdd.ClipName = ClipElement.Text;
                ClipToAdd.URL = ClipElement.GetAttribute("href");
                ClipToAdd.Streamer = ClipToAdd.URL.Split("/")[3];
                ClipToAdd.ClipNum = i;
                Clips.Add(ClipToAdd);
            }
            browser.Close();
            return Clips;
        }

        public static List<Controller.ClipInfo> DownloadClips(List<Controller.ClipInfo> Clips, string OutputFolder)
        {
            // check if file location exists
            Console.WriteLine(OutputFolder);
            if(!Directory.Exists(OutputFolder))
            {
                Console.WriteLine("Output folder does not exist.");
                return null;
            }

            // Setups Headless Selenium Chrome
            var options = new ChromeOptions()
            {
                BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
            };
            options.AddArguments(new List<string>() { "headless", "disable-gpu", "--log-level=3" });
            var browser = new ChromeDriver(options);

            string downloadURL;
            WebClient web = new WebClient();
            foreach (Controller.ClipInfo Clip in Clips)
            {
                // Determine Clipr URL
                string CliprURL = "https://clipr.xyz/" + Clip.URL.Split(".twitch.tv/")[1];

                // Goes to GameURL page
                browser.Navigate().GoToUrl(CliprURL);

                // Wait for webpage to load, TODO: Automate this instead of 2 second guess?
                Thread.Sleep(2000);

                var DownloadElements = browser.FindElements(By.XPath(@"//a"));

                // Find download URL
                foreach(var DownloadElement in DownloadElements)
                {
                    if(DownloadElement.GetAttribute("href")!=null && DownloadElement.GetAttribute("href").StartsWith("https://clips-media-assets2.twitch.tv/"))
                    {
                        downloadURL = DownloadElement.GetAttribute("href");

                        // Downloads file and saves download location
                        Clip.SaveLocation = OutputFolder + @"/" + Clip.ClipNum + ".mp4";
                        web.DownloadFile(downloadURL, Clip.SaveLocation);
                        break;
                    }     
                }
            }
            browser.Close();
            return Clips;
        }
    }
}
