using Microsoft.Identity.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Identity.Firebase.Services
{
    public class BaseService
    {
        protected ApplicationDbContext db;

        public BaseService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public BaseService(){}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}