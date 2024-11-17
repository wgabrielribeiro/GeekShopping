
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQConsumerConsumer : BackgroundService
    {
        private readonly OrderRepository _repository;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumerConsumer(OrderRepository repository, IConnection connection, IModel channel)
        {
            _repository = repository;
            _connection = connection;
            _channel = channel;

            //var factory = new ConnectionFactory
            //{
            //    HostName = "localhost",
            //    Password = "guest",
            //    UserName = "guest"
            //};

            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "checkoutQueue", false, false, false, arguments: null);

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());

                CheckoutHeadeVO vo = JsonSerializer.Deserialize<CheckoutHeadeVO>(content);
                ProcessOrder(vo).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeadeVO vo)
        {
            OrderHeader orderHeader = new OrderHeader
            {
                UserId = vo.UserId,
                FirstName = vo.FirstName,
                LastName = vo.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = vo.CardNumber,
                CouponCode = vo.CouponCode,
                CVV = vo.CVV,
                DiscountAmount = vo.DiscountAmount,
                Email = vo.Email,
                ExpiryMothYear = vo.ExpiryMothYear,
                OrderTime = DateTime.Now,
                PurchaseAmount = vo.PurchaseAmount,
                PaymentStatus = false,
                Phone = vo.Phone,
                PurchaseDate = vo.DateTime
            };

            foreach (var order in orderHeader.OrderDetails)
            {
                OrderDetail detail = new()
                {
                    ProductId = order.ProductId,
                    ProductName = order.ProductName,
                    Price = order.Price,
                    Count = order.Count
                };

                orderHeader.CartTotalItens += detail.Count;

                orderHeader.OrderDetails.Add(detail);
            }

        }
    }
}
