// © 2022 Jong-il Hong

using Newtonsoft.Json;

namespace Haruby.Hugo.SupportApp;

public struct GalleryFrontMatterImage
{
    public string Url;
}
public struct GalleryFrontMatter
{
    public static GalleryFrontMatter Empty => new()
    {
        Title = string.Empty,
        Tags = Array.Empty<string>(),
        Slug = string.Empty,
        ThumbnailImageUrl = string.Empty,
        Images = Array.Empty<GalleryFrontMatterImage>(),
        NaverBlogUrl = string.Empty,
        PixivUrl = string.Empty,
        ArtstationUrl = string.Empty,
        TwitterUrl = string.Empty,
    };

    [JsonProperty("title")]
    public string Title;
    [JsonProperty("date")]
    public DateTime Date;
    [JsonProperty("tags")]
    public IReadOnlyList<string>? Tags;
    [JsonProperty("slug")]
    public string Slug;
    public string ThumbnailImageUrl;
    public IReadOnlyList<GalleryFrontMatterImage>? Images;
    public string NaverBlogUrl;
    public string PixivUrl;
    public string ArtstationUrl;
    public string TwitterUrl;
}
