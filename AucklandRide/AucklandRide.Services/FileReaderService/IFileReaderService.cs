using System.Threading.Tasks;

namespace Services.FileReaderService
{
    public interface IFileReaderService
    {
        Task<string> ReadFile(string fileName, string folderName = "");
    }
}
