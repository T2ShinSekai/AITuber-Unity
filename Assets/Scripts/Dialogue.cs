using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;


public class Dialogue : MonoBehaviour
{

    public TextScroller textScroller;
    public TextToSpeech textToSpeech;

    private const string AGENT_API = "http://127.0.0.1:8000/dialogue";



    public void execute(List<Message> messages)
    {


        string csvText = GetMessagesAsCSV(messages);

        if (textToSpeech != null)
        {
            StartCoroutine(SendDialogueRequest(csvText, (generated_message) =>
            {
                if (!string.IsNullOrEmpty(generated_message))
                {
                    generated_message = generated_message.Replace("\n", "");
                    textToSpeech.ConvertTextToSpeech(generated_message);
                }
            }));
        }
        else
        {
            Debug.LogWarning("textToSpeech is null!");
        }

        // messagesがnullまたは空の場合、何もしない
        if (messages == null || messages.Count == 0)
        {
            return;
        }

        // textScrollerがnullでない場合、掲示板に表示
        if (textScroller != null)
        {
            foreach (Message msg in messages)
            {
                textScroller.AddMessage(msg.name, msg.message);
            }
        }
        else
        {
            Debug.LogWarning("textScroller is null!");
        }

    }

    private string GetMessagesAsCSV(List<Message> messages)
    {
        List<string> csvLines = new List<string>();

        // ヘッダー行を追加
        csvLines.Add("name, message");

        foreach (Message msg in messages)
        {
            csvLines.Add($"{msg.name}, {msg.message}");
        }

        return string.Join("\n", csvLines);

    }

    //private async Task SendDialogueTask(List<Message> messages)
    public IEnumerator SendDialogueRequest(string message, System.Action<string> callback)
    {

        string escapedMessage = message.Replace("\"", "\\\"").Replace("\n", "\\n");
        string jsonBody = $"{{\"message\":\"{escapedMessage}\"}}";
        Debug.Log($"jsonBody: {jsonBody}");


        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest www = new UnityWebRequest(AGENT_API, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.downloadHandler.text);
            callback(null); // エラー時はnullを返す
        }
        else
        {
            callback(www.downloadHandler.text); // 成功時はレスポンステキストを返す
        }
    }



}
