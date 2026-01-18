using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NEWWW
{
    public partial class CreateJobPost : System.Web.UI.Page
    {
        static string connStr = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

        protected void btnCreate_Click(object sender, EventArgs e)
        {

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Jobs
                (job_title, description, company, location, salary_string,
                 min_annual_salary, max_annual_salary, date_posted, final_url, source_url, isUserCreated)
                VALUES
                (@job_title, @description, @company, @location, @salary_string,
                 @min_annual_salary, @max_annual_salary, @date_posted, @final_url, @source_url, @isUserCreated)
            ", conn))
                {
                    cmd.Parameters.AddWithValue("@job_title", txtJobTitle.Text.ToString());
                    cmd.Parameters.AddWithValue("@company", txtCompany.Text.ToString());
                    cmd.Parameters.AddWithValue("@location", txtLocation.Text.ToString());
                    cmd.Parameters.AddWithValue("@description", txtDescription.Text.ToString());
                    cmd.Parameters.AddWithValue("@salary_string", txtSalaryString.Text.ToString());

                    cmd.Parameters.Add("@min_annual_salary", SqlDbType.Int)
    .Value = string.IsNullOrWhiteSpace(txtMinSalary.Text)
        ? (object)DBNull.Value
        : int.Parse(txtMinSalary.Text);

                    cmd.Parameters.Add("@max_annual_salary", SqlDbType.Int)
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

                    //cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
                    cmd.Parameters.Add("@isUserCreated", SqlDbType.Bit).Value = chkIsUserCreated.Checked;

                    cmd.ExecuteNonQuery();
                    Debug.WriteLine($"New job created {txtJobTitle.Text}");
                }
            }
            Response.Redirect("JobListingResult.aspx");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Debug.WriteLine($"Create a job yess");
            }
        }
    }

}