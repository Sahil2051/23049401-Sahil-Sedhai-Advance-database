using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class Movies : System.Web.UI.Page
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
                if (!int.TryParse(txtDuration.Text, out var duration))
                {
                    ShowError("Duration must be a valid number.");
                    return;
                }

                if (!DateTime.TryParse(txtReleaseDate.Text, out var releaseDate))
                {
                    ShowError("Release date is invalid.");
                    return;
                }

                var values = new Dictionary<string, object>
                {
                    ["Movie_Title"] = txtMovieTitle.Text.Trim(),
                    ["Movie_Duration"] = duration,
                    ["Movie_Language"] = txtLanguage.Text.Trim(),
                    ["Movie_Genre"] = txtGenre.Text.Trim(),
                    ["Movie_Release_Date"] = releaseDate
                };

                _repository.Insert("Movie", values);
                lblStatus.Text = "Movie record inserted.";

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
                if (!int.TryParse(hfMovieId.Value, out var movieId))
                {
                    ShowError("Select a movie to update.");
                    return;
                }

                if (!int.TryParse(txtDuration.Text, out var duration))
                {
                    ShowError("Duration must be a valid number.");
                    return;
                }

                if (!DateTime.TryParse(txtReleaseDate.Text, out var releaseDate))
                {
                    ShowError("Release date is invalid.");
                    return;
                }

                const string sql = @"UPDATE Movie
                                     SET Movie_Title = @Movie_Title,
                                         Movie_Duration = @Movie_Duration,
                                         Movie_Language = @Movie_Language,
                                         Movie_Genre = @Movie_Genre,
                                         Movie_Release_Date = @Movie_Release_Date
                                     WHERE Movie_Id = @Movie_Id";

                var parameters = new[]
                {
                    new SqlParameter("@Movie_Title", txtMovieTitle.Text.Trim()),
                    new SqlParameter("@Movie_Duration", duration),
                    new SqlParameter("@Movie_Language", txtLanguage.Text.Trim()),
                    new SqlParameter("@Movie_Genre", txtGenre.Text.Trim()),
                    new SqlParameter("@Movie_Release_Date", releaseDate),
                    new SqlParameter("@Movie_Id", movieId)
                };

                _dataAccess.ExecuteNonQuery(sql, parameters);
                lblStatus.Text = "Movie record updated.";
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
                if (!int.TryParse(hfMovieId.Value, out var movieId))
                {
                    ShowError("Select a movie to delete.");
                    return;
                }

                const string sql = "DELETE FROM Movie WHERE Movie_Id = @Movie_Id";
                _dataAccess.ExecuteNonQuery(sql, new[] { new SqlParameter("@Movie_Id", movieId) });
                lblStatus.Text = "Movie record deleted.";

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

        protected void gvMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(gvMovies.SelectedDataKey.Value);
            var row = _repository.GetById("Movie", id);
            if (row == null)
            {
                return;
            }

            hfMovieId.Value = id.ToString();
            txtMovieTitle.Text = row["Movie_Title"].ToString();
            txtDuration.Text = row["Movie_Duration"].ToString();
            txtLanguage.Text = row["Movie_Language"].ToString();
            txtGenre.Text = row["Movie_Genre"].ToString();
            txtReleaseDate.Text = Convert.ToDateTime(row["Movie_Release_Date"]).ToString("yyyy-MM-dd");

            lblStatus.Text = "Editing selected movie.";
            lblStatus.CssClass = "status";
        }

        private void BindGrid()
        {
            gvMovies.DataSource = _repository.GetAll("Movie");
            gvMovies.DataBind();
        }

        private void ClearFormFields()
        {
            hfMovieId.Value = string.Empty;
            txtMovieTitle.Text = string.Empty;
            txtDuration.Text = string.Empty;
            txtLanguage.Text = string.Empty;
            txtGenre.Text = string.Empty;
            txtReleaseDate.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = "status error";
        }
    }
}
