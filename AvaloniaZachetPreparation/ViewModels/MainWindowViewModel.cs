using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AvaloniaZachetPreparation.Models;
using System.Collections.ObjectModel;

namespace AvaloniaZachetPreparation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
   private readonly _41pProductsContext _db;
   public ObservableCollection<ProductCardVm> Cards { get; } = new();

   public MainWindowViewModel(_41pProductsContext db)
   {
      _db = db;
      LoadCards();
   }

   private void LoadCards()
   {
      var cards = _db.Products
         .Include(p => p.IdProductTypeNavigation)
         .Include(p => p.IdMaterialTypeNavigation)
         .Include(p => p.ProductWorkshops)
         .Select(p => new ProductCardVm
         {
            Type = p.IdProductTypeNavigation.ProductType1,
            ProductName = p.Name,
            ProductTime = (int)p.ProductWorkshops.Sum(w => w.Time),
            Article = p.Article,
            MinCostPartner = p.MinCostPartner,
            MainMaterial = p.IdMaterialTypeNavigation.MaterialType1
         })
         .ToList();
      
      Cards.Clear();
      foreach (var card in cards)
         Cards.Add(card);
   }
}