// © 2022 Jong-il Hong

using System.Text;
using Newtonsoft.Json;

namespace Haruby.Hugo.SupportApp;

public static class Program
{
    static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("Command argument is empty.");
            return;
        }

        string command = args[0];

        Console.WriteLine("Working directory:" + Environment.CurrentDirectory);

        if (string.Equals(command, "gallery-generate-filename", StringComparison.OrdinalIgnoreCase))
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Target file path not provided.");
                return;
            }
            GenerateGalleryFilename(args[1]);
        }
        else
        {
            Console.WriteLine($"'{command}' is unknown command.");
        }
    }

    struct NameExtPair
    {
        public string Name;
        public string Ext;

        public NameExtPair(string name, string ext)
        {
            Name = name;
            Ext = ext;
        }
    }
    static void GenerateGalleryFilename(string filePath)
    {
        const int Length = 12;

        string content = File.ReadAllText(filePath);
        string dir = Path.GetDirectoryName(filePath) ?? string.Empty;
        string filename = Path.GetFileNameWithoutExtension(filePath);
        string ext = Path.GetExtension(filePath);

        GalleryFrontMatter frontMatter = GetGalleryFrontMatter(content);

        if (frontMatter.Date == default)
        {
            Console.WriteLine("Date is invalid.");
            return;
        }
        TimeSpan offset = frontMatter.Date - DateTime.MinValue;
        double msec = offset.TotalMinutes;
        string slug = msec.ToString(new string('0', Length));
        Console.WriteLine(slug);

        if (filename == slug)
        {
            Console.WriteLine("Already filename is set.");
            return;
        }

        string targetPath = Path.Combine(dir, slug + ext);
        File.Move(filePath, targetPath);
        
        string sourceStaticDirPath = Path.Combine(Environment.CurrentDirectory, "static", "gallery", filename);
        string targetStaticDirPath = Path.Combine(Environment.CurrentDirectory, "static", "gallery", slug);
        List<NameExtPair> staticFilenames = new();
        foreach (var file in Directory.GetFiles(sourceStaticDirPath))
        {
            MoveFileTo(file, slug);
            staticFilenames.Add(new NameExtPair(Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
        }
        Directory.Delete(sourceStaticDirPath);
        staticFilenames.Sort((l, r) => l.Name.CompareTo(r.Name));
        
        NameExtPair? thumbnailFilePairN = staticFilenames.FirstOrDefault(pair => string.Equals(pair.Name, "t", StringComparison.OrdinalIgnoreCase));
        if (thumbnailFilePairN.HasValue)
        {
            NameExtPair p = thumbnailFilePairN.Value;
            frontMatter.ThumbnailImageUrl = $"/gallery/{slug}/{p.Name}{p.Ext}";
        }
        
        List<GalleryFrontMatterImage> images = new();
        frontMatter.Images = images;
        foreach (var p in staticFilenames)
        {
            if (char.ToLower(p.Name[0]) != 'p')
            {
                continue;
            }
            images.Add(new GalleryFrontMatterImage()
            {
                Url = $"/gallery/{slug}/{p.Name}{p.Ext}",
            });
        }

        string json = Serialize(frontMatter);
        File.WriteAllText(targetPath, json);
    }

    static string MoveFileTo(string filePath, string anotherDirectoryName)
    {
        string filename = Path.GetFileName(filePath);
        string dir = Path.GetDirectoryName(filePath) ?? string.Empty;
        string parentDir = Path.GetDirectoryName(dir) ?? string.Empty;
        string targetDir = Path.Combine(parentDir, anotherDirectoryName);
        Directory.CreateDirectory(targetDir);
        string targetPath = Path.Combine(targetDir, filename);
        File.Move(filePath, targetPath);
        return targetPath;
    }

    static GalleryFrontMatter GetGalleryFrontMatter(string source)
    {
        return JsonConvert.DeserializeObject<GalleryFrontMatter>(source);
    }
    static string Serialize(GalleryFrontMatter frontMatter)
    {
        using StringWriter sw = new();
        using JsonTextWriter jw = new(sw);
        jw.Formatting = Formatting.Indented;
        jw.IndentChar = ' ';
        jw.Indentation = 3;
        
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(jw, frontMatter);

        return sw.ToString();
    }
}