using uVK.Helpers;
using uVK.Model;

namespace uVK.ViewModel
{
    class SettingsViewModel:BaseViewModel
    {
        public string Firstname { get; }
        public string Lastname { get; }
        public SettingsViewModel()
        {            
            Firstname = UserDatas.Name;
            Lastname = UserDatas.Surname;
        }
        public RelayCommand Logout
        {
            get
            {
                return new RelayCommand((obj) =>
                {
                    SettingsModel.Logout();
                });
            }
        }
    }
}
