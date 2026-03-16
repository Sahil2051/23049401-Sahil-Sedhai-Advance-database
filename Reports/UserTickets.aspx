<%@ Page Title="User Tickets Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserTickets.aspx.cs" Inherits="CinemaTicketSystem.Reports.UserTickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title"><i class="fa-solid fa-user-check" aria-hidden="true"></i> User Tickets Report</h2>

    <section class="card report-card">
        <h3><i class="fa-solid fa-filter" aria-hidden="true"></i> Tickets Purchased In Last 6 Months</h3>
        <div class="form-grid report-filter-grid">
            <div class="field">
                <label for="ddlUsers">User</label>
                <asp:DropDownList ID="ddlUsers" runat="server" />
            </div>
        </div>
        <div class="actions">
            <asp:Button ID="btnLoad" runat="server" Text="Generate Report" CssClass="btn btn-primary" OnClick="btnLoad_Click" />
        </div>
        <span class="loading-indicator"><i class="fa-solid fa-spinner fa-spin" aria-hidden="true"></i>Generating report...</span>
        <asp:Label ID="lblStatus" runat="server" CssClass="status" />
    </section>

    <section class="card">
        <asp:GridView ID="gvUserTickets" runat="server" CssClass="gridview" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="User_Name" HeaderText="User Name" />
                <asp:BoundField DataField="Movie_Title" HeaderText="Movie Title" />
                <asp:BoundField DataField="Show_Date" HeaderText="Show Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Show_Time" HeaderText="Show Time" />
                <asp:BoundField DataField="Seat_Number" HeaderText="Seat Number" />
                <asp:BoundField DataField="Ticket_Price" HeaderText="Ticket Price" DataFormatString="{0:N2}" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
