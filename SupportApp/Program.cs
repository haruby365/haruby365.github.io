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

        if (string.Equals(command, "gallery-generate-slug", StringComparison.OrdinalIgnoreCase))
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

    const string NewGalleryName = "_NewGallery";
    const string ThumbnailFilename = "t";
    const string ImageFilenamePrefix = "p";

    static void GenerateNewGallery()
    {
        string targetPath = Path.Combine(Environment.CurrentDirectory, ContentDirectoryName, GalleryDirectoryName, NewGalleryName + ".md");

        GalleryFrontMatter frontMatter = GalleryFrontMatter.Empty;
        string json = Serialize(frontMatter);
        File.WriteAllText(targetPath, json);

        string targetStaticPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, NewGalleryName);
        Directory.CreateDirectory(targetStaticPath);

        Console.WriteLine("MarkdownFile: " + targetPath);
        Console.WriteLine("StaticDir: " + targetStaticPath);
        Console.WriteLine("Usage:");
        Console.WriteLine("    1. Edit 'date' field in the 'MarkdownFile'.");
        Console.WriteLine("        * Maybe also want to set 'title', 'tags' and social URLs.");
        Console.WriteLine("        * Must not touch 'slug', 'ThumbnailImageUrl', 'Images'.");
        Console.WriteLine("    2. Copy/paste image files to the 'StaticDir'.");
        Console.WriteLine($"    3. Edit thumbnail image filename to '{ThumbnailFilename}'.");
        Console.WriteLine($"        e.g. {ThumbnailFilename}.png, {ThumbnailFilename}.jpg etc.");
        Console.WriteLine($"    4. Edit image filenames to starts with '{ImageFilenamePrefix}'.");
        Console.WriteLine($"       e.g. {ImageFilenamePrefix}.jpg, {ImageFilenamePrefix}0.jpg, {ImageFilenamePrefix}1.jpg etc.");
        Console.WriteLine("        * The images will be sorted by their filenames.");
        Console.WriteLine("    5. Execute 'gallery-generate-slug' command with the 'MarkdownFile' path.");
        Console.WriteLine("    * Don't forget to set \"gallery\": \"/:slug/\" at 'permalinks' in the 'config.json' for Hugo.");
    }

    static void GenerateGalleryFilename(string filePath)
    {
        const int Length = 12;
        const char FillChar = '1';

        string content = File.ReadAllText(filePath);
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
        if (frontMatter.Slug == slug)
        {
            Console.WriteLine("Already slug is set.");
            return;
        }
        frontMatter.Slug = slug;

        string sourceStaticDirPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, NewGalleryName);
        string targetStaticDirPath = Path.Combine(Environment.CurrentDirectory, StaticDirectoryName, GalleryDirectoryName, slug);
        Directory.Move(sourceStaticDirPath, targetStaticDirPath);
        string[] files = Directory.GetFiles(targetStaticDirPath);
        List<NameExtPair> staticFilenames = new();
        staticFilenames.AddRange(from file in files
                                 select new NameExtPair(Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
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
            if (!p.Name.StartsWith(ImageFilenamePrefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            images.Add(new GalleryFrontMatterImage()
            {
                Url = $"/{GalleryDirectoryName}/{slug}/{p.Name}{p.Ext}",
            });
        }

        string json = Serialize(frontMatter);
        File.WriteAllText(filePath, json);
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
    
    const string ContentDirectoryName = "content";
    const string StaticDirectoryName = "static";
    const string GalleryDirectoryName = "gallery";
}