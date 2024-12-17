namespace WOTBlyatz.Models
{
    public class Mod
    {
        public int Id { get; set; }            // Уникальный идентификатор мода
        public string Name { get; set; }       // Название мода
        public string Description { get; set; } // Краткое описание
        public string ImageUrl { get; set; }   // Путь к изображению мода
        public string DownloadUrl { get; set; } // Ссылка на скачивание архива


        public bool IsSubscriptionRequired { get; set; } // Требуется ли подписка
    }

}
