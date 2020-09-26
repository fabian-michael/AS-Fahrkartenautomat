using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Fahrkartenautomat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Ticket[] _singleTickets = {
            new Ticket { Name = "Berlin AB", Price = 2.9f, ReducedPrice = 1.8f, Type = TicketType.SINGLE },
            new Ticket { Name = "Berlin BC", Price = 3.3f, ReducedPrice = 2.3f, Type = TicketType.SINGLE },
            new Ticket { Name = "Berlin ABC", Price = 3.6f, ReducedPrice = 2.6f, Type = TicketType.SINGLE },
        };

        public event PropertyChangedEventHandler PropertyChanged;

        private Cart _cart = new Cart();
        public Cart Cart => _cart;

        private Ticket _selectedTicket;
        public Ticket SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                _selectedTicket = value;
                ticketsPanel.Children.OfType<Button>().ToList().ForEach(b => b.Background = SystemColors.WindowBrush);
                RaisePropertyChangedEvent("SelectedTicket");
                RaisePropertyChangedEvent("SelectedTicketPrice");
                RaisePropertyChangedEvent("SelectedTotalPrice");
                RaisePropertyChangedEvent("SelectedTicketPanelEnabled");
            }
        }

        private bool _useReducedPrice;
        public bool UseReducedPrice
        {
            get => _useReducedPrice;
            set
            {
                _useReducedPrice = value;
                RaisePropertyChangedEvent("UseReducedPrice");
                RaisePropertyChangedEvent("SelectedTicketPrice");
                RaisePropertyChangedEvent("SelectedTotalPrice");
            }
        }

        private int _ammount;
        public int Ammount
        {
            get => _ammount;
            set
            {
                _ammount = value;
                RaisePropertyChangedEvent("Ammount");
                RaisePropertyChangedEvent("SelectedTotalPrice");
            }
        }
        public float SelectedTicketPrice => (_useReducedPrice ? _selectedTicket?.ReducedPrice : _selectedTicket?.Price) ?? 0;
        public float SelectedTotalPrice => Ammount * SelectedTicketPrice;

        private bool _isPayment;
        public bool IsPayment
        {
            get => _isPayment;
            set
            {
                _isPayment = value;
                RaisePropertyChangedEvent("IsPayment");
                RaisePropertyChangedEvent("IsNotPayment");
                RaisePropertyChangedEvent("SelectedTicketPanelEnabled");
                RaisePropertyChangedEvent("ToPay");
                RaisePropertyChangedEvent("Change");
            }
        }
        public bool IsNotPayment => !IsPayment;
        public bool SelectedTicketPanelEnabled => SelectedTicket != null && IsNotPayment;

        private float _moneyInserted = 0;
        public float MoneyInserted
        {
            get => _moneyInserted;
            set {
                _moneyInserted = value;
                RaisePropertyChangedEvent("MoneyInserted");
                RaisePropertyChangedEvent("ToPay");
                RaisePropertyChangedEvent("Change");
            }
        }
        public float TotalPrice => Cart.Items.Aggregate(0f, (curr, next) => curr + (next.ReducedPrice ? next.Ticket.ReducedPrice : next.Ticket.Price));
        public float ToPay => MathF.Max(0, TotalPrice - MoneyInserted);
        public float Change => MathF.Max(0, - TotalPrice + MoneyInserted);

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            foreach(var ticket in _singleTickets)
            {
                Button btn = new Button
                {
                    Width = 80,
                    Height = 80,
                    Content = ticket.Name
                };
                btn.DataContext = ticket;
                btn.Click += new RoutedEventHandler(TicketButton_Click);
                btn.Background = SystemColors.WindowBrush;

                ticketsPanel.Children.Add(btn);

                Ammount = 1;
            }
        }
       private void PaymentCompleted()
       {
            MessageBox.Show($"Vielen Dank. Ihr Rückgeld: {Change:c}");
            Reset();
        }

        private void Reset()
        {
            SelectedTicket = null;
            MoneyInserted = 0;
            Ammount = 1;
            IsPayment = false;
            UseReducedPrice = false;
            Cart.Clear();
            RaisePropertyChangedEvent("Cart");
        }



        private void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[1-9][0-9]$");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void TicketButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            SelectedTicket = btn.DataContext as Ticket;
            
            btn.Background = Brushes.LightSkyBlue;
        }

        private void PayBtn_Click(object sender, RoutedEventArgs e)
        {
            IsPayment = true;
        }

        private void CancelPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            IsPayment = false;
        }

        private void InsertMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;
            float value;
            float.TryParse(btn.Tag as string, out value);
            MoneyInserted += value/100;

            if (ToPay == 0) PaymentCompleted();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Cart.AddItem(new CartItem
            {
                Ticket = SelectedTicket,
                ReducedPrice = UseReducedPrice,
                Ammount = Ammount
            });
            SelectedTicket = null;
            Ammount = 1;
            UseReducedPrice = false;
            RaisePropertyChangedEvent("Cart");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
    }
}