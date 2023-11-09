namespace Entities
{
    public class ProductLikes : BaseEntity
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
    }
}
