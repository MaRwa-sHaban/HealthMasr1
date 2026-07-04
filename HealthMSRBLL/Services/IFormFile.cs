namespace HealthMSR.BLL.Services
{
    public interface IFormFile
    {
        int Length { get; }
        string ContentType { get; }
        ReadOnlySpan<char> FileName { get; }

        Task CopyToAsync(FileStream stream);
    }
}