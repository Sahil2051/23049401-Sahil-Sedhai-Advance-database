<%@ Page Title="Theaters" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Theaters.aspx.cs" Inherits="CinemaTicketSystem.Theaters" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Theater Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Theater Form</h3>
            <asp:HiddenField ID="hfTheaterId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtTheaterName">Theater Name</label><asp:TextBox ID="txtTheaterName" runat="server" /></div>
                <div class="field"><label for="txtTheaterCity">City</label><asp:TextBox ID="txtTheaterCity" runat="server" /></div>
                <div class="field"><label for="txtTheaterLocation">Location</label><asp:TextBox ID="txtTheaterLocation" runat="server" /></div>
                <div class="field"><label for="txtTheaterContact">Contact Number</label><asp:TextBox ID="txtTheaterContact" runat="server" /></div>
                <div class="field"><label for="txtTotalHalls">Total Halls</label><asp:TextBox ID="txtTotalHalls" runat="server" TextMode="Number" /></div>
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
            <h3>Theaters</h3>
            <asp:GridView ID="gvTheaters" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Theater_Id"
                OnSelectedIndexChanged="gvTheaters_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Theater_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Theater_Name" HeaderText="Name" />
                    <asp:BoundField DataField="Theater_City" HeaderText="City" />
                    <asp:BoundField DataField="Theater_Location" HeaderText="Location" />
                    <asp:BoundField DataField="Theater_Contact_Number" HeaderText="Contact" />
                    <asp:BoundField DataField="Theater_Total_Halls" HeaderText="Total Halls" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
