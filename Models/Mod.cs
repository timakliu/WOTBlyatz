namespace WOTBlyatz.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsSubscriptionRequired { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now; // Дата добавления
        public double Rating { get; set; } // Рейтинг
        public int DownloadCount { get; set; } // Количество скачиваний
        public List<ModCategory> Categories { get; set; } // Категории мода
    }

    public enum ModCategory
    {
        Remodeling,
        Animation,
        Trash,
        Skins,
        Other


    }
}
