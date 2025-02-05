using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Data;
using LibraryManagementMVC.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Entities
{
    public class UserEntity : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole UserRole { get; set; }

        public ICollection<OrderEntity> Orders { get; set; }
    }
}
