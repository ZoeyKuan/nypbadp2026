using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;

namespace NEWWW
{
    public partial class ApplyJob : System.Web.UI.Page
    {
        //<!-- can use CssClass for mass visibility or asp:Panel ID="pnlEdit" runat="server"--->

        protected string job_id;
        protected string job_title;
        protected string company;
        protected string location;
        protected string salary_string;
        protected decimal? min_annual_salary;
        protected decimal? max_annual_salary;
        protected DateTime? date_posted;
        protected string description;
        protected string final_url;
        protected string source_url;
        protected bool isUserCreated;

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

        public void GetNonUserJob(long id)
        {
            List<Job> jobs = Session["Jobs"] as List<Job>;
            if (jobs != null)
            {
                Job job = null;
                foreach (Job j in jobs)
                {
                    if (j.id == id) { job = j; break; }
                }
                Debug.WriteLine($"Job {job.job_title}");
                long job_id = id;

                job_title = job.job_title;
                description = job.description;
                company = job.company;
                location = job.location;
                salary_string = job.salary_string;

                min_annual_salary = job.min_annual_salary;
                max_annual_salary = job.max_annual_salary;

                date_posted = job.date_posted;

                final_url = job.final_url;
                source_url = job.source_url;

                isUserCreated = job.isUserCreated;
                DataBind();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                long id = long.Parse(Request.QueryString["id"]);
                Debug.WriteLine(id);
                // check if id exists in sql <- user created
                SqlConnection conn = new SqlConnection(connStr);
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Jobs WHERE JobId=@id", conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@id", id);
                    // check for rows yes
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows) Debug.WriteLine("IT WORKSSS");
                        //if (!reader.HasRows) GetNonUserJob(id);// check json text <- api / non user
                        while (reader.Read())
                        {
                            long job_id = reader.GetInt64(0);

                            job_title = reader.GetString(1);
                            description = reader.GetString(2);
                            company = reader.GetString(3);
                            location = reader.GetString(4);
                            salary_string = reader.IsDBNull(5) ? null : reader.GetString(5);

                            min_annual_salary = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);
                            max_annual_salary = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7);

                            date_posted = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8);

                            final_url = reader.IsDBNull(9) ? null : reader.GetString(9);
                            source_url = reader.IsDBNull(10) ? null : reader.GetString(10);

                            isUserCreated = reader.GetBoolean(11);
                            DataBind();
                        }
                    }
                }
            }
        }
        // save job after clicking button -- user created
        static void SaveUserCreatedListing(int userId, long jobId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO SavedJobs (UserId, JobId)
                VALUES (@UserId, @JobId);", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@JobId", jobId);

                        cmd.ExecuteNonQuery();
                        Debug.WriteLine($"Saved JobId {jobId} for UserId {userId}");
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // UNIQUE constraint violation
                Debug.WriteLine($"JobId {jobId} already exists for UserId {userId}");
                // Optional: ignore, log, or show user-friendly message
            }
            catch (Exception ex)
            {
                // Any other unexpected error
                Debug.WriteLine($"Error saving job: {ex.Message}");
                throw; // rethrow if you want it handled upstream
            }
        }

        protected void btnDeleteJob_Click(object sender, EventArgs e)
        {
            long jobId = long.Parse(Request.QueryString["id"]);
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                DELETE FROM SavedJobs WHERE JobId = @JobId;", conn))
                    {
                        cmd.Parameters.AddWithValue("@JobId", jobId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            Debug.WriteLine($"No SavedJob found with Id {jobId}");
                        }
                        else
                        {
                            Debug.WriteLine($"Deleted SavedJob with Id {jobId}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL error deleting SavedJob {jobId}: {ex.Message}");
                throw; // remove if you want to silently fail
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
            Response.Redirect("JobListingResult.aspx");
        }
        protected void btnDeletePosting_Click(object sender, EventArgs e)
        {
            long jobId = long.Parse(Request.QueryString["id"]);
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                DELETE FROM Jobs WHERE JobId = @JobId;", conn))
                    {
                        cmd.Parameters.Add("@JobId", SqlDbType.Int).Value = jobId;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            Debug.WriteLine($"No Job found with Id {jobId}");
                        }
                        else
                        {
                            Debug.WriteLine($"Deleted Job with Id {jobId}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Common cause: FK constraint (job still referenced in SavedJobs, etc.)
                Debug.WriteLine($"SQL error deleting Job {jobId}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error deleting Job {jobId}: {ex.Message}");
                throw;
            }

            Response.Redirect("JobListingResult.aspx");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            long id = long.Parse(Request.QueryString["id"]);
            //long UserId = long.Parse(Request.QueryString["userid"]);
            //SaveUserCreatedListing(UserId,id);
            SaveUserCreatedListing(67, id);
            Response.Redirect("JobListingResult.aspx");
        }
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            btnEdit.Visible = false;
            btnSaveEdit.Visible = true;

            lblJobTitle.Visible = true;
            lblJobTitle.Text = job_title;
            txtJobTitle.Visible = true;

            txtJobTitle.Text = lblJobTitle.Text;
            txtCompany.Visible = true;
            txtLocation.Visible = true;
            Label1.Visible = true;
            Label2.Visible = true;
            Label3.Visible = true;
            Label4.Visible = true;
            Label5.Visible = true;
            txtDescription.Visible = true;
            txtMinSalary.Visible = true;
            txtMaxSalary.Visible = true;
            txtFinalUrl.Visible = true;
            txtSourceUrl.Visible = true;
            txtDatePosted.Visible = true;
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            long id = long.Parse(Request.QueryString["id"]);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
            UPDATE Jobs SET
    job_title = @job_title,
    company = @company,
    location = @location,
    description = @description,
    date_posted = @date_posted,
    final_url = @final_url,
    source_url = @source_url,
    min_annual_salary = @min_salary,
    max_annual_salary = @max_salary,
    isUserCreated = TRUE,
WHERE JobId = @id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@job_title", txtJobTitle.Text.ToString());
                    cmd.Parameters.AddWithValue("@company", txtCompany.Text.ToString());
                    cmd.Parameters.AddWithValue("@location", txtLocation.Text.ToString());
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.ToString());

                    cmd.Parameters.Add("@min_salary", SqlDbType.Int)
    .Value = string.IsNullOrWhiteSpace(txtMinSalary.Text)
        ? (object)DBNull.Value
        : int.Parse(txtMinSalary.Text);

                    cmd.Parameters.Add("@max_salary", SqlDbType.Int)
                        .Value = string.IsNullOrWhiteSpace(txtMaxSalary.Text)
                            ? (object)DBNull.Value
                            : int.Parse(txtMaxSalary.Text);

                    cmd.Parameters.Add("@final_url", SqlDbType.NVarChar, 500).Value = txtFinalUrl.Text;
                    cmd.Parameters.Add("@source_url", SqlDbType.NVarChar, 500)
              .Value = string.IsNullOrEmpty(txtSourceUrl.Text)
                    ? (object)DBNull.Value
                    : txtSourceUrl.Text;
                    cmd.Parameters.Add("@date_posted", SqlDbType.DateTime)
              .Value = string.IsNullOrEmpty(txtDatePosted.Text)
                    ? (object)DBNull.Value
                    : DateTime.Parse(txtDatePosted.Text);

                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }

}