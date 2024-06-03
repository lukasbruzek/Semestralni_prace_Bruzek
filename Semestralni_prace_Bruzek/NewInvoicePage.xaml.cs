using Semestralka;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Semestralka_Bruzek
{
    public partial class NewInvoicePage : Page
    {
        private ObservableCollection<BankInfo> bankAccounts;
        public ObservableCollection<InvoiceItem> invoiceItems = new ObservableCollection<InvoiceItem>();
        private Customer currentCustomer;
        private bool isEditingExistingInvoice;
        int InvoiceId;
        bool isCompanyHere = true;

        public NewInvoicePage(ObservableCollection<BankInfo> bankAccounts)
        {
            InitializeComponent();
            this.bankAccounts = bankAccounts;

            cbBankAccounts.ItemsSource = bankAccounts;
            cbBankAccounts.DisplayMemberPath = "BankName";
            lvInvoiceItems.ItemsSource = invoiceItems;
            isEditingExistingInvoice = false;
            lvInvoiceItems.MouseDoubleClick += LvInvoiceItems_MouseDoubleClick;
            UpdateCustomerButtonText();
            LoadCompanyInfo();
            InvoiceText.Text = "Nová faktura";
        }

        private void LvInvoiceItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvInvoiceItems.SelectedItem is InvoiceItem selectedItem)
            {
                bool isVATPayer = GetIsVATPayer();
                NewItemPage newItemPage = new NewItemPage(this, isVATPayer, selectedItem);

                Window newItemWindow = new Window();
                newItemWindow.Content = newItemPage;
                newItemWindow.ShowDialog();
            }
            UpdateTotalPrice();
        }
        public NewInvoicePage(ObservableCollection<BankInfo> bankAccounts, int invoiceID) : this(bankAccounts)
        {
            this.bankAccounts = bankAccounts;

            cbBankAccounts.ItemsSource = bankAccounts;
            isEditingExistingInvoice = true;
            LoadInvoice(invoiceID);
        }

        private void LoadInvoice(int invoiceID)
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectInvoiceQuery = "SELECT * FROM Invoices WHERE InvoiceID = @InvoiceID";
            string selectItemsQuery = "SELECT * FROM InvoiceItems WHERE InvoiceID = @InvoiceID";

            InvoiceText.Text = "Oprava faktury";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(selectInvoiceQuery, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int bankID = reader.GetInt32(1);
                            currentCustomer = new Customer
                            {
                                ICO = reader.GetString(2),
                                DIC = reader.GetString(3),
                                Address = reader.GetString(4),
                                Country = reader.GetString(5),
                                Email = reader.GetString(6),
                                Phone = reader.GetString(7),
                                CompanyName = reader.GetString(9),
                            };

                            cbBankAccounts.SelectedItem = bankAccounts.FirstOrDefault(b => b.ID == bankID);
                            SetCustomer(currentCustomer);
                            txtTotalPrice.Text = $"Celková cena: {reader.GetDecimal(8):C}";
                        }
                    }
                }
                InvoiceId = invoiceID;

                using (SQLiteCommand command = new SQLiteCommand(selectItemsQuery, connection))
                {
                    command.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        invoiceItems.Clear();
                        while (reader.Read())
                        {
                            invoiceItems.Add(new InvoiceItem
                            {
                                ItemName = reader.GetString(2),
                                Quantity = reader.GetInt32(3),
                                Price = Convert.ToDecimal(reader.GetDouble(4)),
                                Tax = Convert.ToDecimal(reader.GetDouble(5)),
                                Total = Convert.ToDecimal(reader.GetDouble(6))
                            });
                        }
                    }
                }
            }

            if (invoiceItems.Count > 0)
            {
                lvInvoiceItems.Visibility = Visibility.Visible;
            }
            else
            {
                lvInvoiceItems.Visibility = Visibility.Hidden;
            }
            UpdateCustomerButtonText();
            LoadCompanyInfo();
        }
        private bool LoadCompanyInfo()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT * FROM CompanyInfo";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtSupplierName.Text = $"Název firmy: {reader["CompanyName"].ToString()}";
                                txtSupplierICO.Text = $"IČO: {reader["ICO"].ToString()}";
                                txtSupplierDIC.Text = $"DIČ: {reader["DIC"].ToString()}";
                                txtSupplierCountry.Text = $"Země: {reader["Country"].ToString()}";
                                txtSupplierEmail.Text = $"Email: {reader["Email"].ToString()}";
                                txtSupplierIsVATPayer.Text = $"Je plátce DPH: {(Convert.ToBoolean(reader["IsVATPayer"]) ? "Ano" : "Ne")}";
                                return true;
                            }
                            else
                            {
                                isCompanyHere = false;
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání informací o firmě: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            bool isVATPayer = GetIsVATPayer();

            NewItemPage newItemPage = new NewItemPage(this, isVATPayer);

            Window newItemWindow = new Window();
            newItemWindow.Content = newItemPage;
            newItemWindow.ShowDialog();
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            NewCustomerPage newCustomerPage = new NewCustomerPage(this, currentCustomer);

            Window newCustomerWindow = new Window();
            newCustomerWindow.Content = newCustomerPage;
            newCustomerWindow.ShowDialog();
            UpdateCustomerButtonText();
        }

        private void UpdateCustomerButtonText()
        {
            if (currentCustomer != null)
            {
                btnAddCustomer.Content = "Oprava odběratele";
            }
            else
            {
                btnAddCustomer.Content = "Přidat odběratele";
            }
        }

        private bool GetIsVATPayer()
        {
            bool isVATPayer = false;

            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT IsVATPayer FROM CompanyInfo";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            isVATPayer = Convert.ToBoolean(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při načítání informací o firmě: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isVATPayer;
        }

        public void AddItemToList(InvoiceItem item)
        {
            invoiceItems.Add(item);
            if (invoiceItems.Count > 0)
            {
                lvInvoiceItems.Visibility = Visibility.Visible;
            }
            UpdateTotalPrice();
        }

        public void SetCustomer(Customer customer)
        {
            currentCustomer = customer;
            txtCustomerICO.Text = $"IČO: {customer.ICO}";
            txtCustomerDIC.Text = $"DIČ: {customer.DIC}";
            txtCompanyName.Text = $"Název firmy: {customer.CompanyName}";
            txtCustomerAddress.Text = $"Adresa: {customer.Address}";
            txtCustomerCountry.Text = $"Země: {customer.Country}";
            txtCustomerEmail.Text = $"Email: {customer.Email}";
            txtCustomerPhone.Text = $"Telefon: {customer.Phone}";
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (var item in invoiceItems)
            {
                totalPrice += item.Total;
            }
            txtTotalPrice.Text = $"Celková cena: {totalPrice:C}";
        }

        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (cbBankAccounts.SelectedItem == null)
            {
                MessageBox.Show("Vyberte bankovní účet, pokud není založen, založte ho v záložce Banka.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (currentCustomer == null)
            {
                MessageBox.Show("Nezadal jste žádného odběratele.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (invoiceItems.Count == 0)
            {
                MessageBox.Show("Nezadal jste žádnou položku.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!isCompanyHere)
            {
                MessageBox.Show("Založte prosím vaší firmu v záložce Firma.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (isEditingExistingInvoice == false)
            {
                BankInfo selectedBank = (BankInfo)cbBankAccounts.SelectedItem;
                int nextInvoiceNumber = GetNextInvoiceNumber();

                string connectionString = "Data Source=InvoiceDB.db;Version=3;";
                string insertInvoiceQuery = @"INSERT INTO Invoices (InvoiceID, BankID, CustomerICO, CustomerDIC, CustomerAddress, CustomerCountry, CustomerEmail, CustomerPhone, TotalPrice, CompanyName)
                   VALUES (@InvoiceID, @BankID, @CustomerICO, @CustomerDIC, @CustomerAddress, @CustomerCountry, @CustomerEmail, @CustomerPhone, @TotalPrice, @CompanyName);";
                string insertItemQuery = @"INSERT INTO InvoiceItems (InvoiceID, ItemName, Quantity, Price, Tax, Total) VALUES (@InvoiceID, @ItemName, @Quantity, @Price, @Tax, @Total);";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand(insertInvoiceQuery, connection))
                            {
                                command.Parameters.AddWithValue("@InvoiceID", nextInvoiceNumber);
                                command.Parameters.AddWithValue("@BankID", selectedBank.ID);
                                command.Parameters.AddWithValue("@CustomerICO", currentCustomer.ICO);
                                command.Parameters.AddWithValue("@CustomerDIC", currentCustomer.DIC);
                                command.Parameters.AddWithValue("@CustomerAddress", currentCustomer.Address);
                                command.Parameters.AddWithValue("@CustomerCountry", currentCustomer.Country);
                                command.Parameters.AddWithValue("@CustomerEmail", currentCustomer.Email);
                                command.Parameters.AddWithValue("@CustomerPhone", currentCustomer.Phone);
                                command.Parameters.AddWithValue("@TotalPrice", invoiceItems.Sum(item => item.Total));
                                command.Parameters.AddWithValue("@CompanyName", currentCustomer.CompanyName);

                                command.ExecuteNonQuery();

                                long lastRowID = connection.LastInsertRowId;

                                foreach (var item in invoiceItems)
                                {
                                    using (SQLiteCommand itemCommand = new SQLiteCommand(insertItemQuery, connection))
                                    {
                                        itemCommand.Parameters.AddWithValue("@InvoiceID", nextInvoiceNumber);
                                        itemCommand.Parameters.AddWithValue("@ItemName", item.ItemName);
                                        itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                        itemCommand.Parameters.AddWithValue("@Price", item.Price);
                                        itemCommand.Parameters.AddWithValue("@Tax", item.Tax);
                                        itemCommand.Parameters.AddWithValue("@Total", item.Total);

                                        itemCommand.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Faktura byla úspěšně uložena.", "Informace", MessageBoxButton.OK, MessageBoxImage.Information);

                            NavigationService.Navigate(new InvoiceListPage());
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Chyba při ukládání faktury: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                BankInfo selectedBank = (BankInfo)cbBankAccounts.SelectedItem;


                string connectionString = "Data Source=InvoiceDB.db;Version=3;";
                string updateInvoiceQuery = @"UPDATE Invoices SET BankID = @BankID, CustomerICO = @CustomerICO, CustomerDIC = @CustomerDIC, 
                                   CustomerAddress = @CustomerAddress, CustomerCountry = @CustomerCountry, CustomerEmail = @CustomerEmail, 
                                   CustomerPhone = @CustomerPhone, TotalPrice = @TotalPrice WHERE InvoiceID = @InvoiceID, CompanyName = @CompanyName";
                string deleteItemsQuery = @"DELETE FROM InvoiceItems WHERE InvoiceID = @InvoiceID";
                string insertItemQuery = @"INSERT INTO InvoiceItems (InvoiceID, ItemName, Quantity, Price, Tax, Total) 
                               VALUES (@InvoiceID, @ItemName, @Quantity, @Price, @Tax, @Total)";

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand command = new SQLiteCommand(updateInvoiceQuery, connection))
                            {
                                command.Parameters.AddWithValue("@BankID", selectedBank.ID);
                                command.Parameters.AddWithValue("@CustomerICO", currentCustomer.ICO);
                                command.Parameters.AddWithValue("@CustomerDIC", currentCustomer.DIC);
                                command.Parameters.AddWithValue("@CustomerAddress", currentCustomer.Address);
                                command.Parameters.AddWithValue("@CustomerCountry", currentCustomer.Country);
                                command.Parameters.AddWithValue("@CustomerEmail", currentCustomer.Email);
                                command.Parameters.AddWithValue("@CustomerPhone", currentCustomer.Phone);
                                command.Parameters.AddWithValue("@TotalPrice", invoiceItems.Sum(item => item.Total));
                                command.Parameters.AddWithValue("@CompanyName", currentCustomer.CompanyName);
                                command.Parameters.AddWithValue("@InvoiceID", InvoiceId);

                                command.ExecuteNonQuery();
                            }

                            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteItemsQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@InvoiceID", InvoiceId);
                                deleteCommand.ExecuteNonQuery();
                            }

                            foreach (var item in invoiceItems)
                            {
                                using (SQLiteCommand itemCommand = new SQLiteCommand(insertItemQuery, connection))
                                {
                                    itemCommand.Parameters.AddWithValue("@InvoiceID", InvoiceId);
                                    itemCommand.Parameters.AddWithValue("@ItemName", item.ItemName);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    itemCommand.Parameters.AddWithValue("@Price", item.Price);
                                    itemCommand.Parameters.AddWithValue("@Tax", item.Tax);
                                    itemCommand.Parameters.AddWithValue("@Total", item.Total);

                                    itemCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show("Faktura byla úspěšně aktualizována.", "Informace", MessageBoxButton.OK, MessageBoxImage.Information);

                            NavigationService.Navigate(new InvoiceListPage());
                        }
                        catch (SQLiteException ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Chyba při aktualizaci faktury: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private int GetNextInvoiceNumber()
        {
            int nextInvoiceNumber = 1;
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT MAX(InvoiceID) FROM Invoices";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                {
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        nextInvoiceNumber = Convert.ToInt32(result) + 1;
                    }
                }
            }

            return nextInvoiceNumber;
        }
    }
}
