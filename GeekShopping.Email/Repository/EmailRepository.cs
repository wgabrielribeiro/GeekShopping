using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<SqlContext> _Context;

        public EmailRepository(DbContextOptions<SqlContext> Context)
        {
            _Context = Context;
        }

        public async Task LogEmail(UpdatePaymentResultmessage message)
        {
            EmailLog email = new EmailLog()
            {
                Email = message.Email,
                SentDate = DateTime.Now,
                log = $"Order - {message.OrderId} has been created successfully!"
            };


            await using var _db = new SqlContext(_Context);
            
            _db.Email.Add(email);
            await _db.SaveChangesAsync();

        }
    }
}
