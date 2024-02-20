using YoutubeExplode;

namespace YouTubeDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set the output directory path here
            string outputDirectory = @"C:\Users\Artak\Downloads";

            // // List of YouTube video URLs to download
            // List<string> videoUrls = new List<string>
            // {
            //     
            // };
            
            // videoUrls.Add(ytLink);

            Console.WriteLine("Input link: ");
            
            try
            {
                string ytLink = Console.ReadLine();
                await DownloadYouTubeVideo(ytLink, outputDirectory);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
        }

        static async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            // Sanitize the video title to remove invalid characters from the file name
            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            // Get all available muxed streams
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}