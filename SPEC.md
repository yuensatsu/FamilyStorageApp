# SPEC.md

## 目的

この仕様書は、Android向け3Dミニゲームシリーズ「お祭りミニゲーム」の第1弾として作成する「金魚すくい3D」の仕様を定義します。

初学者でも理解・改修しやすい構成を優先し、複雑な設計よりも「Unity上で遊べる」「Google Playリリースへ進められる」ことを重視します。

## プロジェクト概要

| 項目 | 内容 |
| --- | --- |
| プロジェクト名 | FamilyStorageApp |
| ゲームシリーズ | お祭りミニゲーム |
| 第1弾タイトル | 金魚すくい3D |
| 対象プラットフォーム | Android |
| 開発エンジン | Unity |
| 言語 | C# |
| 視点 | 3D |
| 通信 | なし |
| 保存方式 | ローカル保存 |

## 最重要方針

- 初学者が理解しやすい構成にする。
- Inspectorで調整できる値は`[SerializeField]`にする。
- 1クラス1責務を意識する。
- 必要以上に抽象化しない。
- まずはCube、Sphere、PlaneなどUnity標準オブジェクトで遊べるプロトタイプを完成させる。
- Google Play公開に向けて、Android実機で動作確認できる状態を目指す。

## 実装しない機能

以下は本仕様の対象外です。

- オンラインランキング
- フレンド機能
- マルチプレイ
- アカウント機能
- サーバー通信
- 課金機能
- ガチャ
- クラウドセーブ
- 大規模イベント機能

## 常に実装したい機能

お祭りミニゲームシリーズ共通で、以下を実装対象とします。

- スコアや時間など、自己記録更新を目指せるゲーム性
- ローカル保存
- リザルト画面
- SNS共有機能
- リトライしやすい構成

## シーン構成

以下の3シーンを基本構成とします。

```text
Assets/Scenes
├─ TitleScene.unity
├─ GoldfishScene.unity
└─ ResultScene.unity
```

### TitleScene

タイトル画面です。

必須要素:

- ゲームタイトル表示
- スタートボタン
- ハイスコア表示
- 必要に応じて音量ON/OFFボタン

主な遷移:

```text
StartButton
└─ GoldfishSceneへ遷移
```

### GoldfishScene

金魚すくいを遊ぶメインシーンです。

必須要素:

- 水槽
- 金魚
- ポイ
- スコア表示
- 残り時間表示
- ポイ耐久値表示
- 一時停止、または終了時リザルト表示

主な終了条件:

- 残り時間が0になる
- ポイ耐久値が0になる

### ResultScene

ゲーム結果を表示するシーンです。

必須要素:

- 今回スコア
- すくった金魚の数
- ハイスコア
- ハイスコア更新表示
- リトライボタン
- タイトルへ戻るボタン
- SNS共有ボタン

初期プロトタイプでは、`GoldfishScene`内のリザルトPanelで代替してもよいです。ただし、最終的には3シーン構成に揃えることを目標とします。

## フォルダ構成

`Assets`配下は以下を基本構成とします。

```text
Assets
├─ Scripts
│  ├─ Managers
│  ├─ Common
│  ├─ GoldfishScoop
│  └─ UI
├─ Scenes
├─ Prefabs
├─ Materials
├─ Audio
└─ Models
```

## 名前空間・命名規則

### 名前空間

```text
FamilyStorageApp.Common
FamilyStorageApp.Managers
FamilyStorageApp.GoldfishScoop
```

### C#命名規則

- クラス名: `PascalCase`
- enum名: `PascalCase`
- publicメソッド名: `PascalCase`
- publicプロパティ名: `PascalCase`
- privateフィールド名: `camelCase`
- 定数名: `PascalCase`
- ファイル名とクラス名は一致させる

### コメント方針

- 初学者が理解しやすいように、主要なpublicメソッドにはXMLコメントを付ける。
- 複雑な処理には短い説明コメントを入れる。
- 処理の流れが分かるように、主要イベントでは`Debug.Log`を使用する。

## 共通クラス仕様

### GameState

ゲーム全体の状態を表すenumです。

値:

```text
Title
Ready
Playing
Result
Pause
```

### GameRecord

ゲームごとの自己記録を保持するデータクラスです。

フィールド:

```text
string gameId
int bestScore
float bestTime
int playCount
string lastPlayedAt
```

用途:

- `SaveManager`によるローカル保存
- タイトル画面やリザルト画面での記録表示

## Manager仕様

### GameManager

ゲーム全体の状態を管理します。

責務:

- `Title / Ready / Playing / Result / Pause`を扱う
- ゲーム開始
- ゲーム終了
- ポーズ
- ポーズ解除
- リトライ

