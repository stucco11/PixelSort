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
using System.Windows;
using System.Windows.Input;

namespace PixelSort.ViewModel
{
    // Enum for the sorting modes
    // Extend treats the image as one line
    // Sprial will do the same as extend but put everything back in a spiral formation
    public enum AdditionalOptionsEnum
    {
        Extend,
        Spiral,
        None
    }

    // The direction that the pixels are being sorted in
    public enum DirectionEnum
    {
        Right,
        Down,
        Left,
        Up
    }

    // What color is being sorted
    public enum RGBEnum
    {
        Red,
        Green,
        Blue
    }

    // Which of the four pixel properties the image will be sorted by
    public enum SortingMethodsEnum
    {
        [Description("Brightness")] Brightness,
        [Description("Hue")] Hue,
        [Description("Saturation")] Saturation,
        [Description("RGB")] RGB
    }

    public class BasePageViewModel : BaseViewModel, IPageViewModel, INotifyPropertyChanged
    {

        // Associated with Additional Options
        private AdditionalOptionsEnum _AddOps = AdditionalOptionsEnum.None;
        private string _AddOpsText = "None";

        // Text to dictate the bounds for things like brightness
        private string _BoundText = "0.0 and 1.0";
        private double _LowerBound = 0.0;
        private double _UpperBound = 1.0;

        // Stores a copy of _collectionEnum
        private ObservableCollection<string> _collectionEnum = null;

        // Associated with the selected color to sort by
        private RGBEnum _ColorChecked = RGBEnum.Red;
        private string _ColorText = "Red";

        // Associated with the selected sorting direction
        private DirectionEnum _Direction = DirectionEnum.Right;
        private string _DirectionText = "Left to Right";

        // Hold the values for the number of partitions that are needed
        private int _HorizontalPartitions = 0;
        private int _VerticalPartitions = 0;

        // TODO: Visibility for the Horizontal panel orientation
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;

        // Visibility for the Vertical panel orientation
        private Visibility _VerticalPanelVisibility = Visibility.Visible;

        // Visibility for the RGB options
        private Visibility _RGBVisibility = Visibility.Collapsed;

        // Associated variables for the image
        private string _imagePath = "";
        private string _PixelDimensions = "File not loaded";

        // Enbabled/Disabled flags for processing and saving the image
        private Boolean _ProcessEnabled = false;
        private Boolean _SaveEnabled = false;

        // Sort that has been selected for the image
        private SortingMethodsEnum _SelectedSort = SortingMethodsEnum.Brightness;

        // Filepath for the sorted image
        private string _SortedImage = "";

        // Stored bitmap of the image to be processed
        private Bitmap image;

        // Instantiations for model classes
        private Model.Save imageSaveTool = new Model.Save();
        private Sorts sorts = new Sorts();

        // TODO: Used for horizontal/vertical layouts, not implemented yet
        private int visible = 0;

        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;

        // Property for AddOps, sets text values in the UI depending on the value of _AddOps
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

        // Property for AddOpsText on the ViewModel, will update UI when changed
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

        // When Extend is checked, sets AddOps to Extend
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

        // When Spiral is checked, AddOps is updated to reflect it
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

        // If Extend or Spiral aren't checked, AddOps is none
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

        // Property for the BoundText on the UI
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

        // Property that access the UpperBound number on the UI
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

        // Property for the LowerBound in the UI
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

        // Property for the checked color, will update ColorText depending on _ColorChecked
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

        // Updates the ColorText Property on the ViewModel when changed
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

        // When Red is cheked, ColorChecked is updated
        public object RedChecked
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ColorChecked = RGBEnum.Red;
                }));
            }
        }

        // Command Property on the ViewModel, will trigger when button is pressed
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

        // When green is checked, ColorChecked is updated to Green
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

        // Changes the value of _Direction and DirectionText when DirectionChecked is changed in the UI
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
                        DirectionText = "Top to Bottom";
                        break;
                }
            }
        }

        // Updates the UI to set text of value
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

        // When right is checked, DirectionChecked is updated to Right
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

        // When left is checked, the DirectionChecked is updated to Left
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

        // When Up is clicked, DirectionChecked is updated
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

        // Updates DirectionChecked when the Down Button is pressed
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

        // Property for the HorizontalPartitions in the UI
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

        // Property that is conntected to the VerticalPartitions value on the UI
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

        // When the user selects a new image, the _imagepath is updated and SortedImage is erased
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

        // When the user loads a new image, the save button is disabled, the image process button is enabled, and PixelDimensions is updated
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

        // Property for the dimensions of the loaded image
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

        // List of the availble sorting methods
        public ObservableCollection<string> SortingMethods
        {
            get
            {
                return GetEnumDescriptions();
            }
        }

        // When a sorting method is chosen, the specific properties that can be altered for the sorting 
        // method are changed, and only the properties that apply to a specific method are presented
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

        // Visibility Property for the RGBOptions in the UI
        public Visibility RGBOptions
        {
            get { return _RGBVisibility; }
            set
            {
                _RGBVisibility = value;
                NotifyPropertyChanged();
            }
        }

        // Property for the Button to process the loaded image
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

        // Command for the Process Image Button. Sorts the image based on the selected options, 
        // saves it to a temp location, sets the sorted image to the temp image, and enables the save button
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

        // When the Save Button needs to change states, this is called and sets the button on the UI to the passed in value
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

        // When the Save button is clicked, the SaveImageMethod is returned as a RelayCommand
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

        // Stores the path to the SortedImage in the temp folder
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

        // When pressed, swaps the filepath of the image that the user wants sorted to the temp location of the sorted image
        public object EditSorted
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    ImagePath = SortedImage;
                }));
            }
        }

        // TODO: Used to switch the orientation between horizontal and vertical
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

        // TODO: Used to for the image panel orientation
        public Visibility HorizontalPanelVisibility
        {
            get { return _HorizontalPanelVisibility; }
            set
            {
                _HorizontalPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        // TODO: Used to determine the visibility of the Vertical Orientation
        public Visibility VerticalPanelVisibility
        {
            get { return _VerticalPanelVisibility; }
            set
            {
                _VerticalPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        /*
         * Method that is called when an image needs to be saved.
         * Presents the user with a saveDialog and allows them to save their image as a .jpg, .png, or a .bmp.
         * Assuming that the entered name of the image isn't blank, the image will be stored into a memoryStream.
         * That memory stream is then converted to a byte array, which is then written to the proper location with
         * FileStream. Both MemoryStream and FileStream are disposed after this occurs
         */
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

        // Allows the UI to update the specific element that is updated when called
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Used for the Drop Down List of options that can be used for the Sorting Methods.
        // Each Enum is stored with a string label in a dictionary, whih is then returned as an
        // ObservableCollection
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

        /* 
         * Presents the user with a file dialog that allows them to load images that are .jpg or .png files.
         * If they have a valid selection, then ImagePath is the filepath of the image
         */
        private bool LoadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPG/PNG Images (*.jpg, *.png)|*.jpg;*.png|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            // If the user doesn't have a selection, NULL is assigned
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                ImagePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        // TODO: Used to switch the orientation of the image panels
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