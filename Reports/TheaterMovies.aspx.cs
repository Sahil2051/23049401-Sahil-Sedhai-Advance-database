using System;
using CinemaTicketSystem.App_Code;

namespace CinemaTicketSystem.Reports
{
    public partial class TheaterMovies : System.Web.UI.Page
    {
        private readonly DatabaseHelper _db = new DatabaseHelper();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindReport();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindReport()
        {
            const string sql = @"SELECT
                                    t.Theater_Name,
                                    h.Hall_Number,
                                    m.Movie_Title,
                                    s.Show_Date,
                                    s.Show_Time
                                FROM [Show] s
                                INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                ORDER BY t.Theater_Name, h.Hall_Number, s.Show_Date, s.Show_Time";

            var table = _db.ExecuteQuery(sql);
            gvTheaterMovies.DataSource = table;
            gvTheaterMovies.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
