using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    [SerializeField] private RectTransform _outerBox;
    [SerializeField] private RectTransform _innerBox;


    public void Setup(Rect rect)
    {
        RectTransform mainRect = GetComponent<RectTransform>();
        mainRect.sizeDelta = new Vector2(rect.width, rect.height);
        mainRect.position = rect.position;

        _outerBox.sizeDelta = new Vector2(rect.width,rect.height);
        _innerBox.sizeDelta = new Vector2(rect.width, rect.height/3);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
