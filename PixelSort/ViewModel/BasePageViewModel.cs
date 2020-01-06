using Microsoft.Win32;
using PixelSort.EventHandling;
using PixelSort.Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PixelSort.ViewModel
{
    public class BasePageViewModel : BaseViewModel, IPageViewModel, INotifyPropertyChanged
    {
        private Boolean _ProcessEnabled = false;
        private Boolean _SaveEnabled = false;

        private int visible = 0;

        private string _imagePath = "";
        private string _PixelDimensions = "Dimensions: File not loaded";
        private string _SortedImage = "";

        private Visibility _VerticalPanelVisibility = Visibility.Visible;
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;

        private ICommand _goToSettings;

        private Bitmap image;
        private Model.ImageConverter imageSaveTool = new Model.ImageConverter();
        private Sorts sorts = new Sorts();


        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;

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
                    image = sorts.SortByBrightness(image);
                    imageSaveTool.Save(image);
                    SortedImage = imageSaveTool.SavedImagePath;
                    SaveEnabled = true;
                    ProcessEnabled = false;
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
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JPG Image|*.jpg|PNG Image|*.png|Bitmap Image| *.bmp";
            saveDialog.Title = "Save a PixelSorted Image";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveDialog.FileName != "")
            {
                string saveName = saveDialog.FileName;

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(@saveName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        switch (saveName)
                        {

                        }
                        image.Save(memory, ImageFormat.Jpeg);
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
                Filter = "JPG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                ImagePath = openFileDialog.FileName;
                image = new Bitmap(@ImagePath);
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