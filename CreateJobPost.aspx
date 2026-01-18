<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateJobPost.aspx.cs" Inherits="NEWWW.CreateJobPost" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlJob" runat="server">

    <!-- Job Title -->
    <div class="form-row">
        <asp:Label ID="lblJobTitleLabel" runat="server" Text="Job Title:" CssClass="form-label" />
        <asp:Label ID="lblJobTitle" runat="server" />
        <asp:TextBox ID="txtJobTitle" runat="server"  CssClass="form-control" />
    </div>

    <!-- Company -->
    <div class="form-row">
        <asp:Label ID="lblCompanyLabel" runat="server" Text="Company:" />
        <asp:Label ID="lblCompany" runat="server" />
        <asp:TextBox ID="txtCompany" runat="server"  />
    </div>

    <!-- Location -->
    <div class="form-row">
        <asp:Label ID="lblLocationLabel" runat="server" Text="Location:" />
        <asp:Label ID="lblLocation" runat="server" />
        <asp:TextBox ID="txtLocation" runat="server"  />
    </div>

    <!-- Salary String -->
    <div class="form-row">
        <asp:Label ID="lblSalaryStringLabel" runat="server" Text="Salary:" />
        <asp:Label ID="lblSalaryString" runat="server" />
        <asp:TextBox ID="txtSalaryString" runat="server"  />
    </div>

    <!-- Min / Max Salary -->
    <div class="form-row">
        <asp:Label ID="lblMinSalaryLabel" runat="server" Text="Min Annual Salary:" />
        <asp:Label ID="lblMinSalary" runat="server" />
        <asp:TextBox ID="txtMinSalary" runat="server"  />

        <asp:Label ID="lblMaxSalaryLabel" runat="server" Text="Max Annual Salary:" />
        <asp:Label ID="lblMaxSalary" runat="server" />
        <asp:TextBox ID="txtMaxSalary" runat="server"  />
    </div>

    <!-- Date Posted -->
    <div class="form-row">
        <asp:Label ID="lblDatePostedLabel" runat="server" Text="Date Posted:" />
        <asp:Label ID="lblDatePosted" runat="server" />
        <asp:TextBox ID="txtDatePosted" runat="server"  TextMode="Date" />
    </div>

    <!-- Description -->
    <div class="form-row">
        <asp:Label ID="lblDescriptionLabel" runat="server" Text="Description:" />
        <asp:Label ID="lblDescription" runat="server" />
        <asp:TextBox ID="txtDescription" runat="server"
                     TextMode="MultiLine" Rows="6"  />
    </div>

    <!-- Final URL -->
    <div class="form-row">
        <asp:Label ID="lblFinalUrlLabel" runat="server" Text="Apply URL:" />
        <asp:HyperLink ID="lnkFinalUrl" runat="server" Target="_blank" />
        <asp:TextBox ID="txtFinalUrl" runat="server"  />
    </div>

    <!-- Source URL -->
    <div class="form-row">
        <asp:Label ID="lblSourceUrlLabel" runat="server" Text="Source URL:" />
        <asp:HyperLink ID="lnkSourceUrl" runat="server" Target="_blank" />
        <asp:TextBox ID="txtSourceUrl" runat="server"  />
    </div>

    <!-- User Created -->
    <div class="form-row">
        <asp:Label ID="lblIsUserCreatedLabel" runat="server" Text="User Created:" />
        <asp:Label ID="lblIsUserCreated" runat="server" />
        <asp:CheckBox ID="chkIsUserCreated" runat="server"  />
    </div>

    <!-- Buttons -->
    <div class="form-row">
        <asp:Button ID="btnCreate" runat="server" Text="Edit" OnClick="btnCreate_Click" />
    </div>

</asp:Panel>
</asp:Content>
