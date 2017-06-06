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
using System.IO;
using System.Windows;

namespace Gallery
{
    class ViewModel : INotifyPropertyChanged
    {
        public enum DrawingTool
        {
            Rectangle,
            Triangle,
            Circle,
            Line,
            Eraser,
            Pencil,
            Paintbrush
        }

        public ViewModel()
        {
            Debug.WriteLine("INIT VIEWMODEL");
            System.Windows.Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            _canExecute = true;
            CanExecuteS = false;
            CanExecuteR= false;
            Kolor = Colors.Black;
            KolorBrush = new SolidColorBrush(Colors.Black);
            ListboxItems = new ObservableCollection<Item>();
            InitFav();
        }

        public ObservableCollection<Item> ListboxItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged ;
        private Uri filepath;
        private string name;
        private DrawingTool drawingTool = DrawingTool.Pencil;
        private bool leftMouseButtonDown = false;
        private System.Drawing.Point currentPoint = new System.Drawing.Point();
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
        private bool _canExecuteR;
        public bool CanExecuteR
        {
            get
            {
                return _canExecuteR;
            }
            set
            {
                _canExecuteR = value;
                RaisePropertyChanged(this, "CanExecuteR");
            }
        }
        private bool _canExecuteS = true;
        public bool CanExecuteS
        {
            get
            {
                return _canExecuteS;
            }
            set
            {
                _canExecuteS = value;
                RaisePropertyChanged(this, "CanExecuteS");
            }
        }

        private ICommand _openFile;
        public ICommand OpenFile
        {
            get
            {
                Debug.WriteLine("GET OPENFILE");
                return _openFile ?? (_openFile = new CommandHandler(param => Open(), _canExecute));
            }
        }

        private ICommand _openfromFav;
        public ICommand OpenFromFav
        {
            get
            {
                Debug.WriteLine("GET openfav");
                return _openfromFav ?? (_openfromFav = new CommandHandler(param => OpenFromFavorite(param), _canExecute));
            }
        }

        private ICommand _saveFile;
        public ICommand SaveFile
        {
            get
            {
                Debug.WriteLine("Save file");
                return _saveFile ?? (_saveFile = new CommandHandler(param => Save(), _canExecute));
            }
        }

        private ICommand _pickColor;
        public ICommand PickColor
        {
            get
            {
                Debug.WriteLine("change color ");
                return _pickColor ?? (_pickColor = new CommandHandler(param => ChangeColor(), _canExecute));
            }
        }

        private ICommand _addtoFav;
        public ICommand AddtoFav
        {
            get
            {
                Debug.WriteLine("add to fav ");
                return _addtoFav ?? (_addtoFav = new CommandHandler(param => AddtoFavourites(), _canExecute));
            }
        }

        private ICommand _removeFav;
        public ICommand RemoveFav
        {
            get
            {
                Debug.WriteLine("rem from fav ");
                return _removeFav ?? (_removeFav = new CommandHandler(param => RemoveFromFavourites(param), _canExecute));
            }
        }

        private ICommand _startDrawing;
        public ICommand StartDrawing
        {
            get
            {
                Debug.WriteLine("Drawing started");
                return _startDrawing ?? (_startDrawing = new CommandHandler(param => BeginDraw(param), _canExecute));
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
        
        private static Grid _CanvasContent = new Grid();
        public Grid CanvasContent
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
            line.X2 = 555;
            line.Y2 = 555;

            CanvasContent.Children.Add(line);
            RaisePropertyChanged(this, "CanvasContent");
            //CanvasContent.Width = 800;
            //CanvasContent.Height = 800;
            Transform transform = CanvasContent.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            CanvasContent.LayoutTransform = null;

            // Get the size of canvas
            int width = (int)CanvasContent.ActualWidth;
            int height = (int)CanvasContent.ActualHeight;
            Size size = new Size(width, height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            CanvasContent.Measure(size);
            CanvasContent.Arrange(new System.Windows.Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(CanvasContent);

            // Create a file stream for saving image
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ShowDialog();
            using (FileStream outStream = new FileStream(sfd.FileName, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            //    // Restore previously saved layout
            CanvasContent.LayoutTransform = transform;
            //}


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
                CanvasContent = new Grid();
                filepath = new Uri(openfiledialog.FileName, UriKind.RelativeOrAbsolute);
                name= System.IO.Path.GetFileName(filepath.LocalPath);
                WorkspaceImage = filepath;
                CanExecuteS = true;
                Image loadedImage = new Image();
                loadedImage.Source = new BitmapImage(WorkspaceImage);
                CanvasContent.Children.Add(loadedImage);

                Debug.WriteLine("filepath" + filepath);
                Debug.WriteLine("absolutepath"+filepath.AbsolutePath);
                Debug.WriteLine("absoluteuri" + filepath.AbsoluteUri);
            }
        }
        public void OpenFromFavorite(object param)
        {
            CanvasContent = new Grid();
            Item item = ListboxItems[(int)param];
            Debug.WriteLine("INDEX FAV: " + (int)param);
            filepath = new Uri(item.ImagePath, UriKind.RelativeOrAbsolute);
            name = item.Name;
            WorkspaceImage = filepath;
            Image loadedImage = new Image();
            loadedImage.Source = new BitmapImage(WorkspaceImage);
            CanvasContent.Children.Add(loadedImage);

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

       private void RemoveFromFavourites(object param)
        {
            if ((int)param>-1)
            {
                ListboxItems.RemoveAt((int)param);

            }
            if (ListboxItems.Count < 1) CanExecuteR= false;
        }
       private void AddtoFavourites()
        {
            Debug.WriteLine("ADDTOFAV");
            Item item = new Item(name, filepath);
            if (!ListboxItems.Contains(item))
            ListboxItems.Add(item);
            Debug.WriteLine("LISTBOX COUNT:" + ListboxItems.Count);
            CanExecuteS = false;
            CanExecuteR = true;
        }

        protected void RaisePropertyChanged(object sender, string propertyName)
        {
            if (PropertyChanged != null)
            {
                Debug.WriteLine("PROPERTY CHANGED : SENDER/NAME"+sender.ToString()+"/"+propertyName);

                PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void StartDrawing()
        {
            switch (drawingTool)
            {
                case DrawingTool.Pencil:
                    break;
                case DrawingTool.Paintbrush:
                    break;
                case DrawingTool.Eraser:
                    break;
                case DrawingTool.Line:
                    break;
                case DrawingTool.Triangle:
                    break;
                case DrawingTool.Circle:
                    break;
                case DrawingTool.Rectangle:
                    break;
            }
            leftMouseButtonDown = true;
            //begin = e.GetPosition(mainCanvas);
            //mainCanvas.CaptureMouse();
            //Mouse.Capture(mainCanvas);
            //lastRectangle = new Rectangle();
            //lastRectangle.Stroke = new SolidColorBrush(rectangleColor);
            //lastRectangle.StrokeThickness = 1;
            //Canvas.SetLeft(lastRectangle, begin.X);
            //Canvas.SetTop(lastRectangle, begin.Y);
            //mainCanvas.Children.Add(lastRectangle);
        }

        private void Draw(System.Windows.Input.MouseEventArgs e)
        {

        }

        private void EndDrawing(MouseButtonEventArgs e)
        {

        }
    }

    

    // w stylu new CommandHandler(() => MyAction(), _canExecute))
    public class CommandHandler : ICommand
    {
        private Action<object> _action;
        private bool _canExecute;
        public CommandHandler(Action<object> action, bool canExecute)
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
            _action(parameter);
        }
    }
}
