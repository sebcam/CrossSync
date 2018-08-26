using System;
using System.Collections.Generic;
using System.Text;

namespace CrossSync.Xamarin.Services
{
  public interface IErrorService
  {
    void ShowError(string errorMessage);
  }
}
