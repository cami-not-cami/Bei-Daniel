using Bei_Daniel.Helpers;
using DinnerBoxd.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bei_Daniel.ViewModel
{
    internal class SettingsViewModel : BaseClass
    {
        private string _invoiceNumberInput;
        public string InvoiceNumberInput
        {
            get => _invoiceNumberInput;
            set
            {
                _invoiceNumberInput = value;
                OnPropertyChanged(nameof(InvoiceNumberInput));
            }
        }

            public ICommand SetInvoiceStartNumber { get; set; }

            public SettingsViewModel()
            {
                SetInvoiceStartNumber = new RelayCommand(ExecuteSetInvoiceStartNumber);
                LoadCurrentInvoiceNumber();
            }

            private void LoadCurrentInvoiceNumber()
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            var invoiceFile = Path.Combine(folder, "invoice_counter.txt");
            if (File.Exists(invoiceFile))
            {
                InvoiceNumberInput = File.ReadAllText(invoiceFile).Trim();
            }
            else
            {
                InvoiceNumberInput = "1";
            }
        }

        private void ExecuteSetInvoiceStartNumber()
        {
            if (string.IsNullOrWhiteSpace(InvoiceNumberInput))
            {
                MessageBox.Show("Invoice number cannot be empty.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetInvoiceStartNumberExecute(InvoiceNumberInput);
            MessageBox.Show("Invoice number updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SetInvoiceStartNumberExecute(string invoiceNumber)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            var invoiceFile = Path.Combine(folder, "invoice_counter.txt");
            
            Directory.CreateDirectory(folder);
            File.WriteAllText(invoiceFile, invoiceNumber);
        }
    }
}
