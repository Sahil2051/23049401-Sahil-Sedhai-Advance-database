<%@ Page Title="Movie Occupancy Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MovieOccupancy.aspx.cs" Inherits="CinemaTicketSystem.Reports.MovieOccupancy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title"><i class="fa-solid fa-chart-column" aria-hidden="true"></i> Movie Occupancy Report</h2>

    <section class="card report-card">
        <h3><i class="fa-solid fa-filter" aria-hidden="true"></i> Top 3 Theaters By Occupancy Percentage</h3>
        <div class="form-grid report-filter-grid">
            <div class="field">
                <label for="ddlMovies">Movie</label>
                <asp:DropDownList ID="ddlMovies" runat="server" />
            </div>
        </div>
        <div class="actions">
            <asp:Button ID="btnLoad" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnLoad_Click" />
        </div>
        <span class="loading-indicator"><i class="fa-solid fa-spinner fa-spin" aria-hidden="true"></i>Generating report...</span>
        <asp:Label ID="lblStatus" runat="server" CssClass="status" />
    </section>

    <section class="card">
        <asp:GridView ID="gvMovieOccupancy" runat="server" CssClass="gridview" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Theater_Name" HeaderText="Theater Name" />
                <asp:BoundField DataField="Paid_Tickets" HeaderText="Paid Tickets" />
                <asp:BoundField DataField="Total_Seats" HeaderText="Total Seats" />
                <asp:BoundField DataField="Occupancy_Percentage" HeaderText="Occupancy %" DataFormatString="{0:N2}" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
