using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using STYLY.Http;
using STYLY.Http.Service;
using UnityEngine.UI;
using UnityEditor.VersionControl;
using System.Text.RegularExpressions;

public class TestsForHttpScript
{
    [UnityTest]
    public IEnumerator Get_Text_SuccessCase()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://styly.inc/ja/news/raised-funds-from-mitsubishicorporation/";
        bool shouldSucceed = true;

        var request = Http.Get(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);
        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_Text_FailCase_404()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://styly.inc/404";
        bool shouldSucceed = false;

        var request = Http.Get(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);
        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_Text_FailCase_http()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "http://styly.inc/";
        bool shouldSucceed = false;

        var request = Http.Get(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);
        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_AudioClip_SuccessCase()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://p.scdn.co/mp3-preview/6ff34bac977ed78af387400a6b336986ba2387b3?cid=f161737260a84397b956f48a107dfc41?dummy.mp3";
        bool shouldSucceed = true;

        var request = Http.GetAudioClip(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        var audioClip = ResultResponse.AudioClip;
        Debug.Log("AudioClip Length: " + audioClip.length);
        if (audioClip.length == 0)
        {
            Assert.Fail("The length of AudioClip is 0");
        }

        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_AudioClip_FailCase_UrlWithoutExtension()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://p.scdn.co/mp3-preview/6ff34bac977ed78af387400a6b336986ba2387b3?cid=f161737260a84397b956f48a107dfc41";
        bool shouldSucceed = false;

        var request = Http.GetAudioClip(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        LogAssert.Expect(LogType.Error, new Regex("^Unable to determine the audio type from the URL"));

        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_AudioClip_SuccessCase_UrlWithoutExtension_WithCorrectAudioType()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://p.scdn.co/mp3-preview/6ff34bac977ed78af387400a6b336986ba2387b3?cid=f161737260a84397b956f48a107dfc41";
        bool shouldSucceed = true;

        var request = Http.GetAudioClip(url, AudioType.MPEG)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_AudioClip_FailCase_UrlWithoutExtension_WithWrongAudioType()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://p.scdn.co/mp3-preview/6ff34bac977ed78af387400a6b336986ba2387b3?cid=f161737260a84397b956f48a107dfc41";
        bool shouldSucceed = false;

        var request = Http.GetAudioClip(url, AudioType.WAV)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        LogAssert.Expect(LogType.Error, new Regex("^Error: Cannot create FMOD::Sound instance for clip"));
        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_Texture_SuccessCase()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://styly.inc/wp-content/uploads/2024/02/0bddd0f4e69eb4b1ff95b35546b7ac21.jpg";
        bool shouldSucceed = true;

        var request = Http.GetTexture(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        Texture texture = null;
        if (ResultResponse != null)
        {
            texture = ResultResponse.Texture;
            Debug.Log("Texture Width: " + texture.width);
            Debug.Log("Texture Height: " + texture.height);
        }

        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Get_Texture_FailCase_NonImage()
    {
        bool? isMethodSucceeded = null;
        HttpResponse ResultResponse = null;

        string url = "https://styly.inc/";
        bool shouldSucceed = false;

        var request = Http.GetTexture(url)
            .UseCache(CacheType.DoNotUseCache)
            .OnSuccess(response => { isMethodSucceeded = true; ResultResponse = response; })
            .OnError(response => { isMethodSucceeded = false; ResultResponse = response; })
            .Send();

        yield return new WaitUntil(() => isMethodSucceeded != null);

        Texture texture = null;
        if (ResultResponse.Texture != null)
        {
            texture = ResultResponse.Texture;
            Debug.Log("Texture Width: " + texture.width);
            Debug.Log("Texture Height: " + texture.height);
        }

        TestFunctions.LogAndAssert(ResultResponse, isMethodSucceeded, shouldSucceed);
        yield return null;
    }


}
