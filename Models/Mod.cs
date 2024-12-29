namespace WOTBlyatz.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsSubscriptionRequired { get; set; }
        public DateTime DateAdded { get; set; } // Дата добавления
        public double Rating { get; set; } // Рейтинг
        public List<ModCategory> Categories { get; set; } // Категории мода
    }

    // Перечисление категорий
    public enum ModCategory
    {
        Textures,
        Models,
        Banned
    }

}
