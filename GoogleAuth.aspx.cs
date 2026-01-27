using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace dishaaccounts
{
    public partial class GoogleAuth : Page
    {
        private string Cs => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod != "POST")
            {
                WriteJson(false, "POST only", null);
                return;
            }

            string body;
            using (var sr = new StreamReader(Request.InputStream))
                body = sr.ReadToEnd();

            string mode = JsonGet(body, "mode");   // "signup" or "login"
            string token = JsonGet(body, "token");
            string role = JsonGet(body, "role");   // only for signup

            if (string.IsNullOrWhiteSpace(token))
            {
                WriteJson(false, "Missing token", null);
                return;
            }

            var t = VerifyGoogleToken(token);
            if (t == null)
            {
                WriteJson(false, "Invalid Google token", null);
                return;
            }

            string expectedAud = ConfigurationManager.AppSettings["GoogleClientId"];
            if (!string.Equals(t.Aud, expectedAud, StringComparison.Ordinal))
            {
                WriteJson(false, "GoogleClientId mismatch", null);
                return;
            }

            using (var con = new SqlConnection(Cs))
            {
                con.Open();

                // 1) Existing by GoogleId
                var row = FindByGoogleId(con, t.Sub);
                if (row != null)
                {
                    FormsAuthentication.SetAuthCookie(row.Username, false);
                    WriteJson(true, null, RedirectForRole(row.Role));
                    return;
                }

                // 2) Existing by email -> link GoogleId
                var byEmail = FindByEmail(con, t.Email);
                if (byEmail != null)
                {
                    LinkGoogleId(con, t.Email, t.Sub);
                    FormsAuthentication.SetAuthCookie(byEmail.Username, false);
                    WriteJson(true, null, RedirectForRole(byEmail.Role));
                    return;
                }

                // 3) New user
                if (!string.Equals(mode, "signup", StringComparison.OrdinalIgnoreCase))
                {
                    WriteJson(false, "No account found. Please sign up with Google first (choose a role).", null);
                    return;
                }

                // signup mode must have role
                if (role != "Recruiter") role = "JobSeeker";

                string username = EnsureUniqueUsername(con, MakeUsername(t.Name, t.Email));

                using (var ins = new SqlCommand(
                    @"INSERT INTO dbo.Users (Username, Email, PasswordSalt, PasswordHash, UserRole, GoogleId)
                      VALUES (@u,@e,NULL,NULL,@r,@g)", con))
                {
                    ins.Parameters.AddWithValue("@u", username);
                    ins.Parameters.AddWithValue("@e", t.Email);
                    ins.Parameters.AddWithValue("@r", role);
                    ins.Parameters.AddWithValue("@g", t.Sub);
                    ins.ExecuteNonQuery();
                }

                FormsAuthentication.SetAuthCookie(username, false);
                WriteJson(true, null, RedirectForRole(role));
            }
        }

        // ===== Token verification via tokeninfo =====
        private class TokenInfo { public string Email, Sub, Name, Aud; }

        private static TokenInfo VerifyGoogleToken(string idToken)
        {
            try
            {
                string url = "https://oauth2.googleapis.com/tokeninfo?id_token=" + HttpUtility.UrlEncode(idToken);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    string json = sr.ReadToEnd();
                    return new TokenInfo
                    {
                        Email = JsonGet(json, "email"),
                        Sub = JsonGet(json, "sub"),
                        Name = JsonGet(json, "name"),
                        Aud = JsonGet(json, "aud")
                    };
                }
            }
            catch { return null; }
        }

        // ===== DB helpers =====
        private class UserRow { public string Username; public string Role; }

        private static UserRow FindByGoogleId(SqlConnection con, string googleId)
        {
            using (var cmd = new SqlCommand("SELECT Username, UserRole FROM dbo.Users WHERE GoogleId=@g", con))
            {
                cmd.Parameters.AddWithValue("@g", googleId);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new UserRow { Username = r["Username"].ToString(), Role = r["UserRole"].ToString() };
                }
            }
        }

        private static UserRow FindByEmail(SqlConnection con, string email)
        {
            using (var cmd = new SqlCommand("SELECT Username, UserRole FROM dbo.Users WHERE Email=@e", con))
            {
                cmd.Parameters.AddWithValue("@e", email);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new UserRow { Username = r["Username"].ToString(), Role = r["UserRole"].ToString() };
                }
            }
        }

        private static void LinkGoogleId(SqlConnection con, string email, string googleId)
        {
            using (var cmd = new SqlCommand("UPDATE dbo.Users SET GoogleId=@g WHERE Email=@e", con))
            {
                cmd.Parameters.AddWithValue("@g", googleId);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.ExecuteNonQuery();
            }
        }

        private static string RedirectForRole(string role)
            => (role == "Recruiter") ? "AccountRecruiter.aspx" : "AccountJobSeeker.aspx";

        // ===== Username generation =====
        private static string MakeUsername(string name, string email)
        {
            string baseU = !string.IsNullOrWhiteSpace(name) ? name : email.Split('@')[0];
            baseU = baseU.Trim().ToLowerInvariant();

            var sb = new System.Text.StringBuilder();
            foreach (char c in baseU)
                if (char.IsLetterOrDigit(c) || c == '_' || c == '.') sb.Append(c);

            string u = sb.ToString();
            if (u.Length < 3) u = "user" + DateTime.UtcNow.Ticks.ToString().Substring(10);
            if (u.Length > 20) u = u.Substring(0, 20);
            return u;
        }

        private static string EnsureUniqueUsername(SqlConnection con, string baseU)
        {
            string u = baseU;
            int n = 0;
            while (true)
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE Username=@u", con))
                {
                    cmd.Parameters.AddWithValue("@u", u);
                    if ((int)cmd.ExecuteScalar() == 0) return u;
                }
                n++;
                u = baseU + n;
            }
        }

        // ===== JSON helpers =====
        private static string JsonGet(string json, string key)
        {
            if (json == null) return null;
            string k = "\"" + key + "\"";
            int i = json.IndexOf(k, StringComparison.OrdinalIgnoreCase);
            if (i < 0) return null;
            i = json.IndexOf(':', i);
            if (i < 0) return null;
            i++;
            while (i < json.Length && json[i] == ' ') i++;
            if (i >= json.Length || json[i] != '"') return null;
            i++;
            int j = i;
            while (j < json.Length)
            {
                if (json[j] == '"' && json[j - 1] != '\\') break;
                j++;
            }
            if (j >= json.Length) return null;
            return json.Substring(i, j - i).Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        private void WriteJson(bool ok, string error, string redirect)
        {
            Response.ContentType = "application/json";
            if (ok)
                Response.Write("{\"ok\":true,\"redirect\":\"" + redirect + "\"}");
            else
                Response.Write("{\"ok\":false,\"error\":\"" + HttpUtility.JavaScriptStringEncode(error ?? "") + "\"}");
            Response.End();
        }
    }
}