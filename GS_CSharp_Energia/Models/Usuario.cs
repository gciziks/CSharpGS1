namespace EnergiaMain.Models
{
    public class Usuario
    {
        public string Username { get; }
        public string Password { get; }
        public string Name { get; }
        public bool IsAdmin { get; }

        public Usuario(string username, string password, string name, bool isAdmin = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("O campo Usuário não pode ser vazio!");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("O campo Senha não pode ser vazio!");

            Username = username;
            Password = password;
            Name = name;
            IsAdmin = isAdmin;
        }
    }
}