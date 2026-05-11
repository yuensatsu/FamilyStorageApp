# ARCH.md

## 目的

このドキュメントは、Unity/C#で実装された金魚すくい3Dプロトタイプの全体構造を、新しく参加したエンジニアがすぐ把握できるようにまとめたものです。

現時点のリポジトリは、Unityプロジェクト全体ではなく、`Assets/`配下のスクリプトと空フォルダを中心にした「Unityへ取り込むための初期プロトタイプ」です。`ProjectSettings/`、`Packages/manifest.json`、`.csproj`、`.sln`、`*.asmdef`はまだ含まれていません。

## `.cursorrules`確認結果

リポジトリ全体を確認した結果、`.cursorrules`は存在しませんでした。

そのため、明文化されたCursor固有の規約は未定義です。現在のコードから読み取れるプロジェクト内規約は以下です。

- 名前空間は`FamilyStorageApp.<Area>`形式にする
  - `FamilyStorageApp.Common`
  - `FamilyStorageApp.Managers`
  - `FamilyStorageApp.GoldfishScoop`
- クラス名とファイル名は一致させる
- クラス名、メソッド名、プロパティ名は`PascalCase`
- privateフィールドは`camelCase`
- Inspectorで調整したい値は`[SerializeField] private`で定義する
- 初学者向けにXMLコメントと`Debug.Log`を多めに入れる
- シーン上の参照はInspector設定を基本にしつつ、プロトタイプでは`FindObjectOfType`で補助する
- 過度な抽象化を避け、1クラス1責務を意識する

### 規約との整合状況

現在のコードは、おおむね上記の観測規約に沿っています。

注意点として、`Assets/Scripts/UI/`フォルダは存在しますが、現在の`UIManager`は`Assets/Scripts/Managers/`に配置されています。これは「UI表示更新を管理するManager」として見ると自然ですが、将来的にUI専用クラスが増える場合は`Assets/Scripts/UI/`へ分離する余地があります。

## リポジトリ全体構造

```text
/
├─ ARCH.md
├─ README.md
└─ Assets
   ├─ Audio
   │  └─ .gitkeep
   ├─ Materials
   │  └─ .gitkeep
   ├─ Models
   │  └─ .gitkeep
   ├─ Prefabs
   │  └─ .gitkeep
   ├─ Scenes
   │  └─ .gitkeep
   └─ Scripts
      ├─ Common
      │  ├─ GameRecord.cs
      │  └─ GameState.cs
      ├─ GoldfishScoop
      │  ├─ Goldfish.cs
      │  ├─ GoldfishGameManager.cs
      │  ├─ GoldfishSpawner.cs
      │  └─ PoiController.cs
      ├─ Managers
      │  ├─ GameManager.cs
      │  ├─ InputManager.cs
      │  ├─ SaveManager.cs
      │  ├─ SceneLoadManager.cs
      │  ├─ ScoreManager.cs
      │  ├─ ShareManager.cs
      │  ├─ SoundManager.cs
      │  ├─ TimerManager.cs
      │  └─ UIManager.cs
      └─ UI
         └─ .gitkeep
```

## レイヤー構造

```text
FamilyStorageApp
├─ Common
│  ├─ GameState
│  └─ GameRecord
├─ Managers
│  ├─ GameManager
│  ├─ ScoreManager
│  ├─ TimerManager
│  ├─ SaveManager
│  ├─ UIManager
│  ├─ InputManager
│  ├─ ShareManager
│  ├─ SoundManager
│  └─ SceneLoadManager
└─ GoldfishScoop
   ├─ GoldfishGameManager
   ├─ Goldfish
   ├─ GoldfishSpawner
   └─ PoiController
```

### Common

共通のデータ型を置くレイヤーです。特定のミニゲームに依存しません。

### Managers

スコア、タイマー、保存、入力、UI、音、シーン遷移、共有など、ゲーム横断で使う機能を置くレイヤーです。

### GoldfishScoop

