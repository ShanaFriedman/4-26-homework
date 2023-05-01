using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace April26Homework.Data
{
    public class ImageRepository
    {
        private string _connectionString;
        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Image> GetImages()
        {
            ImageDbContext context = new(_connectionString);
            return context.Images.ToList();
        }
        public void AddImage(Image i)
        {
            ImageDbContext context = new(_connectionString);
            context.Images.Add(i);
            context.SaveChanges();
        }
        public Image GetImage(int id)
        {
            ImageDbContext context = new(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }
        public void AddLike(int id)
        {
            ImageDbContext context = new(_connectionString);
            context.Database.ExecuteSqlInterpolated($"UPDATE Images SET Likes = likes + 1 WHERE Id = {id}");
        }
        public int GetLikes(int id)
        {
            ImageDbContext context = new(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id).Likes;
        }
    }
}
