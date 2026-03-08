<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CinemaTicketSystem._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Dashboard Overview</h2>

    <div class="dashboard-grid">
        <article class="stat-card">
            <div class="stat-label">Total Users</div>
            <asp:Label ID="lblUsersCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Users.aspx">Manage Users</a></div>
        </article>

        <article class="stat-card">
            <div class="stat-label">Total Movies</div>
            <asp:Label ID="lblMoviesCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Movies.aspx">Manage Movies</a></div>
        </article>

        <article class="stat-card">
            <div class="stat-label">Total Theaters</div>
            <asp:Label ID="lblTheatersCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Theaters.aspx">Manage Theaters</a></div>
        </article>

        <article class="stat-card">
            <div class="stat-label">Total Shows</div>
            <asp:Label ID="lblShowsCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Shows.aspx">Manage Shows</a></div>
        </article>

        <article class="stat-card">
            <div class="stat-label">Total Tickets</div>
            <asp:Label ID="lblTicketsCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Tickets.aspx">Manage Tickets</a></div>
        </article>
    </div>

    <div class="widget-grid">
        <section class="widget">
            <div class="widget-title">Report Center</div>
            <ul class="widget-list">
                <li><a runat="server" href="~/Reports/UserTickets.aspx">User Tickets (last 6 months)</a></li>
                <li><a runat="server" href="~/Reports/TheaterMovies.aspx">Theater Movies schedule</a></li>
                <li><a runat="server" href="~/Reports/MovieOccupancy.aspx">Movie Occupancy top halls</a></li>
            </ul>
        </section>

        <section class="widget">
            <div class="widget-title">Operations Snapshot</div>
            <div class="widget-metric-row">
                <span>Bookings</span>
                <asp:Label ID="lblBookingsCount" runat="server" CssClass="widget-metric-value" />
            </div>
            <div class="widget-metric-row">
                <span>Halls</span>
                <asp:Label ID="lblHallsCount" runat="server" CssClass="widget-metric-value" />
            </div>
            <div class="widget-metric-row">
                <span>Generated</span>
                <asp:Label ID="lblGeneratedAt" runat="server" CssClass="widget-metric-value" />
            </div>
        </section>
    </div>

</asp:Content>
