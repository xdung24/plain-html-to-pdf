﻿using System.ComponentModel;
using System.Net;
using PlainHtmlToPdf.Core.Utils;

namespace PlainHtmlToPdf.Core.Handlers;

/// <summary>
/// On download file async complete, success or fail.
/// </summary>
/// <param name="imageUri">The online image uri</param>
/// <param name="filePath">the path to the downloaded file</param>
/// <param name="error">the error if download failed</param>
/// <param name="canceled">is the file download request was canceled</param>
public delegate void DownloadFileAsyncCallback(Uri imageUri, string filePath, Exception error, bool canceled);

/// <summary>
/// Handler for downloading images from the web.<br/>
/// Single instance of the handler used for all images downloaded in a single html, this way if the html contains more
/// than one reference to the same image it will be downloaded only once.<br/>
/// Also handles corrupt, partial and canceled downloads by first downloading to temp file and only if successful moving to cached 
/// file location.
/// </summary>
internal class ImageDownloader : IDisposable
{
    /// <summary>
    /// the web client used to download image from URL (to cancel on dispose)
    /// </summary>
    private readonly List<WebClient> _clients = new List<WebClient>();

    /// <summary>
    /// dictionary of image cache path to callbacks of download to handle multiple requests to download the same image 
    /// </summary>
    private readonly Dictionary<string, List<DownloadFileAsyncCallback>> _imageDownloadCallbacks = new Dictionary<string, List<DownloadFileAsyncCallback>>();

    public ImageDownloader()
    {
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
    }

    /// <summary>
    /// Makes a request to download the image from the server and raises the <see cref="cachedFileCallback"/> when it's down.<br/>
    /// </summary>
    /// <param name="imageUri">The online image uri</param>
    /// <param name="filePath">the path on disk to download the file to</param>
    /// <param name="async">is to download the file sync or async (true-async)</param>
    /// <param name="cachedFileCallback">This callback will be called with local file path. If something went wrong in the download it will return null.</param>
    public void DownloadImage(Uri imageUri, string filePath, bool async, DownloadFileAsyncCallback cachedFileCallback)
    {
        ArgChecker.AssertArgNotNull(imageUri, "imageUri");
        ArgChecker.AssertArgNotNull(cachedFileCallback, "cachedFileCallback");

        // to handle if the file is already been downloaded
        bool download = true;
        lock (_imageDownloadCallbacks)
        {
            if (_imageDownloadCallbacks.ContainsKey(filePath))
            {
                download = false;
                _imageDownloadCallbacks[filePath].Add(cachedFileCallback);
            }
            else
            {
                _imageDownloadCallbacks[filePath] = new List<DownloadFileAsyncCallback> { cachedFileCallback };
            }
        }

        if (download)
        {
            var tempPath = Path.GetTempFileName();
            if (async)
                ThreadPool.QueueUserWorkItem(downloadImageFromUrlAsync, new DownloadData(imageUri, tempPath, filePath));
            else
                downloadImageFromUrl(imageUri, tempPath, filePath);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        releaseObjects();
    }


    #region Private/Protected methods

    /// <summary>
    /// Download the requested file in the URI to the given file path.<br/>
    /// Use async sockets API to download from web, <see cref="onDownloadImageAsyncCompleted"/>.
    /// </summary>
    private void downloadImageFromUrl(Uri source, string tempPath, string filePath)
    {
        try
        {
            using (var client = new WebClient())
            {
                _clients.Add(client);
                client.DownloadFile(source, tempPath);
                onDownloadImageCompleted(client, source, tempPath, filePath, null, false);
            }
        }
        catch (Exception ex)
        {
            onDownloadImageCompleted(null, source, tempPath, filePath, ex, false);
        }
    }

    /// <summary>
    /// Download the requested file in the URI to the given file path.<br/>
    /// Use async sockets API to download from web, <see cref="onDownloadImageAsyncCompleted"/>.
    /// </summary>
    /// <param name="data">key value pair of URL and file info to download the file to</param>
    private void downloadImageFromUrlAsync(object data)
    {
        var downloadData = (DownloadData)data;
        try
        {
            var client = new WebClient();
            _clients.Add(client);
            client.DownloadFileCompleted += onDownloadImageAsyncCompleted;
            client.DownloadFileAsync(downloadData.Uri, downloadData.TempPath, downloadData);
        }
        catch (Exception ex)
        {
            onDownloadImageCompleted(null, downloadData.Uri, downloadData.TempPath, downloadData.FilePath, ex, false);
        }
    }

    /// <summary>
    /// On download image complete to local file.<br/>
    /// If the download canceled do nothing, if failed report error.
    /// </summary>
    private void onDownloadImageAsyncCompleted(object sender, AsyncCompletedEventArgs e)
    {
        var downloadData = (DownloadData)e.UserState;
        try
        {
            using (var client = (WebClient)sender)
            {
                client.DownloadFileCompleted -= onDownloadImageAsyncCompleted;
                onDownloadImageCompleted(client, downloadData.Uri, downloadData.TempPath, downloadData.FilePath, e.Error, e.Cancelled);
            }
        }
        catch (Exception ex)
        {
            onDownloadImageCompleted(null, downloadData.Uri, downloadData.TempPath, downloadData.FilePath, ex, false);
        }
    }

    /// <summary>
    /// Checks if the file was downloaded and raises the cachedFileCallback from <see cref="_imageDownloadCallbacks"/>
    /// </summary>
    private void onDownloadImageCompleted(WebClient client, Uri source, string tempPath, string filePath, Exception error, bool cancelled)
    {
        if (!cancelled)
        {
            if (error == null)
            {
                var contentType = CommonUtils.GetResponseContentType(client);
                if (contentType == null || !contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    error = new InvalidOperationException("Failed to load image, not image content type: " + contentType);
                }

            }

            if (error == null)
            {
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Move(tempPath, filePath);
                    }
                    catch (Exception ex)
                    {
                        error = new InvalidOperationException("Failed to move downloaded image from temp to cache location", ex);
                    }
                }

                error = File.Exists(filePath) ? null : (error ?? new InvalidOperationException("Failed to download image, unknown error"));
            }
        }

        List<DownloadFileAsyncCallback> callbacksList;
        lock (_imageDownloadCallbacks)
        {
            if (_imageDownloadCallbacks.TryGetValue(filePath, out callbacksList))
                _imageDownloadCallbacks.Remove(filePath);
        }

        if (callbacksList != null)
        {
            foreach (var cachedFileCallback in callbacksList)
            {
                try
                {
                    cachedFileCallback(source, filePath, error, cancelled);
                }
                catch
                { }
            }
        }
    }

    /// <summary>
    /// Release the image and client objects.
    /// </summary>
    private void releaseObjects()
    {
        _imageDownloadCallbacks.Clear();
        while (_clients.Count > 0)
        {
            try
            {
                var client = _clients[0];
                client.CancelAsync();
                client.Dispose();
                _clients.RemoveAt(0);
            }
            catch
            { }
        }
    }

    #endregion


    #region Inner class: DownloadData

    private sealed class DownloadData
    {
        public readonly Uri Uri;
        public readonly string TempPath;
        public readonly string FilePath;

        public DownloadData(Uri uri, string tempPath, string filePath)
        {

            Uri = uri;
            TempPath = tempPath;
            FilePath = filePath;
        }
    }

    #endregion
}
