using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MapItemData
{
    public string PanoramaPath;
    public Vector3 MapItemPosition;
    public int Level;
}


[Serializable]
public struct PanoramsData
{
    public List<PanoramaData> Data;
    public List<MapItemData> MapData;
    public string MapLayoutImage;

    public PanoramsData(List<PanoramaData> data)
    {
        Data = data;
        MapData = null;
        MapLayoutImage = null;
    }

    public PanoramsData(List<PanoramaData> data, List<MapItemData> mapData)
    {
        Data = data;
        MapData = mapData;
        MapLayoutImage = null;
    }

    public PanoramsData(List<PanoramaData> data, List<MapItemData> mapData, string mapLayoutImage)
    {
        Data = data;
        MapData = mapData;
        MapLayoutImage = mapLayoutImage;
    }
}

[Serializable]
public struct PanoramaData
{
    public string ImageLink;
    public List<ArrowInfo> ArrowsInfo;
    public List<StairsInfo> StairsInfo;
}

[Serializable]
public struct Bindings
{
    public List<TrenObjectInfo> Data;

    public Bindings(Dictionary<string, Panorama> panorams)
    {
        Data = new List<TrenObjectInfo>();
        foreach (var panorama in panorams.Values)
        {
            if (panorama.TrenObjectsInfo != null)
            {
                for (int i = 0; i < panorama.TrenObjectsInfo.Count; i++)
                {
                    Data.Add(panorama.TrenObjectsInfo[i]);
                }
            }
        }
    }
}

[Serializable]
public struct LevelInfo
{
    public int Index;
    public List<string> PanoramaFilenames;
}

[Serializable]
public class Panorama
{
    public string ImageLink;
    public List<ArrowInfo> ArrowsInfo;
    public List<TrenObjectInfo> TrenObjectsInfo;

    public PanoramaData PanoramaData
    {
        get
        {
            var data = new PanoramaData();
            data.ImageLink = ImageLink;
            var arrowsInfo = new List<ArrowInfo>();
            var stairsInfo = new List<StairsInfo>();

            foreach(var info in ArrowsInfo)
            {
                if(info.ArrowType == (int)ArrowType.STAIRS)
                {
                    stairsInfo.Add((StairsInfo)info);
                }
                else
                {
                    arrowsInfo.Add(info);
                }
            }

            data.ArrowsInfo = arrowsInfo;
            data.StairsInfo = stairsInfo;

            return data;
        }
    }

    public Panorama(PanoramaData data)
    {
        ImageLink = data.ImageLink;

        var arrowsInfo = new List<ArrowInfo>();
        foreach (var info in data.ArrowsInfo)
            arrowsInfo.Add(info);
        foreach (var info in data.StairsInfo)
            arrowsInfo.Add(info);
        ArrowsInfo = arrowsInfo;
    }

    public Panorama(string link)
    {
        ImageLink = link;
        ArrowsInfo = new List<ArrowInfo>();
    }

    public Panorama(string link, Arrow arrow)
    {
        ImageLink = link;
        ArrowsInfo = new List<ArrowInfo>();

        var arrowInfo = new ArrowInfo();
        arrowInfo.Position = arrow.LocalPosition;
        arrowInfo.Rotation = arrow.Rotation;
        arrowInfo.ArrowType = (int)arrow.ArrowType;
        arrowInfo.NextPanoramaFilename = arrow.NextPanorama.ImageLink;

        AddArrow(arrowInfo);
    }

    public void AddArrow(ArrowInfo info)
    {
        if (ArrowsInfo != null)
        {
            ArrowsInfo.Add(info);
        }
        else
        {
            Debug.LogError("Arrows data initialization error.");
        }
    }
    public void AddTrenObject(TrenObjectInfo info)
    {
        if (TrenObjectsInfo == null)
        {
            TrenObjectsInfo = new List<TrenObjectInfo>();
        }

        TrenObjectsInfo.Add(info);
    }
}

[Serializable]
public class ArrowInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 NextPanoramaEulers;

    public int ArrowType;
    public string NextPanoramaFilename;
}

[Serializable]
public class StairsInfo : ArrowInfo
{
    public bool StairsIsUp;

    public ArrowInfo ArrowInfo
    {
        set
        {
            Position             = value.Position;
            Rotation             = value.Rotation;
            NextPanoramaEulers   = value.NextPanoramaEulers;
            ArrowType            = value.ArrowType;
            NextPanoramaFilename = value.NextPanoramaFilename;
        }
    }
}

[Serializable]
public class TrenObjectInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
    public string TrenObjectName;
    public string TemplateName;
    public string PanoramName;
}

[Serializable]
public struct TemplateType
{
    public string Name;
    public GameObject TemplateObject;
}