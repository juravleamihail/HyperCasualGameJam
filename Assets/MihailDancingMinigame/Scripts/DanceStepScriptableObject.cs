using UnityEngine;

[CreateAssetMenu(fileName = "DanceStep", menuName = "DanceStep", order = 1)]
public class DanceStepScriptableObject : ScriptableObject
{
    [SerializeField] public Sprite icon;
    [SerializeField] private Animation animation;
}
