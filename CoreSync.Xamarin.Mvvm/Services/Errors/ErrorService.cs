using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CoreSync.Xamarin.Services;

namespace CoreSync.Xamarin.Mvvm.Services.Errors
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
