<%@ Page Title="Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tickets.aspx.cs" Inherits="CinemaTicketSystem.Tickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Ticket Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Ticket Form</h3>
            <asp:HiddenField ID="hfTicketId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtSeatNumber">Seat Number</label><asp:TextBox ID="txtSeatNumber" runat="server" /></div>
                <div class="field"><label for="txtTicketStatus">Ticket Status</label><asp:TextBox ID="txtTicketStatus" runat="server" /></div>
                <div class="field"><label for="txtTicketPrice">Ticket Price</label><asp:TextBox ID="txtTicketPrice" runat="server" /></div>
                <div class="field"><label for="ddlBooking">Booking</label><asp:DropDownList ID="ddlBooking" runat="server" /></div>
            </div>

            <div class="actions">
                <asp:Button ID="btnInsert" runat="server" Text="Insert" CssClass="btn btn-primary" OnClick="btnInsert_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-warning" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-ghost" OnClick="btnClear_Click" CausesValidation="false" />
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status" />
        </section>

        <section class="card">
            <h3>Tickets</h3>
            <asp:GridView ID="gvTickets" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Ticket_Id"
                OnSelectedIndexChanged="gvTickets_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Ticket_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Seat_Number" HeaderText="Seat Number" />
                    <asp:BoundField DataField="Ticket_Status" HeaderText="Status" />
                    <asp:BoundField DataField="Ticket_Price" HeaderText="Price" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Booking_Id" HeaderText="Booking ID" />
                    <asp:BoundField DataField="User_Name" HeaderText="User" />
                    <asp:BoundField DataField="Movie_Title" HeaderText="Movie" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
