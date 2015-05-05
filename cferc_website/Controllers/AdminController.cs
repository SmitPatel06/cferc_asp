﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cferc_website.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            return View();
        }

    }
}