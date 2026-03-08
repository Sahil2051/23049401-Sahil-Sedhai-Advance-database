<%@ Page Title="Shows" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shows.aspx.cs" Inherits="CinemaTicketSystem.Shows" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Show Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Show Form</h3>
            <asp:HiddenField ID="hfShowId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtShowDate">Show Date</label><asp:TextBox ID="txtShowDate" runat="server" TextMode="Date" /></div>
                <div class="field"><label for="txtShowTime">Show Time</label><asp:TextBox ID="txtShowTime" runat="server" TextMode="Time" /></div>
                <div class="field"><label for="txtShowRating">Show Rating</label><asp:TextBox ID="txtShowRating" runat="server" /></div>
                <div class="field"><label for="ddlMovie">Movie</label><asp:DropDownList ID="ddlMovie" runat="server" /></div>
                <div class="field"><label for="ddlHall">Hall</label><asp:DropDownList ID="ddlHall" runat="server" /></div>
            </div>

            <div class="actions">
                <asp:Button ID="btnInsert" runat="server" Text="Insert" CssClass="btn btn-primary" OnClick="btnInsert_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-secondary" OnClick="btnUpdate_Click" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" CausesValidation="false" />
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status" />
        </section>

        <section class="card">
            <h3>Shows</h3>
            <asp:GridView ID="gvShows" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Show_Id"
                OnSelectedIndexChanged="gvShows_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Show_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Show_Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Show_Time" HeaderText="Time" />
                    <asp:BoundField DataField="Show_Rating" HeaderText="Rating" />
                    <asp:BoundField DataField="Movie_Title" HeaderText="Movie" />
                    <asp:BoundField DataField="Hall_Number" HeaderText="Hall" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
