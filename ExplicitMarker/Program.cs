using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using TagLib.Id3v2;

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
                    var outputFile = Path.Combine(destination.FullName, fn.Name.Replace(".mp3", ".aac"));
                    try
                    {

                        ConvertToAAC(fn.FullName, outputFile);
                        Console.WriteLine("Converted: {0} - {1}", artist, title);
                        SetExplicit(outputFile);
                        Console.WriteLine("Marked as explicit: {0} - {1}", artist, title);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
                    Console.WriteLine("Only one file per folder in the demo, sorry.");
                    break;
                }
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Error: Invalid path to the source folder.");
                Console.ReadLine();
            }
        }

        static void ConvertToAAC(string input, string output)
        {
            using (var reader = new MediaFoundationReader(input))
                MediaFoundationEncoder.EncodeToAac(reader, output);
        }

        static void SetExplicit(string file)
        {
            var f = TagLib.File.Create(file);
            var t = (TagLib.Id3v2.Tag)f.GetTag(TagTypes.Id3v2);
            var p = PrivateFrame.Get(t, "ITUNESADVISORY", true);
            p.PrivateData = System.Text.Encoding.Unicode.GetBytes("1");
            f.Save();
        }
    }
}
