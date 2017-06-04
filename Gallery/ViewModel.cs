using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
namespace Gallery
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Debug.WriteLine("INIT VIEWMODEL");
            _canExecute = true;
        }
        public event PropertyChangedEventHandler PropertyChanged ;
        private Uri filepath;
        private bool _canExecute;
        private ICommand AddToFavourites { get; set; }

        private ICommand _openFile;
        public ICommand OpenFile
        {
            get
            {
                Debug.WriteLine("GET OPENFILE");
                return _openFile ?? (_openFile = new CommandHandler(() => Open(), _canExecute));
            }
        }

        private Uri _WorkspaceImage = new Uri("C:/Users/menf/Pictures/Przechwytywanie.png");
        public Uri WorkspaceImage
        {
            get
            {
                Debug.WriteLine("get obraz");
                return _WorkspaceImage;
              
            }
            set
            {
                Debug.WriteLine("Niby ustawiam obraz");
                _WorkspaceImage = value;
                RaisePropertyChanged(this,"WorkspaceImage");
            }
        }
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
                RaisePropertyChanged(this,"FirstName");
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
                RaisePropertyChanged(this,"LastName");
            }
        }
        private void Save()
        {
            // logika odpowiedzialna za zapis
        }
        public void Open()
        {
            Debug.WriteLine("OPEN");
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|"+"JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            openfiledialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openfiledialog.Title = "Please select an image file.";
            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                filepath = new Uri(openfiledialog.FileName, UriKind.RelativeOrAbsolute);
                WorkspaceImage = filepath;
            }
        }
        private void Dummy()
        {
            // logika odpowiedzialna za cos
        }
        protected void RaisePropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                Debug.WriteLine("PROPERTY CHANGED : SENDER/NAME"+sender.ToString()+"/"+propertyName);

                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }



    }

    // w stylu new CommandHandler(() => MyAction(), _canExecute))
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

        public void Execute(object parameter)
        {
            Debug.WriteLine("EXECUTE");
            _action();
        }
    }
}
