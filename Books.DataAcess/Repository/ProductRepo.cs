using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using Books.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class ProductRepo : Repository<Product>, IProductRepo
    {
        private readonly ApplicationDbContext _db;
        public ProductRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            if (obj == null) return;
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id, isTrack: false);
            if (objFromDba != null)
            {
                objFromDba.Title = obj.Title;
                objFromDba.Description = obj.Description;
                objFromDba.ISBN = obj.ISBN;
                objFromDba.Author = obj.Author;
                objFromDba.ListPrice = obj.ListPrice;
                objFromDba.Price = obj.Price;
                objFromDba.Price50 = obj.Price50;
                objFromDba.Price100 = obj.Price100;
                objFromDba.CategoryId = obj.CategoryId;
                objFromDba.CoverTypeId = obj.CoverTypeId;
                if (obj.ImageUrl != null)
                    objFromDba.ImageUrl = obj.ImageUrl;               
                _db.Update(objFromDba);
            }

        }

        public Pagination<Product> GetAllWithPagination(Pagination<Product> pagingModel, string includedProps = null)
        {
            IQueryable<Product> query = _db.Products;
            pagingModel.RecordsTotal = query.Count();
            var textSearch = pagingModel.Filter.TextSearch;
            if (textSearch != null && textSearch.Trim().Length > 0)
            {              
                query = query
                    .Where(x => x.Author.Contains(textSearch) || x.Title.Contains(textSearch) || x.ISBN.Contains(textSearch))
                    .Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
                pagingModel.Filter.Start = 0;
                pagingModel.RecordsFiltered = query.Count();
                pagingModel.RecordsTotal = query.Count();
            }
            else
            {
                query = query.Skip(pagingModel.Filter.Start).Take(pagingModel.Filter.Length);
            }
            query = base.IncludeProperty(query, includedProps);
            pagingModel.Data = query;
            return pagingModel;
        }
    }
}
