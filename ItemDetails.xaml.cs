using Datenlotsen.Data;
using Datenlotsen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Datenlotsen
{
    /// <summary>
    /// Interaction logic for ItemDetails.xaml
    /// </summary>
    public partial class ItemDetails : Window
    {
        public InventoryItem SelectedItem { get; set; } // Property to hold the selected inventory item

        public ItemDetails(InventoryItem selectedItem) // Constructor to initialize the window with the selected item
        {
            InitializeComponent(); // Initializes the XAML components (UI controls)

            SelectedItem = selectedItem; // Set the SelectedItem property to the item passed into the constructor

            // Bind the selected item data to the UI elements
            NameTextBox.Text = SelectedItem.Name; // Set the NameTextBox text to the selected item's name
            CategoryTextBox.Text = SelectedItem.Category; // Set the CategoryTextBox text to the selected item's category
            StockTextBox.Text = SelectedItem.StockQuantity.ToString(); // Set the StockTextBox text to the selected item's stock quantity

            // Set the ComboBox value based on the Stock value of the selected item
            var stockComboBoxItem = FilterComboBox.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Tag.ToString() == SelectedItem.Stock.ToString()); // Find the ComboBox item that matches the stock value

            if (stockComboBoxItem != null)
            {
                FilterComboBox.SelectedItem = stockComboBoxItem; // Set the ComboBox selected item to the found item
            }

            // Set the window size to 80% of the screen width and height
            double screenWidth = SystemParameters.PrimaryScreenWidth; // Get the primary screen width
            double screenHeight = SystemParameters.PrimaryScreenHeight; // Get the primary screen height
            this.Width = screenWidth * 0.8;  // Set window width to 80% of screen width
            this.Height = screenHeight * 0.8; // Set window height to 80% of screen height
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Center the window on the screen
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) // Event handler for the Save button click
        {
            // Validate the input fields before saving

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == "Item Name")
            {
                MessageBox.Show("Item Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); // Show an error message
                return; // Exit the method early if validation fails
            }

            // Validate Stock Quantity: Ensure it's a non-negative integer
            if (string.IsNullOrWhiteSpace(StockTextBox.Text) || !int.TryParse(StockTextBox.Text, out int stockQuantity) || stockQuantity < 0)
            {
                MessageBox.Show("Stock Quantity must be a non-negative integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); // Show an error message
                return; // Exit the method early if validation fails
            }

            // Get the selected stock status (In Stock or Out of Stock) from the ComboBox
            var selectedComboBoxItem = FilterComboBox.SelectedItem as ComboBoxItem; // Get the selected item from the ComboBox
            bool isInStock = selectedComboBoxItem != null && bool.TryParse(selectedComboBoxItem.Tag.ToString(), out bool stockValue) && stockValue; // Parse the stock value from the ComboBox Tag

            // If validation passes, update the selected item with the new data
            SelectedItem.Name = NameTextBox.Text; // Update the Name property of the selected item
            SelectedItem.Category = CategoryTextBox.Text; // Update the Category property of the selected item
            SelectedItem.StockQuantity = stockQuantity; // Update the Stock Quantity property of the selected item
            SelectedItem.Stock = isInStock; // Update the Stock (availability) property of the selected item
            SelectedItem.LastUpdated = DateTime.Now; // Set the Last Updated timestamp to the current date and time

            // Save the updated item back to the database
            using (var context = new InventoryDbContext()) // Open a database context to interact with the database
            {
                var existingItem = context.InventoryItems.Find(SelectedItem.Id); // Find the item in the database by its ID
                if (existingItem != null)
                {
                    // Update the fields of the existing item with the new values
                    existingItem.Name = SelectedItem.Name;
                    existingItem.Category = SelectedItem.Category;
                    existingItem.StockQuantity = SelectedItem.StockQuantity;
                    existingItem.Stock = SelectedItem.Stock; // Update stock availability
                    existingItem.LastUpdated = DateTime.Now; // Update LastUpdated timestamp

                    context.SaveChanges();  // Save the changes to the database
                }
            }

            MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); // Show success message

            // Optionally, close the window and return to the main window
            this.DialogResult = true; // Set DialogResult to true indicating success
            this.Close(); // Close the ItemDetails window
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) // Event handler for the Cancel button click
        {
            this.Close(); // Close the ItemDetails window without saving any changes
        }

    }
}
