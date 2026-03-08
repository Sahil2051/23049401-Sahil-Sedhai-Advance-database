<%@ Page Title="Report - User Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportUserTickets.aspx.cs" Inherits="CinemaTicketSystem.ReportUserTickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">User Tickets Report</h2>

    <section class="card report-card">
        <h3>Tickets Purchased In Last 6 Months</h3>
        <div class="form-grid report-filter-grid">
            <div class="field">
                <label for="ddlUsers">User</label>
                <asp:DropDownList ID="ddlUsers" runat="server" />
            </div>
        </div>
        <div class="actions">
            <asp:Button ID="btnLoad" runat="server" CssClass="btn btn-primary" Text="Load Report" OnClick="btnLoad_Click" />
        </div>
        <asp:Label ID="lblStatus" runat="server" CssClass="status" />
    </section>

    <section class="card">
        <asp:GridView ID="gvUserTickets" runat="server" CssClass="gridview" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Ticket_Id" HeaderText="Ticket ID" />
                <asp:BoundField DataField="Seat_Number" HeaderText="Seat" />
                <asp:BoundField DataField="Ticket_Price" HeaderText="Price" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="Booking_Date" HeaderText="Booking Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Movie_Title" HeaderText="Movie" />
                <asp:BoundField DataField="Show_Date" HeaderText="Show Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Show_Time" HeaderText="Show Time" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