### ScoreManager

現在スコアを管理します。

必須メソッド:

```text
AddScore(int point)
ResetScore()
GetScore()
```

仕様:

- 金魚をすくった時に点数を加算する。
- ゲーム開始時、またはリトライ時に0へ戻す。
- 0以下の点数は原則加算しない。

### TimerManager

制限時間を管理します。

仕様:

- 初期値は60秒。
- ゲーム開始でカウントダウンを始める。
- ゲーム終了で停止する。
- リトライ時に60秒へ戻す。
- 残り時間が0になったらゲーム終了イベントを呼ぶ。

必須メソッド:

```text
StartTimer()
StopTimer()
ResetTimer()
```

### SaveManager

`PlayerPrefs`でローカル保存します。

保存対象:

- ハイスコア
- 最速タイム
- プレイ回数
- 最終プレイ日時

仕様:

- キー名は定数化する。
- `gameId`ごとに保存キーを分ける。
- 今後ミニゲームが増えても記録が混ざらないようにする。

保存キー例:

```text
GoldfishScoop_HighScore
GoldfishScoop_BestTime
GoldfishScoop_PlayCount
GoldfishScoop_LastPlayedAt
```

### UIManager

UI表示更新を担当します。

表示対象:

- スコア
- 残り時間
- ポイ耐久値
- リザルト
- ハイスコア
- リトライボタン
- SNS共有ボタン

仕様:

- 表示用TextやButtonはInspectorで設定できる。
- 初期プロトタイプでは`UnityEngine.UI`を使用してよい。
- 将来的にTextMeshProへ移行してもよい。

### SoundManager

BGMとSEを管理します。

仕様:

- BGM再生
- BGM停止
- SE再生
- 音量ON/OFF

初期プロトタイプでは最低限の実装でよいです。

### ShareManager

SNS共有を担当します。

仕様:

- Android実機では共有Intentを使う。
- Unity Editor上では`Debug.Log`で共有文を表示する。
- 共有文は日本語とハッシュタグを含む。

共有文例:

```text
金魚すくい3Dで18匹すくいました！
スコア：180点
#お祭りミニゲーム #金魚すくい
```

### SceneLoadManager

シーン遷移を担当します。

必須遷移:

- タイトルへ戻る
- 金魚すくいシーンへ移動
- リザルトシーンへ移動
- リトライ

シーン名:

```text
TitleScene
GoldfishScene
ResultScene
```

### InputManager

マウス入力とタッチ入力を共通化します。

仕様:

- Android実機ではタッチ入力を扱う。
- Unity Editorではマウス入力を扱う。
- ドラッグ位置を取得できる。
- タップした瞬間を取得できる。
- 離した瞬間を取得できる。

## 金魚すくい3D仕様

### ゲームルール

- 制限時間は60秒。
- プレイヤーはポイを操作する。
- 金魚をすくうとスコアが加算される。
- 金魚ごとに点数が違う。
- ポイには耐久値がある。
- 金魚をすくうたびにポイ耐久値が減る。
- 制限時間が0になるとゲーム終了。
- ポイ耐久値が0になるとゲーム終了。
- 終了後にリザルト画面を表示する。
- ハイスコアを保存する。
- SNSへ結果を共有できる。

### 初期パラメータ

| 項目 | 初期値 | 備考 |
| --- | --- | --- |
| 制限時間 | 60秒 | `TimerManager`で管理 |
| ポイ耐久値 | 10 | `GoldfishGameManager`で管理 |
| 通常金魚の点数 | 10点 | `Goldfish`のInspectorで調整 |
| 同時表示金魚数 | 10匹 | `GoldfishSpawner`で調整 |
| すくう範囲 | 0.7 | `PoiController.scoopRadius`で調整 |

数値は初期値であり、Unity Inspectorで調整可能にします。

### 操作仕様

Androidスマホ前提です。

- 画面ドラッグでポイを移動する。
- タップ、または指を離したタイミングで「すくう」判定を行う。
- まずはシンプルな操作でよい。
- Unity Editor上ではマウス操作でも動くようにする。

詳細:

```text
Pointer Down
└─ 設定によりすくう判定

Pointer Drag
└─ ポイ移動

Pointer Up
└─ 設定によりすくう判定
```

初期設定では、離したタイミングで金魚をすくう仕様を推奨します。

## GoldfishScoopクラス仕様

### GoldfishGameManager

金魚すくい専用のゲーム進行を管理します。

責務:

- ゲーム開始
- ゲーム終了
- 金魚をすくった時の処理
- ポイ耐久値の管理
- スコア管理との連携
- タイマー管理との連携
- 保存管理との連携
- UI管理との連携
- 共有管理との連携

連携対象:

