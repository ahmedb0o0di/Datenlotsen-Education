using Datenlotsen.Data;
using Datenlotsen.Models;
using Datenlotsen.ViewModels;
using Microsoft.EntityFrameworkCore;
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
    public partial class AddItem : Window
    {
        public AddItem() // Constructor for the AddItem class
        {
            InitializeComponent(); // Initializes the XAML components (UI controls)


            this.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Position the window at the center of the screen
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e) // Event handler for the "Add Item" button click
        {
            // Validate Name field: Check if the Name field is empty or has the placeholder text
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == "Item Name")
            {
                MessageBox.Show("Item Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); // Show an error message
                return; // Exit the method early if validation fails
            }

            // Validate Stock Quantity field: Check if the Stock field is empty or has the placeholder text
            if (string.IsNullOrWhiteSpace(StockTextBox.Text) || StockTextBox.Text == "Quantity")
            {
                MessageBox.Show("Stock Quantity is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); // Show an error message
                return; // Exit the method early if validation fails
            }

            // Try parsing the stock quantity into an integer and check that it's non-negative
            if (!int.TryParse(StockTextBox.Text, out int stockQuantity) || stockQuantity < 0)
            {
                MessageBox.Show("Stock Quantity must be a non-negative integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); // Show an error message
                return; // Exit the method early if validation fails
            }

            var selectedComboBoxItem = FilterComboBox.SelectedItem as ComboBoxItem; // Get the selected item in the ComboBox (for stock status)
            bool isInStock = false; // Default value if no item is selected

            // If a valid ComboBox item is selected, get the stock status from the Tag property
            if (selectedComboBoxItem != null)
            {
                bool.TryParse(selectedComboBoxItem.Tag.ToString(), out isInStock); // Convert the Tag to a boolean value indicating stock status
            }

            // Create the new inventory item using the user input
            var newItem = new InventoryItem
            {
                Name = NameTextBox.Text, // Set the Name property from the text input
                Category = CategoryTextBox.Text, // Set the Category property from the text input
                StockQuantity = stockQuantity, // Set the Stock Quantity property from the validated input
                Stock = isInStock,  // Set the Stock property based on the selected stock status
                LastUpdated = DateTime.Now // Set the Last Updated property to the current date and time
            };

            // Open a database context to interact with the inventory database
            using (var context = new InventoryDbContext())
            {
                context.InventoryItems.Add(newItem); // Add the new item to the database
                try
                {
                    context.SaveChanges(); // Save the changes to the database
                }
                catch (DbUpdateException ex) // Catch any exceptions that occur during the database save operation
                {
                    MessageBox.Show($"Error: {ex.InnerException?.Message}"); // Show an error message with the exception details
                }
            }

            MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); // Show success message

            // Close the AddItem window and return DialogResult as true to indicate success
            this.DialogResult = true;
            this.Close();
        }

        private void StockTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) // Event handler for previewing text input in the StockTextBox
        {
            e.Handled = !int.TryParse(e.Text, out _); // Block non-numeric input in the StockTextBox (only allow integers)
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) // Event handler for the Cancel button click
        {
            this.Close(); // Close the AddItem window without saving any data
        }

    }
}
