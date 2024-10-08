﻿using CsvHelper;
using MarketMonitorApp.Entities;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarketMonitorApp.Services
{
    public interface IDistributorDetailsService
    {
        Distributor GetDistributorByName(string name);
        Actualization AddActualization(List<Product> products, Distributor distributor, Category category);
        List<Product> CompareProducts(Actualization actualization, Category category);
        bool ExportProductsToCsv(List<Product> comparedProdcuts, Actualization actualization, string categoryName, string pathToSaveFile);
        public List<Product> LastUpdatedProducts(Actualization actualization, Category category);
        public Category GetCategoryByLink(string link);

    }

    public class DistributorDetailsService : IDistributorDetailsService
    {
        private readonly MarketMonitorDbContext _context;
        private readonly IFileWriter _fileWriter;

        public DistributorDetailsService(MarketMonitorDbContext context, IFileWriter fileWriter)
        {
            _context = context;
            _fileWriter = fileWriter;
        }

        public Distributor GetDistributorByName(string name)
        {
            var distributor = _context.Distributors
                                           .Include(d => d.Categories)
                                           .FirstOrDefault(d => d.Name == name);
            if (distributor == null)
            {
                return null;
            }
            return distributor;
        }

        public Category GetCategoryByLink(string link)
        {
            var category = _context.Categories.FirstOrDefault(p => p.LinkToCategory == link);
            if (category == null)
            {
                return null;
            }
            return category;
        }

        public Actualization AddActualization(List<Product> products, Distributor distributor, Category category)
        {
            var newActualization = new Actualization();
            newActualization.Products = products;
            newActualization.DistributorId = distributor.Id;
            newActualization.Distributor = distributor;
            newActualization.CategoryId = category.Id;

            _context.Actualizations.Add(newActualization);
            _context.SaveChanges();
            return newActualization;
        }

        public List<Product> CompareProducts(Actualization actualization, Category category)
        {
            var productsTab = LastUpdatedProducts(actualization, category);
            if (productsTab == null || !productsTab.Any())
            {
                return new List<Product>();
            }

            var latestUpdatedProducts = actualization.Products.ToList();

            if (!latestUpdatedProducts.Any())
            {
                return new List<Product>();
            }

            List<Product> comparedProducts = new List<Product>();

            var productsTabIds = new HashSet<string>(productsTab.Select(p => p.IdProduct));

            foreach (var product in latestUpdatedProducts)
            {
                if (!productsTabIds.Contains(product.IdProduct))
                {
                    product.IsNew = true;
                    comparedProducts.Add(product);
                }
                else
                {
                    var originalProduct = productsTab.FirstOrDefault(p => p.IdProduct == product.IdProduct);
                    if (originalProduct != null && originalProduct.Price != product.Price)
                    {
                        comparedProducts.Add(product);
                    }
                }
            }

            IsProductDeleted(productsTab, latestUpdatedProducts, comparedProducts);

            _context.SaveChanges();

            return comparedProducts;
        }

        public void IsProductDeleted(List<Product> oldProducts, List<Product> newProdcuts, List<Product> comparedProducts)
        {
            var newProductIds = new HashSet<string>(newProdcuts.Select(p => p.IdProduct));

            foreach (var product in oldProducts)
            {
                if (!newProductIds.Contains(product.IdProduct))
                {
                    product.IsNew = false;
                    product.IsDeleted = true;
                    comparedProducts.Add(product);
                }
            }
        }

        public bool ExportProductsToCsv(List<Product> comparedProducts, Actualization actualization, string categoryName, string pathToSaveFile)
        {
            if (comparedProducts == null || comparedProducts.Count == 0)
            {
                return false;
            }

            var distributorName = actualization.Distributor.Name;

            DateTime currentData = DateTime.Now;

            string currentDataConverted = currentData.ToString("dd MM yyyy__HH-mm-ss");
            string fileName = $"{distributorName}-{categoryName}-{currentDataConverted}-products.csv";
            string fullPath = Path.Combine(pathToSaveFile, fileName);

            _fileWriter.WriteToFile(fullPath, comparedProducts);

            return true;
        }

        public virtual List<Product> LastUpdatedProducts(Actualization actualization, Category category)
        {
            var actualizations = _context.Actualizations.Max(p => p.Id);

            if (actualizations >= 1)
            {
                var lastActualization = _context.Actualizations
                    .Include(p => p.Products)
                    .Where(p => p.DistributorId == actualization.DistributorId && p.CategoryId == category.Id)
                    .OrderByDescending(p => p.Id)
                    .Skip(1) // Pomija ostatni dodany element, czyli bierze przedostatni
                    .FirstOrDefault();

                if (lastActualization != null)
                {
                    return lastActualization.Products.ToList();
                }
            }
            return new List<Product>();
        }
    }
}