金魚すくい3D専用のルール、金魚の挙動、ポイ操作、生成処理を置くレイヤーです。

## 主要クラスの役割

### `FamilyStorageApp.Common`

#### `GameState`

ゲーム全体の状態を表すenumです。

```text
Title
Ready
Playing
Result
Pause
```

主に`GameManager`と`GoldfishGameManager`が利用します。

#### `GameRecord`

ローカル保存する自己記録のデータクラスです。

保持する値:

- `gameId`
- `bestScore`
- `bestTime`
- `playCount`
- `lastPlayedAt`

`SaveManager`が`PlayerPrefs`へ保存・読み込みする時のデータ形として使います。

### `FamilyStorageApp.Managers`

#### `GameManager`

ゲーム全体の状態を管理する薄いハブです。

主な責務:

- `GameState`の変更
- ゲーム開始
- ゲーム終了
- ポーズ
- ポーズ解除
- リトライ指示

依存:

```text
GameManager
└─ SceneLoadManager
```

`RetryGame()`では`SceneLoadManager.RetryGoldfishScene()`を呼び、金魚すくいシーンを再読み込みします。

#### `ScoreManager`

現在スコアだけを管理します。

主な責務:

- `AddScore(int point)`
- `ResetScore()`
- `GetScore()`
- スコア変更イベント`OnScoreChanged`の発火

依存はほぼなく、他クラスから呼ばれるシンプルな状態管理クラスです。

#### `TimerManager`

制限時間を管理します。初期値はInspectorで調整可能で、現在は60秒想定です。

主な責務:

- `StartTimer()`
- `StopTimer()`
- `ResetTimer()`
- `GetRemainingTime()`
- `GetTimeLimit()`
- 残り時間変更イベント`OnTimeChanged`
- 時間切れイベント`OnTimeUp`

依存:

```text
TimerManager
└─ UnityEngine.Time
```

`OnTimeUp`は`GoldfishGameManager.EndGame()`に接続されます。

#### `SaveManager`

`PlayerPrefs`を使ったローカル保存を担当します。

主な責務:

- 記録読み込み
- 記録保存
- ハイスコア更新
- 最速タイム更新
- プレイ回数更新
- 保存データ削除

依存:

```text
SaveManager
└─ GameRecord
```

保存キーは`gameId`付きで生成されます。

例:

```text
GoldfishScoop_HighScore
GoldfishScoop_BestTime
GoldfishScoop_PlayCount
GoldfishScoop_LastPlayedAt
```

#### `UIManager`

画面表示の更新を担当します。現時点ではUnity標準の`UnityEngine.UI.Text`と`Button`を使います。

主な責務:

- スコア表示更新
- 残り時間表示更新
- ポイ耐久値表示更新
- ハイスコア表示更新
- リザルト表示
- リザルト非表示
- リトライボタンの処理登録
- SNS共有ボタンの処理登録

依存:

```text
UIManager
└─ UnityEngine.UI
   ├─ Text
   └─ Button
```

#### `InputManager`

タッチ入力とマウス入力を共通化します。

主な責務:

- ドラッグ中の画面座標取得
- 押した瞬間の判定
- 押し続けている判定
- 離した瞬間の判定

利用側は`Input.touchCount`や`Input.GetMouseButton`を直接見ずに、`InputManager`のプロパティを参照します。

依存:

```text
InputManager
└─ UnityEngine.Input
```

#### `ShareManager`

SNS共有を担当します。

主な責務:

- 金魚すくい用の共有文作成
- Android実機で共有Intentを開く
- Unityエディタでは`Debug.Log`で共有文を表示する

依存:

```text
ShareManager
├─ UnityEngine
└─ AndroidJavaClass / AndroidJavaObject
```

Android共有は以下の条件でのみ有効です。

```csharp
#if UNITY_ANDROID && !UNITY_EDITOR
```

#### `SoundManager`

BGMとSEの再生を担当します。

主な責務:

- BGM再生
- BGM停止
- SE再生
- 音量ON/OFF

