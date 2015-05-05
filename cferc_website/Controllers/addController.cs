using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cferc_website.Repository;
using cferc_website.Models;
using System.IO;

namespace cferc_website.Controllers
{
    public class addController : Controller
    {
        private repoAddData addModel;

        public addController()
        {
            this.addModel = new repoAddData(new MartinezDBEntities());
        }



        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult uploadcsv(HttpPostedFileBase FileUpload)
        {
            if (FileUpload.ContentLength > 0)
            {
                string name = Path.GetFileName(FileUpload.FileName);
                string path = Path.Combine(Server.MapPath("~/Uploads"), name);
                //try uploading file to server
                try
                {
                    FileUpload.SaveAs(path);
                    ViewData["Feedback"] = "Upload Successful.";
                }
                catch (Exception ex)
                {
                    ViewData["Feedback"] = ex.Message;
                }

                try
                {
                    ViewData["FeedBack"] += " " + addModel.insertFromCsv(path);
                }
                catch (Exception ex)
                {
                    ViewData["Feedback"] = ex.Message;
                }

            }

            return View(ViewData["FeedBack"]);
        }



        [HttpPost]
        public JsonResult postArea(area pArea)
        {
            addModel.checkArea(pArea);
            //areaRepository.insertArea(pArea);
            return Json(pArea, "application/json");
        }

        public JsonResult getAreas()
        {
            var areasToSend = from area in addModel.context().areas
                              select new
                              {
                                  areaID = area.areaID,
                                  areaName = area.areaName

                              };
            return Json(areasToSend, JsonRequestBehavior.AllowGet);
        }


        //industries
        [HttpPost]
        public JsonResult postindustry(industry pIndustry)
        {
            addModel.checkIndustry(pIndustry);
            return Json(pIndustry, "application/json");
        }

        public JsonResult getindustries()
        {
            var industriesToSend = from industry in addModel.context().industries
                                   select new
                                   {
                                       industryID = industry.industryID,
                                       industryName = industry.industryName

                                   };
            return Json(industriesToSend, JsonRequestBehavior.AllowGet);
        }


        //measures
        [HttpPost]
        public JsonResult postmeasure(measure pMeasure)
        {
            addModel.checkMeasure(pMeasure);
            return Json(pMeasure, "application/json");
        }

        public JsonResult getmeasures()
        {
            var measuresToSend = from measure in addModel.context().measures
                                 select new
                                 {
                                     measureID = measure.measureID,
                                     measureName = measure.measureName
                                 };

            return Json(measuresToSend, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult postdata(series pData)
        {
                
            string test = "test";

            addModel.checkSeries(pData);
            return Json(test, "application/json");
        }


    }
}
