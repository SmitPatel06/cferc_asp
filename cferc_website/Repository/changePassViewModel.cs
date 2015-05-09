using cferc_website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cferc_website.Repository
{
    public class changePassViewModel
    {
        public userTable user { get; set; }
        public string oldPassInput { get; set; }
        public string newPassInput { get; set; }
        public string newPassInput2 { get; set; }
    }

    public class changePassAdminViewModel
    {
        private List<userTable> users;
        public int selectedUserId { get; set; }

        public IEnumerable<SelectListItem> usersSelect
        {
            get
            {
                var query = (from records in users
                             select new SelectListItem
                             {
                                 Value = records.userID.ToString(),
                                 Text = records.userName
                             });
                return query;
            }

        }
        
        public string oldPassInput { get; set; }
        public string newPassInput { get; set; }
        public string newPassInput2 { get; set; }

        public changePassAdminViewModel(List<userTable> pUsers)
        {
            users = pUsers;
        }

        public changePassAdminViewModel()
        {
        }

    }

    public class deleteViewModel
    {
        private List<userTable> users;
        public int selectedUserId { get; set; }

        public IEnumerable<SelectListItem> usersSelect
        {
            get
            {
                var query = (from records in users
                             select new SelectListItem
                             {
                                 Value = records.userID.ToString(),
                                 Text = records.userName
                             });
                return query;
            }

        }
        public deleteViewModel(List<userTable> pUsers)
        {
            users = pUsers;
        }

    public deleteViewModel()
        {
        }
    }


}