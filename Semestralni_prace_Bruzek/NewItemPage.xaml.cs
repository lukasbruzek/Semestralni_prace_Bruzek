using System.Windows;
using System.Windows.Controls;

namespace Semestralka_Bruzek
{
    public partial class NewItemPage : Page
    {
        private NewInvoicePage _newInvoicePage;
        private InvoiceItem _editingItem;

        public NewItemPage(NewInvoicePage newInvoicePage, bool isVATPayer)
        {
            InitializeComponent();
            _newInvoicePage = newInvoicePage;
            if (isVATPayer == false)
            {
                cbVatRates.IsEnabled = false;
            }
        }

        public NewItemPage(NewInvoicePage newInvoicePage, bool isVATPayer, InvoiceItem itemToEdit) : this(newInvoicePage, isVATPayer)
        {
            _editingItem = itemToEdit;

            txtItemName.Text = _editingItem.ItemName;
            txtQuantity.Text = _editingItem.Quantity.ToString();
            txtPrice.Text = _editingItem.Price.ToString();
            txtTotal.Text = _editingItem.Total.ToString();

            cbVatRates.SelectedItem = cbVatRates.Items.OfType<VAT>().FirstOrDefault(v => v.VatPercentage == _editingItem.Tax);
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (cbVatRates.SelectedItem is VAT selectedVat)
            {
                string itemName = txtItemName.Text;
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    MessageBox.Show("Prosím, zadejte název položky.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Prosím, zadejte platné množství (kladné celé číslo).", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Prosím, zadejte platnou cenu (kladné desetinné číslo).", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtTotal.Text, out decimal total) || total <= 0)
                {
                    MessageBox.Show("Prosím, zadejte platný celkový součet (kladné desetinné číslo).", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                InvoiceItem newItem = new InvoiceItem
                {
                    ItemName = itemName,
                    Quantity = quantity,
                    Price = price,
                    Tax = selectedVat.VatPercentage,
                    Total = total
                };

                if (_editingItem != null)
                {
                    int index = _newInvoicePage.invoiceItems.IndexOf(_editingItem);
                    _newInvoicePage.invoiceItems[index] = newItem;
                }
                else
                {
                    _newInvoicePage.AddItemToList(newItem);
                }

                Window.GetWindow(this).Close();
            }
            else
            {
                MessageBox.Show("Prosím, vyberte sazbu DPH.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void cbVatRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            if (decimal.TryParse(txtQuantity.Text, out decimal quantity) &&
                decimal.TryParse(txtPrice.Text, out decimal price) &&
                cbVatRates.SelectedItem is VAT selectedVat)
            {
                decimal totalPrice = quantity * price * (1 + selectedVat.VatPercentageForCalculation);
                txtTotal.Text = totalPrice.ToString();
            }
        }

        private void txtQuantity_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void txtPrice_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (_editingItem != null)
            {
                _newInvoicePage.invoiceItems.Remove(_editingItem);
                Window.GetWindow(this).Close();
            }
        }
    }
}