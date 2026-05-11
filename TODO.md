# TODO.md

## 目的

`SPEC.md`で定義された仕様と現在の実装状況を比較し、未実装機能・不足しているテスト・リファクタリングが必要な箇所を優先順位付きで管理します。

## 比較時の前提

- リポジトリ内に`SPEC.md`は見つかりませんでした。
- そのため、本TODOでは以下を比較元の仕様として扱います。
  - これまでの依頼内容で定義された「金魚すくい3D」仕様
  - `ARCH.md`に整理済みの現行アーキテクチャ
  - 現在の`Assets/Scripts`配下の実装
- 現在のリポジトリはUnityプロジェクト全体ではなく、`Assets/`配下のスクリプトと空フォルダ中心の初期プロトタイプです。

## ステータス管理ルール

各項目は以下のいずれかのステータスで管理します。

- `[未着手]`: まだ実装・対応していない
- `[進行中]`: 一部実装済み、または調査・作業中
- `[完了]`: 現在のコードベースで対応済み

優先度は以下の基準です。

- `P0`: Google Play向けに「遊べる最小プロトタイプ」として必須
- `P1`: リリース品質・保守性・初心者の改修しやすさに重要
- `P2`: 体験改善、拡張性、後続開発で対応したい

## 仕様と実装状況の要約

| 項目 | 現在の状態 | メモ |
| --- | --- | --- |
| フォルダ構成 | `[完了]` | `Assets/Scripts`、`Scenes`、`Prefabs`、`Materials`、`Audio`、`Models`は作成済み |
| 共通データ | `[完了]` | `GameState`、`GameRecord`は実装済み |
| 共通Manager | `[進行中]` | 主要Managerは実装済み。ただしシーン上の接続・運用ルールは未完成 |
| 金魚すくいロジック | `[進行中]` | スコア、タイマー、ポイ耐久値、すくう判定はスクリプト上で実装済み |
| Unityシーン | `[未着手]` | `TitleScene`、`GoldfishScene`、`ResultScene`の実体は未作成 |
| Prefab | `[未着手]` | 金魚Prefab、ポイPrefab、UI Prefabは未作成 |
| ローカル保存 | `[進行中]` | `PlayerPrefs`保存処理は実装済み。実機・Unity上での確認は未実施 |
| SNS共有 | `[進行中]` | Android Intent処理は実装済み。実機確認は未実施 |
| サウンド | `[進行中]` | `SoundManager`は実装済み。ゲームイベントには未接続 |
| テスト | `[未着手]` | EditMode/PlayModeテストは未作成 |
| Androidビルド設定 | `[未着手]` | `ProjectSettings`、Package設定、署名、解像度、権限確認など未配置 |

## P0: 遊べる最小プロトタイプに必須

### P0-01 `[未着手]` 正式な`SPEC.md`を追加する

**分類:** ドキュメント / 仕様管理

**現状:**

- リポジトリ内に`SPEC.md`が存在しない。
- 仕様比較が会話内容と`ARCH.md`依存になっている。

**対応内容:**

- 仕様の単一参照元として`SPEC.md`を追加する。
- 以下を明記する。
  - 対象プラットフォーム: Android
  - シーン構成
  - ゲームルール
  - 操作仕様
  - 保存仕様
  - SNS共有仕様
  - 実装しない機能
  - リリース前チェックリスト

**完了条件:**

- `SPEC.md`がリポジトリに存在する
- `TODO.md`と`ARCH.md`から参照できる

### P0-02 `[未着手]` Unityプロジェクトとして開ける構成を追加する

**分類:** プロジェクト基盤

**現状:**

- `Assets/`は存在するが、`ProjectSettings/`、`Packages/manifest.json`がない。
- Unity Hubからそのまま開ける完全なUnityプロジェクトではない可能性が高い。

**対応内容:**

- Unityバージョンを決める。
- `ProjectSettings/`と`Packages/`を追加する。
- Android Build Support前提の設定を行う。

**完了条件:**

- Unity Editorでプロジェクトを開ける
- Consoleにコンパイルエラーが出ない

### P0-03 `[未着手]` 必須3シーンを作成する

**分類:** シーン実装

**現状:**

- `Assets/Scenes/.gitkeep`のみ存在。
- `TitleScene`、`GoldfishScene`、`ResultScene`の実体がない。

**対応内容:**

- `TitleScene.unity`を作成する。
- `GoldfishScene.unity`を作成する。
- `ResultScene.unity`を作成する。
- Build Settingsに3シーンを登録する。

**完了条件:**

