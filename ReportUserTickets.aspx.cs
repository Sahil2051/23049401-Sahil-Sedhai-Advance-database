using System;
using System.Data;
using System.Data.SqlClient;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class ReportUserTickets : System.Web.UI.Page
    {
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUsers();
                BindReport();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindUsers()
        {
            var users = _dataAccess.ExecuteDataTable("SELECT User_Id, User_Name FROM [User] ORDER BY User_Name");
            ddlUsers.DataSource = users;
            ddlUsers.DataTextField = "User_Name";
            ddlUsers.DataValueField = "User_Id";
            ddlUsers.DataBind();
            ddlUsers.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select User --", ""));
        }

        private void BindReport()
        {
            if (!int.TryParse(ddlUsers.SelectedValue, out var userId))
            {
                gvUserTickets.DataSource = new DataTable();
                gvUserTickets.DataBind();
                lblStatus.Text = "Select a user to load report data.";
                return;
            }

            const string sql = @"SELECT
                                    t.Ticket_Id,
                                    t.Seat_Number,
                                    t.Ticket_Price,
                                    b.Booking_Date,
                                    m.Movie_Title,
                                    s.Show_Date,
                                    s.Show_Time
                                FROM Ticket t
                                INNER JOIN Booking b ON b.Booking_Id = t.Booking_Id
                                INNER JOIN [Show] s ON s.Show_Id = b.Show_Id
                                INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                WHERE b.User_Id = @UserId
                                  AND b.Booking_Date >= DATEADD(MONTH, -6, CAST(GETDATE() AS DATE))
                                ORDER BY b.Booking_Date DESC, t.Ticket_Id DESC";

            var table = _dataAccess.ExecuteDataTable(sql, new[] { new SqlParameter("@UserId", userId) });
            gvUserTickets.DataSource = table;
            gvUserTickets.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
