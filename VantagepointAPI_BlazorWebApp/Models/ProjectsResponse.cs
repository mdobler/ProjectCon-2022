using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VantagepointAPI_BlazorWebApp.Models
{
    /// <summary>
    /// a project list that matches the expected json response
    /// </summary>
    public class ProjectsResponse : List<ProjectResponse>
    {

    }

    /// <summary>
    /// only fields with a corresponding class property will be mapped.
    /// all other fields will be ignored
    /// </summary>
    public class ProjectResponse
    {
        public string WBS1 { get; set; }
        public string WBS2 { get; set; }
        public string WBS3 { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
    }
}
