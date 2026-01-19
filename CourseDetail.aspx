<%@ Page Title="Course Detail" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CourseDetail.aspx.cs" Inherits="TrixyVideoCourses.CourseDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">

        <div class="ca-header">
            <asp:Label ID="lblCourseTitle" runat="server" Text="Course" Font-Size="24px" Font-Bold="true"></asp:Label>
            <a href="Courses.aspx">Back to Courses &gt;</a>
        </div>

        <asp:Label ID="lblCourseMeta" runat="server" />
        <br /><br />
        <asp:Label ID="lblCourseDesc" runat="server" />

        <hr />

        <h3>Videos</h3>

        <asp:Repeater ID="rptVideos" runat="server">
            <ItemTemplate>
                <div class="article-item" style="margin-bottom:15px;">
                    <div class="content">
                        <h3 style="margin:0 0 6px 0;"><%# Eval("Title") %></h3>
                        <a href='<%# "VideoLearn.aspx?videoId=" + Eval("VideoId") %>'>Watch</a>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div style="padding:12px;border:1px solid #ddd;border-radius:8px;background:#fff;">
                No videos found for this course.
            </div>
        </asp:Panel>

    </div>
</asp:Content>
