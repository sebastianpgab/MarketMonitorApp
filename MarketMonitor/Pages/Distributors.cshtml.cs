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

        public IActionResult OnPost(string selectedDate)
        {
            if (!string.IsNullOrEmpty(selectedDate))
            {
                var actualization = _marketMonitorDbContext.Actualizations
                    .Where(a => a.LastActualization.Date == DateTime.ParseExact(selectedDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).Date)
                    .ToList();

                if (actualization.Any())
                {
                    _marketMonitorDbContext.Actualizations.RemoveRange(actualization);
                    _marketMonitorDbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Akcja zosta³a wykonana pomyœlnie!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Nie znaleziono wpisów do usuniêcia!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Proszê wybraæ datê!";
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
