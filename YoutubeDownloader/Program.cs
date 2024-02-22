using YoutubeExplode;

namespace YouTubeDownloader
{
    class Program
    {
        protected Program()
        {
        }

        private static async Task Main()
        {
            // Set the output directory path here
            string outputDirectory = @"D:\YoutubeDownloader";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(outputDirectory);
                Console.WriteLine("The directory was created successfully at {0}.",
                    Directory.GetCreationTime(outputDirectory));

                // Delete the directory.
                // di.Delete();
                // Console.WriteLine("The directory was deleted successfully.");
            }
            catch (IOException e)
            {
                Console.WriteLine("The process failed: " + e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("The process failed: " + e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("The process failed: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Success!");
            }

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