using Microsoft.Win32;
using PixelSort.EventHandling;
using PixelSort.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PixelSort.ViewModel
{
    public enum SortingMethodsEnum
    {
        [Description("Brightness")] Brightness,
        [Description("Hue")] Hue
    }
    public class BasePageViewModel : BaseViewModel, IPageViewModel, INotifyPropertyChanged
    {
        // Stores a copy of _collectionEnum
        private ObservableCollection<string> _collectionEnum = null;

        private SortingMethodsEnum _SelectedSort = SortingMethodsEnum.Brightness;

        private Boolean _ProcessEnabled = false;
        private Boolean _SaveEnabled = false;

        private int visible = 0;

        private double _LowerBright = 0.0;
        private double _UpperBright = 1.0;

        private string _imagePath = "";
        private string _PixelDimensions = "Dimensions: File not loaded";
        private string _SortedImage = "";

        private Visibility _VerticalPanelVisibility = Visibility.Visible;
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;
        private Visibility _BrightnessOptionsVisibility = Visibility.Visible;

        private ICommand _goToSettings;

        private Bitmap image;
        private Model.Save imageSaveTool = new Model.Save();
        private Sorts sorts = new Sorts();


        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;

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
                }
                if (_SelectedSort == SortingMethodsEnum.Hue)
                {
                    BrightnessOptions = Visibility.Collapsed;
                }

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

        public ObservableCollection<string> SortingMethods
        {
            get
            {
                return GetEnumDescriptions();
            }
        }

        private ObservableCollection<string> GetEnumDescriptions()
        {
            // Creates a Dictionary set to an enum value and a string
            Dictionary<SortingMethodsEnum, string> GameTagDescriptions = new Dictionary<SortingMethodsEnum, string>()
            {
                { SortingMethodsEnum.Brightness, "Brightness" },
                { SortingMethodsEnum.Hue, "Hue" }
            };

            // Sets _collectionEnum to a new ObservableCollection of Dictionary.values
            _collectionEnum = new ObservableCollection<string>(GameTagDescriptions.Values);

            // Returns _collectionEnum
            return _collectionEnum;
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

        public Visibility HorizontalPanelVisibility
        {
            get { return _HorizontalPanelVisibility; }
            set
            {
                _HorizontalPanelVisibility = value;
                NotifyPropertyChanged();
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
                    LoadImage(LoadImageClick, null);
                    SaveEnabled = false;
                    ProcessEnabled = true;
                    PixelDimensions = (Image.FromFile(ImagePath).Width.ToString() + " x " + Image.FromFile(ImagePath).Height.ToString());
                }));
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

        public ICommand ProcessImage
        {
            get
            {
                return (new RelayCommand(x =>
                {
                    image = sorts.Sort(ImagePath, SelectedSort, LowerBright, UpperBright);
                    imageSaveTool.SaveImage(image);
                    SortedImage = imageSaveTool.SavedImagePath;
                    SaveEnabled = true;
                    NotifyPropertyChanged();
                }));
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
        public Visibility VerticalPanelVisibility
        {
            get { return _VerticalPanelVisibility; }
            set
            {
                _VerticalPanelVisibility = value;
                NotifyPropertyChanged();
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadImage(object sender, RoutedEventArgs e)
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
            }
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