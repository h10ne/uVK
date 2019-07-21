using System.Windows;
using uVK.Helpers;
using uVK.Model;

namespace uVK.ViewModel
{
    class AuthVeiwModel:BaseViewModel
    {
        private string _login;
        private string _password;
        private Visibility _errorVisibility = Visibility.Hidden;
        public string Login { set { _login = value; OnPropertyChanged(nameof(Login)); } get => _login;
        }
        public string Password { get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public Visibility ErrorVisibility { get => _errorVisibility;
            set { _errorVisibility = value; OnPropertyChanged(nameof(ErrorVisibility)); } }
        public RelayCommand Restore
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    AuthModel.Restore();
                });
            }
        }        
        public RelayCommand Authorize
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    AuthModel.GetAuth(Login, Password);
                    ErrorVisibility = Visibility.Visible;
                }, (obj) => Login != null && Password != null) ;
            }
        }
    }
}
