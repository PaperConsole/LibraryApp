
namespace LibraryApp.Models {
    public class Book
    {

        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public int PublishedYear { get; set; }

        public bool IsAvailable { get; set; }
    
        public List<User>? Borrowers { get; set; }


}
}