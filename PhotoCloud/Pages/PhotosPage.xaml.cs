using System;
using System.Collections.Generic;
using System.IO;

using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;

using PhotoCloud.ViewModels;
using PhotoCloud.Models;

namespace PhotoCloud.Pages
{
    public sealed partial class PhotosPage : Page
    {
        //Creating ViewModel object
        private readonly PhotosViewModel viewModel;
        
        //constructor
        public PhotosPage()
        {
            //default staff created when creating visual studio project
            this.InitializeComponent();

            //initializing viewmodel object
            this.viewModel = new PhotosViewModel();

            //this UI context (viewmodel) received out custom PhotosViewModel
            this.DataContext = this.viewModel;

            //window colors. found that on the web
            this.CustomizeTitleBar();
        }


        //occurs when page is opened
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get photos when page loaded
            await this.viewModel.GetPhotosAsync();

            //default base method
            base.OnNavigatedTo(e);
        }

        private async void UploadClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //class that opens File browser window
            var picker = new FileOpenPicker();

            picker.FileTypeFilter.Add(".jpg");

            //opening file browser window
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                //invoke viewModel method that uploads photo
                await this.viewModel.UploadPhotoAsync(file);
            }
        }

        //occurs when selected photo changed by mouse click
        private void SelectedPhotoChanged(object sender, SelectionChangedEventArgs e)
        {
            var gridView = (GridView)sender;

            if (gridView.SelectedItem != null)
            {
                //tells view model that photo IS SELECTED
                this.viewModel.IsImageSelected = true;
                this.viewModel.SelectedPhoto = (Photo)gridView.SelectedItem;
            }
            else
            {
                //tells view model that photo IS NOT SELECTED
                this.viewModel.IsImageSelected = false;
                this.viewModel.SelectedPhoto = null;
            }
        }

        private async void RemoveClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await this.viewModel.RemoveAsync();
        }

        //occurs when clicking download button
        private async void DownloadClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.viewModel.SelectedPhoto != null)
            {
                //class that shows dialog where to save file
                var picker = new FileSavePicker();

                //adding opening computer location
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                //adding filters
                picker.FileTypeChoices.Add("Image", new List<string> { ".jpg" });

                //suggested file name
                picker.SuggestedFileName = Path.GetFileName(this.viewModel.SelectedPhoto.Url);

                //open window to choose where to save file
                var file = await picker.PickSaveFileAsync();

                if (file != null)
                {
                    //saving file
                    await this.viewModel.DownloadFileAsync(file);
                }
            }
        }

        private async void RefreshClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await this.viewModel.GetPhotosAsync();
        }

        private void CustomizeTitleBar()
        {
            
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.BackgroundColor = Colors.Green;
            titleBar.ForegroundColor = Colors.White;

            titleBar.InactiveBackgroundColor = Colors.Green;
            titleBar.InactiveForegroundColor = Colors.White;

            titleBar.ButtonInactiveBackgroundColor = Colors.Green;
            titleBar.ButtonInactiveForegroundColor = Colors.White;

            titleBar.ButtonBackgroundColor = Colors.Green;
            titleBar.ButtonForegroundColor = Colors.White;

            titleBar.ButtonHoverBackgroundColor = Colors.Green;
            titleBar.ButtonHoverForegroundColor = Colors.White;

            titleBar.ButtonPressedBackgroundColor = Colors.Green;
            titleBar.ButtonPressedForegroundColor = Colors.Black;
        }
    }
}
