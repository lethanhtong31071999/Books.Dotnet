﻿using Books.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.DataAcess.Repository.IRepository
{
    public interface IDetailProcessRepo : IRepository<DetailProcess>
    {
        public void Update();
    }
}
