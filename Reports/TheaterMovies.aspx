<%@ Page Title="Theater Movies Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TheaterMovies.aspx.cs" Inherits="CinemaTicketSystem.Reports.TheaterMovies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Theater Movies Report</h2>

    <section class="card report-card">
        <h3>Movie Shows In Each Theater Hall</h3>
        <div class="form-grid report-filter-grid">
            <div class="field">
                <label for="ddlTheaters">Theater</label>
                <asp:DropDownList ID="ddlTheaters" runat="server" />
            </div>
        </div>
        <div class="actions">
            <asp:Button ID="btnLoad" runat="server" Text="Load Report" CssClass="btn btn-primary" OnClick="btnLoad_Click" />
        </div>
        <asp:Label ID="lblStatus" runat="server" CssClass="status" />
    </section>

    <section class="card">
        <asp:GridView ID="gvTheaterMovies" runat="server" CssClass="gridview" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Theater_Name" HeaderText="Theater Name" />
                <asp:BoundField DataField="Hall_Number" HeaderText="Hall Number" />
                <asp:BoundField DataField="Movie_Title" HeaderText="Movie Title" />
                <asp:BoundField DataField="Show_Date" HeaderText="Show Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Show_Time" HeaderText="Show Time" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
