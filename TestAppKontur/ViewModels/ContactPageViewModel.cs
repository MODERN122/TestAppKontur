using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using TestAppKontur.Models;
using Xamarin.Essentials;

namespace TestAppKontur.ViewModels
{
    class ContactPageViewModel : BindableBase, INavigatedAware
    {
        private Contact _currentContact;
        private IPageDialogService _dialogService;

        public DelegateCommand CallCommand { get; }
        public Contact CurrentContact
        {
            get => _currentContact;
            set => SetProperty(ref _currentContact, value);
        }
        public ContactPageViewModel(IPageDialogService dialogService)
        {
            _dialogService = dialogService;
            CallCommand = new DelegateCommand(Call);
        }

        private void Call()
        {
            try
            {
                PhoneDialer.Open(CurrentContact.Phone);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
                _dialogService.DisplayAlertAsync("Ошибка", "Номер пустой", "ОК");
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
                //Crashes.TrackError(ex);
                _dialogService.DisplayAlertAsync("Ошибка", "Звонок не поддерживается на устройстве," +
                    " попробуйте обновить устройство", "ОК");
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                //Crashes.TrackError(ex);
                _dialogService.DisplayAlertAsync("Ошибка", "Неизвестная ошибка", "ОК");
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters["contact"] is Contact contact)
            {
                CurrentContact = contact;
            }
        }
    }
}
