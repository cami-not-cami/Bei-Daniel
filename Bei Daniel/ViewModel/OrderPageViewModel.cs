using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Bei_Daniel.Helpers;
using Bei_Daniel.Models;
using Bei_Daniel.Utils;
using Bei_Daniel.View.Pages;
using DinnerBoxd.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static Bei_Daniel.Utils.InvoiceUtils;

namespace Bei_Daniel.ViewModel
{
    class OrderPageViewModel : BaseClass
    {
        #region Properties

        private double _price;
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;

                OnPropertyChanged(nameof(Price));
            }
        }
        private long _amount;
        public long Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        private List<string> _quantity;
        public List<string> Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;

                OnPropertyChanged(nameof(Quantity));
            }
        }
        private string _selectedQT;
        public string SelectedQT
        {
            get => _selectedQT;
            set
            {
                _selectedQT = value;

                OnPropertyChanged(nameof(SelectedQT));
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate == value)
                    return;

                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));

                // Optional: if you want to refresh lines/totals when date changes, uncomment:
                // CalculateLines();
                
            }
        }
        private int _restaurantId;
        private readonly AppDbContext _appDbContext = new AppDbContext();
        public ObservableCollection<Order> Orders { get; set; }
        private List<string> _productsName;
        public List<string> ProductsName
        {
            get => _productsName;
            set
            {
                _productsName = value;

                OnPropertyChanged(nameof(ProductsName));
            }
        }
        private string _selectedProduct;
        public string SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;

                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
        private string _productInput;
        public string ProductInput
        {
            get => _productInput;
            set
            {
                if (_productInput == value)
                    return;

                // store the raw input first
                _productInput = value;
                OnPropertyChanged(nameof(ProductInput));

                // Resolve input (scale number => product name) - null-safe
                var resolvedName = ProductUtils.GetProductNameFromInput(
                    (_productInput ?? string.Empty).Trim(),
                    _appDbContext
                );

                if (!string.IsNullOrEmpty(resolvedName))
                {
                    var product = _appDbContext.Products.FirstOrDefault(p =>
                        p.Name == resolvedName
                    );
                    if (product != null)
                    {
                        // update SelectedProduct and ProductInput to the resolved product name
                        _selectedProduct = product.Name; // set field directly to avoid recursion
                        OnPropertyChanged(nameof(SelectedProduct));

                        _productInput = product.Name; // update the displayed text to resolved name
                        OnPropertyChanged(nameof(ProductInput));
                        return;
                    }
                }

                // if nothing found, clear selection (optional) or keep the typed text
                SelectedProduct = null;
            }
        }

        private string _restaurantName;
        public string RestaurantName
        {
            get { return _restaurantName; }
            set
            {
                _restaurantName = value;
                OnPropertyChanged(nameof(RestaurantName));
            }
        }
        private double _pageTotal;
        public double PageTotal
        {
            get { return _pageTotal; }
            set
            {
                _pageTotal = value;

                OnPropertyChanged(nameof(PageTotal));
            }
        }

        #endregion
        public ICommand AddOrderCommand { get; set; }
        public ICommand DeleteOrderCommand { get; set; }

        public ICommand PrintOrder { get; set; }
        public ICommand EditRestaurantCommand { get; set; }


        public OrderPageViewModel(int restaurantId)
        {
            _restaurantId = restaurantId;
            SelectedDate = DateTime.Today;
            LoadItemsForRestaurant(_restaurantId);
            ProductsName = _appDbContext.Products.Select(p => p.Name).ToList();
            Quantity = OrderUtils.QUANTITY_TYPES;
            AddOrderCommand = new RelayCommand(AddOrder);
            PrintOrder = new RelayCommand(PrintOrderCommand);
            RestaurantName = _appDbContext
                .Restaurants.Where(p => p.Id == _restaurantId)
                .Select(p => p.Name)
                .FirstOrDefault();

            PageTotal = OrderUtils.GetOrderTotalWith10Percent(_restaurantId);

            DeleteOrderCommand = new RelayCommand<Order>(DeleteOrder);
            EditRestaurantCommand = new RelayCommand(EditRestaurant);
        }

        private void EditRestaurant()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Navigate(new ClientManagerOverview(_restaurantId));
            }
        }

        private void LoadItemsForRestaurant(long restaurantId)
        {
            var orders = OrderUtils.GetAllUnsolvedOrders(restaurantId, _appDbContext);

            Orders = new ObservableCollection<Order>(orders);
            CalculateLines();
        }




        private void PrintOrderCommand()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            List<Order> orders = OrderUtils.GetAllUnsolvedOrders(_restaurantId, _appDbContext).ToList();
            orders = ProductUtils.BundleIdenticalProducts(new ObservableCollection<Order>(orders)).ToList();
            List<InvoiceUtils.Product> products = new List<InvoiceUtils.Product>();
            foreach (var order in orders)
            {
                InvoiceUtils.Product product = new InvoiceUtils.Product();
                product.ProductName = order.Product.Name;
                product.ProductQuantityType = order.ProductQuantityType;
                product.Quantity = order.Amount.ToString() + " " + order.ProductQuantityType;
                product.UnitPrice = order.ProductPrice;
                product.Total = order.ProductPrice * order.Amount;
                products.Add(product);
            }

            products = products.OrderBy(p => p.ProductName).ToList();
            var filePath = RestaurantUtils.generateRestaurantFilePath(_restaurantId, _appDbContext);
            var invoiceNumber = InvoiceUtils.GetInvoiceNumber();
            var document = new ReceiptDocument
            {
                DeliveryDate = DateOnly.FromDateTime(SelectedDate).ToShortDateString(),
                CustomerAddress = RestaurantUtils.GetRestaurantAddressById(_restaurantId, _appDbContext),
                CustomerName = RestaurantUtils.GetRestaurantNameById(_restaurantId, _appDbContext),
                InvoiceNr = invoiceNumber,
                Products = products,
                Netto = OrderUtils.GetOrderTotal(_restaurantId),
            };
            document.GeneratePdf(filePath);
            RestaurantUtils.SolveResturantOrders(_restaurantId, _appDbContext);
            Orders.Clear();
            PageTotal = 0;
            InvoiceUtils.IncrementInvoiceNumber();
            MessageBox.Show($"Receipt saved to: {filePath}");
        }

        private void AddOrder()
        {
            Order order = new Order();
            order.RestaurantId = _restaurantId;
            order.ProductId = ProductUtils.GetProductIdFromName(SelectedProduct, _appDbContext);
            order.Amount = Amount;
            order.ProductPrice = Price;
            order.Data = DateTime.Now;
            order.Solved = false;
            order.ProductQuantityType = SelectedQT;
            //order.CompletedAmount = Amount.ToString() + SelectedQT;

            if (order.ProductId != 0 && Amount != 0 && Price != 0)
            {
                Orders.Add(order);
                OrderUtils.AddOrder(order, _appDbContext);
                PageTotal = OrderUtils.GetOrderTotalWith10Percent(_restaurantId);
                CalculateLines();
            }
            else
            {
                MessageBox.Show("Produkt, Quantität und Preis darf nicht leer sein.");
                return;
            }
        }

        private void DeleteOrder(Order order)
        {
            if (order == null)
                return;

            var result = MessageBox.Show(
                $"Möchten Sie {order.Product.Name} wirklich löschen?",
                "Bestätigung",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                //OrderUtils.SolveOrder(order.Id, _appDbContext); why?
                OrderUtils.RemoveOrderById(order.Id, _appDbContext);
                Orders.Remove(order);
                PageTotal = OrderUtils.GetOrderTotalWith10Percent(_restaurantId);
            }
        }

        private void CalculateLines()
        {
            foreach (var item in Orders)
            {
                item.InLineTotal = (item.ProductPrice * item.Amount);
            }
        }
    }
}
