using System;
using System.Web.UI;
using CinemaTicketSystem.DataAccess;

namespace CinemaTicketSystem
{
    public partial class _Default : Page
    {
        private readonly CinemaRepository _repository = new CinemaRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TryLoadCounts();
            }
        }

        private void TryLoadCounts()
        {
            try
            {
                LoadCounts();
            }
            catch
            {
                // Keep the dashboard usable even when the database is not available yet.
                lblUsersCount.Text = "0";
                lblMoviesCount.Text = "0";
                lblTheatersCount.Text = "0";
                lblShowsCount.Text = "0";
                lblTicketsCount.Text = "0";
                lblBookingsCount.Text = "0";
                lblHallsCount.Text = "0";
                lblGeneratedAt.Text = "Database unavailable";
            }
        }

        private void LoadCounts()
        {
            lblUsersCount.Text = _repository.GetCount("User").ToString();
            lblMoviesCount.Text = _repository.GetCount("Movie").ToString();
            lblTheatersCount.Text = _repository.GetCount("Theater").ToString();
            lblShowsCount.Text = _repository.GetCount("Show").ToString();
            lblTicketsCount.Text = _repository.GetCount("Ticket").ToString();
            lblBookingsCount.Text = _repository.GetCount("Booking").ToString();
            lblHallsCount.Text = _repository.GetCount("Hall").ToString();
            lblGeneratedAt.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
    }
}