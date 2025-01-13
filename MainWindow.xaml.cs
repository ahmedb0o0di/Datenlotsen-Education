using Datenlotsen.Data;
using Datenlotsen.Models;
using Datenlotsen.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Datenlotsen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() // Constructor to initialize the main window
        {
            InitializeComponent(); // Initializes the XAML components (UI controls)

            DataContext = new ViewModels.MainViewModel(); // Set the DataContext to the MainViewModel to bind data to the UI elements

            double screenWidth = SystemParameters.PrimaryScreenWidth; // Get the primary screen width
            double screenHeight = SystemParameters.PrimaryScreenHeight; // Get the primary screen height

            this.WindowState = WindowState.Maximized;  // Maximize the window on startup
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) // Event handler for the Delete button click
        {
            // Get the InventoryItem bound to the row
            var button = sender as Button; // Cast sender to a Button
            var itemToDelete = button?.Tag as InventoryItem; // Get the InventoryItem object associated with the button's Tag

            if (itemToDelete == null) return; // If no item to delete, exit the method

            // Confirm deletion
            var result = MessageBox.Show($"Are you sure you want to delete '{itemToDelete.Name}'?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning); // Show confirmation dialog

            if (result == MessageBoxResult.Yes) // If user confirms
            {
                // Delete the item from the database
                using (var context = new InventoryDbContext()) // Open a database context to interact with the database
                {
                    context.InventoryItems.Remove(itemToDelete); // Remove the item from the context
                    context.SaveChanges(); // Save the changes to the database
                }

                // Refresh the UI
                (DataContext as MainViewModel)?.LoadItems(); // Reload the items from the database to reflect the deletion
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) // Event handler for the Search button click
        {
            string searchTerm = SearchTextBox.Text.Trim().ToLower();  // Get the search term entered by the user
            string selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(); // Get the selected filter (Low Stock, In Stock, All)

            using (var context = new InventoryDbContext()) // Open a database context to interact with the database
            {
                // Retrieve all items from the database
                var items = context.InventoryItems.ToList();

                // Apply search term filter for Name, Category, and Stock Quantity
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    items = items.Where(item =>
                        item.Name.ToLower().Contains(searchTerm) || // Check if the item's name contains the search term
                        item.Category.ToLower().Contains(searchTerm) || // Check if the item's category contains the search term
                        item.StockQuantity.ToString().Contains(searchTerm)) // Check if the stock quantity contains the search term
                    .ToList();
                }

                // Apply stock status filter (Low Stock, In Stock, etc.)
                if (selectedFilter == "In Stock")
                {
                    // Filter items that are in stock
                    items = items.Where(item => item.Stock == true).ToList();
                }
                else if (selectedFilter == "Out of Stock")
                {
                    // Filter items that are out of stock
                    items = items.Where(item => item.Stock == false).ToList();
                }

                // Update the DataGrid's ItemsSource to display the filtered list of items
                InventoryDataGrid.ItemsSource = items;
            }
        }

        private void Details_Click(object sender, RoutedEventArgs e) // Event handler for the Details button click
        {
            // Get the selected item from the DataGrid
            InventoryItem selectedItem = (InventoryItem)((Button)sender).Tag; // Get the InventoryItem associated with the button's Tag

            if (selectedItem != null) // If a valid item is selected
            {
                // Open the ItemDetails window and pass the selected item to it
                ItemDetails itemDetailsWindow = new ItemDetails(selectedItem);
                itemDetailsWindow.ShowDialog(); // Show the ItemDetails window

                if (itemDetailsWindow.DialogResult == true) // If the user confirmed the changes in ItemDetails window
                {
                    DataContext = new ViewModels.MainViewModel(); // Reload the data in the main view model
                }
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e) // Event handler for the Add Item button click
        {
            var addNewItem = new AddItem(); // Create a new instance of the AddItem window
            addNewItem.ShowDialog(); // Show the AddItem window

            if (addNewItem.DialogResult == true) // If the user confirmed the new item addition
            {
                DataContext = new ViewModels.MainViewModel(); // Reload the data in the main view model
            }
            // Optionally, close the current window if needed
            // this.Close();
        }

    }
}