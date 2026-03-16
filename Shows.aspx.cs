using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Shows : System.Web.UI.Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                BindGrid();
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DateTime.TryParse(txtShowDate.Text, out var showDate))
                {
                    ShowError("Show date is invalid.");
                    return;
                }

                if (!TimeSpan.TryParse(txtShowTime.Text, out var showTime))
                {
                    ShowError("Show time is invalid.");
                    return;
                }

                if (!int.TryParse(ddlMovie.SelectedValue, out var movieId) || movieId <= 0)
                {
                    ShowError("Select a movie.");
                    return;
                }

                if (!int.TryParse(ddlHall.SelectedValue, out var hallId) || hallId <= 0)
                {
                    ShowError("Select a hall.");
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["Show_Date"] = showDate,
                    ["Show_Time"] = showTime.ToString(@"hh\\:mm"),
                    ["Show_Rating"] = txtShowRating.Text.Trim(),
                    ["Movie_Id"] = movieId,
                    ["Hall_Id"] = hallId
                };

                _repository.Insert("Show", values);
                lblStatus.Text = "Show record inserted.";
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
                if (!int.TryParse(hfShowId.Value, out var showId))
                {
                    ShowError("Select a show to update.");
                    return;
                }

                if (!DateTime.TryParse(txtShowDate.Text, out var showDate))
                {
                    ShowError("Show date is invalid.");
                    return;
                }

                if (!TimeSpan.TryParse(txtShowTime.Text, out var showTime))
                {
                    ShowError("Show time is invalid.");
                    return;
                }

                if (!int.TryParse(ddlMovie.SelectedValue, out var movieId) || movieId <= 0)
                {
                    ShowError("Select a movie.");
                    return;
                }

                if (!int.TryParse(ddlHall.SelectedValue, out var hallId) || hallId <= 0)
                {
                    ShowError("Select a hall.");
                    return;
                }

                const string sql = @"UPDATE Show
                                     SET Show_Date = :Show_Date,
                                         Show_Time = :Show_Time,
                                         Show_Rating = :Show_Rating,
                                         Movie_Id = :Movie_Id,
                                         Hall_Id = :Hall_Id
                                     WHERE Show_Id = :Show_Id";

                var parameters = new[]
                {
                    new OracleParameter(":Show_Date", showDate),
                    new OracleParameter(":Show_Time", showTime.ToString(@"hh\\:mm")),
                    new OracleParameter(":Show_Rating", txtShowRating.Text.Trim()),
                    new OracleParameter(":Movie_Id", movieId),
                    new OracleParameter(":Hall_Id", hallId),
                    new OracleParameter(":Show_Id", showId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Show record updated.";
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
                if (!int.TryParse(hfShowId.Value, out var showId))
                {
                    ShowError("Select a show to delete.");
                    return;
                }

                const string sql = "DELETE FROM Show WHERE Show_Id = :Show_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new OracleParameter(":Show_Id", showId) });
                lblStatus.Text = "Show record deleted.";
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

        protected void gvShows_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvShows.SelectedDataKey.Value);
            var row = _repository.GetById("Show", id);
            if (row == null)
            {
                return;
            }

            hfShowId.Value = id.ToString();
            txtShowDate.Text = Convert.ToDateTime(row["Show_Date"]).ToString("yyyy-MM-dd");
            txtShowTime.Text = row["Show_Time"].ToString();
            txtShowRating.Text = row["Show_Rating"].ToString();
            ddlMovie.SelectedValue = row["Movie_Id"].ToString();
            ddlHall.SelectedValue = row["Hall_Id"].ToString();

            lblStatus.Text = "Editing selected show.";
            lblStatus.CssClass = "status";
        }

        private void BindGrid()
        {
            const string sql = @"SELECT s.Show_Id, s.Show_Date, s.Show_Time, s.Show_Rating, m.Movie_Title, h.Hall_Number
                                 FROM Show s
                                 INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                 INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                 ORDER BY s.Show_Id DESC";
            gvShows.DataSource = _dataAccess.ExecuteDataTable(sql);
            gvShows.DataBind();
        }

        private void BindDropdowns()
        {
            var movieTable = _dataAccess.ExecuteDataTable("SELECT Movie_Id, Movie_Title FROM Movie ORDER BY Movie_Title");
            ddlMovie.DataSource = movieTable;
            ddlMovie.DataTextField = "Movie_Title";
            ddlMovie.DataValueField = "Movie_Id";
            ddlMovie.DataBind();
            ddlMovie.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Movie --", ""));

            var hallTable = _dataAccess.ExecuteDataTable("SELECT Hall_Id, Hall_Number FROM Hall ORDER BY Hall_Number");
            ddlHall.DataSource = hallTable;
            ddlHall.DataTextField = "Hall_Number";
            ddlHall.DataValueField = "Hall_Id";
            ddlHall.DataBind();
            ddlHall.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Hall --", ""));
        }

        private void ClearFormFields()
        {
            hfShowId.Value = string.Empty;
            txtShowDate.Text = string.Empty;
            txtShowTime.Text = string.Empty;
            txtShowRating.Text = string.Empty;
            ddlMovie.SelectedIndex = ddlMovie.Items.Count > 0 ? 0 : -1;
            ddlHall.SelectedIndex = ddlHall.Items.Count > 0 ? 0 : -1;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
