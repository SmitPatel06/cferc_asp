using cferc_website.Models;
using cferc_website.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace cferc_website.Controllers
{
    public class downloadController : Controller
    {
        //constructor so repository model is instantiated 
        private repoDownloadData downloadModel;

        public downloadController()
        {
            this.downloadModel = new repoDownloadData(new Models.MartinezDBEntities());
        }

        public ActionResult Index()
        {
            
            return View();
        }

        //get measures - this will be called from AngularJS whenever index is loaded

        public JsonResult getMeasure()
        {
            return Json(downloadModel.getMeasure(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getIndustry(measure pMeasure)
        {
            var queryResult = downloadModel.getIndustry(pMeasure);
            return Json(queryResult, "application/json");
        }

        [HttpPost]
        public JsonResult getArea(measureIndustry pData)
        {
            measure measureToSend = new measure();
            industry industryToSend = new industry();

            measureToSend.measureID = pData.measureID;
            measureToSend.measureName = pData.measureName;
            industryToSend.industryID = pData.industryID;
            industryToSend.industryName = pData.industryName;

            var queryResult = downloadModel.getArea(measureToSend, industryToSend);
            return Json(queryResult, "application/json");
        }

        [HttpPost]
        public JsonResult getYears(measureIndustryArea pData)
        {
            

            var queryResult = downloadModel.getYears(pData);
            return Json(queryResult, "application/json");
        }

        [HttpPost]
        public JsonResult getData(inputData pData)
        {
            var queryResult = downloadModel.getData(pData);     
            
            return Json(queryResult, "application/json");
        }

        public FileContentResult downloadCsv(inputDataForDownload pData)
        {

            inputData dataToSend = new inputData();
            dataToSend.beginYear = pData.beginYear;
            dataToSend.endYear = pData.endYear;
            dataToSend.industryID = pData.industryID;
            dataToSend.industryName = pData.industryName;
            dataToSend.measureID = pData.measureID;
            dataToSend.measureName = pData.measureName;
            dataToSend.area = JsonConvert.DeserializeObject<List<area>>(pData.area);

            var queryResult = downloadModel.getData(dataToSend);
            string csv = string.Concat(queryResult.Select(result => string.Format("{0},{1},{2}\n", result.areaName, result.year, result.val)));
            Response.AddHeader("Content-Disposition", "attachment; filename=download.csv");
            return File(new System.Text.UTF8Encoding().GetBytes(csv),"text/csv");

        }



    }
}
