using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
//Missing NuGet package - install System.Net.Http from nuget package
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NEWWW
{
    public partial class JobListingResult : System.Web.UI.Page
    {
        public ListOfJobs ListOfjobs = new ListOfJobs();
        static string connStr = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;
        public class Job
        {
            public long id { get; set; }
            public string job_title { get; set; }
            public string description { get; set; }
            public string company { get; set; }
            public string location { get; set; }
            public string salary_string { get; set; }
            public int? min_annual_salary { get; set; }
            public int? max_annual_salary { get; set; }
            public DateTime? date_posted { get; set; }
            public string final_url { get; set; }
            public string source_url { get; set; }
            public bool isUserCreated { get; set; }
        }
        public class ListOfJobs
        {
            [JsonProperty("data")]
            public List<Job> jobs { get; set; }
        }
        static string ExtractSalary(string description, string source_url)
        {
            if (string.IsNullOrEmpty(description)) return null;
            var match = Regex.Match(description,
                @"(\$\d{1,3}(,\d{3})*(\.\d+)?\s*(\/week|\/hour|Hourly)?)");
            return match.Success ? match.Value : $"<a href=\"{source_url}\" target=\"_blank\">View source for salary information</a>";
        }

        // could be removed cos of other pages
        public static void InsertCreatedJobIntoSql(ListOfJobs jobs)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                foreach (Job job in jobs.jobs)
                {//incase a lot of jobs being created at the same time
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Jobs
                (JobId, job_title, description, company, location, salary_string,
                 min_annual_salary, max_annual_salary, date_posted, final_url, source_url, isUserCreated)
                VALUES
                (@id, @job_title, @description, @company, @location, @salary_string,
                 @min_annual_salary, @max_annual_salary, @date_posted, @final_url, @source_url, true)
            ", conn))
                    {
                        //<p><strong>Employment Type:</strong> <%# Eval("EmploymentType") %></p>
                        cmd.Parameters.AddWithValue("@id", job.id);
                        cmd.Parameters.AddWithValue("@job_title", job.job_title);
                        cmd.Parameters.AddWithValue("@description", job.description);
                        cmd.Parameters.AddWithValue("@company", job.company);
                        cmd.Parameters.AddWithValue("@location", job.location);
                        cmd.Parameters.AddWithValue("@salary_string", job.salary_string);

                        cmd.Parameters.AddWithValue("@min_annual_salary",
                            (object)job.min_annual_salary ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@max_annual_salary",
                            (object)job.max_annual_salary ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@date_posted",
                            (object)job.date_posted ?? DBNull.Value);

                        cmd.Parameters.Add("@final_url", SqlDbType.NVarChar, 500).Value = (object)job.final_url ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@source_url", job.source_url);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

        // save job after clicking button -- non user / json
        public void SaveDeserializeJobListings(long Id)
        {
            try
            {
                Job job = null;

                foreach (var jobb in ListOfjobs.jobs)
                {
                    if (jobb.id == Id) { job = jobb; break; }
                    // if min or max salary also null then also grab from job desc??
                    if (jobb.salary_string == null)
                    {
                        // this function needs to detect if its hourly / weekly / monthly etc
                        // if really no salary, say "click to find out" and redirects to final n source url
                        jobb.salary_string = ExtractSalary(jobb.description, jobb.source_url);
                    }

                }
                // saves 1 deserialized job data
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO SavedJobs (UserId, JobId) VALUES (@UserId, @JobId);
                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", 67);
                        cmd.Parameters.AddWithValue("@JobId", job.id);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Jobs ON;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Jobs
                (JobId, job_title, description, company, location, salary_string,
                 min_annual_salary, max_annual_salary, date_posted, final_url, source_url, isUserCreated)
                VALUES
                (@id, @job_title, @description, @company, @location, @salary_string,
                 @min_annual_salary, @max_annual_salary, @date_posted, @final_url, @source_url, @isUserCreated)
            ", conn))
                    {
                        if (job == null) { Debug.WriteLine($"Error: Id not found. Cannot save json/deserialized job data."); return; }
                        //<p><strong>Employment Type:</strong> <%# Eval("EmploymentType") %></p>
                        cmd.Parameters.AddWithValue("@id", job.id);
                        cmd.Parameters.AddWithValue("@job_title", job.job_title);
                        cmd.Parameters.AddWithValue("@description", job.description);
                        cmd.Parameters.AddWithValue("@company", job.company);
                        cmd.Parameters.AddWithValue("@location", job.location);
                        cmd.Parameters.AddWithValue("@salary_string", job.salary_string);

                        cmd.Parameters.AddWithValue("@min_annual_salary",
                            (object)job.min_annual_salary ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@max_annual_salary",
                            (object)job.max_annual_salary ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@date_posted",
                            (object)job.date_posted ?? DBNull.Value);

                        //cmd.Parameters.AddWithValue("@final_url", job.final_url);
                        cmd.Parameters.Add("@final_url", SqlDbType.NVarChar, 500).Value = (object)job.final_url ?? DBNull.Value;
                        cmd.Parameters.AddWithValue("@source_url", job.source_url);
                        cmd.Parameters.AddWithValue("@isUserCreated", job.isUserCreated);

                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Jobs OFF;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine("Invalid JSON: " + ex.Message);
            }
        }

        public static async Task GetTheirstackJobs()
        {
            var client = new HttpClient();
            var json = "{ \"page\": 0, \"limit\": 15, \"job_country_code_or\": [\"SG\"], \"posted_at_max_age_days\": 7 }";
            var apikey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJrdWFuLnpvZXkuZ3Vhbi56dXlpQGdtYWlsLmNvbSIsInBlcm1pc3Npb25zIjoidXNlciIsImNyZWF0ZWRfYXQiOiIyMDI1LTEyLTMwVDE1OjI2OjA2Ljc0Njk4OSswMDowMCJ9.Juo89W1-Ie-QZllNEuEMPS6k-pmVYKp6rdnMpUTNY0A";

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.theirstack.com/v1/jobs/search?token={apikey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync(); //json stuff

            Debug.WriteLine(response.StatusCode);
            Debug.WriteLine(body);
            // saves the metadata / api resp into txt file
            using (StreamWriter jobdata = new StreamWriter("JobListingMetadata.txt"))
            {
                jobdata.Write(@body);
            }
            Console.WriteLine($"Successfully wrote a large paragraph to: {Path.GetFullPath("JobListingMetadata.txt")}");
            // this has issue cos cannot be static -> DeserializeApiForJob();
        }
        public void DeserializeApiForJob()
        {
            var jobdata = Server.MapPath("JobListingMetadata.txt");
            Debug.WriteLine(File.Exists(jobdata));
            string jsonString = File.ReadAllText(jobdata);
            try
            {
                ListOfjobs = JsonConvert.DeserializeObject<ListOfJobs>(jsonString);
                Debug.WriteLine($"Jobs count: {ListOfjobs.jobs.Count}");
            }
            catch (JsonException ex)
            {
                Debug.WriteLine("Invalid JSON: " + ex.Message);
            }
        }
        public static async Task RemainingTokens()
        {
            var client = new HttpClient();
            var apikey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJrdWFuLnpvZXkuZ3Vhbi56dXlpQGdtYWlsLmNvbSIsInBlcm1pc3Npb25zIjoidXNlciIsImNyZWF0ZWRfYXQiOiIyMDI1LTEyLTMwVDE1OjI2OjA2Ljc0Njk4OSswMDowMCJ9.Juo89W1-Ie-QZllNEuEMPS6k-pmVYKp6rdnMpUTNY0A";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.theirstack.com/v0/billing/credit-balance?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJrdWFuLnpvZXkuZ3Vhbi56dXlpQGdtYWlsLmNvbSIsInBlcm1pc3Npb25zIjoidXNlciIsImNyZWF0ZWRfYXQiOiIyMDI1LTEyLTMwVDE1OjI2OjA2Ljc0Njk4OSswMDowMCJ9.Juo89W1-Ie-QZllNEuEMPS6k-pmVYKp6rdnMpUTNY0A"),
                Headers =
        {
            { "Authorization", "Bearer "+apikey },
        },
            };
            using (var response = await client.SendAsync(request))
            {
                //response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(body);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("test, can u see me? hello world!");

            //if (IsPostBack) return;

            //await RemainingTokens();
            //await GetTheirstackJobs();
            DeserializeApiForJob(); // run all the time for latest job data
            var jobitems = new List<Job>();

            //(USER CREATED JOBS) LOADING LISTING DATABASE AS WEB CONTENT
            SqlConnection conn = new SqlConnection(connStr);
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Jobs", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        // process 
                        jobitems.Add(new Job
                        {
                            //<pre><%# Container.DataItem.GetType().ToString() %></pre>
                            id = reader.GetInt64(0),

                            job_title = reader.GetString(1),
                            description = reader.GetString(2),
                            company = reader.GetString(3),
                            location = reader.GetString(4),
                            salary_string = reader.IsDBNull(5) ? null : reader.GetString(5),

                            min_annual_salary = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                            max_annual_salary = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),

                            date_posted = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),

                            final_url = reader.IsDBNull(9) ? null : reader.GetString(9),
                            source_url = reader.IsDBNull(10) ? null : reader.GetString(10),

                            isUserCreated = true // or true if you set this somewhere else

                        });
                    }

                }
            }
            foreach (var job in ListOfjobs.jobs) jobitems.Add(job); //(NON-USER JOBS / json / API) LOADING LISTING DATABASE AS WEB CONTENT

            rptJobs.DataSource = jobitems;
            rptJobs.DataBind();
        }

        protected void rptJobs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Debug.WriteLine(e.CommandName);
            Debug.WriteLine(e.CommandArgument);
            if (e.CommandName == "Apply")
            {
                string jobId = e.CommandArgument.ToString();
                var jobitems = new List<Job>();
                DeserializeApiForJob();
                foreach (var job in ListOfjobs.jobs) jobitems.Add(job);
                Session["Jobs"] = jobitems;
                Response.Redirect($"ApplyJob.aspx?id={jobId}");
            }
        }

    }

}