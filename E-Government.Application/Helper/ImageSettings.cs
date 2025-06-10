using Microsoft.AspNetCore.Http;

namespace Talabat.Admin.DashBoard.Helpers
{
    public static class ImageSettings
    {
        public static string Upload(IFormFile file , string FolderName)
        {
            var FolderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images", FolderName);
            var FileName = Guid.NewGuid() +file.FileName;

            var FilePath = Path.Combine(FolderPath,FileName);

            var fs = new FileStream(FilePath,FileMode.Create);
            file.CopyTo(fs);
            return Path.Combine("images\\License",FileName);
        }

        public static void DeleteFile(string FolderName , string FileName) 
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName);

            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}
