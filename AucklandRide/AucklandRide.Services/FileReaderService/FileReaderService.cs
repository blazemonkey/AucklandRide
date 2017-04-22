using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Services.FileReaderService
{
    public class FileReaderService : IFileReaderService
    {
        public async Task<string> ReadFile(string fileName, string folderName = "")
        {
            var installedLocation = Package.Current.InstalledLocation;
            var folder = await installedLocation.GetFolderAsync(folderName);

            var file = await folder.GetFileAsync(fileName);
            var content = await FileIO.ReadTextAsync(file);

            return content;
        }
    }
}
