using Bei_Daniel.Helpers;
using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using DinnerBoxd.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bei_Daniel.ViewModel
{
    internal class MonthyInvoiceViewModel : BaseClass
    {
        private DateOnly _startDate;
        public DateOnly StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(StartDatePicker)); // keep proxy in sync
            }
        }

        // Proxy property for DatePicker binding
        public DateTime? StartDatePicker
        {
            get { return _startDate.ToDateTime(TimeOnly.MinValue); }
            set
            {
                if (value.HasValue)
                {
                    _startDate = DateOnly.FromDateTime(value.Value);
                    OnPropertyChanged(nameof(StartDate));
                    OnPropertyChanged(nameof(StartDatePicker));
                }
            }
        }

        private DateOnly _endDate;
        public DateOnly EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                OnPropertyChanged(nameof(EndDatePicker)); // keep proxy in sync
            }
        }

        // Proxy property for DatePicker binding
        public DateTime? EndDatePicker
        {
            get { return _endDate.ToDateTime(TimeOnly.MinValue); }
            set
            {
                if (value.HasValue)
                {
                    _endDate = DateOnly.FromDateTime(value.Value);
                    OnPropertyChanged(nameof(EndDate));
                    OnPropertyChanged(nameof(EndDatePicker));
                }
            }
        }


        private Restaurant _restaurant;
        public Restaurant Restaurant
        {
            get { return _restaurant; }
            set
            {
                _restaurant = value;
                OnPropertyChanged(nameof(Restaurant));
            }
        }


        public MonthyInvoiceViewModel(int restaurantId)
        {
            _restaurant = RestaurantUtils.GetRestaurantById(restaurantId, new AppDbContext());
            StartDate = DateOnly.FromDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }

        public void PrintIntervalSelectedInvoice(int restaurantId)
        {
            InvoiceUtils.GenerateMonthlyInvoicePdf(restaurantId, StartDate, EndDate, new AppDbContext());
        }
    }
}