using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cferc_website.Models;
using System.Web.Helpers;

namespace cferc_website.Repository
{
    public class repoPublicAccessQuery
    {
        private MartinezDBEntities db;

        public repoPublicAccessQuery(MartinezDBEntities pContextClass)
        {
            this.db = pContextClass;
        }

        public MartinezDBEntities context()
        {
            return db;
        }


    }


    public class publicDataFromClient
    {
        public string areaName { get; set; }
        public string year { get; set; }
    }
}