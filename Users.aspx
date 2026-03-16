<%@ Page Title="Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="CinemaTicketSystem.Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title"><i class="fa-solid fa-users" aria-hidden="true"></i> User Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3><i class="fa-solid fa-id-card" aria-hidden="true"></i> User Form</h3>
            <asp:HiddenField ID="hfUserId" runat="server" />

            <div class="form-grid">
                <div class="field">
                    <label for="txtUserName">Name</label>
                    <asp:TextBox ID="txtUserName" runat="server" />
                </div>
                <div class="field">
                    <label for="txtUserEmail">Email</label>
                    <asp:TextBox ID="txtUserEmail" runat="server" TextMode="Email" />
                </div>
                <div class="field">
                    <label for="txtUserContact">Contact Number</label>
                    <asp:TextBox ID="txtUserContact" runat="server" />
                </div>
                <div class="field">
                    <label for="txtUserAddress">Address</label>
                    <asp:TextBox ID="txtUserAddress" runat="server" />
                </div>
                <div class="field">
                    <label for="txtRegistrationDate">Registration Date</label>
                    <asp:TextBox ID="txtRegistrationDate" runat="server" TextMode="Date" />
                </div>
            </div>

            <div class="actions">
                <asp:Button ID="btnInsert" runat="server" Text="Add User" CssClass="btn btn-primary" OnClick="btnInsert_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-warning" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-ghost" OnClick="btnClear_Click" CausesValidation="false" />
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status" />
        </section>

        <section class="card">
            <h3><i class="fa-solid fa-table-list" aria-hidden="true"></i> Users</h3>
            <asp:GridView ID="gvUsers" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="User_Id"
                OnSelectedIndexChanged="gvUsers_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="User_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="User_Name" HeaderText="Name" />
                    <asp:BoundField DataField="User_Email" HeaderText="Email" />
                    <asp:BoundField DataField="User_Contact_Number" HeaderText="Contact" />
                    <asp:BoundField DataField="User_Address" HeaderText="Address" />
                    <asp:BoundField DataField="User_Registration_Date" HeaderText="Registered" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
