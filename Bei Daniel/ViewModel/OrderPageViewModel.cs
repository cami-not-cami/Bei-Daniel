using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Bei_Daniel.Helpers;
using Bei_Daniel.Models;
using Bei_Daniel.Utils;
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
        private int _restaurantId;
        private readonly AppDbContext _appDbContext = MainViewModel._appDbContext;
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

        public OrderPageViewModel(int restaurantId)
        {
            _restaurantId = restaurantId;
            LoadItemsForRestaurant(_restaurantId);
            ProductsName = _appDbContext.Products.Select(p => p.Name).ToList();
            Quantity = OrderUtils.QUANTITY_TYPES;
            AddOrderCommand = new RelayCommand(AddOrder);
            RestaurantName = _appDbContext
                .Restaurants.Where(p => p.Id == _restaurantId)
                .Select(p => p.Name)
                .FirstOrDefault();

            PageTotal = OrderUtils.GetOrderTotalWith10Percent(_restaurantId);

            DeleteOrderCommand = new RelayCommand<Order>(DeleteOrder);
        }

        private void LoadItemsForRestaurant(long restaurantId)
        {
            var orders = OrderUtils.GetAllUnsolvedOrders(restaurantId, _appDbContext);

            Orders = new ObservableCollection<Order>(orders);
            CalculateLines();
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
            //order.CompletedAmount = Amount.ToString() + SelectedQT;

            if (order.ProductId != 0 && Amount != 0 && Price != 0)
            {
                Orders.Add(order);
                OrderUtils.AddOrder(order, _appDbContext);
                PageTotal = OrderUtils.GetOrderTotalWith10Percent(_restaurantId);
                CalculateLines();
                QuestPDF.Settings.License = LicenseType.Community;

                var products = new List<InvoiceUtils.Product>
                {
                    new()
                    {
                        Quantity = "8 KG",
                        ProductName = "Champignons",
                        UnitPrice = 3.60,
                        Total = 28.80,
                    },
                    new()
                    {
                        Quantity = "72 STK",
                        ProductName = "Salat",
                        UnitPrice = 1.20,
                        Total = 86.40,
                    },
                    new()
                    {
                        Quantity = "325.5 KG",
                        ProductName = "Kartoffel",
                        UnitPrice = 0.90,
                        Total = 292.50,
                    },
                };

                var document = new ReceiptDocument
                {
                    LogoPath = @"C:\path\to\logo.png",
                    Products = products,
                    Netto = 724.60,
                };

                var filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "receipt.pdf"
                );
                document.GeneratePdf(filePath);
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
                OrderUtils.SolveOrder(order.Id, _appDbContext);
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
