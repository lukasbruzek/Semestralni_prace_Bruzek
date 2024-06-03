using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using Semestralka_Bruzek;

namespace Semestralka
{
    public partial class BankListPage : Page
    {
        private ObservableCollection<BankInfo> banks;
        public ObservableCollection<BankInfo> GetBankAccounts()
        {
            return banks;
        }
        public BankListPage()
        {
            InitializeComponent();
            banks = new ObservableCollection<BankInfo>();
            lvBanks.ItemsSource = banks;
            LoadBanks();
        }

        private void LoadBanks()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT * FROM BankInfo";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            banks.Clear();
                            while (reader.Read())
                            {
                                BankInfo bank = new BankInfo
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    BankName = reader["BankName"].ToString(),
                                    AccountNumber = reader["AccountNumber"].ToString(),
                                    BankCode = reader["BankCode"].ToString(),
                                    IBAN = reader["IBAN"].ToString(),
                                    SWIFT = reader["SWIFT"].ToString()
                                };

                                banks.Add(bank);
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error loading banks: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBank_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Bank());
        }

        private void EditBank_Click(object sender, RoutedEventArgs e)
        {
            if (lvBanks.SelectedItem is BankInfo selectedBank)
            {
                NavigationService?.Navigate(new Bank(selectedBank));
            }
            else
            {
                MessageBox.Show("Please select a bank to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBank_Click(object sender, RoutedEventArgs e)
        {
            if (lvBanks.SelectedItem is BankInfo selectedBank)
            {
                string connectionString = "Data Source=InvoiceDB.db;Version=3;";
                string checkInvoicesQuery = "SELECT COUNT(*) FROM Invoices WHERE BankID = @BankID";
                string deleteQuery = "DELETE FROM BankInfo WHERE ID = @ID";

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand checkCommand = new SQLiteCommand(checkInvoicesQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@BankID", selectedBank.ID);
                            long invoiceCount = (long)checkCommand.ExecuteScalar();

                            if (invoiceCount > 0)
                            {
                                MessageBox.Show("Tuto banku nelze odstranit, protože existují faktury, které ji používají.", "Varování", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }

                        using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@ID", selectedBank.ID);
                            deleteCommand.ExecuteNonQuery();
                            banks.Remove(selectedBank);
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Chyba při mazání banky: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Prosím, vyberte banku k odstranění.", "Varování", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}