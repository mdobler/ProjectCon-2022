using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VantagepointAPI_BlazorWebApp.Models
{
    public class UDIC_Comment
    {
        public string UDIC_UID { get; set; }
        public string CustNumber { get; set; }
        public string CustName { get; set; }
        public string CustProject { get; set; }
        public string CustEmail { get; set; }
        public string CustComment { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModUser { get; set; }
        public DateTime ModDate { get; set; }
    }
}
