using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VantagepointAPI_BlazorWebApp.Models
{

    public class ProjectTeamsResponse : List<ProjectTeamResponse>
    {

    }

    /// <summary>
    /// only fields with a corresponding class property will be mapped.
    /// all other fields will be ignored
    /// </summary>
    public class ProjectTeamResponse
    {
        public string WBS1 { get; set; }
        public string WBS2 { get; set; }
        public string WBS3 { get; set; }
        public string Employee { get; set; }
        public string Role { get; set; }
        public string RoleDescription { get; set; }
        public string TeamStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RecordID { get; set; }
        
    }


    public class ProjectTeamUpdate
    {
        public string WBS1 { get; set; } = "";
        public string WBS2 { get; set; } = "";
        public string WBS3 { get; set; } = "";
        public string Employee { get; set; } = "";
        public string RoleDescription { get; set; } = "";
        public string RecordID { get; set; }
        public OriginalValues _originalValues { get; set; } = new OriginalValues(); 
        public string _transType { get; set; } = "U";
    }

    public class OriginalValues {
        public string RoleDescription { get; set; } = ""; 
    }

    public class ProjectTeamUpdates : List<ProjectTeamUpdate> {}

    public class EMProjectAssoc
    {
        [JsonPropertyName("EMPROJECTASSOC")]
        public ProjectTeamUpdates Items { get; set; } = new ProjectTeamUpdates();
    }
}

