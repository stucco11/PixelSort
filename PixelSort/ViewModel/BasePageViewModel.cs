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
using System.Windows.Navigation;

namespace PixelSort.ViewModel
{
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
        private Visibility _BrightnessOptionsVisibility = Visibility.Visible;

        // Stores a copy of _collectionEnum
        private ObservableCollection<string> _collectionEnum = null;

        private RGBEnum _ColorChecked = RGBEnum.Red;
        private bool _ExtendSort = false;
        private string _ColorText = "Sort by: Red";
        private ICommand _goToSettings;
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;
        private int _HorizontalPartitions = 0;
        private string _imagePath = "";
        private double _LowerBright = 0.0;
        private string _PixelDimensions = "Dimensions: File not loaded";
        private Boolean _ProcessEnabled = false;
        private Visibility _RGBVisibility = Visibility.Collapsed;
        private string _RotationText = "Angle of rotation: 0°";
        private int _RotationValue = 0;
        private Boolean _SaveEnabled = false;
        private SortingMethodsEnum _SelectedSort = SortingMethodsEnum.Brightness;
        private string _SortedImage = "";

        private double _UpperBright = 1.0;

        private Visibility _VerticalPanelVisibility = Visibility.Visible;

        private int _VerticalPartitions = 0;

        private Bitmap image;

        private Model.Save imageSaveTool = new Model.Save();

        private Sorts sorts = new Sorts();

        private int visible = 0;

        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;

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

        public Visibility BrightnessOptions
        {
            get { return _BrightnessOptionsVisibility; }
            set
            {
                _BrightnessOptionsVisibility = value;
                NotifyPropertyChanged();
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

                    default:
                        ColorText = "Green";
                        break;
                }
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
                _ColorText = "Sort by: " + value;
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

        public double LowerBright
        {
            get
            {
                return _LowerBright;
            }
            set
            {
                _LowerBright = value;
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
                _PixelDimensions = value == null ? "Dimensions: 0 x 0" : "Dimensions: " + value;
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
                    image = sorts.Sort(ImagePath, SelectedSort, LowerBright, UpperBright, HorizontalPartitions, VerticalPartitions, RotationValue, ColorChecked, ExtendSort);
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

        public string RotationText
        {
            get
            {
                return _RotationText;
            }
            set
            {
                _RotationText = value;
                NotifyPropertyChanged();
            }
        }

        public int RotationValue
        {
            get
            {
                return _RotationValue;
            }
            set
            {
                _RotationValue = value;
                RotationText = "Angle of rotation: " + RotationValue + "°";
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
                    BrightnessOptions = Visibility.Visible;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.Hue)
                {
                    BrightnessOptions = Visibility.Collapsed;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.Saturation)
                {
                    BrightnessOptions = Visibility.Collapsed;
                    RGBOptions = Visibility.Collapsed;
                }
                if (_SelectedSort == SortingMethodsEnum.RGB)
                {
                    BrightnessOptions = Visibility.Collapsed;
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

        public double UpperBright
        {
            get
            {
                return _UpperBright;
            }
            set
            {
                _UpperBright = value;
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