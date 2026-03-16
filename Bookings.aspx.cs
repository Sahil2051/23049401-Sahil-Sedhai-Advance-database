using System;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Bookings : System.Web.UI.Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUsers();
                BindShows();
                BindGrid();
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm(out var bookingDate, out var totalAmount, out var userId, out var showId))
                {
                    return;
                }

                const string sql = @"INSERT INTO Booking (Booking_Date, Booking_Status, Total_Amount, User_Id, Show_Id)
                                     VALUES (:Booking_Date, :Booking_Status, :Total_Amount, :User_Id, :Show_Id)";
                var parameters = new[]
                {
                    new OracleParameter(":Booking_Date", bookingDate),
                    new OracleParameter(":Booking_Status", txtBookingStatus.Text.Trim()),
                    new OracleParameter(":Total_Amount", totalAmount),
                    new OracleParameter(":User_Id", userId),
                    new OracleParameter(":Show_Id", showId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Booking record inserted.";
                lblStatus.CssClass = "status";
                ClearFormFields();
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(hfBookingId.Value, out var bookingId))
                {
                    ShowError("Select a booking to update.");
                    return;
                }

                if (!ValidateForm(out var bookingDate, out var totalAmount, out var userId, out var showId))
                {
                    return;
                }

                const string sql = @"UPDATE Booking
                                     SET Booking_Date = :Booking_Date,
                                         Booking_Status = :Booking_Status,
                                         Total_Amount = :Total_Amount,
                                         User_Id = :User_Id,
                                         Show_Id = :Show_Id
                                     WHERE Booking_Id = :Booking_Id";
                var parameters = new[]
                {
                    new OracleParameter(":Booking_Date", bookingDate),
                    new OracleParameter(":Booking_Status", txtBookingStatus.Text.Trim()),
                    new OracleParameter(":Total_Amount", totalAmount),
                    new OracleParameter(":User_Id", userId),
                    new OracleParameter(":Show_Id", showId),
                    new OracleParameter(":Booking_Id", bookingId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Booking record updated.";
                lblStatus.CssClass = "status";
                ClearFormFields();
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(hfBookingId.Value, out var bookingId))
                {
                    ShowError("Select a booking to delete.");
                    return;
                }

                const string sql = "DELETE FROM Booking WHERE Booking_Id = :Booking_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new OracleParameter(":Booking_Id", bookingId) });
                lblStatus.Text = "Booking record deleted.";
                lblStatus.CssClass = "status";
                ClearFormFields();
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFormFields();
            lblStatus.Text = string.Empty;
            lblStatus.CssClass = "status";
        }

        protected void gvBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvBookings.SelectedDataKey.Value);
            var row = _repository.GetById("Booking", id);
            if (row == null)
            {
                return;
            }

            hfBookingId.Value = id.ToString();
            txtBookingDate.Text = Convert.ToDateTime(row["Booking_Date"]).ToString("yyyy-MM-dd");
            txtBookingStatus.Text = row["Booking_Status"].ToString();
            txtTotalAmount.Text = Convert.ToDecimal(row["Total_Amount"]).ToString("F2", CultureInfo.InvariantCulture);
            ddlUser.SelectedValue = row["User_Id"].ToString();
            ddlShow.SelectedValue = row["Show_Id"].ToString();

            lblStatus.Text = "Editing selected booking.";
            lblStatus.CssClass = "status";
        }

        private void BindUsers()
        {
            var table = _dataAccess.ExecuteDataTable("SELECT User_Id, User_Name FROM AppUser ORDER BY User_Name");
            ddlUser.DataSource = table;
            ddlUser.DataTextField = "User_Name";
            ddlUser.DataValueField = "User_Id";
            ddlUser.DataBind();
            ddlUser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select User --", ""));
        }

        private void BindShows()
        {
            const string sql = @"SELECT s.Show_Id,
                                 m.Movie_Title || ' | ' || TO_CHAR(s.Show_Date, 'YYYY-MM-DD') || ' ' || s.Show_Time AS Show_Label
                             FROM Show s
                                 INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                 ORDER BY s.Show_Date DESC, s.Show_Time DESC";
            var table = _dataAccess.ExecuteDataTable(sql);
            ddlShow.DataSource = table;
            ddlShow.DataTextField = "Show_Label";
            ddlShow.DataValueField = "Show_Id";
            ddlShow.DataBind();
            ddlShow.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Show --", ""));
        }

        private void BindGrid()
        {
            const string sql = @"SELECT b.Booking_Id,
                                        b.Booking_Date,
                                        b.Booking_Status,
                                        b.Total_Amount,
                                        u.User_Name,
                                 m.Movie_Title || ' | ' || TO_CHAR(s.Show_Date, 'YYYY-MM-DD') || ' ' || s.Show_Time AS Show_Detail
                                 FROM Booking b
                             INNER JOIN AppUser u ON u.User_Id = b.User_Id
                             INNER JOIN Show s ON s.Show_Id = b.Show_Id
                                 INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                 ORDER BY b.Booking_Id DESC";
            gvBookings.DataSource = _dataAccess.ExecuteDataTable(sql);
            gvBookings.DataBind();
        }

        private bool ValidateForm(out DateTime bookingDate, out decimal totalAmount, out int userId, out int showId)
        {
            bookingDate = DateTime.MinValue;
            totalAmount = 0m;
            userId = 0;
            showId = 0;

            if (string.IsNullOrWhiteSpace(txtBookingStatus.Text))
            {
                ShowError("Booking status is required.");
                return false;
            }

            if (!DateTime.TryParse(txtBookingDate.Text, out bookingDate))
            {
                ShowError("Booking date is invalid.");
                return false;
            }

            if (!decimal.TryParse(txtTotalAmount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out totalAmount) && !decimal.TryParse(txtTotalAmount.Text, out totalAmount))
            {
                ShowError("Total amount is invalid.");
                return false;
            }

            if (!int.TryParse(ddlUser.SelectedValue, out userId))
            {
                ShowError("Select a user.");
                return false;
            }

            if (!int.TryParse(ddlShow.SelectedValue, out showId))
            {
                ShowError("Select a show.");
                return false;
            }

            return true;
        }

        private void ClearFormFields()
        {
            hfBookingId.Value = string.Empty;
            txtBookingDate.Text = string.Empty;
            txtBookingStatus.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            ddlUser.SelectedIndex = ddlUser.Items.Count > 0 ? 0 : -1;
            ddlShow.SelectedIndex = ddlShow.Items.Count > 0 ? 0 : -1;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
