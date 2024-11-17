namespace GeekShopping.Email.Messages
{
    public class UpdatePaymentResultmessage
    {
        public long OrderId { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }
    }
}