- タイトルから金魚すくいへ遷移できる
- ゲーム終了後にリザルトへ遷移、またはリザルトUIを表示できる
- リトライ・タイトル戻りが動作する

### P0-04 `[未着手]` GoldfishSceneに必要なGameObjectを配置する

**分類:** シーン実装

**現状:**

- スクリプトはあるが、シーン上のGameObject接続は未作成。

**対応内容:**

- `GameManagers`オブジェクトを作成し、以下を追加する。
  - `GameManager`
  - `ScoreManager`
  - `TimerManager`
  - `SaveManager`
  - `ShareManager`
  - `SoundManager`
  - `SceneLoadManager`
  - `InputManager`
  - `UIManager`
  - `GoldfishGameManager`
- `GoldfishSpawner`を配置する。
- `Poi`を配置する。
- `Canvas`とHUDを配置する。

**完了条件:**

- Unity EditorでPlayした時に、タイマー・スコア・ポイ耐久値が表示される
- マウス操作でポイを動かせる

### P0-05 `[未着手]` 金魚Prefabを作成する

**分類:** Prefab / ゲームプレイ

**現状:**

- `Goldfish`スクリプトは実装済み。
- Prefabは未作成。

**対応内容:**

- 仮の`Sphere`で金魚Prefabを作る。
- `Goldfish`をアタッチする。
- Colliderを設定する。
- 必要に応じてLayerを`Goldfish`に分ける。
- `GoldfishSpawner`へPrefabを割り当てる。

**完了条件:**

- Play時に金魚が生成される
- 金魚がランダムに泳ぐ
- ポイですくうと消える

### P0-06 `[未着手]` ポイPrefabまたはシーン上のポイを作成する

**分類:** Prefab / 操作

**現状:**

- `PoiController`は実装済み。
- ポイの見た目・Collider・配置は未作成。

**対応内容:**

- 仮の`Cylinder`または薄い`Cube`でポイを作る。
- `PoiController`をアタッチする。
- `Camera`、`InputManager`、`GoldfishGameManager`参照を設定する。
- `scoopRadius`と`poiHeight`を調整する。

**完了条件:**

- Androidタッチ、Unity Editorマウスの両方でポイが動く
- 離したタイミングですくう判定が走る

### P0-07 `[進行中]` ゲーム終了条件をUnity上で検証する

**分類:** ゲームルール

**現状:**

- コード上は以下を実装済み。
  - 制限時間0で終了
  - ポイ耐久値0で終了
- Unity Editor上での実行確認は未実施。

**対応内容:**

- `TimerManager.OnTimeUp`から`GoldfishGameManager.EndGame()`が呼ばれるか確認する。
- 金魚をすくってポイ耐久値が0になった時に終了するか確認する。
- 終了後に入力を受け付けないことを確認する。

**完了条件:**

- 60秒経過でリザルト表示される
- ポイ耐久値0でリザルト表示される
- 終了後にスコアが増えない

### P0-08 `[進行中]` リザルト表示を完成させる

**分類:** UI / ゲームルール

**現状:**

- `UIManager.ShowResult()`と`GoldfishGameManager.EndGame()`は実装済み。
- 実際の`ResultPanel`や`ResultScene`は未作成。

**対応内容:**

- `GoldfishScene`内でリザルトPanelを表示するか、`ResultScene`へ遷移するか方針を決める。
- 仕様の3シーン構成を優先するなら、結果データの受け渡し方法を決める。
- リトライボタンとSNS共有ボタンを配置する。

**完了条件:**

- 終了後にスコア、すくった数、ハイスコアが表示される
- リトライできる
- SNS共有ボタンが押せる

### P0-09 `[進行中]` ハイスコア保存の動作確認を行う

**分類:** 保存

**現状:**

- `SaveManager`は実装済み。
- `PlayerPrefs`の読み書き実機・Editor確認は未実施。

**対応内容:**

- 初回プレイ時の保存を確認する。
- ハイスコア更新時のみ更新されることを確認する。
- アプリ再起動後もハイスコアが残ることを確認する。

**完了条件:**

- Editor再生停止後もハイスコアが読める
- Android実機でもハイスコアが保持される

### P0-10 `[未着手]` Android実機でSNS共有を確認する

**分類:** Android / 共有

**現状:**

- `ShareManager`にAndroid Intent処理は実装済み。
- 実機確認は未実施。

**対応内容:**

- Androidビルドを作成する。
- 共有ボタンから共有シートが開くか確認する。
- 共有文の日本語とハッシュタグが崩れないか確認する。

