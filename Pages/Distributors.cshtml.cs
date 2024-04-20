using MarketMonitorApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MarketMonitorApp.Pages
{
    public class DistributorsModel : PageModel
    {
        private readonly MarketMonitorDbContext _marketMonitorDbContext;
        public List<Distributor> Distributors { get; set; }
        public DistributorsModel(MarketMonitorDbContext marketMonitorDbContext)
        {
            _marketMonitorDbContext =  marketMonitorDbContext;
        }
        public void OnGet()
        {
           Distributors = _marketMonitorDbContext.Distributors.ToList();
        }
    }
}
