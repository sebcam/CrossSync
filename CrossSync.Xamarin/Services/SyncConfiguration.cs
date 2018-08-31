namespace CrossSync.Xamarin.Services
{
  public class SyncConfiguration
  {
    public string ApiBaseUrl { get; set; }
    public string TombstoneUri { get; set; } = "api/tombstone";
  }
}