```text
ScoreManager
TimerManager
SaveManager
UIManager
ShareManager
GameManager
```

### Goldfish

金魚1匹の挙動を管理します。

責務:

- 水槽内をランダムに泳ぐ
- 点数を持つ
- すくわれたら消える
- すくわれた時に`GoldfishGameManager`へ通知する

### GoldfishSpawner

金魚生成を管理します。

責務:

- 金魚を一定数生成する
- 金魚が減ったら追加生成する
- Prefabから生成する
- 生成範囲をInspectorで設定できるようにする

### PoiController

ポイ操作とすくう判定を管理します。

責務:

- 入力に合わせてポイを移動する
- 金魚との当たり判定を行う
- すくう処理を行う
- 必要に応じてポイ耐久値を減らす

## UI仕様

### ゲーム中HUD

表示項目:

- 現在スコア
- 残り時間
- ポイ耐久値

### リザルト画面

表示項目:

- 今回のスコア
- すくった金魚の数
- ハイスコア
- ハイスコア更新表示

操作:

- リトライ
- タイトルへ戻る
- SNS共有

## 保存仕様

保存方式:

```text
PlayerPrefs
```

保存タイミング:

- ゲーム終了時
- ハイスコア更新時
- プレイ回数更新時

読み込みタイミング:

- タイトル画面表示時
- ゲーム準備時
- リザルト表示時

## サウンド仕様

初期プロトタイプでは任意ですが、以下に対応できる設計にします。

- タイトルBGM
- ゲーム中BGM
- 金魚をすくった時のSE
- ポイが破れた時のSE
- リザルト表示SE
- 音量ON/OFF

## Android仕様

### 入力

- タッチ操作を正式対応とする。
- マルチタッチは初期プロトタイプでは不要。
- 1本指操作を基本とする。

### 共有

- Androidの共有Intentを使う。
- アプリ独自のSNSログインやAPI連携は行わない。

### Google Playリリースに向けた確認項目

- Android実機で起動できる
- タッチ操作で遊べる
- 60秒ゲームが最後まで進む
- リザルトが表示される
- ハイスコアが保存される
- アプリ再起動後も記録が残る
- 共有Intentが開く
- 強制終了しない
- 画面比率違いでUIが大きく崩れない

## テスト仕様

### EditModeテスト候補

- `ScoreManager`
  - スコア加算
  - スコアリセット
  - 0以下の点数を加算しない
- `TimerManager`
  - タイマーリセット
  - 制限時間設定
- `SaveManager`
  - ハイスコア更新条件
  - プレイ回数更新
- `ShareManager`
  - 共有文生成

### PlayModeテスト候補

- ゲーム開始で`Playing`になる
- 金魚をすくうとスコアが増える
- 金魚をすくうとポイ耐久値が減る
- タイムアップで`Result`になる
- ポイ耐久値0で`Result`になる
- リトライでスコア・時間・耐久値が初期化される

## 受け入れ条件

### 最小プロトタイプ完了条件

- Unity Editorで`GoldfishScene`を再生できる。
- マウスでポイを動かせる。
- 金魚が一定数生成される。
- 金魚がランダムに泳ぐ。
- ポイで金魚をすくえる。
- スコアが加算される。
- ポイ耐久値が減る。
- 60秒経過、またはポイ耐久値0でゲームが終了する。
- リザルトが表示される。
- リトライできる。

### Android確認完了条件

- Android実機で起動できる。
- タッチ操作でポイを動かせる。
- 共有ボタンでAndroid共有シートが開く。
- ハイスコアが保存される。
- アプリを再起動してもハイスコアが残る。

### Google Playリリース前完了条件

- `TitleScene / GoldfishScene / ResultScene`が存在する。
- Build Settingsに必要シーンが登録されている。
- Android App Bundleを作成できる。
- 必要なアイコン、アプリ名、バージョンが設定されている。
- プライバシーポリシーやストア掲載情報の準備方針が決まっている。

## 今後の拡張方針

第2弾以降のミニゲームを追加する場合、以下の構造を維持します。

```text
Assets/Scripts
├─ Common
├─ Managers
├─ GoldfishScoop
├─ ShootingGallery
└─ YoYoFishing
```

共通機能は`Managers`に置き、各ミニゲーム専用のルールやPrefab制御は専用フォルダに分けます。

`SaveManager`は`gameId`ごとに記録を分けます。

例:

```text
GoldfishScoop_HighScore
ShootingGallery_HighScore
YoYoFishing_HighScore
```

## 関連ドキュメント

- `ARCH.md`: 現在の実装構造、主要クラス、依存関係、データフロー
- `TODO.md`: 本仕様と実装状況の差分、優先順位付きタスク
