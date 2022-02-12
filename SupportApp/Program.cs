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
        if (string.Equals(command, "gallery-generate-new", StringComparison.OrdinalIgnoreCase))
        {
            GenerateNewGallery();
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

    static void GenerateNewGallery()
    {
        string targetPath = Path.Combine(Environment.CurrentDirectory, ContentDirectoryName, GalleryDirectoryName, "_New.md");

        GalleryFrontMatter frontMatter = GalleryFrontMatter.Empty;
        string json = Serialize(frontMatter);
        File.WriteAllText(targetPath, json);

        string targetStaticPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, "_New");
        Directory.CreateDirectory(targetStaticPath);
    }

    static void GenerateGalleryFilename(string filePath)
    {
        const int Length = 12;
        const char FillChar = '1';
        const string ThumbnailFilename = "t";
        const string ImageFilenamePrefix = "p";

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
        long msec = (long)offset.TotalMinutes;
        StringBuilder builder = new(msec.ToString("0"));
        while (builder.Length < Length)
        {
            builder.Insert(0, FillChar);
        }
        string slug = builder.ToString();
        Console.WriteLine(slug);

        if (filename == slug)
        {
            Console.WriteLine("Already filename is set.");
            return;
        }

        string targetPath = Path.Combine(dir, slug + ext);
        File.Move(filePath, targetPath);
        
        string sourceStaticDirPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, filename);
        string targetStaticDirPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, slug);
        List<NameExtPair> staticFilenames = new();
        foreach (var file in Directory.GetFiles(sourceStaticDirPath))
        {
            MoveFileTo(file, slug);
            staticFilenames.Add(new NameExtPair(Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
        }
        Directory.Delete(sourceStaticDirPath);
        staticFilenames.Sort((l, r) => l.Name.CompareTo(r.Name));
        
        NameExtPair? thumbnailFilePairN = staticFilenames.FirstOrDefault(pair => string.Equals(pair.Name, ThumbnailFilename, StringComparison.OrdinalIgnoreCase));
        if (thumbnailFilePairN.HasValue)
        {
            NameExtPair p = thumbnailFilePairN.Value;
            frontMatter.ThumbnailImageUrl = $"/{GalleryDirectoryName}/{slug}/{p.Name}{p.Ext}";
        }
        
        List<GalleryFrontMatterImage> images = new();
        frontMatter.Images = images;
        foreach (var p in staticFilenames)
        {
            if (!p.Name.ToLower().StartsWith(ImageFilenamePrefix))
            {
                continue;
            }
            images.Add(new GalleryFrontMatterImage()
            {
                Url = $"/{GalleryDirectoryName}/{slug}/{p.Name}{p.Ext}",
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

    static void PrintCodeTable()
    {
        for (int i = 0; i < byte.MaxValue; i++)
        {
            char c = (char)i;
            if (!char.IsLetterOrDigit(c) && !char.IsSymbol(c) && c == '`')
            {
                continue;
            }
            Console.WriteLine($"{{{{ $codeTable.Set `{c}` {i} }}}}");
        }
    }
    
    const string ContentDirectoryName = "content";
    const string StaticDirectoryName = "static";
    const string GalleryDirectoryName = "gallery";
}