using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using STYLY.Http;
using STYLY.Http.Service;
using System.Threading.Tasks;
using STYLY.Http.Service.Unity;

namespace WebrequestVisualScriptingNodes
{
    [UnitShortTitle("Get Texture")]
    [UnitTitle("Get Texture")]
    [UnitCategory("Web Request")]
    [UnitSubtitle("Get Texture with URL")]
    public class GetTexture : Unit
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

        [DoNotSerialize]
        public ValueOutput result;

        [DoNotSerialize]
        public ValueOutput progress;

        private Texture resultValue;
        private float progressValue;
        private float progressValuePrev;
        protected override void Definition()
        {
            inputTrigger = ControlInputCoroutine("inputTrigger", Enter);
            outputTrigger = ControlOutput("Success");
            outputTrigger_Error = ControlOutput("Error");
            outputTrigger_Progress = ControlOutput("Progress");

            URL = ValueInput<string>("URL", "");
            result = ValueOutput<Texture>("Texture", (flow) => resultValue);
            progress = ValueOutput<float>("Progress Value", (flow) => progressValue);
        }

        private IEnumerator Enter(Flow flow)
        {
            string url = flow.GetValue<string>(URL);
            HttpResponse httpResponse = null;
            UniTask.Create(async () => { httpResponse = await GetTextureTask(url); }).Forget();

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
                resultValue = httpResponse.Texture;
                yield return outputTrigger;
            }
            else
            {
                Debug.LogAssertion("Error:" + httpResponse.StatusCode);
                yield return outputTrigger_Error;
            }
        }

        private async UniTask<HttpResponse> GetTextureTask(string url)
        {
            HttpResponse httpResponse = await Http.GetTexture(url)
            .UseCache(CacheType.UseCacheAlways)
            .OnError(response => Debug.Log(response.StatusCode))
            .OnNetworkError(response => Debug.Log("NetWorkError"))
            .OnDownloadProgress(progress => progressValue = progress)
            .SendAsync();
            return httpResponse;
        }



    }
}