現時点では他クラスからまだ参照されていません。今後、金魚をすくった時のSEやタイトルBGMなどを接続する想定です。

#### `SceneLoadManager`

シーン遷移を担当します。

主な責務:

- タイトルシーンへ移動
- 金魚すくいシーンへ移動
- リザルトシーンへ移動
- リトライ

現在使うシーン名:

```text
TitleScene
GoldfishScene
ResultScene
```

依存:

```text
SceneLoadManager
└─ UnityEngine.SceneManagement.SceneManager
```

### `FamilyStorageApp.GoldfishScoop`

#### `GoldfishGameManager`

金魚すくい専用のゲーム進行を管理する中心クラスです。

主な責務:

- ゲーム準備
- ゲーム開始
- ゲーム終了
- 金魚をすくった時の処理
- ポイ耐久値管理
- スコア加算
- タイマー終了時のゲーム終了
- ハイスコア保存
- UI更新
- SNS共有呼び出し

依存:

```text
GoldfishGameManager
├─ GameManager
├─ ScoreManager
├─ TimerManager
├─ SaveManager
├─ ShareManager
├─ UIManager
├─ GameState
├─ GameRecord
└─ Goldfish
```

イベント接続:

```text
TimerManager.OnTimeUp
└─ GoldfishGameManager.EndGame

TimerManager.OnTimeChanged
└─ GoldfishGameManager.HandleTimeChanged

ScoreManager.OnScoreChanged
└─ GoldfishGameManager.HandleScoreChanged

UIManager retry button
└─ GoldfishGameManager.StartGame

UIManager share button
└─ GoldfishGameManager.ShareResult
```

#### `Goldfish`

金魚1匹の挙動を担当します。

主な責務:

- 水槽内をランダムに泳ぐ
- 点数を持つ
- すくわれたら`GoldfishGameManager`へ通知する
- すくわれた後、自分自身を削除する

依存:

```text
Goldfish
└─ GoldfishGameManager
```

#### `GoldfishSpawner`

金魚を生成し、一定数を維持します。

主な責務:

- 初期金魚生成
- 減った金魚の補充
- Prefabから生成
- 生成範囲のInspector調整
- Sceneビューで生成範囲をGizmo表示

依存:

```text
GoldfishSpawner
└─ Goldfish prefab
```

#### `PoiController`

プレイヤーが操作するポイを担当します。

主な責務:

- 入力に応じてポイを移動
- 離したタイミング、または押したタイミングで「すくう」判定
- ポイ周辺の金魚を`Physics.OverlapSphere`で検出
- 近い金魚を1匹すくう
- 空振り時に耐久値を減らす設定にも対応

依存:

```text
PoiController
├─ InputManager
├─ GoldfishGameManager
└─ Goldfish
```

`InputManager`が見つからない場合は、プロトタイプの動作優先でポイ自身に自動追加します。

## 金魚すくいのデータフロー

### 1. シーン開始

```text
Unity Scene Start
└─ GoldfishGameManager.Awake
   ├─ Manager参照を探す
   ├─ TimerManager.OnTimeUpへEndGameを登録
   ├─ TimerManager.OnTimeChangedへUI更新を登録
   ├─ ScoreManager.OnScoreChangedへUI更新を登録
   └─ UIManagerのボタンへRetry/Share処理を登録
```

### 2. ゲーム開始

```text
GoldfishGameManager.StartGame
├─ PrepareGame
│  ├─ 状態をReadyへ
│  ├─ ポイ耐久値を初期化
│  ├─ すくった数を0へ
│  ├─ ScoreManager.ResetScore
│  ├─ TimerManager.ResetTimer
│  ├─ UIManager.HideResult
│  └─ UI更新
├─ 状態をPlayingへ
├─ SaveManager.AddPlayCount
└─ TimerManager.StartTimer
```

### 3. プレイ中の入力

