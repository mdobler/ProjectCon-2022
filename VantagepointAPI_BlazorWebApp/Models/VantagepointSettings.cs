using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VantagepointAPI_BlazorWebApp.Models
{
    public class VantagepointSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string database { get; set; }
        public string Client_Id { get; set; }
        public string client_secret { get; set; }
        public string culture { get; set; }
    }
}
