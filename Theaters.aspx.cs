using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Theaters : System.Web.UI.Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtTotalHalls.Text, out var totalHalls))
                {
                    ShowError("Total halls must be a valid number.");
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["Theater_Name"] = txtTheaterName.Text.Trim(),
                    ["Theater_City"] = txtTheaterCity.Text.Trim(),
                    ["Theater_Location"] = txtTheaterLocation.Text.Trim(),
                    ["Theater_Contact_Number"] = txtTheaterContact.Text.Trim(),
                    ["Theater_Total_Halls"] = totalHalls
                };

                _repository.Insert("Theater", values);
                lblStatus.Text = "Theater record inserted.";

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
                if (!int.TryParse(hfTheaterId.Value, out var theaterId))
                {
                    ShowError("Select a theater to update.");
                    return;
                }

                if (!int.TryParse(txtTotalHalls.Text, out var totalHalls))
                {
                    ShowError("Total halls must be a valid number.");
                    return;
                }

                const string sql = @"UPDATE Theater
                                     SET Theater_Name = :Theater_Name,
                                         Theater_City = :Theater_City,
                                         Theater_Location = :Theater_Location,
                                         Theater_Contact_Number = :Theater_Contact_Number,
                                         Theater_Total_Halls = :Theater_Total_Halls
                                     WHERE Theater_Id = :Theater_Id";

                var parameters = new[]
                {
                    new OracleParameter(":Theater_Name", txtTheaterName.Text.Trim()),
                    new OracleParameter(":Theater_City", txtTheaterCity.Text.Trim()),
                    new OracleParameter(":Theater_Location", txtTheaterLocation.Text.Trim()),
                    new OracleParameter(":Theater_Contact_Number", txtTheaterContact.Text.Trim()),
                    new OracleParameter(":Theater_Total_Halls", totalHalls),
                    new OracleParameter(":Theater_Id", theaterId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Theater record updated.";
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
                if (!int.TryParse(hfTheaterId.Value, out var theaterId))
                {
                    ShowError("Select a theater to delete.");
                    return;
                }

                const string sql = "DELETE FROM Theater WHERE Theater_Id = :Theater_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new OracleParameter(":Theater_Id", theaterId) });
                lblStatus.Text = "Theater record deleted.";

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

        protected void gvTheaters_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvTheaters.SelectedDataKey.Value);
            var row = _repository.GetById("Theater", id);
            if (row == null)
            {
                return;
            }

            hfTheaterId.Value = id.ToString();
            txtTheaterName.Text = row["Theater_Name"].ToString();
            txtTheaterCity.Text = row["Theater_City"].ToString();
            txtTheaterLocation.Text = row["Theater_Location"].ToString();
            txtTheaterContact.Text = row["Theater_Contact_Number"].ToString();
            txtTotalHalls.Text = row["Theater_Total_Halls"].ToString();

            lblStatus.Text = "Editing selected theater.";
            lblStatus.CssClass = "status";
        }

        private void BindGrid()
        {
            gvTheaters.DataSource = _repository.GetAll("Theater");
            gvTheaters.DataBind();
        }

        private void ClearFormFields()
        {
            hfTheaterId.Value = string.Empty;
            txtTheaterName.Text = string.Empty;
            txtTheaterCity.Text = string.Empty;
            txtTheaterLocation.Text = string.Empty;
            txtTheaterContact.Text = string.Empty;
            txtTotalHalls.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
