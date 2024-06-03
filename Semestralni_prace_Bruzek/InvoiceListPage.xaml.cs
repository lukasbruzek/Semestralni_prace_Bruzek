using Semestralka;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Semestralka_Bruzek
{
    public partial class InvoiceListPage : Page
    {
        private ObservableCollection<Invoice> invoices = new ObservableCollection<Invoice>();

        public InvoiceListPage()
        {
            InitializeComponent();
            LoadInvoices();
            lvInvoices.ItemsSource = invoices;
        }

        private void LoadInvoices()
        {
            string connectionString = "Data Source=InvoiceDB.db;Version=3;";
            string selectQuery = "SELECT * FROM Invoices ORDER BY InvoiceID DESC";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                invoices.Add(new Invoice
                                {
                                    InvoiceID = reader.GetInt32(0),
                                    CustomerICO = reader.GetString(2),
                                    CustomerName = reader.GetString(9),
                                    TotalPrice = reader.GetDecimal(8)
                                });
                            }
                        }
                    }
                }
            }
        }

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            Button editButton = (Button)sender;
            int invoiceID = (int)editButton.Tag;

            BankListPage bankListPage = new BankListPage();
            NavigationService.Navigate(new NewInvoicePage(bankListPage.GetBankAccounts(), invoiceID));
        }
    }
}
