using System;
using System.Data;
using System.Data.SqlClient;
using CinemaTicketSystem.App_Code;

namespace CinemaTicketSystem.Reports
{
    public partial class UserTickets : System.Web.UI.Page
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

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
            var users = _db.ExecuteQuery("SELECT User_Id, User_Name FROM [User] ORDER BY User_Name");
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
                                    u.User_Name,
                                    m.Movie_Title,
                                    s.Show_Date,
                                    s.Show_Time,
                                    t.Seat_Number,
                                    t.Ticket_Price
                                FROM Ticket t
                                INNER JOIN Booking b ON b.Booking_Id = t.Booking_Id
                                INNER JOIN [User] u ON u.User_Id = b.User_Id
                                INNER JOIN [Show] s ON s.Show_Id = b.Show_Id
                                INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                WHERE u.User_Id = @UserId
                                  AND b.Booking_Date >= DATEADD(MONTH, -6, CAST(GETDATE() AS DATE))
                                ORDER BY s.Show_Date DESC, s.Show_Time DESC";

            var table = _db.ExecuteQuery(sql, new SqlParameter("@UserId", userId));
            gvUserTickets.DataSource = table;
            gvUserTickets.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
