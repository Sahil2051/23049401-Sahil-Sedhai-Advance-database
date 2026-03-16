using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Users : System.Web.UI.Page
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
                if (!ValidateUserInput(out var registrationDate))
                {
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["User_Name"] = txtUserName.Text.Trim(),
                    ["User_Email"] = txtUserEmail.Text.Trim(),
                    ["User_Contact_Number"] = txtUserContact.Text.Trim(),
                    ["User_Address"] = txtUserAddress.Text.Trim(),
                    ["User_Registration_Date"] = registrationDate
                };

                _repository.Insert("User", values);
                lblStatus.Text = "User record inserted.";

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
                if (!int.TryParse(hfUserId.Value, out var userId))
                {
                    ShowError("Select a user to update.");
                    return;
                }

                if (!ValidateUserInput(out var registrationDate))
                {
                    return;
                }

                const string sql = @"UPDATE AppUser
                                     SET User_Name = :User_Name,
                                         User_Email = :User_Email,
                                         User_Contact_Number = :User_Contact_Number,
                                         User_Address = :User_Address,
                                         User_Registration_Date = :User_Registration_Date
                                     WHERE User_Id = :User_Id";

                var parameters = new[]
                {
                    new OracleParameter(":User_Name", txtUserName.Text.Trim()),
                    new OracleParameter(":User_Email", txtUserEmail.Text.Trim()),
                    new OracleParameter(":User_Contact_Number", txtUserContact.Text.Trim()),
                    new OracleParameter(":User_Address", txtUserAddress.Text.Trim()),
                    new OracleParameter(":User_Registration_Date", registrationDate),
                    new OracleParameter(":User_Id", userId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "User record updated.";
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
                if (!int.TryParse(hfUserId.Value, out var userId))
                {
                    ShowError("Select a user to delete.");
                    return;
                }

                const string sql = "DELETE FROM AppUser WHERE User_Id = :User_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new OracleParameter(":User_Id", userId) });
                lblStatus.Text = "User record deleted.";

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

        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvUsers.SelectedDataKey.Value);
            var row = _repository.GetById("User", id);
            if (row == null)
            {
                return;
            }

            hfUserId.Value = id.ToString();
            txtUserName.Text = row["User_Name"].ToString();
            txtUserEmail.Text = row["User_Email"].ToString();
            txtUserContact.Text = row["User_Contact_Number"].ToString();
            txtUserAddress.Text = row["User_Address"].ToString();
            txtRegistrationDate.Text = Convert.ToDateTime(row["User_Registration_Date"]).ToString("yyyy-MM-dd");

            lblStatus.Text = "Editing selected user.";
            lblStatus.CssClass = "status";
        }

        private bool ValidateUserInput(out DateTime registrationDate)
        {
            registrationDate = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtUserEmail.Text) || string.IsNullOrWhiteSpace(txtUserContact.Text) || string.IsNullOrWhiteSpace(txtUserAddress.Text))
            {
                ShowError("All user fields are required.");
                return false;
            }

            if (!Regex.IsMatch(txtUserEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowError("Email format is invalid.");
                return false;
            }

            if (!DateTime.TryParse(txtRegistrationDate.Text, out registrationDate))
            {
                ShowError("Registration date is invalid.");
                return false;
            }

            return true;
        }

        private void BindGrid()
        {
            gvUsers.DataSource = _repository.GetAll("User");
            gvUsers.DataBind();
        }

        private void ClearFormFields()
        {
            hfUserId.Value = string.Empty;
            txtUserName.Text = string.Empty;
            txtUserEmail.Text = string.Empty;
            txtUserContact.Text = string.Empty;
            txtUserAddress.Text = string.Empty;
            txtRegistrationDate.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
