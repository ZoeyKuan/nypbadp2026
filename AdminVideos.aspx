<%@ Page Title="Admin - Videos" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeFile="AdminVideos.aspx.cs" Inherits="TrixyVideoCourses.AdminVideos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="ca-header">
            <h1>Admin: Manage Videos</h1>
            <a href="Courses.aspx">Back &gt;</a>
        </div>

    

      <div class="admin-card">
    <h3 style="margin-top:0;">Add Video</h3>

    <div class="admin-form">
        <div class="field-row">
            <label>Course ID</label>
            <asp:TextBox ID="txtCourseId" runat="server" CssClass="admin-input" TextMode="Number" Min="1" placeholder="e.g. 1" />
        </div>

        <div class="field-row">
            <label>Video Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="admin-input" placeholder="e.g. What is RIASEC?" />
        </div>

        <div class="field-row">
            <label>YouTube URL</label>
            <asp:TextBox ID="txtUrl" runat="server" CssClass="admin-input" placeholder="Paste YouTube link (watch / youtu.be / shorts)" />
        </div>

        <div class="field-row">
            <label>Order Index</label>
            <asp:TextBox ID="txtOrder" runat="server" CssClass="admin-input" TextMode="Number" Min="1" placeholder="e.g. 1" />
        </div>

        <div class="admin-actions">
            <asp:Button ID="btnAdd" runat="server" Text="Add Video" CssClass="admin-primary" OnClick="btnAdd_Click" />
        </div>
    </div>

    <div style="margin-top:10px;">
        <asp:Label ID="Label1" runat="server" ForeColor="Red" />
    </div>
</div>

        <br />
        <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />

        <hr />

        <h3>Existing Videos</h3>

        <asp:GridView ID="gvVideos" runat="server"
    AutoGenerateColumns="false"
    DataKeyNames="VideoId"
    OnRowEditing="gvVideos_RowEditing"
    OnRowCancelingEdit="gvVideos_RowCancelingEdit"
    OnRowUpdating="gvVideos_RowUpdating"
    OnRowCommand="gvVideos_RowCommand"
    GridLines="None"
    CssClass="table-grid"
    HeaderStyle-CssClass="table-grid-header"
    RowStyle-CssClass="table-grid-row"
    AlternatingRowStyle-CssClass="table-grid-row-alt">

    <Columns>
        <asp:BoundField DataField="VideoId" HeaderText="VideoId" ReadOnly="true" />
        <asp:BoundField DataField="CourseId" HeaderText="CourseId" />
        <asp:BoundField DataField="Title" HeaderText="Title" />
        <asp:BoundField DataField="VideoUrl" HeaderText="Video URL" />
        <asp:BoundField DataField="OrderIndex" HeaderText="Order" />

        <asp:CommandField ShowEditButton="true"
            ButtonType="Button"
            EditText="Edit"
            UpdateText="Save"
            CancelText="Cancel"
            ControlStyle-CssClass="btn btn-edit" />

        <asp:TemplateField HeaderText="Delete">
            <ItemTemplate>
                <asp:Button
                    ID="btnDelete"
                    runat="server"
                    Text="Delete"
                    CssClass="btn btn-delete"
                    CommandName="Del"
                    CommandArgument='<%# Eval("VideoId") %>'
                    OnClientClick="return confirm('Delete this video?');" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>




    </div>
</asp:Content>
