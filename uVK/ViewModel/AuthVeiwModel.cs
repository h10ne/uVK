using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using uVK.Model;

namespace uVK.ViewModel
{
    class AuthVeiwModel:BaseViewModel
    {
        private string _login;
        private string _password;
        public string Login { set { _login = value; OnPropertyChanged(nameof(Login)); } get { return _login; } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public RelayCommand Authorize
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    AuthModel.GetAuth(Login, Password);
                }, (obj) => Login != null && Password != null) ;
            }
        }
    }
}
