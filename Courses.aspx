<%@ Page Title="Courses" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="Courses.aspx.cs" Inherits="TrixyVideoCourses.Courses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



    <div class="page-container">

        <div class="ca-header">
            <h1>Available Courses</h1>
            <a href="Default.aspx">Back &gt;</a>
        </div>


        <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
            <div style="padding: 12px; border: 1px solid #ddd; border-radius: 8px; background: #fff;">
                No courses found. Please seed dbo.Courses.
            </div>
        </asp:Panel>

        <div class="article-grid">
            <asp:Repeater ID="rptCourses" runat="server">
                <ItemTemplate>
                    <div class="article-item">
                        <img src="<%# Eval("ThumbnailUrl") %>"
                            alt="Course thumbnail"
                            onerror="this.src='https://via.placeholder.com/300x180?text=Course';" />

                        <div class="content">
                            <h3><%# Eval("Title") %></h3>

                            <div class="course-meta">
    <span class="course-category"><%# Eval("Category") %></span>
    <span class="level-badge" data-level='<%# Eval("Level") %>'>
        <%# Eval("Level") %>
    </span>
</div>

<p class="course-desc"><%# Eval("Description") %></p>


                            <a class="btn-link" href='<%# "CourseDetail.aspx?id=" + Eval("CourseId") %>'>View course
                            </a>

                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>

</asp:Content>
