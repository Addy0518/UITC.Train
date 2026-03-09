namespace Lab.API.Dapper.Models
{
    public class UserInsertDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class UserViewDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }

    public class UserUpdateDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public List<BookDTO>? UserBooks { get; set; }
    }

    public class UserAndBooksDTO
    {
        public User users { get; set; }

        public List<Book>? books { get; set; }
    }

    public class BookDTO
    {
        public int? BookId { get; set; }

        public int? UserId { get; set; }

        public string BookName { get; set; }

        public string BookPrice { get; set; }
    }
}
