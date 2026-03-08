<%@ Page Title="Movies" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Movies.aspx.cs" Inherits="CinemaTicketSystem.Movies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="page-title">Movie Management</h2>

    <div class="crud-layout">
        <section class="card">
            <h3>Movie Form</h3>
            <asp:HiddenField ID="hfMovieId" runat="server" />

            <div class="form-grid">
                <div class="field"><label for="txtMovieTitle">Movie Title</label><asp:TextBox ID="txtMovieTitle" runat="server" /></div>
                <div class="field"><label for="txtDuration">Duration (minutes)</label><asp:TextBox ID="txtDuration" runat="server" TextMode="Number" /></div>
                <div class="field"><label for="txtLanguage">Language</label><asp:TextBox ID="txtLanguage" runat="server" /></div>
                <div class="field"><label for="txtGenre">Genre</label><asp:TextBox ID="txtGenre" runat="server" /></div>
                <div class="field"><label for="txtReleaseDate">Release Date</label><asp:TextBox ID="txtReleaseDate" runat="server" TextMode="Date" /></div>
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
            <h3>Movies</h3>
            <asp:GridView ID="gvMovies" runat="server" CssClass="gridview" AutoGenerateColumns="False" DataKeyNames="Movie_Id"
                OnSelectedIndexChanged="gvMovies_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Movie_Id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Movie_Title" HeaderText="Title" />
                    <asp:BoundField DataField="Movie_Duration" HeaderText="Duration" />
                    <asp:BoundField DataField="Movie_Language" HeaderText="Language" />
                    <asp:BoundField DataField="Movie_Genre" HeaderText="Genre" />
                    <asp:BoundField DataField="Movie_Release_Date" HeaderText="Release Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:CommandField ShowSelectButton="True" SelectText="Edit" CausesValidation="false" />
                </Columns>
            </asp:GridView>
        </section>
    </div>
</asp:Content>
