using cferc_website.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cferc_website.Models;

namespace cferc_website.Controllers
{
    public class EmploymentController : Controller
    {

        private repoPublicAccessQuery publicModel;

        public EmploymentController()
        {
            this.publicModel = new repoPublicAccessQuery(new MartinezDBEntities());
        }

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult data(publicDataFromClient dataToSend)
        {

            string area = dataToSend.areaName;
            string year = dataToSend.year;
            string sqlString = @"select * from regional_db
                                where measureName like '%employment%' AND
                                year =" + year + @"AND 
                                areaName like '%" + area + @"%' AND industryID IN ('11','21','22','23','31-33','42','44-45','48-9',
                                '51','52','53','54','55','56','61','62','71','72','81','92')
                                Order by value";
            var data = publicModel.context().regional_db.SqlQuery(sqlString);


            return Json(data, "application/json");
            //return View(data);


        }

    }
}
