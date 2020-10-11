using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MADO.CLI
{
    public class GooglePlayHelper
    {
        public static GooglePlayHelper Instance = new GooglePlayHelper();
        private bool Initialized { get; set; }
        private AndroidPublisherService Service { get; set; }
        private string CredentialsPath { get; set; }

        private GooglePlayHelper()
        {
        }
        public async Task Initialize(string credsPath)
        {
            this.Initialized = false;
            this.CredentialsPath = credsPath;
            UserCredential credential;
            using (var stream = new FileStream(this.CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, new[] { AndroidPublisherService.Scope.Androidpublisher }, "user", CancellationToken.None);
            }
            this.Service = new AndroidPublisherService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MADO"
            });
            this.Initialized = true;
        }
        private async Task<string> GetEditId(string packageName, string timeout = "60")
        {
            AppEdit edit = await this.Service.Edits.Insert(new AppEdit() { ExpiryTimeSeconds = timeout }, packageName).ExecuteAsync();
            return edit.Id;
        }
        
        public async Task UploadAPK(string apkPath, DeploymentParameters parameters)
        {
            Logger.instance.LogInfo("Uploading APK ...");
            string packageName = parameters.PackageName;
            if (!Initialized)
            {
                throw new Exception("GooglePlay helper must be initialized first");
            }
            if (this.Service == null) { return; }
            string editId = await GetEditId(packageName);

            //1- Uploads the APK to the artifact library
            if (!File.Exists(apkPath))
            {
                throw new FileNotFoundException($"Apk file not found '{apkPath}'");
            }
            FileStream stream = new FileStream(apkPath, FileMode.Open);
            IUploadProgress progress = await this.Service.Edits.Apks.Upload(packageName, editId, stream, "application/octet-stream").UploadAsync();

            //2- Create a new release
            Track track = new Track();
            track.TrackValue = parameters.TrackName;
            TrackRelease release = new TrackRelease();
            release.Name = $"{parameters.ReleasePrefix}.VC{parameters.VersionCode}.V{parameters.VersionName}";
            long versionCodeLong = -1;
            if(!long.TryParse(parameters.VersionCode, out versionCodeLong))
            {
                throw new Exception("Failed to parse version code");
            }
            release.VersionCodes = new List<long?>() { versionCodeLong };
            release.Status = parameters.ReleaseStatus;
            track.Releases = new List<TrackRelease>() {
                release
            };
            await this.Service.Edits.Tracks.Update(track, packageName, editId, track.TrackValue).ExecuteAsync();
            await this.Service.Edits.Commit(packageName, editId).ExecuteAsync();
            Logger.instance.LogInfo("APK uploaded successfully !");
        }
    }
}






//public async Task<List<string>> GetLanguages(string packageName)
//{
//    List<StoreListing> listings = await GetAllListings(packageName);
//    return listings?.Select(x => x.Language).ToList();
//}

///*Listings
// =============================================================================================*/
//#region Listings
//public async Task<List<StoreListing>> GetAllListings(string packageName)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return null; }
//    string editId = await GetEditId(packageName);
//    ListingsListResponse listingListResponse = await this.Service.Edits.Listings.List(packageName, editId).ExecuteAsync();
//    if (listingListResponse != null && listingListResponse.Listings != null)
//    {
//        List<StoreListing> result = new List<StoreListing>();
//        foreach (Listing listing in listingListResponse.Listings)
//        {
//            result.Add(listing.ToStoreListing(packageName));
//        }
//        return result;
//    }
//    return null;
//}
//public async Task<StoreListing> GetListing(string packageName, string language)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return null; }
//    string editId = await GetEditId(packageName);
//    Listing listing = await this.Service.Edits.Listings.Get(packageName, editId, language).ExecuteAsync();
//    return listing.ToStoreListing(packageName);
//}
//public async Task UpdateListing(StoreListing source)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return; }
//    string editId = await GetEditId(source.PackageName);
//    Listing target = await this.Service.Edits.Listings.Get(source.PackageName, editId, source.Language).ExecuteAsync();
//    if (target.Title != source.Title || target.ShortDescription != source.ShortDescription || target.FullDescription != source.FullDescription)
//    {
//        if (target.Title != source.Title)
//        {
//            target.Title = source.Title;
//        }
//        if (target.ShortDescription != source.ShortDescription)
//        {
//            target.ShortDescription = source.ShortDescription;
//        }
//        if (target.FullDescription != source.FullDescription)
//        {
//            target.FullDescription = source.FullDescription;
//        }
//        await Service.Edits.Listings.Patch(target, source.PackageName, editId, source.Language).ExecuteAsync();
//        AppEdit commit = await this.Service.Edits.Commit(source.PackageName, editId).ExecuteAsync();
//    }
//}
//#endregion


///*Images
// ============================================================*/
//#region Images
//public async Task<string> GetIcon(string packageName, string language)
//{
//    List<string> images = await GetImages(packageName, language, ImageTypeEnum.Icon);
//    return images?.First();
//}
//public async Task<List<string>> GetImages(string packageName, string language, ImageTypeEnum imageType)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return null; }
//    string editId = await GetEditId(packageName);
//    ImagesListResponse imagesListResponse = await this.Service.Edits.Images.List(packageName, editId, language, imageType).ExecuteAsync();
//    return imagesListResponse?.Images.Select(i => i.Url).ToList();
//}
//#endregion



///*APKS
// ============================================================*/
//#region APKS
//public async Task<long?> GetLatestVersionCode(string packageName, StoreTrack storeTrack)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return null; }
//    string editId = await GetEditId(packageName);
//    Track track = await this.Service.Edits.Tracks.Get(packageName, editId, storeTrack.ToValue()).ExecuteAsync();
//    long? latest = track.Releases.Where(r => r.Status == "completed")?.First()?.VersionCodes?.First();
//    return latest;
//}

//public async Task<List<string>> GetTrackNames(string packageName)
//{
//    await CheckInitialization();
//    if (this.Service == null) { return null; }
//    string editId = await GetEditId(packageName);
//    var tracks = await Service.Edits.Tracks.List(packageName, editId).ExecuteAsync();
//    List<string> trackNames = tracks.Tracks.Select(t => t.TrackValue).ToList();
//    return trackNames;
//}