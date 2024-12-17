using Microsoft.AspNetCore.Identity;

namespace WOTBlyatz.Models
{


    public class ApplicationUser : IdentityUser
    {
        // Здесь можно добавлять дополнительные свойства для пользователей
        public bool IsAdmin { get; set; }  // Имя пользователя
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
        public DateTime DateOfSubscription { get; set; }

        public Status UserStatus { get; set; } = Status.Active;



    }
    public enum Status
    {
        Active,
        Banned,
    }

}
