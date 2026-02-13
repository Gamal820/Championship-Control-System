namespace Championship_Control_System.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public int MatchId { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Match Match { get; set; } = null!;
    }
}
