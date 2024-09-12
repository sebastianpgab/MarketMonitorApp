using MarketMonitorApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace MarketMonitorApp.Pages
{
    public class DistributorsModel : PageModel
    {
        private readonly MarketMonitorDbContext _marketMonitorDbContext;
        public List<Distributor> Distributors { get; set; }
        public List<string> DatesActualizations { get; set; }
        public DistributorsModel(MarketMonitorDbContext marketMonitorDbContext)
        {
            _marketMonitorDbContext = marketMonitorDbContext;
        }
        public void OnGet()
        {
            Distributors = _marketMonitorDbContext.Distributors.ToList();
            DatesActualizations = GetFormattedUniqueDates();
        }

        //To improve
        public IActionResult OnPost(string selectedDate)
        {
            if (!string.IsNullOrEmpty(selectedDate))
            {
                // Logika usuniêcia aktualizacji na podstawie wybranej daty
                var actualizationToRemove = _marketMonitorDbContext.Actualizations
                    .Select(a => a.LastActualization.ToString("d", CultureInfo.InvariantCulture)).ToList();

                var x = actualizationToRemove.FirstOrDefault(p => p.Equals(selectedDate));

                if (actualizationToRemove != null)
                {
                    //_marketMonitorDbContext.Actualizations.Remove(actualizationToRemove);
                    //_marketMonitorDbContext.SaveChanges();
                }
            }

            return RedirectToPage();
        }

        public List<string> GetFormattedUniqueDates()
        {
            return _marketMonitorDbContext.Actualizations
                .Select(p => p.LastActualization.Date)
                .Distinct()
                .Select(date => date.ToString("d", CultureInfo.InvariantCulture))
                .ToList();
        }

    }
}