**完了条件:**

- Android実機で共有Intentが開く
- 共有文が仕様通り表示される

## P1: リリース品質・保守性に重要

### P1-01 `[未着手]` EditModeテストを追加する

**分類:** テスト

**現状:**

- テストファイルが存在しない。

**対象候補:**

- `ScoreManager`
  - 加算
  - リセット
  - 0以下を加算しない
- `TimerManager`
  - リセット
  - 制限時間設定
- `SaveManager`
  - キー生成方針
  - ハイスコア更新条件
- `ShareManager`
  - 共有文生成

**完了条件:**

- `Assets/Tests/EditMode`配下にテストがある
- Unity Test Runnerで成功する

### P1-02 `[未着手]` PlayModeテストを追加する

**分類:** テスト

**現状:**

- ゲーム進行を自動確認するテストがない。

**対象候補:**

- ゲーム開始で状態が`Playing`になる
- 金魚をすくうとスコアが増える
- ポイ耐久値が減る
- 時間切れで`Result`になる
- リトライで状態が初期化される

**完了条件:**

- `Assets/Tests/PlayMode`配下にテストがある
- Unity Test Runnerで成功する

### P1-03 `[進行中]` リトライ方式を統一する

**分類:** リファクタリング

**現状:**

- `GameManager.RetryGame()`はシーン再読み込み。
- `UIManager`のRetry Buttonは`GoldfishGameManager.StartGame()`へ接続され、同一シーン内リセット。
- 2つのリトライ経路が混在している。

**対応案:**

1. 初学者向けに同一シーン内リセットへ統一する。
2. 完全初期化優先でシーン再読み込みへ統一する。

**推奨:**

- プロトタイプ段階では同一シーン内リセットへ統一。
- 金魚やポイ位置も初期化が必要になったらシーン再読み込みへ切り替える。

**完了条件:**

- リトライボタンと`GameManager.RetryGame()`の挙動が一致している
- `ARCH.md`にも方針が反映されている

### P1-04 `[未着手]` `GoldfishGameManager`の責務を少し整理する

**分類:** リファクタリング

**現状:**

- `GoldfishGameManager`が以下をまとめて担当している。
  - ゲーム進行
  - UI更新
  - 保存
  - 共有
  - スコア連携
  - タイマー連携

**対応案:**

- 完成優先のため現状でも許容。
- ただし、肥大化する場合は以下を分ける。
  - `GoldfishResultData`
  - `GoldfishResultPresenter`
  - `GoldfishRuleSettings`

**完了条件:**

- 1メソッドが長くなりすぎていない
- UI表示文言が`UIManager`側に寄っている
- 保存・共有の呼び出しが読みやすい

### P1-05 `[未着手]` `FindObjectOfType`依存を減らす

**分類:** リファクタリング

**現状:**

- 初心者向けプロトタイプとして`FindObjectOfType`フォールバックを使っている。
- 大きくなると参照関係が見えにくくなる。

**対応内容:**

- 原則Inspector参照を設定する。
- 未設定時だけWarningを出す。
- 自動探索は開発初期用としてコメントに残す。

**完了条件:**

- 主要参照はInspectorで接続されている
- Missing Reference時のエラーが分かりやすい

### P1-06 `[未着手]` `InputManager.RefreshInput()`の呼び出しタイミングを整理する

**分類:** リファクタリング / 入力

**現状:**

- `InputManager.Update()`でも`RefreshInput()`を呼ぶ。
- `PoiController.HandleInput()`からも`RefreshInput()`を呼ぶ。
- Script Execution Orderに依存しにくい一方で、同一フレーム内で二重更新される可能性がある。

**対応案:**

- `InputManager.Update()`のみで入力更新する。
- または`PoiController`から明示的に更新する方式に統一し、`InputManager.Update()`を削除する。

**完了条件:**

- 1フレーム1回だけ入力状態を更新する
- タップ/リリース判定が取りこぼされない

### P1-07 `[未着手]` `UIManager.Bind*`のイベント解除方針を決める

**分類:** リファクタリング / UI

**現状:**

- `BindRetryButton`と`BindShareButton`は`AddListener`する。
- 複数回Bindするとリスナーが重複する可能性がある。
- ただし、`RemoveAllListeners`はInspector設定まで消すリスクがある。

**対応案:**

- 専用の登録済みフラグを持つ。
- `OnDestroy`で登録したリスナーだけ解除する。
- もしくはボタン接続はInspectorに寄せる。

**完了条件:**

