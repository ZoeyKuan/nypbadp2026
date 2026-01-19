using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrixyVideoCourses
{
    public partial class AdminVideos : Page
    {
        private string cs => ConfigurationManager.ConnectionStrings["JobPortalConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            lblMsg.Text = "";

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT VideoId, CourseId, Title, VideoUrl, OrderIndex
                    FROM dbo.CourseVideos
                    WHERE IsActive = 1
                    ORDER BY CourseId, OrderIndex;", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvVideos.DataSource = dt;
                    gvVideos.DataBind();
                }
            }
            catch (SqlException)
            {
                lblMsg.Text = "Unable to load videos. Please try again later.";
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

          
            if (!int.TryParse(txtCourseId.Text.Trim(), out int courseId) || courseId <= 0 ||
    !int.TryParse(txtOrder.Text.Trim(), out int orderIndex) || orderIndex <= 0)
            {
                lblMsg.Text = "Course ID and Order Index must be positive numbers greater than 0.";
                return;
            }

            string title = txtTitle.Text.Trim();
            string urlInput = txtUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(urlInput))
            {
                lblMsg.Text = "Title and URL are required.";
                return;
            }

            string embedUrl = ToYoutubeEmbedUrl(urlInput);

            
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO dbo.CourseVideos (CourseId, Title, VideoUrl, OrderIndex, IsActive)
                    VALUES (@CourseId, @Title, @Url, @OrderIndex, 1);", con))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Url", embedUrl);
                    cmd.Parameters.AddWithValue("@OrderIndex", orderIndex);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                txtTitle.Text = "";
                txtUrl.Text = "";
                txtOrder.Text = "";

                BindGrid();
            }
            catch (SqlException)
            {
                lblMsg.Text = "Failed to add video. Please try again.";
            }
        }

        
        protected void gvVideos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvVideos.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvVideos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvVideos.EditIndex = -1;
            BindGrid();
        }

        protected void gvVideos_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblMsg.Text = "";

            int videoId = Convert.ToInt32(gvVideos.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvVideos.Rows[e.RowIndex];


            string courseIdStr = ((TextBox)row.Cells[1].Controls[0]).Text.Trim();
            string title = ((TextBox)row.Cells[2].Controls[0]).Text.Trim();
            string urlInput = ((TextBox)row.Cells[3].Controls[0]).Text.Trim();
            string orderStr = ((TextBox)row.Cells[4].Controls[0]).Text.Trim();

       
            if (!int.TryParse(courseIdStr, out int courseId) || courseId <= 0 ||
    !int.TryParse(orderStr, out int orderIndex) || orderIndex <= 0)
            {
                lblMsg.Text = "Course ID and Order Index must be positive numbers greater than 0.";
                return;
            }


            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(urlInput))
            {
                lblMsg.Text = "Title and URL are required.";
                return;
            }

            string embedUrl = ToYoutubeEmbedUrl(urlInput);

          
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.CourseVideos
                    SET CourseId = @CourseId,
                        Title = @Title,
                        VideoUrl = @Url,
                        OrderIndex = @OrderIndex
                    WHERE VideoId = @VideoId;", con))
                {
                    cmd.Parameters.AddWithValue("@CourseId", courseId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Url", embedUrl);
                    cmd.Parameters.AddWithValue("@OrderIndex", orderIndex);
                    cmd.Parameters.AddWithValue("@VideoId", videoId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                gvVideos.EditIndex = -1;
                BindGrid();
            }
            catch (SqlException)
            {
                lblMsg.Text = "Update failed. Please try again.";
            }
        }

       
        protected void gvVideos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                int videoId = Convert.ToInt32(e.CommandArgument);

                try
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE dbo.CourseVideos
                        SET IsActive = 0
                        WHERE VideoId = @VideoId;", con))
                    {
                        cmd.Parameters.AddWithValue("@VideoId", videoId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    BindGrid();
                }
                catch (SqlException)
                {
                    lblMsg.Text = "Delete failed. Please try again.";
                }
            }
        }

     
      
        private string ToYoutubeEmbedUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = input.Trim();

            if (input.Contains("youtube.com/embed/"))
                return input;

            if (input.Contains("youtu.be/"))
            {
                string id = input.Split(new[] { "youtu.be/" }, StringSplitOptions.None)[1]
                                 .Split('?', '&')[0];
                return "https://www.youtube.com/embed/" + id;
            }

            if (input.Contains("watch?v="))
            {
                string id = input.Split(new[] { "watch?v=" }, StringSplitOptions.None)[1]
                                 .Split('?', '&')[0];
                return "https://www.youtube.com/embed/" + id;
            }

            if (input.Contains("/shorts/"))
            {
                string id = input.Split(new[] { "/shorts/" }, StringSplitOptions.None)[1]
                                 .Split('?', '&')[0];
                return "https://www.youtube.com/embed/" + id;
            }

            return input;
        }
    }
}
