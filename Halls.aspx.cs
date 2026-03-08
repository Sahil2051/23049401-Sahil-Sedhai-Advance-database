using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Halls : System.Web.UI.Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTheaters();
                BindGrid();
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateForm(out var seatingCapacity, out var theaterId))
                {
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["Hall_Number"] = txtHallNumber.Text.Trim(),
                    ["Hall_Seating_Capacity"] = seatingCapacity,
                    ["Hall_Type"] = txtHallType.Text.Trim(),
                    ["Hall_Status"] = txtHallStatus.Text.Trim(),
                    ["Theater_Id"] = theaterId
                };

                _repository.Insert("Hall", values);
                lblStatus.Text = "Hall record inserted.";
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
                if (!int.TryParse(hfHallId.Value, out var hallId))
                {
                    ShowError("Select a hall to update.");
                    return;
                }

                if (!ValidateForm(out var seatingCapacity, out var theaterId))
                {
                    return;
                }

                const string sql = @"UPDATE Hall
                                     SET Hall_Number = @Hall_Number,
                                         Hall_Seating_Capacity = @Hall_Seating_Capacity,
                                         Hall_Type = @Hall_Type,
                                         Hall_Status = @Hall_Status,
                                         Theater_Id = @Theater_Id
                                     WHERE Hall_Id = @Hall_Id";

                var parameters = new[]
                {
                    new SqlParameter("@Hall_Number", txtHallNumber.Text.Trim()),
                    new SqlParameter("@Hall_Seating_Capacity", seatingCapacity),
                    new SqlParameter("@Hall_Type", txtHallType.Text.Trim()),
                    new SqlParameter("@Hall_Status", txtHallStatus.Text.Trim()),
                    new SqlParameter("@Theater_Id", theaterId),
                    new SqlParameter("@Hall_Id", hallId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Hall record updated.";
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
                if (!int.TryParse(hfHallId.Value, out var hallId))
                {
                    ShowError("Select a hall to delete.");
                    return;
                }

                const string sql = "DELETE FROM Hall WHERE Hall_Id = @Hall_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new SqlParameter("@Hall_Id", hallId) });
                lblStatus.Text = "Hall record deleted.";
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

        protected void gvHalls_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvHalls.SelectedDataKey.Value);
            var row = _repository.GetById("Hall", id);
            if (row == null)
            {
                return;
            }

            hfHallId.Value = id.ToString();
            txtHallNumber.Text = row["Hall_Number"].ToString();
            txtSeatingCapacity.Text = row["Hall_Seating_Capacity"].ToString();
            txtHallType.Text = row["Hall_Type"].ToString();
            txtHallStatus.Text = row["Hall_Status"].ToString();
            ddlTheater.SelectedValue = row["Theater_Id"].ToString();
            lblStatus.Text = "Editing selected hall.";
            lblStatus.CssClass = "status";
        }

        private void BindTheaters()
        {
            var table = _dataAccess.ExecuteDataTable("SELECT Theater_Id, Theater_Name FROM Theater ORDER BY Theater_Name");
            ddlTheater.DataSource = table;
            ddlTheater.DataTextField = "Theater_Name";
            ddlTheater.DataValueField = "Theater_Id";
            ddlTheater.DataBind();
            ddlTheater.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Theater --", ""));
        }

        private void BindGrid()
        {
            const string sql = @"SELECT h.Hall_Id, h.Hall_Number, h.Hall_Seating_Capacity, h.Hall_Type, h.Hall_Status, t.Theater_Name
                                 FROM Hall h
                                 INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                 ORDER BY h.Hall_Id DESC";
            gvHalls.DataSource = _dataAccess.ExecuteDataTable(sql);
            gvHalls.DataBind();
        }

        private bool ValidateForm(out int seatingCapacity, out int theaterId)
        {
            seatingCapacity = 0;
            theaterId = 0;

            if (string.IsNullOrWhiteSpace(txtHallNumber.Text) || string.IsNullOrWhiteSpace(txtHallType.Text) || string.IsNullOrWhiteSpace(txtHallStatus.Text))
            {
                ShowError("All hall fields are required.");
                return false;
            }

            if (!int.TryParse(txtSeatingCapacity.Text, out seatingCapacity) || seatingCapacity <= 0)
            {
                ShowError("Seating capacity must be a positive number.");
                return false;
            }

            if (!int.TryParse(ddlTheater.SelectedValue, out theaterId))
            {
                ShowError("Select a theater.");
                return false;
            }

            return true;
        }

        private void ClearFormFields()
        {
            hfHallId.Value = string.Empty;
            txtHallNumber.Text = string.Empty;
            txtSeatingCapacity.Text = string.Empty;
            txtHallType.Text = string.Empty;
            txtHallStatus.Text = string.Empty;
            ddlTheater.SelectedIndex = ddlTheater.Items.Count > 0 ? 0 : -1;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
