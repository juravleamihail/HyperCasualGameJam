using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private Text textComp;

    public void Init(string newText)
    {
        textComp.text = newText;
        gameObject.AddComponent<FollowTarget>().SelectTarget(GameManager.Instance.popupEndPoint, 25);
    }
}
