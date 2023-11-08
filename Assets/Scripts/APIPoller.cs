using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIPoller : MonoBehaviour
{

    [System.Serializable]
    public class MessageList
    {
        public List<Message> messages;
    }

    private string apiUrl = "http://127.0.0.1:8000/get_new_messages";
    public float pollInterval = 2.0f;
    public Dialogue dialogue;

    private void Start()
    {
        StartCoroutine(PollAPI());
    }

    private IEnumerator PollAPI()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            request.SetRequestHeader("Content-Type", "application/json");

            using (request)
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    MessageList messageList = JsonUtility.FromJson<MessageList>(request.downloadHandler.text);

                    if (dialogue != null && messageList != null && messageList.messages != null && messageList.messages.Count > 0)
                    {
                        dialogue.execute(messageList.messages);
                    }
                    else
                    {
                        if (dialogue == null)
                        {
                            Debug.LogError("Dialogue reference is null!");
                        }
                        if (messageList == null || messageList.messages == null)
                        {
                            Debug.LogError("Message list is null!");
                        }
                    }

                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    Debug.LogError($"Response Code: {request.responseCode}");
                }
            }

            yield return new WaitForSeconds(pollInterval);
        }
    }
}
