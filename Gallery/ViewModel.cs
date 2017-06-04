using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gallery
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            //SaveCmd = new RelayCommand(pars => Save());
        }
        public event PropertyChangedEventHandler PropertyChanged = null;
        public ICommand AddToFavourites { get; set; }
        private Image _WorkspaceImage = null;
        private string _FirstName = null;
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = null;
                OnPropertyChanged("FirstName");
            }
        }
        private string _LastName = null;
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = null;
                OnPropertyChanged("LastName");
            }
        }
        private void Save()
        {
            // logika odpowiedzialna za zapis
        }
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
