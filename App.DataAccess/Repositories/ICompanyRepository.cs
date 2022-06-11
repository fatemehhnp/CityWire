﻿using App.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.DataAccess.Repositories
{
    public interface ICompanyRepository
    {
        Company GetById(int id);
    }
}
