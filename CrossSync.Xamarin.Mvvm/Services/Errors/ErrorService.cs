using System;
using Acr.UserDialogs;
using CrossSync.Xamarin.Services;

namespace CrossSync.Xamarin.Mvvm.Services.Errors
{
  public class ErrorService : IErrorService
  {
    private readonly IUserDialogs dialogs;

    public ErrorService(IUserDialogs dialogs)
    {
      this.dialogs = dialogs;
    }
    public void ShowError(string error)
    {
      using (dialogs.Toast(new ToastConfig(error) { Duration = new TimeSpan(0, 0, 2), Position = ToastPosition.Top })) { }
    }
  }
}
