using UnityEngine.Events;

public static class UIEventManager
{
    public static UnityAction ShowWinUI;

    public static UnityAction<string> ShowDefeatUI;

    public static UnityAction<float> UpdateManaBarUI;

    public static UnityAction<string> GotCollectableToUI;
}
