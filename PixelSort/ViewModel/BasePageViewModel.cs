using Microsoft.Win32;
using PixelSort.EventHandling;
using PixelSort.Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PixelSort.ViewModel
{
    public class BasePageViewModel : BaseViewModel, IPageViewModel, INotifyPropertyChanged
    {
        private ICommand _goToSettings;
        private Visibility _HorizontalPanelVisibility = Visibility.Collapsed;
        private string _imagePath = "/link.jpg";
        private Visibility _VerticalPanelVisibility = Visibility.Visible;
        private int visible = 0;
        // Event Handler for allows UI updates when called
        public event PropertyChangedEventHandler PropertyChanged;
        //ImageToColor imageToColorConvert = new ImageToColor();
        

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

        private Image _SortedImage;

        public Image SortedImage
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
        public ICommand Process
        {
            get
            {
                /*
                Color[,] toProcess = imageToColorConvert.ImageToColorArray(ImagePath);
                SimpleSort simple = new SimpleSort(toProcess);
                SavedImage = simple.Sort();
                */
                return (_goToSettings = new RelayCommand(x =>
                {
                    NotifyPropertyChanged();
                }));
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                SortedImage = null;
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
                }));
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

        private object LoadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };
            Nullable<bool> result = openFileDialog.ShowDialog();
 
            if (result == true)
            {
                ImagePath = openFileDialog.FileName;
            }
            return ImagePath;
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