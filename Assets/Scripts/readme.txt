● 実装メモ


1.VRoid から VRM をエクスポート
https://www.youtube.com/watch?v=aaks0yg9ZyU&t=4s
    1.VRoid Studio を開く
    2.VRM Export
    3.Export
    4.VRMファイルができる

2.Unityプロジェクトの作成

3.UniVRM のインストール
https://github.com/vrm-c/UniVRM
https://github.com/vrm-c/UniVRM/releases/tag/v0.114.0

    1.VRM0 をダウンロード UniVRM-0.114.0_d90b.unitypackage
    2.Install Package
    3.いくつかの行をコメントアウト(https://www.youtube.com/watch?v=aaks0yg9ZyU&t=4s)

4.VRM to Unity
https://vrm.dev/en/univrm/import/univrm_import.html

    1.VRM0 -> Import From VRM0.x から、エクスポートしたvrmを指定する


3,4 もしくは、ずんだもんをインポートすると、3,4を行わなくてもよい
https://3d.nicovideo.jp/works/td84147
https://sketchfab.com/3d-models/zundamon-1fe78280bbe04107afdafad1fbdecd7c



5.Mixamo を使ってアニメーションを実装する
    https://www.youtube.com/watch?v=4iQIqB5ewOw
    1.FBX Exporter パッケージをインストールする
    2.Hirarchyから、むうちゃんを選び、FBX to Export で、エクスポートする（プロジェクトウィンドウに生成される）
    3.mixamo へ - https://www.mixamo.com/#/
    4.先程のfbxファイルをアップロードする
    5.ほしいアニメーションをクリックして、Downloadボタンを押下する（Format:fbx for unity）
    6.Unityに戻り、Animations フォルダを作成し、その中に入れる
    7.Unityからファイルを選択し、Rig->Animation Typeで、Humanoid にして、Apply を押下
    8.Unityで、先程のアニメーションを展開し、その中のアニメーションfbxを選択して、複製する
    9.複製したアニメーションfbxで、全ての Bake Into Poseとをチェック & Based Upon (at start)をfeetに変更する
    10.Hirarchyから、むうちゃんを選び、Animator のApply Root Motion をチェックする
    11.Animationsフォルダに移り、Animations Controller を追加する
    12.Hirarchyから、むうちゃんを選び、Animator のContoller に先程作ったACを追加する
    13.Animator ウィンドウを表示させる
    14.Animator ウィンドウ上に、アニメーションfbx をD&D する

6. ChatGPT実装
https://www.youtube.com/watch?v=MQfVCY9qgEU&list=PLrE-FZIEEls1-c7QifZYzeq50Id08FcJo


7.OpenAI-Unity - https://github.com/srcnalt/OpenAI-Unity
    1.OpenAI-Unityのインストール（Package Manager のgit URL からインストール）
    https://github.com/srcnalt/OpenAI-Unity.git
    Sample: ChatGPT, Stream Response, Dalli, Whisper をインストール
    

8.サーバーから定期的にチャット欄データを取得する
    1.掲示板の実装(TextScroller) - テキストを右から左に流す
    2.掲示板の実装(APIPoller) - 定期的にチャットを取得する
    3.取得したテキストを会話APIを通じてテキストを生成する
    4.テキストを音声に変換する


9.テキストを音声に変換する
    1.ダウンロード&インスコ https://voicevox.hiroshiba.jp/
    2.起動すると、すでにREST APIを利用できる。
        http://127.0.0.1:50021/docs 
    3.TextToSpeech

10. LipSync 口元を音声に合わせて、シンクロさせる
https://www.youtube.com/watch?v=Q4sPGTVylnY

https://developer.oculus.com/downloads/package/oculus-lipsync-unity/
Oculus Lipsync Unity をダウンロード

11.NewAPI
https://newsapi.org/register

 








