using MarketMonitorApp;
using MarketMonitorApp.Entities;
using MarketMonitorApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public class DistributorDetailsModel : PageModel
{
    private readonly MarketMonitorDbContext _context;
    private readonly IDistributorDetailsService _distributorDetailsService;
    private readonly IPriceScraper _priceScraper;

    // Konstruktor przyjmuj¹cy kontekst bazy danych
    public DistributorDetailsModel(MarketMonitorDbContext context, IDistributorDetailsService distributorDetailsService,
        IPriceScraper priceScraper)
    {
        _context = context;
        _distributorDetailsService = distributorDetailsService;
        _priceScraper = priceScraper;
}

    [BindProperty(SupportsGet = true)]
    public string DistributorName { get; set; }
    public Distributor Distributor { get; set; }
    public List<Category> Categories { get; set; }
    [BindProperty]
    public List<string> SelectedOptions { get; set; }

    public void OnGet()
    {
        Distributor = _distributorDetailsService.GetDistributorByName(DistributorName);
        Categories = Distributor.Categories.ToList();

    }

    public void OnPost()
    {
        if (SelectedOptions != null && SelectedOptions.Count > 0)
        {
            Distributor = _distributorDetailsService.GetDistributorByName(DistributorName); 
            if (Distributor != null)
            {
                SaveOptions(DistributorName, SelectedOptions);
            }
        }
    }

    private void SaveOptions(string distributorName, List<string> options)
    {
        var products = _priceScraper.GetPrices(options[0], Distributor).ToList();
        if(products != null) 
        {
            var actualization = _distributorDetailsService.AddActualization(products, Distributor);
             _distributorDetailsService.CompareProducts(actualization, Distributor.Id);
        }
        


    }
}
