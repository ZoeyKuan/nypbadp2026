<%@ Page Title="Watch Video" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="VideoLearn.aspx.cs" Inherits="TrixyVideoCourses.VideoLearn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">

        <div class="ca-header">
            <asp:Label ID="lblVideoTitle" runat="server" Text="Video" Font-Size="24px" Font-Bold="true"></asp:Label>
            <asp:HyperLink ID="lnkBack" runat="server" Text="Back to Course >" NavigateUrl="Courses.aspx"></asp:HyperLink>
        </div>

        <div style="margin-top:15px;">
            <iframe id="player" runat="server"
                    width="100%" height="450"
                    style="border-radius:12px; border:1px solid #ddd;"
                    allowfullscreen>
            </iframe>
        </div>

    </div>
</asp:Content>
