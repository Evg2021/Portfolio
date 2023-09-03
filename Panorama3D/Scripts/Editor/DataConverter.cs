using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class DataConverter : Editor
{
    [MenuItem("Utils/ConvertFromJSONToXML")]
    public static void ConvertFromJSONToXML()
    {
        if (Utilities.ReadData(out var panorams, out _, out _, "json"))
        {
            var allArrows = new List<ArrowInfo>();
            foreach (var panorama in panorams)
            {
                foreach (var arrow in panorama.Value.ArrowsInfo)
                {
                    allArrows.Add(arrow);
                }
            }

            Panorama firstPanorama = null;

            foreach (var panorama in panorams)
            {
                if (firstPanorama == null)
                {
                    if (Utilities.GetNameFromPath(panorama.Key).Split('.')[0] == "1")
                    {
                        firstPanorama = panorama.Value;
                        CreateXML(firstPanorama, "panorama0", allArrows);
                    }
                }

                CreateXML(panorama.Value, null, allArrows);
            }

            Debug.Log("Готово!");
        }
    }

    private static void AddElement(ref XmlDocument doc, ref XmlElement parent, Element data)
    {
        var newElement = doc.CreateElement("element");
        newElement.SetAttribute(nameof(data.strelka), data.strelka.ToString());
        newElement.SetAttribute(nameof(data.angle), data.angle.ToString());
        newElement.SetAttribute(nameof(data.link), data.link.ToString()); 
        newElement.SetAttribute(nameof(data.ladder), data.ladder.ToString());  
        newElement.SetAttribute(nameof(data.door), data.door.ToString());
        newElement.SetAttribute(nameof(data.arrowUp), data.arrowUp.ToString());
        newElement.SetAttribute(nameof(data.arrowDown), data.arrowDown.ToString());
        parent.AppendChild(newElement);
    }
    private static void AddElement(ref XmlDocument doc, ref XmlElement parent, Header data)
    {
        var newElement = doc.CreateElement("element");
        newElement.SetAttribute(nameof(data.rotatesphere), data.rotatesphere.ToString());
        newElement.SetAttribute(nameof(data.angle), data.angle.ToString());
        parent.AppendChild(newElement);
    }

    private static Header GetHeader(int panoramaAngle = 0)
    {
        return new Header()
        {
            rotatesphere = 1,
            angle = -90 + panoramaAngle
        };
    }
    private static Element GetElement(ArrowInfo data, int offset = 0)
    {
        return new Element()
        {
            strelka = 1,
            angle = (int)data.Rotation.eulerAngles.y + offset,
            link = Utilities.GetNameFromPath(data.NextPanoramaFilename),
            ladder = 0,
            door = 0,
            arrowUp = 0,
            arrowDown = 0
        };
    }

    private static void CreateXML(Panorama panorama, string panoramaName = null, List<ArrowInfo> arrows = null)
    {
        XmlSerializer ser = new XmlSerializer(typeof(XmlElement));
        XmlDocument doc = new XmlDocument();
        XmlElement parent = doc.CreateElement("elements");

        ArrowInfo arrowToCurrentPanorama = null;
        if (arrows != null)
        {
            var arrowsToCurrentPanorama = arrows.Where(h => h.NextPanoramaFilename == panorama.ImageLink).ToArray();
            for (int i = 0; i < arrowsToCurrentPanorama.Length; i++)
            {
                if (arrowToCurrentPanorama != null)
                {
                    if (Mathf.Abs(arrowToCurrentPanorama.Rotation.eulerAngles.y) > Mathf.Abs(arrowsToCurrentPanorama[i].Rotation.eulerAngles.y))
                    {
                        arrowToCurrentPanorama = arrowsToCurrentPanorama[i];
                    }
                }
                else
                {
                    arrowToCurrentPanorama = arrowsToCurrentPanorama[i];
                }
            }
        }
        //var arrowsToCurrentPanorama = panorama.ArrowsInfo[0];
        var angle = arrowToCurrentPanorama != null ? (int)arrowToCurrentPanorama.Rotation.eulerAngles.y : 0;
        ClampAngle(ref angle);

        AddElement(ref doc, ref parent, GetHeader(-angle));

        foreach (var arrow in panorama.ArrowsInfo)
        {
            AddElement(ref doc, ref parent, GetElement(arrow, -angle));
        }

        if (string.IsNullOrEmpty(panoramaName))
        {
            panoramaName = Utilities.GetNameFromPath(panorama.ImageLink);
            if (!string.IsNullOrEmpty(panoramaName))
            {
                panoramaName = panoramaName.Split('.')[0];
            }
        }

        string path = Directory.GetCurrentDirectory() + $"\\{panoramaName}.xml";
        var settings = new XmlWriterSettings()
        {
            Encoding = Encoding.GetEncoding("Windows-1251"),
            NewLineHandling = NewLineHandling.None
        };
        using (XmlWriter writer = XmlWriter.Create(path, settings))
        {
            ser.Serialize(writer, parent);
        }
    }

    private static void ClampAngle(ref int angle)
    {
        angle %= 360;
        if (angle < 0)
        {
            angle = 360 + angle;
        }
    }
}

public struct Header
{
    public int rotatesphere;
    public int angle;
}

public struct Element
{
    public int strelka;
    public int angle;
    public string link;
    public int ladder;
    public int door;
    public int arrowUp;
    public int arrowDown;
}