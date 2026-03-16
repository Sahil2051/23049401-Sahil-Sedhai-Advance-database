<%@ Page Title="Bookings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bookings.aspx.cs" Inherits="CinemaTicketSystem.Bookings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Booking Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Booking Form</h3>
            <asp:HiddenField ID="hfBookingId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtBookingDate">Booking Date</label><asp:TextBox ID="txtBookingDate" runat="server" TextMode="Date" /></div>
                <div class="field"><label for="txtBookingStatus">Booking Status</label><asp:TextBox ID="txtBookingStatus" runat="server" /></div>
                <div class="field"><label for="txtTotalAmount">Total Amount</label><asp:TextBox ID="txtTotalAmount" runat="server" /></div>
                <div class="field"><label for="ddlUser">User</label><asp:DropDownList ID="ddlUser" runat="server" /></div>
                <div class="field"><label for="ddlShow">Show</label><asp:DropDownList ID="ddlShow" runat="server" /></div>
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
            <h3>Bookings</h3>
            <asp:GridView ID="gvBookings" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Booking_Id"
                OnSelectedIndexChanged="gvBookings_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Booking_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Booking_Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Booking_Status" HeaderText="Status" />
                    <asp:BoundField DataField="Total_Amount" HeaderText="Total Amount" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="User_Name" HeaderText="User" />
                    <asp:BoundField DataField="Show_Detail" HeaderText="Show" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
