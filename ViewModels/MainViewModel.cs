using CommunityToolkit.Mvvm.ComponentModel;
using Datenlotsen.Data;
using Datenlotsen.Models;
using System.Collections.ObjectModel;

namespace Datenlotsen.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<InventoryItem> _inventoryItems;

        public ObservableCollection<InventoryItem> InventoryItems
        {
            get => _inventoryItems;
            set => SetProperty(ref _inventoryItems, value);
        }

        public MainViewModel()
        {
            LoadItems();
        }

        public void LoadItems()
        {
            try
            {
                using var context = new InventoryDbContext();
              //  context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
               

                InventoryItems = new ObservableCollection<InventoryItem>(context.InventoryItems.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
