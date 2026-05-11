/// <summary>
/// ゲーム全体の状態を表す列挙型です。
/// GameManagerや各ゲーム専用Managerが、この状態を見て処理を切り替えます。
/// </summary>
public enum GameState
{
    Title,
    Ready,
    Playing,
    Result,
    Pause
}
