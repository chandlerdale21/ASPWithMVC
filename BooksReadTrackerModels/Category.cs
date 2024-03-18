namespace BooksReadTrackerModels
{

    public class Category
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public virtual List<Book> Books { get; set; } = new List<Book>();


    }
}
