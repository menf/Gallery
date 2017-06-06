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
            Ellipse,
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
            ZoomValue = 1;
        }

        public ObservableCollection<Item> ListboxItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged ;
        private Uri filepath;
        private string name;
        private DrawingTool drawingTool = DrawingTool.Pencil;
        private System.Drawing.Point beginPoint = new System.Drawing.Point();

        private double zoomValue;
        public double ZoomValue
        {
            get
            {
                return zoomValue;
            }

            set
            {
                zoomValue = value;
                if (CanvasContent != null)
                {
                    CanvasContent.LayoutTransform = new ScaleTransform(zoomValue, zoomValue);

                }

            }
        }

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
                return _startDrawing ?? (_startDrawing = new CommandHandler(param => BeginDrawing((MouseButtonEventArgs)param), _canExecute));
            }
        }

        private ICommand _draw;
        public ICommand Draw
        {
            get
            {
                Debug.WriteLine("Drawing in progress");
                return _draw ?? (_draw = new CommandHandler(param => Drawing((System.Windows.Input.MouseEventArgs)param), _canExecute));
            }
        }

        private ICommand _stopDrawing;
        public ICommand StopDrawing
        {
            get
            {
                Debug.WriteLine("Drawing finished");
                return _stopDrawing ?? (_stopDrawing = new CommandHandler(param => EndDrawing((MouseButtonEventArgs)param), _canExecute));
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

        private static double _CanvasWidth;
        public double CanvasWidth
        {
            get
            {
                return _CanvasWidth;
            }
            set
            {
                _CanvasWidth = value;
                RaisePropertyChanged(this, "CanvasWidth");
            }
        }

        private static double _CanvasJHeight;
        public double CanvasHeight
        {
            get
            {
                return _CanvasJHeight;
            }
            set
            {
                _CanvasJHeight = value;
                RaisePropertyChanged(this, "CanvasHeight");
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
            line.X2 = 555;
            line.Y2 = 555;

            CanvasContent.Children.Add(line);
            RaisePropertyChanged(this, "CanvasContent");
            Transform transform = CanvasContent.LayoutTransform;
            CanvasContent.LayoutTransform = null;
            int width = (int)CanvasWidth;
            int height = (int)CanvasHeight;
            Size size = new Size(width, height);
            CanvasContent.Measure(size);
            CanvasContent.Arrange(new Rect(size));
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(CanvasContent);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = System.IO.Path.GetExtension(name);

            if (sfd.ShowDialog() == DialogResult.OK)
                using (FileStream outStream = new FileStream(sfd.FileName, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
            CanvasContent.LayoutTransform = transform;
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
                CanvasContent = new Canvas();
                filepath = new Uri(openfiledialog.FileName, UriKind.RelativeOrAbsolute);
                name= System.IO.Path.GetFileName(filepath.LocalPath);
                WorkspaceImage = filepath;
                CanExecuteS = true;
                Image loadedImage = new Image();
                BitmapImage loadedBitmap = new BitmapImage(WorkspaceImage);
                loadedImage.Source = loadedBitmap;
                CanvasContent.Children.Add(loadedImage);
                CanvasWidth = loadedBitmap.Width;
                CanvasHeight = loadedBitmap.Height;
                Debug.WriteLine("filepath" + filepath);
                Debug.WriteLine("absolutepath"+filepath.AbsolutePath);
                Debug.WriteLine("absoluteuri" + filepath.AbsoluteUri);
            }
        }
        public void OpenFromFavorite(object param)
        {
            CanvasContent = new Canvas();
            Item item = ListboxItems[(int)param];
            Debug.WriteLine("INDEX FAV: " + (int)param);
            filepath = new Uri(item.ImagePath, UriKind.RelativeOrAbsolute);
            name = item.Name;
            WorkspaceImage = filepath;
            Image loadedImage = new Image();
            BitmapImage loadedBitmap = new BitmapImage(WorkspaceImage);
            loadedImage.Source = loadedBitmap;
            CanvasContent.Children.Add(loadedImage);
            CanvasHeight = loadedBitmap.Height;
            CanvasWidth = loadedBitmap.Width;
            CanExecuteS = true;
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
           // CanExecuteS = false;
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

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            using (StreamWriter writetext = new StreamWriter("fav.txt"))
            {
                foreach (Item item in ListboxItems)
                {
                    writetext.WriteLine(item.Name);
                    writetext.WriteLine(item.ImagePath);

                }

            }
        }

        public void InitFav()
        {
            if (File.Exists("fav.txt"))
            {

                using (StreamReader readtext = new StreamReader("fav.txt"))
                {

                    string line1;
                    string line2;
                    while ((line1 = readtext.ReadLine()) != null)
                    {
                        line2 = readtext.ReadLine();
                        ListboxItems.Add(new Item(line1, new Uri(line2)));
                    }

                }
                CanExecuteR = true;
            }

        }

        Shape figura; 
        public void BeginDrawing(MouseButtonEventArgs e)
        {
            beginPoint.X = (int)e.GetPosition(CanvasContent).X;
            beginPoint.Y = (int)e.GetPosition(CanvasContent).Y;

          
            switch (drawingTool)
            {
                case DrawingTool.Pencil:
                    
                    break;
                case DrawingTool.Paintbrush:
                    break;
                case DrawingTool.Eraser:
                    break;
                case DrawingTool.Line:
                    figura = new Line();
                    figura.Stroke = KolorBrush;
                    figura.StrokeThickness = 2;
                    ((Line)figura).X1 = beginPoint.X;
                    ((Line)figura).Y1 = beginPoint.Y;
                    ((Line)figura).X2 = beginPoint.X;
                    ((Line)figura).Y2 = beginPoint.Y;
                    CanvasContent.Children.Add(figura);
                    break;
                case DrawingTool.Triangle:
                    //figura = new System.Windows.Shapes.Polygon();
                    //((Polygon)figura).Stroke = new SolidColorBrush(Kolor);
                    //((Polygon)figura).StrokeThickness = 1;             
                    //PointCollection polygonPoints = new PointCollection();
                    //polygonPoints.Add(new Point(e.GetPosition(CanvasContent).X,e.GetPosition(CanvasContent).Y));
                    //polygonPoints.Add(Point2);
                    //polygonPoints.Add(Point3);
                    //((Polygon)figura).Points = polygonPoints;

                    break;
                case DrawingTool.Ellipse:
                    figura = new Ellipse();
                    figura.Stroke = KolorBrush;
                    figura.StrokeThickness = 2;
                    CanvasContent.Children.Add(figura);
                    break;
                case DrawingTool.Rectangle:
                    figura = new Rectangle();
                    figura.Stroke = KolorBrush;
                    figura.StrokeThickness = 2;
                    CanvasContent.Children.Add(figura);
                    break;
            }
            ////Point beginPoint = e.GetPosition(CanvasContent);
            ////CanvasContent.CaptureMouse();
            ////Mouse.Capture(CanvasContent);
            ////Rectangle lastRectangle = new Rectangle();
            //lastRectangle.Stroke = new SolidColorBrush(rectangleColor);
            //lastRectangle.StrokeThickness = 1;
            //Canvas.SetLeft(lastRectangle, begin.X);
            //Canvas.SetTop(lastRectangle, begin.Y);
            //mainCanvas.Children.Add(lastRectangle);
        }

        private void Drawing(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && figura!=null)
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
                            ((Line)figura).X2 = e.GetPosition(CanvasContent).X;
                            ((Line)figura).Y2 = e.GetPosition(CanvasContent).Y;
                        break;
                    case DrawingTool.Triangle:
                        break;
                    case DrawingTool.Ellipse:
                        {
                            var current = e.GetPosition(CanvasContent);
                            double x = Math.Min(current.X, beginPoint.X);
                            double y = Math.Min(current.Y, beginPoint.Y);
                            var width = Math.Max(current.X, beginPoint.X) - x;
                            var height = Math.Max(current.Y, beginPoint.Y) - y;
                            Canvas.SetLeft(figura, x);
                            Canvas.SetTop(figura, y);
                            figura.Width = width;
                            figura.Height = height;
                        }
                        break;
                    case DrawingTool.Rectangle:
                        {
                            var current = e.GetPosition(CanvasContent);
                            double x = Math.Min(current.X, beginPoint.X);
                            double y = Math.Min(current.Y, beginPoint.Y);
                            var width = Math.Max(current.X, beginPoint.X) - x;
                            var height = Math.Max(current.Y, beginPoint.Y) - y;
                            Canvas.SetLeft(figura, x);
                            Canvas.SetTop(figura, y);
                            figura.Width = width;
                            figura.Height = height;
                        }
                        break;
                }
            }
      
        }

        private void EndDrawing(MouseButtonEventArgs e)
        {
            int dupa = 6;
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
