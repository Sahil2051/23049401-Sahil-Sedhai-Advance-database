using System;
using System.Data;
using System.Data.SqlClient;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class ReportMovieOccupancy : System.Web.UI.Page
    {
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMovies();
                BindReport();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindMovies()
        {
            var movies = _dataAccess.ExecuteDataTable("SELECT Movie_Id, Movie_Title FROM Movie ORDER BY Movie_Title");
            ddlMovies.DataSource = movies;
            ddlMovies.DataTextField = "Movie_Title";
            ddlMovies.DataValueField = "Movie_Id";
            ddlMovies.DataBind();
            ddlMovies.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Movie --", ""));
        }

        private void BindReport()
        {
            if (!int.TryParse(ddlMovies.SelectedValue, out var movieId))
            {
                gvMovieOccupancy.DataSource = new DataTable();
                gvMovieOccupancy.DataBind();
                lblStatus.Text = "Select a movie to load report data.";
                return;
            }

            const string sql = @"WITH HallSales AS
                                (
                                    SELECT
                                        h.Hall_Id,
                                        h.Hall_Number,
                                        h.Hall_Seating_Capacity,
                                        t.Theater_Name,
                                        COUNT(tk.Ticket_Id) AS Sold_Seats
                                    FROM [Show] s
                                    INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                    INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                    LEFT JOIN Booking b ON b.Show_Id = s.Show_Id
                                    LEFT JOIN Ticket tk ON tk.Booking_Id = b.Booking_Id
                                    WHERE s.Movie_Id = @MovieId
                                    GROUP BY h.Hall_Id, h.Hall_Number, h.Hall_Seating_Capacity, t.Theater_Name
                                )
                                SELECT TOP (3)
                                    Theater_Name,
                                    Hall_Number,
                                    Hall_Seating_Capacity,
                                    Sold_Seats,
                                    CASE
                                        WHEN Hall_Seating_Capacity = 0 THEN 0
                                        ELSE (CAST(Sold_Seats AS DECIMAL(10, 2)) * 100.0) / Hall_Seating_Capacity
                                    END AS Occupancy_Percentage
                                FROM HallSales
                                ORDER BY Occupancy_Percentage DESC, Sold_Seats DESC";

            var table = _dataAccess.ExecuteDataTable(sql, new[] { new SqlParameter("@MovieId", movieId) });
            gvMovieOccupancy.DataSource = table;
            gvMovieOccupancy.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
