using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VRM;
using TMPro;
using System.Collections;

public static class UnityWebRequestExtensions
{
    public static Task<UnityWebRequest> AsTask(this UnityWebRequestAsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();
        asyncOp.completed += _ => tcs.SetResult(asyncOp.webRequest);
        return tcs.Task;
    }
}


public class TextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private VRM.VRMBlendShapeProxy blendShapeProxy; // VRMBlendShapeProxyの参照を追加
    [SerializeField] private GameObject fukidashi; // Fukidashiの参照を追加
    [SerializeField] private TextMeshProUGUI fukidashiText; // Text(TMP)の参照を追加
    [SerializeField] private float padding = 20f; // Fukidashiの上下の余白


    private const string VOICEVOX_API_BASE = "http://localhost:50021";
    private float[] samples = new float[1024];  // 音声のサンプルデータを格納する配列


    private void Start()
    {
        HideFukidashi(); // 初期状態で非表示にする
    }

    public void HideFukidashi()
    {
        fukidashi.SetActive(false); // Fukidashiを非表示
    }

    public void ShowFukidashi(string message)
    {
        // メッセージをTextMeshProUGUIに設定
        fukidashiText.text = message;

        // レイアウトの強制更新を行う
        Canvas.ForceUpdateCanvases();

        // TextMeshProUGUIのpreferredHeightを取得
        float textHeight = fukidashiText.preferredHeight;

        // FukidashiのRectTransformを取得
        RectTransform fukidashiRectTransform = fukidashi.GetComponent<RectTransform>();

        // Fukidashiの高さを調整
        fukidashiRectTransform.sizeDelta = new Vector2(fukidashiRectTransform.sizeDelta.x, textHeight + padding);


        // 吹き出しをアクティブにする
        fukidashi.SetActive(true); // Fukidashiを表示
        StartCoroutine(HideFukidashiAfterSeconds(20)); // 20秒後に非表示にする
    }


    private IEnumerator HideFukidashiAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideFukidashi();
    }



    public async void ConvertTextToSpeech(string generated_message)
    {
        if (string.IsNullOrEmpty(generated_message)) return;


        // 1. テキストをaudio_queryエンドポイントに送信して、query.jsonを取得する
        string queryUrl = $"{VOICEVOX_API_BASE}/audio_query?speaker=1&text={UnityWebRequest.EscapeURL(generated_message)}";
        Debug.Log(queryUrl);
        var queryRequest = new UnityWebRequest(queryUrl, "POST");
        queryRequest.downloadHandler = new DownloadHandlerBuffer();
        queryRequest.SetRequestHeader("Content-Type", "application/json");

        await queryRequest.SendWebRequest().AsTask();

        if (queryRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(queryRequest.error);
            Debug.LogError(queryRequest.downloadHandler.text);
            return;
        }

        string queryJson = queryRequest.downloadHandler.text;

        // 2. 取得したquery.jsonを使用して、synthesisエンドポイントからaudio.wavを取得する
        string synthesisUrl = $"{VOICEVOX_API_BASE}/synthesis?speaker=1";
        Debug.Log(synthesisUrl);
        var synthesisRequest = new UnityWebRequest(synthesisUrl, "POST");
        byte[] synthesisBodyRaw = Encoding.UTF8.GetBytes(queryJson);
        synthesisRequest.uploadHandler = new UploadHandlerRaw(synthesisBodyRaw);
        synthesisRequest.downloadHandler = new DownloadHandlerBuffer();
        synthesisRequest.SetRequestHeader("Content-Type", "application/json");

        await synthesisRequest.SendWebRequest().AsTask();

        if (synthesisRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(synthesisRequest.error);
            return;
        }

        byte[] audioData = synthesisRequest.downloadHandler.data;

        // 3. 取得したaudio.wavを再生する
        var audioClip = ToAudioClip(audioData); 
        if (audioClip != null)
        {
            audioSource.clip = audioClip;  // ここでAudioClipをAudioSourceに設定
            Debug.Log("audioSource.Play(); が再生された");
            audioSource.Play();
        }


        // 吹き出す
        ShowFukidashi(generated_message);


    }

    private AudioClip ToAudioClip(byte[] wavData)
    {

        // WAVデータのヘッダーを解析
        int channels = BitConverter.ToInt16(wavData, 22);
        int sampleRate = BitConverter.ToInt32(wavData, 24);
        int sampleCount = BitConverter.ToInt32(wavData, 40) / 2; // 16-bit samples, so divide by 2
        int startPos = 44; // WAVデータのヘッダーサイズ

        float[] samples = new float[sampleCount];

        // サンプルデータを取得
        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = BitConverter.ToInt16(wavData, startPos + i * 2) / (float)Int16.MaxValue;
        }

        // AudioClipを作成
        AudioClip clip = AudioClip.Create("GeneratedClip", sampleCount, channels, sampleRate, false);
        clip.SetData(samples, 0);

        return clip;
    }

    void Update()
    {
        if (audioSource != null)
        {
            // 音声の音量を取得
            audioSource.GetOutputData(samples, 0);
            float sum = 0.0f;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += samples[i] * samples[i];
            }
            float rmsValue = Mathf.Sqrt(sum / samples.Length);

            // BlendShapeの値を更新
            float blendShapeValue = Mathf.Clamp01(rmsValue * 10.0f);
            if (blendShapeProxy != null)
            {
                blendShapeProxy.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), blendShapeValue);
            }
        }
    }

}