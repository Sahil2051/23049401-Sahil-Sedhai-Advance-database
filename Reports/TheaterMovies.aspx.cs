using System;
using Oracle.ManagedDataAccess.Client;
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
                BindTheaters();
                BindReport();
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (ddlTheaters.Items.Count == 0)
            {
                BindTheaters();
            }

            BindReport();
        }

        private void BindTheaters()
        {
            var theaters = _db.ExecuteQuery("SELECT Theater_Id, Theater_Name FROM Theater ORDER BY Theater_Name");
            var selectedValue = ddlTheaters.SelectedValue;

            ddlTheaters.DataSource = theaters;
            ddlTheaters.DataTextField = "Theater_Name";
            ddlTheaters.DataValueField = "Theater_Id";
            ddlTheaters.DataBind();
            ddlTheaters.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- All Theaters --", ""));

            if (!string.IsNullOrWhiteSpace(selectedValue)
                && ddlTheaters.Items.FindByValue(selectedValue) != null)
            {
                ddlTheaters.SelectedValue = selectedValue;
            }
            else
            {
                ddlTheaters.SelectedIndex = 0;
            }

            if (theaters.Rows.Count == 0)
            {
                lblStatus.Text = "No theaters available. Add theater records first.";
                lblStatus.CssClass = "status error";
            }
        }

        private void BindReport()
        {
            const string sql = @"SELECT
                                    t.Theater_Name,
                                    h.Hall_Number,
                                    m.Movie_Title,
                                    s.Show_Date,
                                    s.Show_Time
                                FROM Show s
                                INNER JOIN Hall h ON h.Hall_Id = s.Hall_Id
                                INNER JOIN Theater t ON t.Theater_Id = h.Theater_Id
                                INNER JOIN Movie m ON m.Movie_Id = s.Movie_Id
                                WHERE t.Theater_Id = NVL(:TheaterId, t.Theater_Id)
                                ORDER BY t.Theater_Name, h.Hall_Number, s.Show_Date, s.Show_Time";

            object theaterId = DBNull.Value;
            if (int.TryParse(ddlTheaters.SelectedValue, out var selectedTheaterId))
            {
                theaterId = selectedTheaterId;
            }

            var table = _db.ExecuteQuery(sql, new OracleParameter(":TheaterId", theaterId));
            gvTheaterMovies.DataSource = table;
            gvTheaterMovies.DataBind();
            lblStatus.Text = table.Rows.Count + " record(s) loaded.";
        }
    }
}
