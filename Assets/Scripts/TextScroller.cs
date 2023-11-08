using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TextScroller : MonoBehaviour
{
    public Transform contentArea; // Contentエリアのトランスフォーム
    public GameObject messagePrefab; // メッセージプレハブ
    public ScrollRect scrollRect; // ScrollRectコンポーネントへの参照
    public float extraHeight;

    private void Awake()
    {
        // ScrollRectコンポーネントの参照を取得します（インスペクタから設定するか、またはここで自動的に見つけます）
        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>();
    }

    public void AddMessage(string userName, string messageText)
    {
        GameObject messageInstance = Instantiate(messagePrefab, contentArea);
        messageInstance.GetComponent<RectTransform>().localScale = Vector3.one; // スケールを1に設定

        TextMeshProUGUI nameTextMesh = messageInstance.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageTextMesh = messageInstance.transform.Find("Message").GetComponent<TextMeshProUGUI>();

        // 名前とメッセージのテキストを設定
        if (nameTextMesh != null) nameTextMesh.text = userName;
        if (messageTextMesh != null) messageTextMesh.text = messageText;

        // レイアウトを更新してテキストの高さを取得
        Canvas.ForceUpdateCanvases();
        float nameHeight = nameTextMesh.GetComponent<RectTransform>().sizeDelta.y;
        float messageHeight = messageTextMesh.GetComponent<RectTransform>().sizeDelta.y;


        // messagePrefabの高さを設定
        RectTransform messageRectTransform = messageInstance.GetComponent<RectTransform>();
        messageRectTransform.sizeDelta = new Vector2(messageRectTransform.sizeDelta.x, nameHeight + messageHeight + extraHeight);

        // Contentのレイアウトを強制的に再構築
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)contentArea);

        // スクロールビューの最下部にスクロールする
        StartCoroutine(ScrollToBottom());
    }



    // コンテンツを最下部にスクロールするコルーチン
    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(0.1f);
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
