using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplicitMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "";
            if(args.Count() > 0)
            {
                path = args[0];
            }

            if(path == "")
            {
                Console.WriteLine("Select the source folder (full path):");
                path = Console.ReadLine();
            }

            var source = new System.IO.DirectoryInfo(path);
            if(source != null)
            {
                var destination = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), source.Name));
                var files = source.GetFiles("*.mp3", SearchOption.AllDirectories);
                Console.WriteLine("Found {0} files in the source folder and all its subfolder.", files.Count());
                foreach(var fn in files)
                {
                    var s = TagLib.File.Create(fn.FullName);
                    var artist = s.GetTag(TagLib.TagTypes.Id3v2).FirstPerformer;
                    var title = s.GetTag(TagLib.TagTypes.Id3v2).Title;
                    Console.WriteLine("Converted: {0} - {1}", artist, title);
                    Console.WriteLine("Marked as explicit: {0} - {1}", artist, title);
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Error: Invalid path to the source folder.");
                Console.ReadLine();
            }
        }

        static byte[] ConvertToAAC(string input, string output)
        {
            using (var reader = new Mp3FileReader(input))
            {
                using (WaveFileWriter writer = new WaveFileWriter(wavFile, reader.WaveFormat))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = reader.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }
    }
}
