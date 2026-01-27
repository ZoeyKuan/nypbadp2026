<%@ Page Title="Login"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="joblisting3.Login" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="container" style="height:100vh;">
        <div class="signin-box">

            <div class="signin-left">
                <h1>Welcome back</h1>
                <p>Log in to SecuredTheJob</p>

                <div class="divider">OR</div>


                <label>Username or Email</label>
                <asp:TextBox ID="txtUser" runat="server" />

                <label>Password</label>
                <asp:TextBox ID="txtPassword"
                             runat="server"
                             TextMode="Password" />

                <asp:Button ID="btnLogin"
                            runat="server"
                            Text="LOGIN"
                            CssClass="signin-btn"
                            OnClick="btnLogin_Click" />

                <asp:Label ID="lblMsg" runat="server" />

                <div class="register-text">
                    No account?
                    <a href="Signup.aspx">Sign up</a>
                </div>
            </div>

            <div class="signin-right"></div>

        </div>
    </div>

</asp:Content>