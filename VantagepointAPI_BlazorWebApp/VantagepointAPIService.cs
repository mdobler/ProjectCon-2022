using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Web;
using VantagepointAPI_BlazorWebApp.Models;

namespace VantagepointAPI_BlazorWebApp
{
    public class VantagepointAPIService
    {
        private VantagepointSettings settings;
        private TokenResponse token;

        /// <summary>
        /// retrieves the configuration settings via
        /// dependency injection (from Startup.cs)
        /// </summary>
        /// <param name="options"></param>
        public VantagepointAPIService(
            IOptions<VantagepointSettings> options
            )
        {
            settings = options.Value;
        }

        /// <summary>
        /// authenticates with Vantagepoint based
        /// on the settings info passed in at creation
        /// </summary>
        /// <returns></returns>
        public async Task<Models.TokenResponse> authenticate()
        {
            //create new rest client
            var client = new RestClient($"{settings.BaseUrl}/token");

            //set up new request
            var request = new RestRequest();
            request.AddHeader("Content-Type",
                "application/x-www-form-urlencoded");
            //request.AddHeader("Authorization", "Bearer {{oauth_token}}");
            request.AddParameter("Username", settings.Username);
            request.AddParameter("Password", settings.Password);
            request.AddParameter("grant_type", "password");
            request.AddParameter("Integrated", "N");
            request.AddParameter("database", settings.database);
            request.AddParameter("Client_Id", settings.Client_Id);
            request.AddParameter("client_secret", settings.client_secret);
            request.AddParameter("culture", settings.culture);

            //query response of type TokenResponse
            var response = await client.PostAsync<TokenResponse>(request);

            //set token info for internal use
            token = response;
            //return token info
            return response;
        }

        /// <summary>
        /// gets a list of projects (no filter)
        /// this method always re-authenticates
        /// with every call. In real life code you
        /// should check for the expiration of the token
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectsResponse> getProjects()
        {
            await authenticate();

            var client = new RestClient(
                $"{settings.BaseUrl}/project?limit=5");

            var request = new RestRequest();
            request.AddHeader(
                "Content-Type",
                "application/json");
            request.AddHeader(
                "Authorization",
                $"bearer {token.access_token}");


            var response = await client.GetAsync<ProjectsResponse>(request);
            return response;
        }

        /// <summary>
        /// searches for projects by name
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<ProjectsResponse> searchProjects(
            string searchString,
            int limit = 10)
        {

            await authenticate();

            var client = new RestClient(
                    $"{settings.BaseUrl}/project" +
                    $"?limit={limit}" +
                    $"&{getFilterHashForProjectNameSearch(searchString)}");

            var request = new RestRequest();
            request.AddHeader(
                "Content-Type",
                "application/json");
            request.AddHeader(
                "Authorization",
                $"bearer {token.access_token}");


            var response = await client.GetAsync<ProjectsResponse>(request);
            return response;
        }

        /// <summary>
        /// creates the necessary filter hash
        /// for searching a name
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public string getFilterHashForProjectNameSearch(string searchString)
        {
            string filterHash = "filterHash[0][seq]=1&" +
                "filterHash[0][name]=Name" +
                $"&filterHash[0][value]={HttpUtility.UrlEncode(searchString)}" +
                "&filterHash[0][tablename]=PR" +
                "&filterHash[0][opp]=like" +
                "&filterHash[0][condition]=AND" +
                //top level only (WBS2 = '')
                "filterHash[1][seq]=2&" +
                "filterHash[1][name]=WBS2" +
                $"&filterHash[1][value]=" +
                "&filterHash[1][tablename]=PR" +
                "&filterHash[1][type]=dropdown" +
                "&filterHash[1][opp]==";
            return filterHash;
        }

        /// <summary>
        /// posts the comment record to Vantagepoint
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<bool> postComment(
            UDIC_Comment comment)
        {
            try
            {
                await authenticate();

                var client = new RestClient($"{settings.BaseUrl}/UDIC/UDIC_Comments");

                var request = new RestRequest();
                request.AddHeader(
                    "Content-Type",
                    "application/json");
                request.AddHeader(
                    "Authorization",
                    $"bearer {token.access_token}");

                var options = new System.Text.Json.JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull | System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
                string jsonString = System.Text.Json.JsonSerializer.Serialize(comment, options);

                request.AddStringBody(jsonString, DataFormat.Json);
                
                var response = await client.PostAsync(request);
                
                return true;
            }
            catch (Exception ex)
            {
                var info = ex.ToString();
                throw new ApplicationException(info);
            }

            
        }

        /// <summary>
        /// retrieve a list of team members for a project
        /// </summary>
        /// <param name="wbs1"></param>
        /// <returns></returns>
        public async Task<ProjectTeamsResponse> getProjectTeams(string wbs1)
        {
            await authenticate();

            var client = new RestClient(
                $"{settings.BaseUrl}/project/{wbs1}/teammember");

            var request = new RestRequest();
            request.AddHeader(
                "Content-Type",
                "application/json");
            request.AddHeader(
                "Authorization",
                $"bearer {token.access_token}");


            var response = await client.GetAsync<ProjectTeamsResponse>(request);

            return response;
        }

        /// <summary>
        /// this function is for presentation purposes only. It only allows the change of the role description.
        /// if other values change, they would need to be added to the "_original values" before the change
        /// The _transType can be U (update), I (insert) or D (delete)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newRoleDescription"></param>
        /// <returns></returns>
        public async Task<bool> updateRoleDescription(ProjectTeamResponse item, string newRoleDescription)
        {
            try
            {
                var update = new ProjectTeamUpdate();

                update.RecordID = item.RecordID;
                update.WBS1 = item.WBS1;
                update.WBS2 = item.WBS2;
                update.WBS3 = item.WBS3;
                update.Employee = item.Employee;
                update._originalValues.RoleDescription = item.RoleDescription;
                update.RoleDescription = newRoleDescription;
                update._transType = "U";

                var content = new EMProjectAssoc();
                content.Items.Add(update);

                await authenticate();

                var client = new RestClient($"{settings.BaseUrl}/project/{update.WBS1}");

                var request = new RestRequest();
                request.AddHeader(
                    "Content-Type",
                    "application/json");
                request.AddHeader(
                    "Authorization",
                    $"bearer {token.access_token}");

                var options = new System.Text.Json.JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull | System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

                string jsonString = System.Text.Json.JsonSerializer.Serialize(content, options);
                request.AddStringBody(jsonString, DataFormat.Json);

                await client.PutAsync(request);
                return true;
            }


            catch (Exception ex)
            {
                var info = ex.ToString();
                throw new ApplicationException(info);
            }
        }
    }

}
