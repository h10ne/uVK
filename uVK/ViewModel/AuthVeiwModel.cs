using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using uVK.Helpers;
using uVK.Model;

namespace uVK.ViewModel
{
    class AuthVeiwModel : ReactiveObject
    {
        [Reactive] public string Login { get; set; }
        [Reactive] public string Password { get; set; }
        [Reactive] public Visibility ErrorVisibility { get; set; } = Visibility.Hidden;

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
                }, (obj) => Login != null && Password != null);
            }
        }
    }
}
