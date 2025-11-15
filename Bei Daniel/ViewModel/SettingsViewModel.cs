using Bei_Daniel.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bei_Daniel.ViewModel
{
    internal class SettingsViewModel : BaseClass
    {
        public ICommand SetInvoiceStartNumber { get; set; }
        public SettingsViewModel()
        {
            
        }

        public void SetInvoiceStartNumberExecute(string invoiceNumber)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            var invoiceFile = Path.Combine(folder, "invoice_counter.txt");
            if (File.Exists(invoiceFile))
            {
                FileStream fs = new FileStream(invoiceFile, FileMode.Open, FileAccess.Write);
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(invoiceNumber);
                }
            }
        }
    }
}
