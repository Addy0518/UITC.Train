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
    }
}
