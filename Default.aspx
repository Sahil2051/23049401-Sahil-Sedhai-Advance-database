<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CinemaTicketSystem._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Dashboard Overview</h2>

    <div class="dashboard-grid">
        <article class="stat-card">
            <div class="card-icon icon-users"><i class="fa-solid fa-users" aria-hidden="true"></i></div>
            <div class="stat-label">Total Users</div>
            <asp:Label ID="lblUsersCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Users.aspx">Manage Users</a></div>
        </article>

        <article class="stat-card">
            <div class="card-icon icon-movies"><i class="fa-solid fa-clapperboard" aria-hidden="true"></i></div>
            <div class="stat-label">Total Movies</div>
            <asp:Label ID="lblMoviesCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Movies.aspx">Manage Movies</a></div>
        </article>

        <article class="stat-card">
            <div class="card-icon icon-bookings"><i class="fa-solid fa-receipt" aria-hidden="true"></i></div>
            <div class="stat-label">Total Bookings</div>
            <asp:Label ID="lblBookingsCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Bookings.aspx">Manage Bookings</a></div>
        </article>

        <article class="stat-card">
            <div class="card-icon icon-tickets"><i class="fa-solid fa-ticket" aria-hidden="true"></i></div>
            <div class="stat-label">Total Tickets</div>
            <asp:Label ID="lblTicketsCount" runat="server" CssClass="stat-number" />
            <div class="card-link"><a runat="server" href="~/Tickets.aspx">Manage Tickets</a></div>
        </article>
    </div>

    <div class="widget-grid">
        <section class="widget">
            <div class="widget-title">Quick Reports</div>
            <div class="quick-report-buttons">
                <a runat="server" href="~/Reports/UserTickets.aspx" class="btn btn-primary">User Tickets</a>
                <a runat="server" href="~/Reports/TheaterMovies.aspx" class="btn btn-primary">Theater Movies</a>
                <a runat="server" href="~/Reports/MovieOccupancy.aspx" class="btn btn-primary">Movie Occupancy</a>
            </div>
        </section>

        <section class="widget">
            <div class="widget-title">System Snapshot</div>
            <div class="widget-metric-row">
                <span>Theaters</span>
                <asp:Label ID="lblTheatersCount" runat="server" CssClass="widget-metric-value" />
            </div>
            <div class="widget-metric-row">
                <span>Shows</span>
                <asp:Label ID="lblShowsCount" runat="server" CssClass="widget-metric-value" />
            </div>
            <div class="widget-metric-row">
                <span>Halls</span>
                <asp:Label ID="lblHallsCount" runat="server" CssClass="widget-metric-value" />
            </div>
            <div class="widget-metric-row">
                <span>Last Updated</span>
                <asp:Label ID="lblGeneratedAt" runat="server" CssClass="widget-metric-value" />
            </div>
        </section>
    </div>

</asp:Content>
