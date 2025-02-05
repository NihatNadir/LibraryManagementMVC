using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Entities
{
    public class OrderEntity : BaseEntity
    {
        public int UserId { get; set; }

        public UserEntity? User { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<BookOrderEntity>? BookOrders { get; set; }
    }
}
