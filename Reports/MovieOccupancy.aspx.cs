using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using CinemaTicketSystem.App_Code;

namespace CinemaTicketSystem.Reports
{
    public partial class MovieOccupancy : System.Web.UI.Page
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

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
            var movies = _db.ExecuteQuery("SELECT Movie_Id, Movie_Title FROM Movie ORDER BY Movie_Title");
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

            const string sql = @"WITH TheaterOccupancy AS
                                (
                                    SELECT
                                        t.Theater_Name,
                                        COUNT(tk.Ticket_Id) AS Paid_Tickets,
                                        SUM(h.Hall_Seating_Capacity) AS Total_Seats
                                    FROM Movie m
                                    INNER JOIN Show s ON s.Movie_Id = m.Movie_Id
                                    INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                    INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                    INNER JOIN Booking b ON b.Show_Id = s.Show_Id
                                    INNER JOIN Ticket tk ON tk.Booking_Id = b.Booking_Id
                                    WHERE m.Movie_Id = :MovieId
                                      AND UPPER(tk.Ticket_Status) = 'PAID'
                                    GROUP BY t.Theater_Name
                                )
                                SELECT
                                    Theater_Name,
                                    Paid_Tickets,
                                    Total_Seats,
                                    CASE WHEN Total_Seats = 0
                                         THEN 0
                                         ELSE ROUND((Paid_Tickets / Total_Seats) * 100, 2)
                                    END AS Occupancy_Percentage
                                FROM TheaterOccupancy
                                ORDER BY Occupancy_Percentage DESC
                                FETCH FIRST 3 ROWS ONLY";

            var table = _db.ExecuteQuery(sql, new OracleParameter(":MovieId", movieId));
            gvMovieOccupancy.DataSource = table;
            gvMovieOccupancy.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
