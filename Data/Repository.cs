using AnyPasteSite.Models;

namespace AnyPasteSite.Data
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _db;
        public Repository(AppDbContext dbContext) => _db = dbContext;

        public void CreateUploadInfo(string ipAddr, string fileName)
        {
            var entity = new UploadInfo() { IpAddr = ipAddr, FileName = fileName };
            _db.UploadInfo.Add(entity);
        }

        public void DeleteUploadInfo(string fileName)
        {
            var entity = _db.UploadInfo.FirstOrDefault(e => e.FileName == fileName);

            if (entity == null)
                return;

            _db.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        }

        public UploadInfo GetUploadInfo(string fileName) => _db.UploadInfo.FirstOrDefault(e => e.FileName == fileName);

        public bool SaveChanges() => _db.SaveChanges() >= 0;
    }
}
