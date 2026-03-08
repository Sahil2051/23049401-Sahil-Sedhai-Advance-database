using System;
using System.Data;
using System.Data.SqlClient;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class ReportTheaterMovies : System.Web.UI.Page
    {
        private readonly SqlDataAccess _dataAccess = new SqlDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTheaters();
                BindReport();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            BindReport();
        }

        private void BindTheaters()
        {
            var theaters = _dataAccess.ExecuteDataTable("SELECT Theater_Id, Theater_Name FROM Theater ORDER BY Theater_Name");
            ddlTheaters.DataSource = theaters;
            ddlTheaters.DataTextField = "Theater_Name";
            ddlTheaters.DataValueField = "Theater_Id";
            ddlTheaters.DataBind();
            ddlTheaters.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- All Theaters --", ""));
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
                                WHERE (@TheaterId IS NULL OR t.Theater_Id = @TheaterId)
                                ORDER BY t.Theater_Name, h.Hall_Number, s.Show_Date, s.Show_Time";

            object theaterParam = DBNull.Value;
            if (int.TryParse(ddlTheaters.SelectedValue, out var theaterId))
            {
                theaterParam = theaterId;
            }

            var table = _dataAccess.ExecuteDataTable(sql, new[] { new SqlParameter("@TheaterId", theaterParam) });
            gvTheaterMovies.DataSource = table;
            gvTheaterMovies.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
