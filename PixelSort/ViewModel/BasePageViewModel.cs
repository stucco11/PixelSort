using Microsoft.Win32;
using PixelSort.EventHandling;
using PixelSort.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace PixelSort.ViewModel
{
    public enum DirectionEnum
    {
        Right,
        Down,
        Left,
        Up
    }
    public enum AdditionalOptionsEnum
    {
        Extend,
        Spiral,
        None
    }
    public enum RGBEnum
    {
        Red,
        Green,
        Blue
    }

    public enum SortingMethodsEnum
    {
        [Description("Brightness")] Brightness,
        [Description("Hue")] Hue,
        [Description("Saturation")] Saturation,
        [Description("RGB")] RGB
    }
    public class BasePageViewModel : BaseViewModel, IPageViewModel, INotifyPropertyChanged
    {

        // Stores a copy of _collectionEnum
        private ObservableCollection<string> _collectionEnum = null;
        private string _DirectionText = "Left to Right";
        private string _BoundText = "0.0 and 1.0";
        private DirectionEnum _Direction = DirectionEnum.Right;
        private RGBEnum _ColorChecked = RGBEnum.Red;
        private AdditionalOptionsEnum _AddOps = AdditionalOptionsEnum.None;
        private bool _ExtendSort = false;
        private string _ColorText = "Red";
        private string _AddOpsText = "None";
        private ICommand _goToSettings;
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;
        private Visibility _ExtendVisibility = Visibility.Visible;
        private int _HorizontalPartitions = 0;
        private string _imagePath = "";
        private double _LowerBound = 0.0;
        private string _PixelDimensions = "File not loaded";
        private Boolean _ProcessEnabled = false;
        private Visibility _RGBVisibility = Visibility.Collapsed;
        private Boolean _SaveEnabled = false;
        private SortingMethodsEnum _SelectedSort = SortingMethodsEnum.Brightness;
        private string _SortedImage = "";

        private double _UpperBound = 1.0;

        private Visibility _VerticalPanelVisibility = Visibility.Visible;

        private int _VerticalPartitions = 0;

        private Bitmap image;

        private Model.Save imageSaveTool = new Model.Save();

        private Sorts sorts = new Sorts();

        private int visible = 0;

        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;

        public string DirectionText
        {
            get
            {
                return _DirectionText;
            }
            set
            {
                _DirectionText = value;
                NotifyPropertyChanged();
            }
        }
        public string BoundText
        {
            get
            {
                return _BoundText;
            }
            set
            {
                _BoundText = value;
                NotifyPropertyChanged();
            }
        }
        public bool ExtendSort
        {
            get
            {
                return _ExtendSort;
            }
            set
            {
                _ExtendSort = value;
            }
        }

        public object RightChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    DirectionChecked = DirectionEnum.Right;
                }));
            }
        }

        public object UpChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    DirectionChecked = DirectionEnum.Up;
                }));
            }
        }

        public object LeftChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    DirectionChecked = DirectionEnum.Left;
                }));
            }
        }

        public object DownChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    DirectionChecked = DirectionEnum.Down;
                }));
            }
        }

        public object BlueChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ColorChecked = RGBEnum.Blue;
                }));
            }
        }

        public object NoneChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    AddOps = AdditionalOptionsEnum.None;
                }));
            }
        }

        public object ExtendChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    AddOps = AdditionalOptionsEnum.Extend;
                }));
            }
        }

        public object SpiralChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    AddOps = AdditionalOptionsEnum.Spiral;
                }));
            }
        }

        public object EditSorted
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ImagePath = SortedImage;
                    SortedImage = ImagePath;
                }));
            }
        }
        public RGBEnum ColorChecked
        {
            get
            {
                return _ColorChecked;
            }
            set
            {
                _ColorChecked = value;
                switch (_ColorChecked)
                {
                    case RGBEnum.Red:
                        ColorText = "Red";
                        break;

                    case RGBEnum.Blue:
                        ColorText = "Blue";
                        break;

                    case RGBEnum.Green:
                        ColorText = "Green";
                        break;

                    default:
                        ColorText = "Green";
                        break;
                }
            }
        }

        public DirectionEnum DirectionChecked
        {
            get
            {
                return _Direction;
            }
            set
            {
                _Direction = value;
                switch (_Direction)
                {
                    case DirectionEnum.Right:
                        DirectionText = "Left to Right";
                        break;

                    case DirectionEnum.Left:
                        DirectionText = "Right to Left";
                        break;

                    case DirectionEnum.Up:
                        DirectionText = "Bottom to Top";
                        break;

                    case DirectionEnum.Down:
                        DirectionText = "To to Bottom";
                        break;
                }
            }
        }

        public AdditionalOptionsEnum AddOps
        {
            get
            {
                return _AddOps;
            }
            set
            {
                _AddOps = value;
                switch (_AddOps)
                {
                    case AdditionalOptionsEnum.Extend:
                        AddOpsText = "Extend";
                        break;

                    case AdditionalOptionsEnum.Spiral:
                        AddOpsText = "Spiral";
                        break;

                    case AdditionalOptionsEnum.None:
                        AddOpsText = "None";
                        break;

                    default:
                        AddOpsText = "None";
                        break;
                }
            }
        }

        public string AddOpsText
        {
            get
            {
                return _AddOpsText;
            }
            set
            {
                _AddOpsText = value;
                NotifyPropertyChanged();
            }
        }

        public string ColorText
        {
            get
            {
                return _ColorText;
            }
            set
            {
                _ColorText = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand GoToSettings
        {
            get
            {
                return _goToSettings ?? (_goToSettings = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToSettings", "");
                }));
            }
        }

        public object GreenChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ColorChecked = RGBEnum.Green;
                }));
            }
        }

        public Visibility HorizontalPanelVisibility
        {
            get { return _HorizontalPanelVisibility; }
            set
            {
                _HorizontalPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility ExtendVisibility
        {
            get
            {
                return _ExtendVisibility;
            }
            set
            {
                _ExtendVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public int HorizontalPartitions
        {
            get
            {
                return _HorizontalPartitions;
            }
            set
            {
                _HorizontalPartitions = value;
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                SortedImage = "";
                NotifyPropertyChanged();
            }
        }

        public ICommand LoadImageClick
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    if (LoadImage(LoadImageClick, null))
                    {
                        SaveEnabled = false;
                        ProcessEnabled = true;
                        PixelDimensions = (Image.FromFile(ImagePath).Width.ToString() + " x " + Image.FromFile(ImagePath).Height.ToString());
                    }
                }));
            }
        }

        public double LowerBound
        {
            get
            {
                return _LowerBound;
            }
            set
            {
                _LowerBound = value;
                NotifyPropertyChanged();
            }
        }

        public string PixelDimensions
        {
            get
            {
                return _PixelDimensions;
            }
            set
            {
                _PixelDimensions = value == null ? "0 x 0" : value;
                NotifyPropertyChanged();
            }
        }

        public Boolean ProcessEnabled
        {
            get
            {
                return _ProcessEnabled;
            }
            set
            {
                _ProcessEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ProcessImage
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    image = sorts.Sort(ImagePath, SelectedSort, LowerBound, UpperBound, HorizontalPartitions, VerticalPartitions, ColorChecked, AddOps, DirectionChecked);
                    imageSaveTool.SaveImage(image);
                    SortedImage = imageSaveTool.SavedImagePath;
                    SaveEnabled = true;
                    NotifyPropertyChanged();
                }));
            }
        }

        public ICommand RedChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ColorChecked = RGBEnum.Red;
                }));
            }
        }
        public Visibility RGBOptions
        {
            get { return _RGBVisibility; }
            set
            {
                _RGBVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public Boolean SaveEnabled
        {
            get
            {
                return _SaveEnabled;
            }
            set
            {
                _SaveEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SaveImage
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    SaveImageMethod();
                }));
            }
        }

        public SortingMethodsEnum SelectedSort
        {
            get
            {
                return _SelectedSort;
            }
            set
            {
                _SelectedSort = value;
                if (_SelectedSort == SortingMethodsEnum.Brightness)
                {
                    BoundText = "0.0 and 1.0";
                    LowerBound = 0.0;
                    UpperBound = 1.0;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.Hue)
                {
                    BoundText = "0.0 and 360.0";
                    LowerBound = 0.0;
                    UpperBound = 360.0;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.Saturation)
                {
                    BoundText = "0.0 and 1.0";
                    LowerBound = 0.0;
                    UpperBound = 1.0;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.RGB)
                {
                    BoundText = "0.0 and 255.0";
                    LowerBound = 0.0;
                    UpperBound = 255.0;
                    RGBOptions = Visibility.Visible;
                }
            }
        }
        public string SortedImage
        {
            get
            {
                return _SortedImage;
            }
            set
            {
                _SortedImage = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> SortingMethods
        {
            get
            {
                return GetEnumDescriptions();
            }
        }

        public ICommand SwitchOrientation
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    TestOrientation();
                }));
            }
        }

        public double UpperBound
        {
            get
            {
                return _UpperBound;
            }
            set
            {
                _UpperBound = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility VerticalPanelVisibility
        {
            get { return _VerticalPanelVisibility; }
            set
            {
                _VerticalPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public int VerticalPartitions
        {
            get
            {
                return _VerticalPartitions;
            }
            set
            {
                _VerticalPartitions = value;
            }
        }
        public void SaveImageMethod()
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "JPG Image|*.jpg|PNG Image|*.png|Bitmap Image| *.bmp",
                Title = "Save a PixelSorted Image",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            saveDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveDialog.FileName != "")
            {
                string saveName = saveDialog.FileName;

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(@saveName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (saveName.EndsWith(".png"))
                        {
                            image.Save(memory, ImageFormat.Png);
                        }
                        else if (saveName.EndsWith(".jpg"))
                        {
                            image.Save(memory, ImageFormat.Jpeg);
                        }
                        else if (saveName.EndsWith(".bmp"))
                        {
                            image.Save(memory, ImageFormat.Bmp);
                        }

                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<string> GetEnumDescriptions()
        {
            // Creates a Dictionary set to an enum value and a string
            Dictionary<SortingMethodsEnum, string> MethodDescriptions = new Dictionary<SortingMethodsEnum, string>()
            {
                { SortingMethodsEnum.Brightness, "Brightness" },
                { SortingMethodsEnum.Hue, "Hue" },
                { SortingMethodsEnum.Saturation, "Saturation" },
                { SortingMethodsEnum.RGB, "RGB" }
            };

            // Sets _collectionEnum to a new ObservableCollection of Dictionary.values
            _collectionEnum = new ObservableCollection<string>(MethodDescriptions.Values);

            // Returns _collectionEnum
            return _collectionEnum;
        }
        private bool LoadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPG/PNG Images (*.jpg, *.png)|*.jpg;*.png|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                ImagePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        private object TestOrientation()
        {
            if (visible == 0)
            {
                VerticalPanelVisibility = Visibility.Collapsed;
                HorizontalPanelVisibility = Visibility.Visible;
                visible = 1;
            }
            else
            {
                VerticalPanelVisibility = Visibility.Visible;
                HorizontalPanelVisibility = Visibility.Collapsed;
                visible = 0;
            }

            return visible;
        }
    }
}