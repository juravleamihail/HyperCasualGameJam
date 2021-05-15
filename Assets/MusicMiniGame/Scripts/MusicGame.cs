using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGame : MonoBehaviour
{
    [SerializeField] float _minPercentage;
    [SerializeField] float _maxPercentage;

    Tuple<float, float, float> _laneSizePercentage;
    Tuple<float, float, float> _laneSize;
    Tuple<Rect, Rect, Rect> _musicBoxRect;
    [SerializeField] MusicSheet _musicSheet;

    private void Awake()
    {
        _laneSizePercentage = GenerateLaneSize();
        _laneSize = ConvertPercentageToValue(_laneSizePercentage);
        _musicBoxRect = GenerateMusicBoxRect(_musicSheet.Lines, _laneSize);
        _musicSheet.MusicBoxes = _musicSheet.CreateMusicBoxes(_musicBoxRect);
        Debug.Log("LaneSize:" + _laneSizePercentage.ToString());
    }

    Tuple<float,float,float> GenerateLaneSize()
    {
        Tuple<float, float, float> laneSize = default;

        float lane1 = UnityEngine.Random.Range(_minPercentage, _maxPercentage);
        float lane2 = UnityEngine.Random.Range(_minPercentage, _maxPercentage);
        float lane3 = 100 - (lane1 + lane2);

        laneSize = new Tuple<float, float, float>(lane1, lane2, lane3);

        return laneSize;
    }

    Tuple<Rect, Rect, Rect> GenerateMusicBoxRect(RectTransform [] lines,Tuple<float,float,float> laneWidth)
    {

        Tuple<Rect, Rect, Rect> laneSize = default;
        bool[] occupiedPositions = new bool[(lines.Length - 2) * 2];

        int lineNr = GeneratePosition(occupiedPositions);
        FillNearPos(occupiedPositions, lineNr);
        Debug.LogError(lineNr+" "+lineNr%2+" "+lineNr/2+" " +lines[lineNr/2].position.y+" "+ lines[lineNr / 2].anchoredPosition+" "+ lines[lineNr / 2].anchoredPosition3D+" "+ lines[lineNr / 2].rect.position+" "+ lines[lineNr / 2].rect.center+" "+lines[lineNr/2].transform.position+" "+ lines[lineNr / 2]);
        float position = (lineNr % 2 == 0) ? lines[lineNr/2].transform.position.y : (lines[Mathf.Clamp(lineNr / 2 - 1, 0, lines.Length)].transform.position.y + lines[Mathf.Clamp(lineNr / 2 - 1, 0, lines.Length)].transform.position.y)/2;
        Rect rect1 = new Rect(new Vector2(laneWidth.Item1/2, position),new Vector2(laneWidth.Item1,60));


        lineNr = GeneratePosition(occupiedPositions);
        FillNearPos(occupiedPositions, lineNr);
        Debug.LogError(lineNr + " " + lineNr % 2 + " " + lineNr / 2 + " " + lines[lineNr / 2].position.y + " " + lines[lineNr / 2].anchoredPosition + " " + lines[lineNr / 2].anchoredPosition3D + " " + lines[lineNr / 2].rect.position + " " + lines[lineNr / 2].rect.center + " " + lines[lineNr / 2].transform.position);
        position = (lineNr % 2 == 0) ? lines[lineNr / 2].transform.position.y : (lines[Mathf.Clamp(lineNr / 2 - 1, 0, lines.Length)].transform.position.y + lines[Mathf.Clamp(lineNr / 2 - 1, 0, lines.Length)].transform.position.y) / 2;
        Rect rect2 = new Rect(new Vector2(laneWidth.Item1 + laneWidth.Item2 / 2, position), new Vector2(laneWidth.Item2, 60));

        lineNr = GeneratePosition(occupiedPositions);
        FillNearPos(occupiedPositions, lineNr);
        Debug.LogError(lineNr + " " + lineNr % 2 + " " + lineNr / 2 + " " + lines[lineNr / 2].position.y + " " + lines[lineNr / 2].anchoredPosition + " " + lines[lineNr / 2].anchoredPosition3D + " " + lines[lineNr / 2].rect.position + " " + lines[lineNr / 2].rect.center + " " + lines[lineNr / 2].transform.position);
        position = (lineNr % 2 == 0) ? lines[lineNr / 2].transform.position.y : (lines[Mathf.Clamp(lineNr / 2 - 1,0,lines.Length)].transform.position.y + lines[Mathf.Clamp(lineNr / 2 - 1, 0, lines.Length)].transform.position.y ) / 2;
        Rect rect3 = new Rect(new Vector2(laneWidth.Item1 + laneWidth.Item2 + laneWidth.Item3 / 2, position), new Vector2(laneWidth.Item3, 60));

        laneSize = new Tuple<Rect, Rect, Rect>(rect1, rect2, rect3);

        return laneSize;
    }


    void FillNearPos(bool[] occupiedPositions,int pos)
    {
        int pos1 = Mathf.Clamp(pos - 1, 0, occupiedPositions.Length - 1);
        int pos2 = Mathf.Clamp(pos - 2, 0, occupiedPositions.Length - 1);

        occupiedPositions[pos] = true;
        occupiedPositions[pos1] = true;
        occupiedPositions[pos2] = true;

        pos1 = Mathf.Clamp(pos + 1, 0, occupiedPositions.Length - 1);
        pos2 = Mathf.Clamp(pos + 2, 0, occupiedPositions.Length - 1);

        occupiedPositions[pos1] = true;
        occupiedPositions[pos2] = true;
    }
    int GeneratePosition(bool[] occupiedPosition)
    {
        bool found = false;
        int pos = 0;
        do
        {
            pos = UnityEngine.Random.Range(0, occupiedPosition.Length-1);
            if (!occupiedPosition[pos])
            {
                found = true;
            }
        }
        while (!found);

        return pos;
    }

    float PercentageToWidth(float percentage)
    {
        return (Screen.width * percentage)/100;
    }

    Tuple<float,float,float> ConvertPercentageToValue(Tuple<float, float, float> laneSizePercentage)
    {
        Tuple<float, float, float> laneSize = new Tuple<float, float, float>(PercentageToWidth(laneSizePercentage.Item1), PercentageToWidth(laneSizePercentage.Item2), PercentageToWidth(laneSizePercentage.Item3));

        return laneSize;
    }
}
