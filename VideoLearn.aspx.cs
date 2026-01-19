using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace TrixyVideoCourses
{
    public partial class VideoLearn : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["videoId"], out int videoId))
            {
                Response.Redirect("Courses.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadVideo(videoId);
            }
        }

        private void LoadVideo(int videoId)
        {
            string cs = ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT v.Title, v.VideoUrl, v.CourseId
                FROM dbo.CourseVideos v
                WHERE v.VideoId = @VideoId;", con))
            {
                cmd.Parameters.AddWithValue("@VideoId", videoId);
                con.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        Response.Redirect("Courses.aspx");
                        return;
                    }

                    lblVideoTitle.Text = r["Title"].ToString();
                    player.Attributes["src"] = r["VideoUrl"].ToString();

                    int courseId = Convert.ToInt32(r["CourseId"]);
                    lnkBack.NavigateUrl = "CourseDetail.aspx?id=" + courseId;
                }
            }
        }
    }
}
