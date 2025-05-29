using System;
using System.Collections.Generic;
using EnergiaMain.Models;

namespace EnergiaMain.Services
{
    public class AuthenticationService
    {
        private List<Usuario> users = new List<Usuario>();
        private Usuario currentUser = null;

        public AuthenticationService()
        {
            users.Add(new Usuario("admin", "admin123", "Administrator", true));
        }

        public bool RegisterUser(string username, string password, string name)
        {
            try
            {
                if (users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    return false;

                users.Add(new Usuario(username, password, name));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            var user = users.Find(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user == null)
                return false;

            currentUser = user;
            return true;
        }

        public void Logout() => currentUser = null;

        public Usuario CurrentUser => currentUser;
        public bool IsLoggedIn => currentUser != null;
    }
}