
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Sample.TodoList.Xamarin.Droid
{
  [Activity(Label = "Sample.TodoList.Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
  {
    protected override void OnCreate(Bundle bundle)
    {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      base.OnCreate(bundle);

      App.Init(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));

      UserDialogs.Init(() => this);

      global::Xamarin.Forms.Forms.Init(this, bundle);
      LoadApplication(new App());
    }
  }
}

