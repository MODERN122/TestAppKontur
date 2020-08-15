using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using TestAppKontur.Models;
using TestAppKontur.Models.Dao;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestAppKontur.ViewModels
{
    class MainPageViewModel : BindableBase, INavigatedAware
    {
        public ObservableCollection<Contact> SearchResults { get; set; } = new ObservableCollection<Contact>();
        private INavigationService _navigationService;
        private IPageDialogService _dialogService;

        private static CancellationTokenSource cancellation;

        public DelegateCommand<string> PerformSearchCommand { get; }
        public DelegateCommand UpdateContactsCommand { get; private set; }
        public DelegateCommand<object> ContactTappedCommand { get; private set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                UpdateContactsCommand.RaiseCanExecuteChanged();
            }
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            PerformSearchCommand = new DelegateCommand<string>(PerformSearch);
            UpdateContactsCommand = new DelegateCommand(UpdateContacts, CanUpdateContacts);
            ContactTappedCommand = new DelegateCommand<object>(ContactTappedAsync, CanContactTapped);
        }

        private void PerformSearch(string query)
        {
            SearchResults.Clear();
            Dao.GetSearchResults(query).ForEach(x => SearchResults.Add(x));
       }

        private bool CanContactTapped(object arg) => true;

        private async void ContactTappedAsync(object obj)
        {
            if (obj is List<object> currentObjs)
            {
                var firstObject = currentObjs.First();
                if (firstObject is Contact contact)
                {
                    var navigationParams = new NavigationParameters();
                    navigationParams.Add("contact", contact);
                    var result = await _navigationService.NavigateAsync("ContactPage", navigationParams);
                    if (!result.Success)
                    {
                        System.Diagnostics.Debugger.Break();
                        await _dialogService.DisplayAlertAsync("Ошибка","Невозможно перейти на страницу контактов","ОК");
                    }
                }
            }
        }

        private bool CanUpdateContacts()
        {
            return !IsLoading;
        }

        private async void UpdateContacts()
        {
            IsLoading = true;
            var result = await Dao.UpdateContacts();
            if (!result)
                await _dialogService.DisplayAlertAsync("Ошибка", "Произошла ошибка при добавлении данных в базу данных из интернета, " +
                    "были загружены старые данные пожалуйста попробуйте позже", "ОК");
            SearchResults.Clear();
            Dao.ContactList.ForEach(x => SearchResults.Add(x));
            IsLoading = false;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            cancellation.Cancel();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            UpdateContacts();
            //Активирую таймер, 
            cancellation = new CancellationTokenSource();
            Device.StartTimer(TimeSpan.FromSeconds(60.0), () =>
            {
                if (cancellation.IsCancellationRequested)
                    return false;
                UpdateContacts();
                return true;
            });
        }
    }
}