- リトライ・共有ボタンが重複実行されない
- Inspectorで設定した他のイベントを壊さない

### P1-08 `[未着手]` Androidリリース向けProjectSettingsを整える

**分類:** Android / リリース準備

**現状:**

- Android向け設定ファイルがリポジトリにない。

**対応内容:**

- Package Nameを設定する。
- Version Code / Version Nameを設定する。
- Portrait/Landscape方針を決める。
- 最小APIレベルを決める。
- IL2CPP/ARM64設定を確認する。
- Keystore運用を決める。

**完了条件:**

- Google Playへアップロード可能なAABを作れる

## P2: 体験改善・将来拡張

### P2-01 `[未着手]` `SoundManager`をゲームイベントへ接続する

**分類:** サウンド

**現状:**

- `SoundManager`は実装済みだが未使用。

**対応内容:**

- ゲーム開始時BGM
- 金魚をすくった時SE
- ポイが破れた時SE
- リザルト表示SE

**完了条件:**

- InspectorでAudioClipを設定できる
- 主要イベントで音が鳴る

### P2-02 `[未着手]` TextMeshPro対応を検討する

**分類:** UI

**現状:**

- `UIManager`は`UnityEngine.UI.Text`を使用している。

**対応内容:**

- TextMeshProを導入するか決める。
- 導入する場合は`Text`を`TMP_Text`へ置き換える。

**完了条件:**

- 日本語表示が安定する
- UIの見た目調整がしやすくなる

### P2-03 `[未着手]` 金魚の種類と点数差をPrefab/ScriptableObject化する

**分類:** 拡張 / ゲーム性

**現状:**

- `Goldfish`は`point`をInspectorで持つ。
- 金魚ごとの見た目・点数設定はまだ体系化されていない。

**対応案:**

- 複数Prefabで対応する。
- または`GoldfishData` ScriptableObjectを作る。

**完了条件:**

- 赤い金魚、黒い金魚、レア金魚などを追加しやすい
- 点数調整がInspectorで完結する

### P2-04 `[未着手]` 水槽の範囲とポイ移動範囲を制限する

**分類:** 操作 / レベル設計

**現状:**

- ポイは画面入力からPlane上へ移動する。
- 水槽外へ移動しない制限は明確ではない。

**対応内容:**

- 移動可能範囲をInspectorで指定する。
- `Mathf.Clamp`でX/Z座標を制限する。

**完了条件:**

- ポイが水槽外へ出ない
- 画面端操作でも破綻しない

### P2-05 `[未着手]` 見た目用の仮Materialを追加する

**分類:** 見た目

**現状:**

- `Assets/Materials`は空。

**対応内容:**

- 水面用Material
- 金魚用Material
- ポイ用Material
- 背景/屋台風Material

**完了条件:**

- 標準オブジェクトだけでも金魚すくいらしく見える

### P2-06 `[未着手]` `ARCH.md`と`TODO.md`を仕様更新に合わせて保守する

**分類:** ドキュメント

**現状:**

- `ARCH.md`は現在の実装構造を記録済み。
- `TODO.md`は`SPEC.md`不在を前提に作成している。

**対応内容:**

- `SPEC.md`追加後にTODOを再比較する。
- 実装完了に合わせてステータスを更新する。

**完了条件:**

- 仕様、構造、TODOの3つが矛盾していない

## 完了済み項目

### `[完了]` 基本フォルダ構成

- `Assets/Scripts/Managers`
- `Assets/Scripts/Common`
- `Assets/Scripts/GoldfishScoop`
- `Assets/Scripts/UI`
- `Assets/Scenes`
- `Assets/Prefabs`
- `Assets/Materials`
- `Assets/Audio`
- `Assets/Models`

### `[完了]` 共通クラス

- `GameState`
- `GameRecord`

### `[完了]` 基本Managerクラス

- `GameManager`
- `UIManager`
- `ScoreManager`
- `TimerManager`
- `SaveManager`
- `SoundManager`
- `ShareManager`
- `SceneLoadManager`
- `InputManager`

### `[完了]` 金魚すくい基本スクリプト

- `GoldfishGameManager`
- `Goldfish`
- `GoldfishSpawner`
- `PoiController`

## 次に着手する推奨順

1. `SPEC.md`を追加する
2. Unityプロジェクトとして開ける構成を追加する
3. `GoldfishScene`を作成する
4. 金魚Prefabとポイを作成する
5. Editorで60秒ゲームとして最後まで遊べることを確認する
6. ハイスコア保存とSNS共有を実機確認する
7. EditMode/PlayModeテストを追加する
