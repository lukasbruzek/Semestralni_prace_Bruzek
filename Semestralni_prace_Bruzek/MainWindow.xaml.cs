using Semestralka;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace Semestralka_Bruzek
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InvoiceListPage invoiceListPage = new InvoiceListPage();
            MainFrame.Navigate(invoiceListPage);
        }


        private void CompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Company companyPage = new Company();
            MainFrame.Navigate(companyPage);
        }

        private void BankMenuItem_Click(object sender, RoutedEventArgs e)
        {
            BankListPage bankListPage = new BankListPage();
            MainFrame.Navigate(bankListPage);
        }

        private void NewInvoice_Click(object sender, RoutedEventArgs e)
        {
            BankListPage bankListPage = new BankListPage();
            NewInvoicePage newInvoicePage = new NewInvoicePage(bankListPage.GetBankAccounts());
            MainFrame.Navigate(newInvoicePage);
        }

        private void LoadInvoice_Click(object sender, RoutedEventArgs e)
        {
            InvoiceListPage invoiceListPage = new InvoiceListPage();
            MainFrame.Navigate(invoiceListPage);
        }
    }
}
