﻿using Books.Data;
using Books.DataAcess.Repository.IRepository;
using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository
{
    public class ShoppingCartRepo : Repository<ShoppingCart>, IShoppingCartRepo
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepo(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int IncrementCount(ShoppingCart objFromDba, int count)
        {
            objFromDba.Count += count;
            return objFromDba.Count;
        }

        public int DecrementCount(ShoppingCart objFromDba, int count)
        {
            objFromDba.Count -= count;
            return objFromDba.Count;
        }

        public void Update(ShoppingCart obj)
        {
            if (obj == null) return;
            var objFromDba = base.GetFirstOrDefault(x => x.Id == obj.Id, isTrack: false);
            if(objFromDba != null)
            {
                objFromDba = obj;
            }
        }
    }
}
