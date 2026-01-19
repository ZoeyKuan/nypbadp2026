using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace TrixyVideoCourses
{
    public partial class CourseDetail : Page
    {
        protected int CourseId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out CourseId))
            {
                Response.Redirect("Courses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourse();
                LoadVideos();
            }
        }

        private void LoadCourse()
        {
            string cs = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT CourseId, Title, Category, Level, Description
                FROM dbo.Courses
                WHERE CourseId = @CourseId AND IsActive = 1;", con))
            {
                cmd.Parameters.AddWithValue("@CourseId", CourseId);
                con.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        Response.Redirect("Courses.aspx");
                        return;
                    }

                    lblCourseTitle.Text = r["Title"].ToString();
                    lblCourseMeta.Text = $"{r["Category"]} • {r["Level"]}";
                    lblCourseDesc.Text = r["Description"].ToString();
                }
            }
        }

        private void LoadVideos()
        {
            string cs = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT VideoId, Title, VideoUrl, OrderIndex
                FROM dbo.CourseVideos
                WHERE CourseId = @CourseId AND IsActive = 1
                ORDER BY OrderIndex;", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@CourseId", CourseId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                rptVideos.DataSource = dt;
                rptVideos.DataBind();

                pnlEmpty.Visible = (dt.Rows.Count == 0);
            }
        }
    }
}
