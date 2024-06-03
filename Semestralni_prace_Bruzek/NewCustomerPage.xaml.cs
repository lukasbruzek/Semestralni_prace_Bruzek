using System.Windows;
using System.Windows.Controls;

namespace Semestralka_Bruzek
{
    public partial class NewCustomerPage : Page
    {
        private NewInvoicePage _newInvoicePage;
        private Customer _currentCustomer;

        public NewCustomerPage(NewInvoicePage newInvoicePage, Customer currentCustomer = null)
        {
            InitializeComponent();
            _newInvoicePage = newInvoicePage;
            _currentCustomer = currentCustomer;

            if (_currentCustomer != null)
            {
                txtICO.Text = _currentCustomer.ICO;
                txtDIC.Text = _currentCustomer.DIC;
                txtCompanyName.Text = _currentCustomer.CompanyName;
                txtAddress.Text = _currentCustomer.Address;
                txtCountry.Text = _currentCustomer.Country;
                txtEmail.Text = _currentCustomer.Email;
                txtPhone.Text = _currentCustomer.Phone;
            }
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer
            {
                ICO = txtICO.Text,
                DIC = txtDIC.Text,
                CompanyName = txtCompanyName.Text,
                Address = txtAddress.Text,
                Country = txtCountry.Text,
                Email = txtEmail.Text,
                Phone = txtPhone.Text
            };

            _newInvoicePage.SetCustomer(newCustomer);
            Window.GetWindow(this).Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
