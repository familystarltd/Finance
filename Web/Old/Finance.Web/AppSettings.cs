using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Web
{
    public class AppSettings
    {
        public string ApplicationName { get; set; } = "Finance Web";
        public int MaxItemsPerList { get; set; } = 15;
        public string DataSource { get; set; }
    }
}