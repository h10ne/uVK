using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uVK.Model;

namespace uVK.ViewModel
{
    class SettingsViewModel:BaseViewModel
    {
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
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
