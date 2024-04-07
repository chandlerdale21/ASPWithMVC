using System.ComponentModel.DataAnnotations;

namespace BooksReadTrackerModels
{
    public class Book
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public byte[]? Image { get; set; }

        [StringLength(2048)]
        public string FilePath { get; set; }
        public virtual int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }



        [StringLength(64)]
        public string UserId { get; set; }
    }


}
