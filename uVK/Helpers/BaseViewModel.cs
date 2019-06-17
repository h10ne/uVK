using System.ComponentModel;

namespace uVK
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        //public event PropertyChangingEventHandler PropertyChanging = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        //public void OnPropertyChanging(string name)
        //{
        //    PropertyChanging(this, new PropertyChangingEventHandler);
        //}
    }
}
