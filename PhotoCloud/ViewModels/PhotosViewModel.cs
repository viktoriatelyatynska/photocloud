using PhotoCloud.Clients;
using PhotoCloud.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace PhotoCloud.ViewModels
{
    public class PhotosViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly PhotoClient client;

        private ObservableCollection<Photo> photos;
        private bool isLoading;
        private bool isImageSelected;
        private int progress;
        private Photo selectedPhoto;

        #endregion

        #region Constructor

        public PhotosViewModel()
        {
            client = new PhotoClient();
            client.ProgressChanged += PhotoClientProgressChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<Photo> Photos
        {
            get
            {
                return photos;
            }

            set
            {
                photos = value;
                NotifyPropertyChanged("Photos");
            }
        }

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }

            set
            {
                isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public bool IsImageSelected
        {
            get
            {
                return isImageSelected;
            }

            set
            {
                isImageSelected = value;
                NotifyPropertyChanged("IsImageSelected");
            }
        }

        public int Progress
        {
            get
            {
                return progress;
            }

            set
            {
                progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        public Photo SelectedPhoto
        {
            get
            {
                return selectedPhoto;
            }

            set
            {
                selectedPhoto = value;
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Public Methods

        public async Task GetPhotosAsync()
        {
            try
            {
                this.Photos = new ObservableCollection<Photo>();

                IsLoading = true;

                var photos = await client.GetPhotosAsync();

                Photos = new ObservableCollection<Photo>(photos.Reverse());
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message + "\n" + e.StackTrace, "Error").ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UploadPhotoAsync(IStorageFile file)
        {
            try
            {
                this.Progress = 0;
                var uploadedPhoto = await client.UploadPhotoAsync(file);
                this.Photos.Insert(0, uploadedPhoto);
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message + "\n" + e.StackTrace, "Error").ShowAsync();
            }
        }

        public async Task DownloadFileAsync(IStorageFile file)
        {
            try
            {
                if (selectedPhoto != null)
                {
                    await this.client.DownloadPhotoAsync(selectedPhoto.Url, file);
                }
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message + "\n" + e.StackTrace, "Error").ShowAsync();
            }
        }

        public async Task RemoveAsync()
        {
            try
            {
                IsLoading = true;

                if (SelectedPhoto != null)
                {
                    await client.RemovePhotoAsync(SelectedPhoto.PhotoId);
                    this.Photos.Remove(selectedPhoto);
                }
            }
            catch(Exception e)
            {
                await new MessageDialog(e.Message + "\n" + e.StackTrace, "Error").ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Events

        private void PhotoClientProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Progress = e.ProgressPercentage;
        }

        #endregion
    }
}
