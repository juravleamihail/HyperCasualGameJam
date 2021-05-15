using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSheet : MonoBehaviour
{
    [SerializeField] private RectTransform[] _linesContainer;
    [SerializeField] private RectTransform[] _lines;
    [SerializeField] private float _verticalSpace;
    [SerializeField] private float _verticalSize;

    [SerializeField] MusicBox _boxPrefab;
    private MusicBox[] _musicBoxes;
    public MusicBox[] MusicBoxes { get { return _musicBoxes; } set { _musicBoxes = value; } }
    public RectTransform [] LinesContainer { get { return _linesContainer; } }

    public RectTransform[] Lines { get { return _lines; } }
    // Start is called before the first frame update

    public MusicBox[] CreateMusicBoxes(Tuple<Rect,Rect,Rect> rects)
    {
        MusicBox[] musicBoxes = new MusicBox[3];
        musicBoxes[0] = CreateMusicBox(rects.Item1);
        musicBoxes[1] = CreateMusicBox(rects.Item2);
        musicBoxes[2] = CreateMusicBox(rects.Item3);

        return musicBoxes;
    }

    MusicBox CreateMusicBox(Rect rect)
    {
        MusicBox musicBox = Instantiate<MusicBox>(_boxPrefab);
        musicBox.transform.SetParent(transform);
        musicBox.transform.localScale = Vector3.one;
        musicBox.Setup(rect);

        return musicBox;
    }
}
