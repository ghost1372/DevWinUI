namespace DevWinUIGallery.Views;

public sealed partial class DiffViewPage : Page
{
    public ObservableCollection<DiffViewMode> ViewModeItems { get; set; } = new ObservableCollection<DiffViewMode>(Enum.GetValues<DiffViewMode>());
    public ObservableCollection<DiffUnifiedMode> UnifiedModeItems { get; set; } = new ObservableCollection<DiffUnifiedMode>(Enum.GetValues<DiffUnifiedMode>());

    public string OldSample { get; } = """
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Shop.Orders;

        public class OrderService
        {
            private readonly IOrderRepository _repository;
            private readonly ILogger _logger;

            public OrderService(IOrderRepository repository, ILogger logger)
            {
                _repository = repository;
                _logger = logger;
            }

            public decimal CalculateTotal(Order order)
            {
                decimal total = 0;
                foreach (var item in order.Items)
                {
                    total += item.Price * item.Quantity;
                }
                return total;
            }

            public IReadOnlyList<Order> GetPending()
            {
                return _repository.GetAll().Where(o => o.Status == "pending").ToList();
            }

            public void Cancel(int id)
            {
                var order = _repository.Find(id);
                order.Status = "cancelled";
                _repository.Save(order);
            }

            public string Describe(Order order)
            {
                return "Order " + order.Id + " has " + order.Items.Count + " items";
            }
        }
        """;

    public string NewSample { get; } = """
        using System;
        using System.Collections.Generic;
        using System.Linq;

        namespace Shop.Orders;

        public class OrderService
        {
            private readonly IOrderRepository _repository;
            private readonly ILogger _logger;

            public OrderService(IOrderRepository repository, ILogger logger)
            {
                _repository = repository;
                _logger = logger;
            }

            public decimal CalculateTotal(Order order)
            {
                ArgumentNullException.ThrowIfNull(order);
                return order.Items.Sum(item => item.Price * item.Quantity);
            }

            public IReadOnlyList<Order> GetPending()
            {
                return _repository.GetAll().Where(o => o.Status == "pending").ToList();
            }

            public void Cancel(int id)
            {
                var order = _repository.Find(id) ?? throw new KeyNotFoundException($"Order {id} not found");
                order.Status = "cancelled";
                _repository.Save(order);
                _logger.Info($"Order {id} cancelled");
            }

            public string Describe(Order order)
            {
                return $"Order {order.Id} has {order.Items.Count} items";
            }
        }
        """;

    public DiffViewPage()
    {
        InitializeComponent();
    }

    private int ToInt(double value) => (int)value;

    private void ReplayButton_Click(object sender, RoutedEventArgs e)
    {
        Sample.Replay();
    }
}
