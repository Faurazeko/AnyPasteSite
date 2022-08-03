using AnyPasteSite.Models;

namespace AnyPasteSite.Data
{
    public interface IRepository
    {
        bool SaveChanges();
        UploadInfo GetUploadInfo(string fileName);
        void CreateUploadInfo(string ipAddr, string fileName);
        void DeleteUploadInfo(string fileName);
    }
}
