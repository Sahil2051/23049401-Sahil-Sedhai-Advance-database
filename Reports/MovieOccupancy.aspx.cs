using System;
using System.Data;
using System.Data.SqlClient;
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

            const string sql = @"WITH HallOccupancy AS
                                (
                                    SELECT
                                        m.Movie_Title,
                                        t.Theater_Name,
                                        h.Hall_Number,
                                        h.Hall_Seating_Capacity,
                                        SUM(CASE WHEN tk.Ticket_Status = 'Paid' THEN 1 ELSE 0 END) AS Paid_Tickets
                                    FROM [Show] s
                                    INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                    INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                    INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                    LEFT JOIN Booking b ON b.Show_Id = s.Show_Id
                                    LEFT JOIN Ticket tk ON tk.Booking_Id = b.Booking_Id
                                    WHERE s.Movie_Id = @MovieId
                                    GROUP BY m.Movie_Title, t.Theater_Name, h.Hall_Number, h.Hall_Seating_Capacity
                                )
                                SELECT TOP (3)
                                    Movie_Title,
                                    Theater_Name,
                                    Hall_Number,
                                    CASE WHEN Hall_Seating_Capacity = 0
                                         THEN 0
                                         ELSE CAST(Paid_Tickets * 100.0 / Hall_Seating_Capacity AS DECIMAL(10, 2))
                                    END AS Occupancy_Percentage
                                FROM HallOccupancy
                                ORDER BY Occupancy_Percentage DESC";

            var table = _db.ExecuteQuery(sql, new SqlParameter("@MovieId", movieId));
            gvMovieOccupancy.DataSource = table;
            gvMovieOccupancy.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
