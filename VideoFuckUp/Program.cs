using System;
using System.IO;
namespace VideoFuckUp
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("No argument provided. Exiting...");
                Environment.Exit(1);
            }

            string videoPath = args[0];
            byte[] mvhdArray = { 0x6D, 0x76, 0x68, 0x64 };
            byte[] durationArray = { 0x44, 0x89 };

            Console.WriteLine($"Video name: {videoPath}");
            
            if(!File.Exists(videoPath))
            {
                Console.WriteLine($"{videoPath} not found.");
                Environment.Exit(1);
            }
            
            byte[] videoFile = File.ReadAllBytes(videoPath);
            string[] vs = videoPath.Split(new char[] { '.' });
            string fileExtension = vs[1];
            if(fileExtension.Equals("mp4"))
            {
                mp4Handler(videoFile, mvhdArray, videoPath);
            }
            else if(fileExtension.Equals("webm"))
            {
                webmHandler(videoFile, durationArray, videoPath);
            }
            
            //6D 76 68 64
        }

        static int SearchBytes(byte[] haystack, byte[] needle)
        {
            int fidx = 0;

            int result = Array.FindIndex(haystack, 0, (byte b) =>
            {
                fidx = (b == needle[fidx]) ? fidx + 1 : 0;
                return (fidx == needle.Length);
            });

            return (result - (needle.Length - 1));
        }

        static void mp4Handler(byte[] videoFile, byte[] mvhdArray, string videoPath)
        {
            int mvhdIndex = SearchBytes(videoFile, mvhdArray);
            mvhdIndex += 16;

            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        videoFile[mvhdIndex + i] = 0x00;
                        break;
                    case 1:
                        videoFile[mvhdIndex + i] = 0x00;
                        break;
                    case 2:
                        videoFile[mvhdIndex + i] = 0x00;
                        break;
                    case 3:
                        videoFile[mvhdIndex + i] = 0x01;
                        break;
                    case 4:
                        videoFile[mvhdIndex + i] = 0x7F;
                        break;
                    case 5:
                        videoFile[mvhdIndex + i] = 0xFF;
                        break;
                    case 6:
                        videoFile[mvhdIndex + i] = 0xFF;
                        break;
                    case 7:
                        videoFile[mvhdIndex + i] = 0xFF;
                        break;
                }
            }

            File.WriteAllBytes($"{videoPath.Substring(0, videoPath.Length - 4)}_fuckedup.mp4", videoFile);
        }

        static void webmHandler(byte[] videoFile, byte[] durationArray, string videoPath)
        {
            int durationIndex = SearchBytes(videoFile, durationArray);
            durationIndex += 2;

            videoFile[durationIndex + 1] = 0x00;

            File.WriteAllBytes($"{videoPath.Substring(0, videoPath.Length - 5)}_fuckedup.webm", videoFile);
        }
    }
}
