using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using STYLY.Http;
using STYLY.Http.Service;
using System.Threading.Tasks;
using STYLY.Http.Service.Unity;
using System;

namespace WebrequestVisualScriptingNodes
{
    [UnitShortTitle("Get AudioClip")]
    [UnitTitle("Get AudioClip")]
    [UnitCategory("Web Request")]
    [UnitSubtitle("Get AudioClip with URL")]
    public class GetAudioClip : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger_Error;

        [DoNotSerialize]
        public ControlOutput outputTrigger_Progress;

        [DoNotSerialize]
        public ValueInput URL;

        // [Inspectable, UnitHeaderInspectable, Serialize]
        // public AudioType AudioType2{get; set;} = AudioType.UNKNOWN;

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput AudioType { get; private set; }

        [DoNotSerialize]
        public ValueOutput result;

        [DoNotSerialize]
        public ValueOutput progress;

        private AudioClip resultValue;
        private float progressValue;
        private float progressValuePrev;
        protected override void Definition()
        {
            inputTrigger = ControlInputCoroutine("inputTrigger", Enter);
            outputTrigger = ControlOutput("Success");
            outputTrigger_Error = ControlOutput("Error");
            outputTrigger_Progress = ControlOutput("Progress");

            URL = ValueInput<string>("URL", String.Empty);
            AudioType = ValueInput("AudioType", UnityEngine.AudioType.UNKNOWN);
            result = ValueOutput<AudioClip>("AudioClip", (flow) => resultValue);
            progress = ValueOutput<float>("Progress Value", (flow) => progressValue);
        }

        private IEnumerator Enter(Flow flow)
        {
            string url = flow.GetValue<string>(URL);
            AudioType audioType = flow.GetValue<AudioType>(AudioType);
            HttpResponse httpResponse = null;
            UniTask.Create(async () => { httpResponse = await GetAudioClipTask(url, audioType); }).Forget();

            while (httpResponse == null)
            {
                if (progressValue != progressValuePrev)
                {
                    yield return outputTrigger_Progress;
                    progressValuePrev = progressValue;
                }
                yield return null;  // Wait until next frame
            }

            if (httpResponse.IsSuccessful)
            {
                resultValue = httpResponse.AudioClip;
                yield return outputTrigger;
            }
            else
            {
                Debug.LogAssertion("Error:" + httpResponse.StatusCode);
                yield return outputTrigger_Error;
            }
        }

        private async UniTask<HttpResponse> GetAudioClipTask(string url, AudioType audioType)
        {
            HttpResponse httpResponse = await Http.GetAudioClip(url, audioType)
            .UseCache(CacheType.UseCacheAlways)
            .OnError(response => Debug.Log(response.StatusCode))
            .OnNetworkError(response => Debug.Log("NetWorkError"))
            .OnDownloadProgress(progress => progressValue = progress)
            .SendAsync();
            return httpResponse;
        }



    }
}

