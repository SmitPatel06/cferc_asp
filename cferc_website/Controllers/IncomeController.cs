using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cferc_website.Models;
using cferc_website.Repository;

namespace cferc_website.Controllers
{
    public class IncomeController : Controller
    {
        private repoPublicAccessQuery publicModel;
        private string[] industries = new string[] {"11","21","22","23","31-33","42","44-45","48-49",
                        "51","52","53","54","55","56","61","62","71","72","81","92"};

        public IncomeController()
        {
            this.publicModel = new repoPublicAccessQuery(new MartinezDBEntities());
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult data(publicDataFromClient dataToSend)
        {
            //List<string> industryList = new List<string>(industries);
            string area = dataToSend.areaName;
            int year = Convert.ToInt32(dataToSend.year);

            var data = (from records in publicModel.context().regional_db
                        where records.areaName == area &&
                        records.year == year &&
                        industries.Contains(records.industryID) &&
                        records.measureName == "personal income"
                        orderby records.value
                        select records);

            return Json(data, "application/json");

        }

        public FileContentResult downloadData(publicDataFromClient pData)
        {
            int year = Convert.ToInt32(pData.year);
            string area = pData.areaName;

            var data = (from records in publicModel.context().data_table
                        where records.series.area.areaName == pData.areaName &&
                        records.year == year &&
                        records.series.measure.measureName == "employment" &&
                        industries.Contains(records.series.industryID)
                        orderby records.value
                        select records).ToList();

            string csv = string.Concat(data.Select(result => string.Format("{0},{1},{2}\n", result.series.area.areaName, result.series.industry.industryName, result.value)));
            Response.AddHeader("Content-Disposition", "attachment; filename=download.csv");
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv");

        }
    }
}
