using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaZachetPreparation.Models;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaZachetPreparation.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
   // Шаг 4-5: константы для фильтра и сортировки.
   private const string AllTypesOption = "Все";
   private const string NoSortOption = "Без сортировки";
   private const string SortAscOption = "Стоимость по возрастанию";
   private const string SortDescOption = "Стоимость по убыванию";

   // Шаг 2: контекст БД и локальный кэш карточек.
   private readonly _41pProductsContext _db;
   private List<ProductCardVm> _allCards = new();

   // Шаг 1/4/5/6: коллекции и команда для UI.
   public ObservableCollection<ProductCardVm> Cards { get; } = new();
   public ObservableCollection<string> AvailableTypes { get; } = new();
   public IRelayCommand<ProductCardVm> DeleteProductCommand { get; }
   public ObservableCollection<string> SortOptions { get; } = new()
   {
      NoSortOption,
      SortAscOption,
      SortDescOption
   };

   private string _searchText = "";
   // Шаг 3: поиск по названию в реальном времени.
   public string SearchText
   {
      get => _searchText;
      set
      {
         if (_searchText == value) return;
         _searchText = value;
         OnPropertyChanged();
         ApplyFilters();
      }
   }

   private string _selectedType = AllTypesOption;
   // Шаг 4: выбранный тип для фильтрации.
   public string SelectedType
   {
      get => _selectedType;
      set
      {
         if (_selectedType == value) return;
         _selectedType = value;
         OnPropertyChanged();
         ApplyFilters();
      }
   }

   private string _selectedSort = NoSortOption;
   // Шаг 5: выбранная сортировка по минимальной стоимости.
   public string SelectedSort
   {
      get => _selectedSort;
      set
      {
         if (_selectedSort == value) return;
         _selectedSort = value;
         OnPropertyChanged();
         ApplyFilters();
      }
   }

   public MainWindowViewModel()
   {
      // Шаг 2: подключение к PostgreSQL через EF Core.
      var options = new DbContextOptionsBuilder<_41pProductsContext>()
         .UseNpgsql("Host=edu.ngknn.ru;Port=5442;Database=41P_products;Username=21P;Password=123")
         .Options;

      _db = new _41pProductsContext(options);
      DeleteProductCommand = new RelayCommand<ProductCardVm>(DeleteProduct);
      LoadCards();
   }

   private void LoadCards()
   {
      // Шаг 2: загружаем карточки из БД и маппим в ProductCardVm.
      _allCards = _db.Products
         .Include(p => p.IdProductTypeNavigation)
         .Include(p => p.IdMaterialTypeNavigation)
         .Include(p => p.ProductWorkshops)
         .Select(p => new ProductCardVm
         {
            Id = p.Id,
            Type = p.IdProductTypeNavigation.ProductType1,
            ProductName = p.Name,
            ProductTime = (int)p.ProductWorkshops.Sum(w => w.Time),
            Article = p.Article,
            MinCostPartner = p.MinCostPartner,
            MainMaterial = p.IdMaterialTypeNavigation.MaterialType1
         })
         .ToList();

      // Шаг 4: формируем список типов для ComboBox фильтра.
      AvailableTypes.Clear();
      AvailableTypes.Add(AllTypesOption);
      foreach (var type in _allCards.Select(x => x.Type.Trim()).Distinct().OrderBy(x => x))
         AvailableTypes.Add(type);

      SelectedType = AllTypesOption;
      SelectedSort = NoSortOption;
      ApplyFilters();
   }

   private void ApplyFilters()
   {
      // Шаг 3-5: применяем поиск + фильтр + сортировку одновременно.
      IEnumerable<ProductCardVm> query = _allCards;

      if (!string.IsNullOrWhiteSpace(SearchText))
         query = query.Where(x => x.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

      if (!string.Equals(SelectedType, AllTypesOption, StringComparison.Ordinal))
         query = query.Where(x => string.Equals(x.Type.Trim(), SelectedType.Trim(), StringComparison.OrdinalIgnoreCase));

      query = SelectedSort switch
      {
         SortAscOption => query.OrderBy(x => x.MinCostPartner),
         SortDescOption => query.OrderByDescending(x => x.MinCostPartner),
         _ => query
      };

      Cards.Clear();
      foreach (var item in query)
         Cards.Add(item);
   }

   private void DeleteProduct(ProductCardVm? card)
   {
      // Шаг 6: удаляем запись из БД и сразу обновляем UI-список.
      if (card is null)
         return;

      var entity = _db.Products.FirstOrDefault(p => p.Id == card.Id);
      if (entity is null)
         return;

      _db.Products.Remove(entity);
      _db.SaveChanges();

      _allCards.RemoveAll(x => x.Id == card.Id);
      ApplyFilters();
   }
}