```text
Player Input
└─ InputManager.RefreshInput
   ├─ Touch入力を読む
   └─ TouchがなければMouse入力を読む

PoiController.Update
├─ GameState.Playingか確認
├─ InputManagerの画面座標を取得
├─ Camera.ScreenPointToRay
├─ 水平Planeとの交点を計算
└─ ポイを移動
```

### 4. すくう処理

```text
Pointer Up / Pointer Down
└─ PoiController.TryScoop
   ├─ Physics.OverlapSphere
   ├─ 範囲内のGoldfishを探す
   ├─ 一番近いGoldfishを選ぶ
   └─ Goldfish.Scoop
      ├─ GoldfishGameManager.OnGoldfishScooped
      │  ├─ scoopedCount++
      │  ├─ ScoreManager.AddScore
      │  ├─ ポイ耐久値を減らす
      │  └─ UI更新
      └─ Destroy(gameObject)
```

### 5. スコア更新

```text
ScoreManager.AddScore
├─ currentScore += point
└─ OnScoreChanged
   └─ GoldfishGameManager.HandleScoreChanged
      └─ UIManager.UpdateScore
```

### 6. タイマー更新

```text
TimerManager.Update
├─ remainingTime -= Time.deltaTime
├─ OnTimeChanged
│  └─ UIManager.UpdateTime
└─ remainingTime <= 0
   └─ OnTimeUp
      └─ GoldfishGameManager.EndGame
```

### 7. ゲーム終了

終了条件:

- 制限時間が0になる
- ポイ耐久値が0になる

```text
GoldfishGameManager.EndGame
├─ 状態をResultへ
├─ TimerManager.StopTimer
├─ ScoreManager.GetScore
├─ SaveManager.SaveHighScoreIfNeeded
├─ SaveManager.SaveBestTimeIfNeeded
├─ UIManager.ShowResult
└─ ハイスコアUI更新
```

### 8. 共有

```text
Share Button
└─ GoldfishGameManager.ShareResult
   ├─ ShareManager.CreateGoldfishShareText
   └─ ShareManager.ShareText
      ├─ Android実機: ACTION_SEND Intent
      └─ Editor: Debug.Log
```

## 依存関係まとめ

```text
Common
├─ GameState
└─ GameRecord

Managers
├─ GameManager
│  └─ SceneLoadManager
├─ ScoreManager
├─ TimerManager
├─ SaveManager
│  └─ GameRecord
├─ UIManager
│  └─ UnityEngine.UI
├─ InputManager
│  └─ UnityEngine.Input
├─ ShareManager
│  └─ Android share intent
├─ SoundManager
│  └─ AudioSource / AudioClip
└─ SceneLoadManager
   └─ SceneManager

GoldfishScoop
├─ GoldfishGameManager
│  ├─ GameManager
│  ├─ ScoreManager
│  ├─ TimerManager
│  ├─ SaveManager
│  ├─ UIManager
│  ├─ ShareManager
│  ├─ GameState
│  └─ GameRecord
├─ Goldfish
│  └─ GoldfishGameManager
├─ GoldfishSpawner
│  └─ Goldfish
└─ PoiController
   ├─ InputManager
   ├─ GoldfishGameManager
   └─ Goldfish
```

## Unityシーン上の推奨配置

### `GoldfishScene`

```text
GoldfishScene
├─ Main Camera
├─ Directional Light
├─ GameManagers
│  ├─ GameManager
│  ├─ ScoreManager
│  ├─ TimerManager
│  ├─ SaveManager
│  ├─ ShareManager
│  ├─ SoundManager
│  ├─ SceneLoadManager
│  ├─ InputManager
│  ├─ UIManager
│  └─ GoldfishGameManager
├─ Canvas
│  ├─ ScoreText
│  ├─ TimeText
│  ├─ PoiDurabilityText
│  ├─ HighScoreText
│  ├─ ResultPanel
│  │  └─ ResultText
│  ├─ RetryButton
│  └─ ShareButton
├─ FishTank
│  └─ Plane
├─ GoldfishSpawner
└─ Poi
   └─ PoiController
```

