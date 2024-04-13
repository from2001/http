using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using STYLY.Http;
using STYLY.Http.Service;
using NUnit.Framework;

public class TestFunctions : MonoBehaviour
{
    static public void LogAndAssert(HttpResponse ResultResponse,bool?isMethodSucceeded, bool shouldSucceed)
    {
        if (((bool)isMethodSucceeded && shouldSucceed) || (!(bool)isMethodSucceeded && !shouldSucceed))
        {
            Debug.Log("isMethodSucceeded: " + isMethodSucceeded);
            Debug.Log("shouldSucceed: " + shouldSucceed);
            Debug.Log("StatusCode: " + ResultResponse.StatusCode);
            Debug.Log("Error Message: " + ResultResponse.Error);
            Debug.Log("Text: " + ResultResponse.Text);
            Assert.Pass("Passed");
        }
        else
        {
            Debug.Log("isMethodSucceeded: " + isMethodSucceeded);
            Debug.Log("shouldSucceed: " + shouldSucceed);
            Debug.Log("StatusCode: " + ResultResponse.StatusCode);
            Debug.Log("Error Message: " + ResultResponse.Error);
            Debug.Log("Text: " + ResultResponse.Text);
            Assert.Fail("Failed");
        }
    }
}
