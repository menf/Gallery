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
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace Gallery
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Debug.WriteLine("INIT VIEWMODEL");
            _canExecute = true;
            Kolor = Colors.Black;
            KolorBrush = new SolidColorBrush(Colors.Black);
            ListboxItems = new ObservableCollection<Item>();
        }
        public ObservableCollection<Item> ListboxItems;
        public event PropertyChangedEventHandler PropertyChanged ;
        private Uri filepath;
        private string name;
        private Color _kolor;
        public Color Kolor
        {
            get
            {
                Debug.WriteLine("get kolor");
                return _kolor;

            }
            set
            {
                Debug.WriteLine("Niby ustawiam kolor");
                _kolor = value;
                RaisePropertyChanged(this, "Kolor");
            }
        }

   

        private SolidColorBrush _kolorbrush;
        public SolidColorBrush KolorBrush
        {
            get
            {
                Debug.WriteLine("get kolorbrush");
                return _kolorbrush;

            }
            set
            {
                Debug.WriteLine("Niby ustawiam kolorbrush");
                _kolorbrush = value;
                RaisePropertyChanged(this, "KolorBrush");
            }
        }


        private bool _canExecute;
        private bool _canExecuteS=true;

        private ICommand _openFile;
        public ICommand OpenFile
        {
            get
            {
                Debug.WriteLine("GET OPENFILE");
                return _openFile ?? (_openFile = new CommandHandler(() => Open(), _canExecute));
            }
        }

        private ICommand _saveFile;
        public ICommand SaveFile
        {
            get
            {
                Debug.WriteLine("Save file");
                return _saveFile ?? (_saveFile = new CommandHandler(() => Save(), _canExecuteS));
            }
        }

        private ICommand _pickColor;
        public ICommand PickColor
        {
            get
            {
                Debug.WriteLine("change color ");
                return _pickColor ?? (_pickColor = new CommandHandler(() => ChangeColor(), _canExecute));
            }
        }

        private ICommand _addtoFav;
        public ICommand AddtoFav
        {
            get
            {
                Debug.WriteLine("add to fav ");
                return _addtoFav ?? (_addtoFav = new CommandHandler(() => AddtoFavourites(), _canExecuteS));
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
        
        private static Canvas _CanvasContent = new Canvas();
        public Canvas CanvasContent
        {
            get
            {
                return _CanvasContent;
            }
            set
            {
                _CanvasContent = value;
                RaisePropertyChanged(this, "CanvasContent");
            }
        }

        private void Save()
        {
            //TODO: jakos pobrac canvas

            //to tylko taki test czy rysuje
            Line line = new Line();
            line.Stroke = Brushes.LimeGreen;
            line.StrokeThickness = 4;
            line.X1 = 5;
            line.Y1 = 5;
            line.X2 = 55;
            line.Y2 = 55;
            CanvasContent.Children.Add(line);
            RaisePropertyChanged(this, "CanvasContent");

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
                name = openfiledialog.FileName;
                filepath = new Uri(name, UriKind.RelativeOrAbsolute);
                WorkspaceImage = filepath;
                Debug.WriteLine("filepath" + filepath);
                Debug.WriteLine("absolutepath"+filepath.AbsolutePath);
                Debug.WriteLine("absoluteuri" + filepath.AbsoluteUri);
            }
        }
        private void Dummy()
        {
            // logika odpowiedzialna za cos
        }
        private void ChangeColor()
        {
            var dialog = new ColorDialog();
            dialog.ShowDialog();
            Kolor = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);
            KolorBrush = new SolidColorBrush(Kolor);
        }

       private void AddtoFavourites()
        {
            ListboxItems.Add(new Item(name, filepath));
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
