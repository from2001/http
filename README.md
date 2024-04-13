# Visual Scripting Nodes for WebRequest

Note: This is a fork of [adrenak's http](https://github.com/adrenak/http) which is a fork of [Dubit's unity-http](https://www.github.com/dubit/unity-http)
  
## What is it?

Visual Scripting Node library for Webrequest which is based on Http Unity package developed by Dubit.
The Http system has a quick and easy API for making http requests within Unity. The Http instance will run the WebRequest coroutines for you so you don't have to create it per request.

![WebRequestVisualScriptingNodes](https://github.com/styly-dev/WebRequest_VisualScriptingNodes/assets/387880/4827b358-abaa-4726-ae82-b9ecffb63778)

## Installation

Prerequisites : [Node.js](https://nodejs.org/en/download/) v16 or above
```
# Install openupm-cli
npm install -g openupm-cli

# Go to your unity project directory
cd YOUR_UNITY_PROJECT_DIR

# Install package: com.styly.webrequest-visualscripting-nodes
openupm add com.styly.webrequest-visualscripting-nodes
```

## How to use with Visual Scripting
Find the WebRequest custom nodes in `WebRequest` category with the fuzzy finder.

- Get Text node
- Get Texture node
- Get AudioClip node
- POST node (To be implemented)
- POST Texture node (To be implemented)

### Get Text node
<img width="1044" alt="Get Text" src="https://github.com/styly-dev/WebRequest_VisualScriptingNodes/assets/387880/46f8b6de-221a-4d0c-aa09-0d4070e3c39b">

### Get Texture node
<img width="1044" alt="Get Texture" src="https://github.com/styly-dev/WebRequest_VisualScriptingNodes/assets/387880/7cb0a49c-a44a-4977-a0cf-e29e35a7daf1">

### Get AudioClip node
<img width="1044" alt="Get Audio Clip" src="https://github.com/styly-dev/WebRequest_VisualScriptingNodes/assets/387880/dc4a933e-f7ea-469e-ad5a-04c18afe2c23">

### Progress
<img width="1044" alt="Progress" src="https://github.com/styly-dev/WebRequest_VisualScriptingNodes/assets/387880/797f0420-3ef7-49ad-9b6e-839812e891b7">

### POST node
To be implemented

### POST Texture node
To be implemented

## Samples (Visual Scripting)

How to install samples

Click `Import` button at `Window` - `Package Manager` - `Packages in Project` - `WebRequest Visual Scripting` - `Samples`

Sample Music: 

* [penguinmusic - Modern Chillout (Future Calm)](https://pixabay.com/music/upbeat-penguinmusic-modern-chillout-future-calm-12641/)

## Features

Original features

* Singleton
* Fluent API for configuration
* Success, error and network error events
* Super headers
* Get, POST, HEAD, DELETE, PUT support
* GetTexture support

New features

* Data cache support
* AudioClip support
* Visual Scripting Nodes support

## Requirements

* Unity 2021.3
* .NET 4.5
* C# 7

## How to use Http with C# code

If you are using an AssemblyDefinition then reference the Http Assembly.  
Import the namespace `using STYLY.Http;`

```c#
// Get text
var request = Http.Get("http://YOURWEBSITE.com/xxxxx.txt")
 .UseCache(CacheType.UseCacheAlways)
 .SetHeader("Authorization", "username:password")
 .OnSuccess(response => Debug.Log(response.Text))
 .OnError(response => Debug.Log(response.StatusCode))
 .OnDownloadProgress(progress => Debug.Log(progress))
 .Send();
```

```c#
//  You may want to use CacheType.UseCacheOnlyWhenOffline option for data feed like RSS or RestAPI

var request = Http.Get("http://YOURWEBSITE.com/xxxxx.json")
 .UseCache(CacheType.UseCacheOnlyWhenOffline) // <= Load data from cache only when offline
 .SetHeader("Authorization", "username:password")
 .OnSuccess(response => Debug.Log(response.Text))
 .OnError(response => Debug.Log(response.StatusCode))
 .OnNetworkError(response => Debug.Log("Network Error"))
 .OnDownloadProgress(progress => Debug.Log(progress))
 .Send();
```

```c#
// Get Texture 
var request = Http.GetTexture("http://YOURWEBSITE.com/xxxxx.jpg")
 .UseCache(CacheType.UseCacheAlways)
 .OnSuccess(response => {
     Debug.Log(response.Texture.width);
     Debug.Log(response.Texture.height);
 })
 .OnError(response => Debug.Log(response.StatusCode))
 .OnDownloadProgress(progress => Debug.Log(progress))
 .Send();
```

```c#
// Get AudioClip 
var request = Http.GetAudioClip("http://YOURWEBSITE.com/xxxxx.mp3")
 .UseCache(CacheType.UseCacheAlways)
 .OnSuccess(response => {
     Debug.Log(response.AudioClip.frequency);
 })
 .OnError(response => Debug.Log(response.StatusCode))
 .OnDownloadProgress(progress => Debug.Log(progress))
 .Send();
```

```c#
// Cache file will be generated based of its URL. So if signed URL is used, a cache file will be created every time since Signed URL changes for each access even for the same content. There are several options to avoid it. 

string url = "https://storage.googleapis.com/aaaaaa/bbbbbb/cccccc.png?Expires=11111111&GoogleAccessId=dddddd&Signature=eeeeeeee"
string[] ignorePatterns;

// Option A: You can set ignore patterns manually.
ignorePatterns = new string[] {
    // for Google Cloud Storage
    "(Expires=[^&]+&?|GoogleAccessId=[^&]+&?|Signature=[^&]+&?)", 
    // for Amazon CloudFront
    "(Expires=[^&]*&?|Signature=[^&]*&?|Key-Pair-Id=[^&]*&?)",  
    // for Azure Blob Storage
    "(se=[^&]*&?|sp=[^&]*&?|sv=[^&]*&?|sr=[^&]*&?|sig=[^&]*&?)"
};

// Option B: You can use the following method to get ignore patterns of signed URLs for major cloud service.
ignorePatterns = CacheUtils.GetSignedUrlIgnorePatters();

var request = Http.Get(url)
.UseCache(CacheType.UseCacheAlways, ignorePatterns)
.OnSuccess(response => Debug.Log(response.Text))
.OnError(response => Debug.Log(response.StatusCode))
.OnDownloadProgress(progress => Debug.Log(progress))
.Send();

// Option C: Set `USE_CLOUD_SIGNED_URL_IN_CACHEUTILS` in `Project Settings` - `Player` - `Scripting Define Symbols`
```

```C#
// Asynchronous call
async void Start()
{
    var url = "http://YOURWEBSITE.com/xxxxx.txt";
    STYLY.Http.Service.HttpResponse httpResponse = await Http.Get(url)
        .UseCache(CacheType.UseCacheAlways)
        .OnSuccess(response => Debug.Log(response.Text))
        .OnError(response => Debug.Log(response.StatusCode))
        .OnDownloadProgress(progress => Debug.Log(progress))
        .SendAsync();

    Debug.Log("Web request task completed.");
    Debug.Log(httpResponse.Text);
}
```


## API

### Http Static Methods

All these methods return a new HttpRequest.  

#### Get

* `Http.Get(string uri)`  
* `Http.GetTexture(string uri)`  
* `Http.GetAudioClip(string uri)`  

#### Post

* `Http.Post(string uri, string postData)`  
* `Http.Post(string uri, WWWForm formData)`  
* `Http.Post(string uri, Dictionary<string, string> formData))`  
* `Http.Post(string uri, List<IMultipartFormSection> multipartForm)`  
* `Http.Post(string uri, byte[] bytes, string contentType)`  

#### Post JSON

* `Http.PostJson(string uri, string json)`  
* `Http.PostJson<T>(string uri, T payload)`

#### Put

* `Http.Put(string uri, byte[] bodyData)`
* `Http.Put(string uri, string bodyData)`

#### Misc

* `Http.Delete(string uri)`  
* `Http.Head(string uri)`  

### Http Request Configuration Methods

All these methods return the HttpRequest instance.  

#### Headers

* `SetHeader(string key, string value)`  
* `SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)`  
* `RemoveHeader(string key)`  
* `RemoveSuperHeaders()`  

#### Events

* `OnSuccess(Action<HttpResonse> response)`  
* `OnError(Action<HttpResonse> response)`  
* `OnNetworkError(Action<HttpResonse> response)`  

#### Progress

* `OnUploadProgress(Action<float> progress)`  
* `OnDownloadProgress(Action<float> progress)`  

#### Configure

* `SetRedirectLimit(int redirectLimit)`
* `SetTimeout(int duration)`

#### Data cache
* `UseCache(CacheType cacheType = CacheType.UseCacheAlways, string[] ignorePatternsForCacheFilePathGeneration = null)`


Redirect limit subject to Unity's documentation.  

* [Redirect Limit Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-redirectLimit.html)

Progress events will invoke each time the progress value has increased, they are subject to Unity's documentation.

* [Upload Progress Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-uploadProgress.html)
* [Download Progress Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-downloadProgress.html)

### Http Request

* `HttpRequest Send()`  
* `void Abort()`  

### Http Response

The callbacks for `OnSuccess`, `OnError` and `OnNetworkError` all return you a `HttpResponse`.  
This has the following properties:  

#### Properties

* `string Url`  
* `bool IsSuccessful`  
* `bool IsHttpError`  
* `bool IsNetworkError`  
* `long StatusCode`  
* `byte[] Bytes`  
* `string Text`  
* `string Error`  
* `Texture Texture`  
* `AudioClip AudioClip`  
* `Dictionary<string, string> ResponseHeaders`  

### Super Headers

Super Headers are a type of Header that you can set once to automatically attach to every Request you’re sending.  
They are Headers that apply to all requests without having to manually include them in each HttpRequest SetHeader call.

* `Http.SetSuperHeader(string key, string value)`
* `Http.RemoveSuperHeader(string key)` returns `bool`
* `Http.GetSuperHeaders()` returns `Dictionary<string, string>`

## JSON Response Example

In this given example, the `response.Text` from `http://mywebapi.com/user.json` is:

```json
{
    "id": 92,
    "username": "jason"
}
```

Create a serializable class that maps the data from the json response to fields

```c#
[Serializable]
public class User
{
    [SerializeField]
    public int id;
    [SerializeField]
    public string username;
}
```

We can listen for the event `OnSuccess` with our handler method `HandleSuccess`

```c#
var request = Http.Get("http://mywebapi.com/user.json")
    .OnSuccess(HandleSuccess)
    .OnError(response => Debug.Log(response.StatusCode))
    .Send();
```

Parse the `response.Text` to the serialized class `User` that we declared earlier by using Unity's built in [JSONUtility](https://docs.unity3d.com/ScriptReference/JsonUtility.html)

```c#
private void HandleSuccess(HttpResponse response)
{
     var user = JsonUtility.FromJson<User>(response.Text);
     Debug.Log(user.username);
}
```
