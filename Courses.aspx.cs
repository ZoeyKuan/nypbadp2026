using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace TrixyVideoCourses
{
    public partial class Courses : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string q = Request.QueryString["q"];
                BindCourses(q);
            }
        }

        private void BindCourses(string q = null)
        {
            string cs = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string sql = @"
                    SELECT CourseId, Title, Category, Level,
                           ISNULL(Description, '') AS Description,
                           ISNULL(ThumbnailUrl, '') AS ThumbnailUrl
                    FROM dbo.Courses
                    WHERE IsActive = 1";

                if (!string.IsNullOrWhiteSpace(q))
                {
                    sql += " AND (Title LIKE @q OR Category LIKE @q)";
                }

                sql += " ORDER BY CreatedAt DESC, CourseId DESC;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        rptCourses.DataSource = dt;
                        rptCourses.DataBind();

                        pnlEmpty.Visible = (dt.Rows.Count == 0);
                    }
                }
            }
        }
    }
}
