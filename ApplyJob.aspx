<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplyJob.aspx.cs" Inherits="NEWWW.ApplyJob" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <main aria-labelledby="jobTitle">
    <asp:Button ID="btnSaveEdit" runat="server" Text="Save Edit" Visible="false" OnClick="btnSaveEdit_Click" />
    
    <asp:Button ID="btnEdit" runat="server" Text="Edit" OnClick="btnEdit_Click" />
    <asp:Button ID="btnSave" runat="server" Text="Save Job" OnClick="btnSave_Click" />
    <asp:Button ID="btnDeleteJob" runat="server" Text="Delete Saved Job" OnClick="btnDeleteJob_Click" />
    <asp:Button ID="btnDeletePosting" runat="server" Text="Delete Created Posting" OnClick="btnDeletePosting_Click" />

    <h2> <asp:Label ID="lblJobTitle" runat="server" Text="<%# job_title %>" /> </h2>
    <asp:TextBox ID="txtJobTitle" runat="server" Visible="false" />

    <h4><%: company %> | <%: location %></h4>
    <asp:TextBox ID="txtCompany" runat="server" Visible="false" />
    <asp:TextBox ID="txtLocation" runat="server" Visible="false" />



    <!-- Salary -->
    <% if (!string.IsNullOrEmpty(salary_string)) { %>
        <p><strong>Salary:</strong> <%: salary_string %></p>
    <% } else if (min_annual_salary.HasValue || max_annual_salary.HasValue) { %>
        <p><strong>Salary:</strong> 
            <%: min_annual_salary?.ToString("C0") %>
            <% if (max_annual_salary.HasValue) { %> - <%: max_annual_salary?.ToString("C0") %><% } %>
        </p>
    <% } %>
    <asp:Label ID="Label0" runat="server" Visible="false" Text="Salary per hour/week/month" />
    <asp:TextBox ID="txtSalaryString" runat="server" Visible="false" />
    <asp:Label ID="Label1" runat="server" Visible="false" Text="Min and Max salary" />
    <asp:TextBox ID="txtMinSalary" runat="server" Visible="false" />
    <asp:TextBox ID="txtMaxSalary" runat="server" Visible="false" />


    <!-- Date Posted -->
    <% if (date_posted.HasValue) { %>
        <p><strong>Date Posted:</strong> <%: date_posted.Value.ToString("MMMM dd, yyyy") %></p>
    <% } %>
    <asp:Label ID="Label2" runat="server" Visible="false" Text="Date Posted:" />
    <asp:TextBox ID="txtDatePosted" runat="server" TextMode="Date" Visible="false" />


    <!-- Job Description -->
    <section><h3>Description</h3>
        <p><%: description %></p>
    <asp:Label ID="Label3" runat="server" Visible="false" Text="Description:" />
        <asp:TextBox ID="txtDescription" runat="server"
                 TextMode="MultiLine" Rows="6" Visible="false" />

    </section>

    <!-- Links -->
    <section>
        <% if (!string.IsNullOrEmpty(final_url)) { %>
            <p><a href="<%: final_url %>" target="_blank">Apply Now</a></p>
        <% } else if (!string.IsNullOrEmpty(source_url)) { %>
            <p><a href="<%: source_url %>" target="_blank">Source Link</a></p>
        <% } %>
    <asp:Label ID="Label4" runat="server" Visible="false" Text="Final URL:" />
        <asp:TextBox ID="txtFinalUrl" runat="server" Visible="false" />

    <asp:Label ID="Label5" runat="server" Visible="false" Text="Source URL:" />
        <asp:TextBox ID="txtSourceUrl" runat="server" Visible="false" />
    </section>

    <!-- Optional User-Created Badge -->
    <% if (isUserCreated) { %>
        <p><em>This job was created by a user of SecuredTheJob.</em></p>
    <% } %>

</main>

</asp:Content>