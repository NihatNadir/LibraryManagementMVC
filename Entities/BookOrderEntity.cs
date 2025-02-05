using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementMVC.Entities
{
    public class BookOrderEntity : BaseEntity
    {
        public int BookId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public OrderEntity? Order { get; set; }
        public BookEntity? Book { get; set; }
    }
}
