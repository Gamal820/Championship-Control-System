namespace Championship_Control_System.ViewModels
{
    public class MyTicketGroupVM
    {
        public int? MatchId { get; set; }
        public string MatchName { get; set; } =String.Empty;
        public string StadiumName { get; set; } = String.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime? LastBookingDate { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
