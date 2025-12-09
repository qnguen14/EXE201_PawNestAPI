namespace PawNest.Repository.Data.Responses.Video;

public class VideoUploadResult
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public double Duration { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long Size { get; set; }
}