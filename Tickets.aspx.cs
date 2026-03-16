using System;
using System.Collections.Generic;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Tickets : System.Web.UI.Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindBookingDropdown();
                BindGrid();
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSeatNumber.Text))
                {
                    ShowError("Seat number is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTicketStatus.Text))
                {
                    ShowError("Ticket status is required.");
                    return;
                }

                if (!decimal.TryParse(txtTicketPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ticketPrice))
                {
                    if (!decimal.TryParse(txtTicketPrice.Text, out ticketPrice))
                    {
                        ShowError("Ticket price is invalid.");
                        return;
                    }
                }

                if (!int.TryParse(ddlBooking.SelectedValue, out var bookingId) || bookingId <= 0)
                {
                    ShowError("Select a booking.");
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["Seat_Number"] = txtSeatNumber.Text.Trim(),
                    ["Ticket_Status"] = txtTicketStatus.Text.Trim(),
                    ["Ticket_Price"] = ticketPrice,
                    ["Booking_Id"] = bookingId
                };

                _repository.Insert("Ticket", values);
                lblStatus.Text = "Ticket record inserted.";
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
                if (!int.TryParse(hfTicketId.Value, out var ticketId))
                {
                    ShowError("Select a ticket to update.");
                    return;
                }

                if (!decimal.TryParse(txtTicketPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ticketPrice))
                {
                    if (!decimal.TryParse(txtTicketPrice.Text, out ticketPrice))
                    {
                        ShowError("Ticket price is invalid.");
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(txtSeatNumber.Text))
                {
                    ShowError("Seat number is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTicketStatus.Text))
                {
                    ShowError("Ticket status is required.");
                    return;
                }

                if (!int.TryParse(ddlBooking.SelectedValue, out var bookingId) || bookingId <= 0)
                {
                    ShowError("Select a booking.");
                    return;
                }

                const string sql = @"UPDATE Ticket
                                     SET Seat_Number = :Seat_Number,
                                         Ticket_Status = :Ticket_Status,
                                         Ticket_Price = :Ticket_Price,
                                         Booking_Id = :Booking_Id
                                     WHERE Ticket_Id = :Ticket_Id";

                var parameters = new[]
                {
                    new OracleParameter(":Seat_Number", txtSeatNumber.Text.Trim()),
                    new OracleParameter(":Ticket_Status", txtTicketStatus.Text.Trim()),
                    new OracleParameter(":Ticket_Price", ticketPrice),
                    new OracleParameter(":Booking_Id", bookingId),
                    new OracleParameter(":Ticket_Id", ticketId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Ticket record updated.";
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
                if (!int.TryParse(hfTicketId.Value, out var ticketId))
                {
                    ShowError("Select a ticket to delete.");
                    return;
                }

                const string sql = "DELETE FROM Ticket WHERE Ticket_Id = :Ticket_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new OracleParameter(":Ticket_Id", ticketId) });
                lblStatus.Text = "Ticket record deleted.";
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

        protected void gvTickets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvTickets.SelectedDataKey.Value);
            var row = _repository.GetById("Ticket", id);
            if (row == null)
            {
                return;
            }

            hfTicketId.Value = id.ToString();
            txtSeatNumber.Text = row["Seat_Number"].ToString();
            txtTicketStatus.Text = row["Ticket_Status"].ToString();
            txtTicketPrice.Text = Convert.ToDecimal(row["Ticket_Price"]).ToString("F2", CultureInfo.InvariantCulture);
            ddlBooking.SelectedValue = row["Booking_Id"].ToString();

            lblStatus.Text = "Editing selected ticket.";
            lblStatus.CssClass = "status";
        }

        private void BindGrid()
        {
            const string sql = @"SELECT t.Ticket_Id,
                                        t.Seat_Number,
                                        t.Ticket_Status,
                                        t.Ticket_Price,
                                        b.Booking_Id,
                                        u.User_Name,
                                        m.Movie_Title
                                 FROM Ticket t
                                 INNER JOIN Booking b ON b.Booking_Id = t.Booking_Id
                             INNER JOIN AppUser u ON u.User_Id = b.User_Id
                             INNER JOIN Show s ON s.Show_Id = b.Show_Id
                                 INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                 ORDER BY t.Ticket_Id DESC";
            gvTickets.DataSource = _dataAccess.ExecuteDataTable(sql);
            gvTickets.DataBind();
        }

        private void BindBookingDropdown()
        {
            var bookingTable = _dataAccess.ExecuteDataTable("SELECT Booking_Id FROM Booking ORDER BY Booking_Id DESC");
            ddlBooking.DataSource = bookingTable;
            ddlBooking.DataTextField = "Booking_Id";
            ddlBooking.DataValueField = "Booking_Id";
            ddlBooking.DataBind();
            ddlBooking.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Booking --", ""));
        }

        private void ClearFormFields()
        {
            hfTicketId.Value = string.Empty;
            txtSeatNumber.Text = string.Empty;
            txtTicketStatus.Text = string.Empty;
            txtTicketPrice.Text = string.Empty;
            ddlBooking.SelectedIndex = ddlBooking.Items.Count > 0 ? 0 : -1;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
