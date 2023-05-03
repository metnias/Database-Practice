using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

public class Database : MonoBehaviour
{
    public class DataScore
    {
        public string ID { get; set; }
        public int Score { get; set; }
    }

    private void Start()
    {

        StartCoroutine(AddScoreCoroutine("tester", 5));
    }

    private IEnumerator AddScoreCoroutine(string id, int score)
    {
        WWWForm form = new();
        form.AddField(nameof(id), id);
        form.AddField(nameof(score), score);

        using (UnityWebRequest www = 
            UnityWebRequest.Post("" + "http://127.0.0.1/addscore.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError(www.error);
            else
            {
                Debug.Log($"AddScore Success : {id} ({score})");
            }
        }

    }

}