### Prefab

```text
Assets/Prefabs
└─ GoldfishPrefab
   ├─ MeshRenderer
   ├─ Collider
   └─ Goldfish
```

## 現時点の注意点

### Unityプロジェクトとしては未完成

このリポジトリには`ProjectSettings`や`Packages`がないため、単体ではUnityプロジェクトとして開けない可能性があります。Unityプロジェクトへ組み込む場合は、これらの`Assets`配下ファイルをUnityプロジェクトに配置してください。

### `UIManager`の配置

`Assets/Scripts/UI/`は空で、`UIManager`は`Assets/Scripts/Managers/`にあります。現在の責務は「UIを管理するManager」なので問題ありませんが、UIコンポーネントが増えたら以下のような分離を検討できます。

```text
Assets/Scripts/UI
├─ ResultView.cs
├─ HudView.cs
└─ TitleView.cs
```

### リトライ経路が2種類ある

現在、リトライには2つの考え方があります。

```text
GameManager.RetryGame
└─ SceneLoadManager.RetryGoldfishScene
   └─ シーンを再読み込み

UIManager Retry Button
└─ GoldfishGameManager.StartGame
   └─ 同じシーン内で状態をリセット
```

初学者向けには、まず`GoldfishGameManager.StartGame`による同一シーン内リトライが分かりやすいです。シーン内の金魚やポイ位置も完全初期化したい場合は、`SceneLoadManager`経由の再読み込みへ統一してください。

### `SoundManager`は未接続

`SoundManager`は実装済みですが、まだ金魚をすくった時のSEやBGMには接続されていません。

次の接続候補:

- ゲーム開始時にBGM再生
- 金魚をすくった時にSE再生
- ポイが破れた時にSE再生
- リザルト表示時にSE再生

### UIはLegacy UI

現在は`UnityEngine.UI.Text`を使っています。TextMeshProを使う場合は、`UIManager`のText型を`TMP_Text`へ変更してください。

## 今後の拡張ポイント

### タイトル・リザルトシーン

現在のゲーム進行は`GoldfishScene`内で完結できます。将来的に3シーン構成へ進める場合は、以下の責務に分けると分かりやすいです。

```text
TitleScene
└─ StartButton
   └─ SceneLoadManager.LoadGoldfishScene

GoldfishScene
└─ GoldfishGameManager
   └─ EndGame後にSceneLoadManager.LoadResultScene

ResultScene
├─ 保存済みスコア表示
├─ RetryButton
│  └─ SceneLoadManager.RetryGoldfishScene
└─ TitleButton
   └─ SceneLoadManager.LoadTitleScene
```

### ミニゲームシリーズ化

今後、射的やヨーヨー釣りなどを追加する場合は、現在の構造を以下のように使えます。

```text
Common
└─ 全ミニゲーム共通の状態・記録

Managers
└─ 全ミニゲーム共通の保存・音・入力・シーン・共有

GoldfishScoop
└─ 金魚すくい専用

ShootingGallery
└─ 射的専用

YoYoFishing
└─ ヨーヨー釣り専用
```

各ミニゲームは`gameId`を変えることで、`PlayerPrefs`の記録を分けられます。

例:

```text
GoldfishScoop_HighScore
ShootingGallery_HighScore
YoYoFishing_HighScore
```

## まとめ

現在のプロジェクトは、Android向け3Dミニゲーム「金魚すくい3D」の初期プロトタイプとして、以下の流れで動く構造です。

```text
InputManager
└─ PoiController
   └─ Goldfish
      └─ GoldfishGameManager
         ├─ ScoreManager
         ├─ TimerManager
         ├─ SaveManager
         ├─ UIManager
         └─ ShareManager
```

初学者が理解しやすいように、各Managerは単純な責務に分けられています。まずは`GoldfishScene`内で遊べる状態を作り、その後に`TitleScene`、`ResultScene`、音、見た目、Prefab、シーン遷移を順番に足していくのが安全です。
