using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.ViewModels
{
    public class NewsAdminViewModel
    {
        private List<Maintenance> _maintenances;
        public NewsAdminViewModel()
        {
            _maintenances = new List<Maintenance>();
        }
        public List<Maintenance> Maintenances
        {
            get
            {
                return _maintenances;
            }
            set
            {
                _maintenances = value;
            }

        }
    }
}