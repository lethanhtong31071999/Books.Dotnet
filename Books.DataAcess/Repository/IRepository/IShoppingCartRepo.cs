﻿using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface IShoppingCartRepo : IRepository<ShoppingCart>
    {
        public int IncrementCount(ShoppingCart objFromDba, int count);
        public int DecrementCount(ShoppingCart objFromDba, int count);
        public void Update(ShoppingCart obj);

    }
}
