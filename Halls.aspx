<%@ Page Title="Halls" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Halls.aspx.cs" Inherits="CinemaTicketSystem.Halls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Hall Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Hall Form</h3>
            <asp:HiddenField ID="hfHallId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtHallNumber">Hall Number</label><asp:TextBox ID="txtHallNumber" runat="server" /></div>
                <div class="field"><label for="txtSeatingCapacity">Seating Capacity</label><asp:TextBox ID="txtSeatingCapacity" runat="server" TextMode="Number" /></div>
                <div class="field"><label for="txtHallType">Hall Type</label><asp:TextBox ID="txtHallType" runat="server" /></div>
                <div class="field"><label for="txtHallStatus">Hall Status</label><asp:TextBox ID="txtHallStatus" runat="server" /></div>
                <div class="field"><label for="ddlTheater">Theater</label><asp:DropDownList ID="ddlTheater" runat="server" /></div>
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
            <h3>Halls</h3>
            <asp:GridView ID="gvHalls" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Hall_Id"
                OnSelectedIndexChanged="gvHalls_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Hall_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Hall_Number" HeaderText="Hall Number" />
                    <asp:BoundField DataField="Hall_Seating_Capacity" HeaderText="Seating Capacity" />
                    <asp:BoundField DataField="Hall_Type" HeaderText="Type" />
                    <asp:BoundField DataField="Hall_Status" HeaderText="Status" />
                    <asp:BoundField DataField="Theater_Name" HeaderText="Theater" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
