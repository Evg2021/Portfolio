using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Utilities
{
    public const string bindingsFilename = "Bindings";

    private static string lastPath = "";
    public static char separator = Path.DirectorySeparatorChar;

    public static Cubemap CubemapFromTexture2D(Texture2D texture)
    {
        int cubedim = texture.width / 6;
        Cubemap cube = new Cubemap(cubedim, TextureFormat.ARGB32, false);
        cube.SetPixels(texture.GetPixels(2 * cubedim, 2 * cubedim, cubedim, cubedim), CubemapFace.NegativeY);
        cube.SetPixels(texture.GetPixels(3 * cubedim, cubedim, cubedim, cubedim), CubemapFace.PositiveX);
        cube.SetPixels(texture.GetPixels(2 * cubedim, cubedim, cubedim, cubedim), CubemapFace.PositiveZ);
        cube.SetPixels(texture.GetPixels(cubedim, cubedim, cubedim, cubedim), CubemapFace.NegativeX);
        cube.SetPixels(texture.GetPixels(0, cubedim, cubedim, cubedim), CubemapFace.NegativeZ);
        cube.SetPixels(texture.GetPixels(2 * cubedim, 0, cubedim, cubedim), CubemapFace.PositiveY);
        cube.Apply();

        return cube;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;

        if (angle > 180.0f)
            return angle - 360;

        return angle;
    }

    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;
        return 360 - angle;
    }

    public static string OpenFilePanel(string expansion)
    {
        var currentPath = lastPath.Length > 0 ? lastPath : Directory.GetCurrentDirectory();

        var path = StandaloneFileBrowser.OpenFilePanel("Open file", currentPath, expansion, false);

        if (path != null && path.Length > 0)
        {
            var relativePath = GetRelativePath(path[0]);
            if (expansion == "jpg")
            {
                lastPath = relativePath.TrimEnd(separator).Remove(relativePath.LastIndexOf(separator) + 1);
            }
            return relativePath;
        }
        else
            Debug.LogError("Path to file is empty.");

        return null;
    }

    public static string OpenBrowserPanel()
    {
        var path = StandaloneFileBrowser.OpenFolderPanel("Choose folder to save", Directory.GetCurrentDirectory(), false);

        if (path != null && path.Length > 0)
            return path[0];
        else
            Debug.LogError("Path to file is empty.");

        return null;
    }

    public static string OpenSaveFilePanel(string expansion)
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save panorams file", Directory.GetCurrentDirectory(), "PanoramsData.json", expansion);

        if (path != null && path.Length > 0)
            return path;
        else
            Debug.LogError("Path to save file is empty.");

        return null;
    }

    public static bool FileExist(string path)
    {
        return File.Exists(path);
    }

    public static string GetRelativePath(string fullPath)
    {
        var relativePath = "";
        var currentPath = Directory.GetCurrentDirectory();
        var splittedCurrentPath = currentPath.Split(separator);

        if (fullPath.Contains(splittedCurrentPath[splittedCurrentPath.Length - 1]))
        {
            return fullPath.Replace(currentPath, ".");
        }

        if (relativePath.Length == 0)
        {
            for (int i = splittedCurrentPath.Length - 2; i >= 0; i--)
            {
                relativePath += ".." + separator;
                if (fullPath.Contains(splittedCurrentPath[i]))
                {
                    var index = fullPath.LastIndexOf(splittedCurrentPath[i]);
                    relativePath += fullPath.Remove(0, index)
                                            .TrimStart(splittedCurrentPath[i]
                                            .ToCharArray())
                                            .Replace(separator + splittedCurrentPath[i] + separator, "");
                    break;
                }
            }
        }

        return relativePath;
    }

    public static string ConvertPathToLink(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            return "localhost:8000" + path.Remove(0, 1).Replace(separator, '/');
        }

        return null;
    }

    public static void SavePanoramaDataInAsset(ref ResourcesPanorams data, Dictionary<string, Panorama> panorams, List<MapItemData> mapData = null, string mapLayoutImage = null, bool isWeb = false)
    {
        if(data != null)
        {
            if (isWeb)
            {
                var assetData = new List<PanoramaData>();
                foreach (var panorama in panorams)
                {
                    var oldData = panorama.Value.PanoramaData;
                    var oldArrows = oldData.ArrowsInfo;
                    var oldStairs = oldData.StairsInfo;

                    var newData = new PanoramaData();
                    var newArrows = new List<ArrowInfo>();
                    var newStairs = new List<StairsInfo>();

                    foreach (var oldArrow in oldArrows)
                    {
                        var newArrow = oldArrow;
                        newArrow.NextPanoramaFilename = ConvertPathToLink(oldArrow.NextPanoramaFilename);
                        newArrows.Add(newArrow);
                    }

                    foreach (var oldStair in oldStairs)
                    {
                        var newStair = oldStair;
                        newStair.NextPanoramaFilename = ConvertPathToLink(oldStair.NextPanoramaFilename);
                        newStairs.Add(newStair);
                    }

                    newData.ImageLink = ConvertPathToLink(oldData.ImageLink);
                    newData.ArrowsInfo = newArrows;
                    newData.StairsInfo = newStairs;
                    assetData.Add(newData);
                }

                var newMapData = new List<MapItemData>();
                if (mapData != null)
                {
                    foreach (var oldMapItem in mapData)
                    {
                        var newMapItem = oldMapItem;
                        newMapItem.PanoramaPath = ConvertPathToLink(oldMapItem.PanoramaPath);
                        newMapData.Add(newMapItem);
                    }
                }

                var newMapLayoutImage = ConvertPathToLink(mapLayoutImage);

                data.Data = new PanoramsData(assetData, newMapData, newMapLayoutImage);
            }
            else
            {
                var assetData = new List<PanoramaData>();
                foreach (var panorama in panorams)
                {
                    assetData.Add(panorama.Value.PanoramaData);
                }

                data.Data = new PanoramsData(assetData, mapData, mapLayoutImage);
            }
        }
    }

    public static string GetPanoramaDataJson(Dictionary<string, Panorama> panorams, List<MapItemData> mapData = null, string mapLayoutImage = null)
    {
        var data = new List<PanoramaData>();
        foreach (var panorama in panorams)
        {
            if (panorama.Value.ArrowsInfo != null && panorama.Value.ArrowsInfo.Count > 0)
            {
                data.Add(panorama.Value.PanoramaData);
            }
        }

        var panoramsData = new PanoramsData(data, mapData, mapLayoutImage);

        return JsonUtility.ToJson(panoramsData);
    }

    public static bool SavePanoramsData(string data, string expansion = "json")
    {
        var path = OpenSaveFilePanel(expansion);
        if (path != null)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(data);
            }

            return true;
        }
        else
        {
            Debug.LogError("Folder to save was not chosen.");
        }

        return false;
    }

    public static void SaveBindingsData(Bindings bindigs)
    {
        var jsonData = JsonUtility.ToJson(bindigs, true);

        CheckStreamingAssetsPath();
        string path = Application.streamingAssetsPath + '\\' + bindingsFilename + ".json";
        using (StreamWriter stream = new StreamWriter(path))
        {
            stream.Write(jsonData);
        }
    }
    public static bool ReadBindingsData(out Bindings data)
    {
        data = default(Bindings);
        if (CheckStreamingAssetsPath())
        {
            var path = Application.streamingAssetsPath + '\\' + bindingsFilename + ".json";
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        data = JsonUtility.FromJson<Bindings>(reader.ReadToEnd());
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Bindings reading error: " + e.Message);
                    return false;
                }
            }
        }
        
        return false;
    }
    public static void AddBindingsDataToPanorams(Bindings data, ref Dictionary<string, Panorama> panorams)
    {
        var registratedObjects = TrenObjectsManager.RegistratedTrenObjects;
        if (registratedObjects != null)
        {
            var listTrenObjects = data.Data;
            foreach (var trenObject in listTrenObjects)
            {
                if (registratedObjects.Any(h => h.Value.TrenObjectName == trenObject.TrenObjectName))
                {
                    if (panorams.TryGetValue(trenObject.PanoramName, out var panorama))
                    {
                        panorama.AddTrenObject(trenObject);
                    }
                }
            }
        }
    }
    public static string GetBindingsJson(Dictionary<string, Panorama> panorams)
    {        
        return GetBindingsJson(new Bindings(panorams));
    }
    public static string GetBindingsJson(Bindings data)
    {
        return JsonUtility.ToJson(data);
    }

    public static void MergePanorama(Panorama firstPanorama, Panorama secondPanorama)
    {
        if(firstPanorama.ArrowsInfo == null)
        {
            firstPanorama.ArrowsInfo = new List<ArrowInfo>();
        }

        if(secondPanorama.ArrowsInfo != null)
        {
            foreach(var arrow in secondPanorama.ArrowsInfo)
            {
                firstPanorama.AddArrow(arrow);
            }
        }
    }

    public static void ParseDataFromJson(string data, out Dictionary<string, Panorama> panorams, out List<MapItemData> mapData, out string mapLayoutName)
    {
        panorams = new Dictionary<string, Panorama>();
        mapData = new List<MapItemData>();
        mapLayoutName = null;

        if (data != null && data.Length > 0)
        {
            PanoramsData panoramsData = JsonUtility.FromJson<PanoramsData>(data);

            foreach (var panoramaData in panoramsData.Data)
            {
                var panorama = new Panorama(panoramaData);

                if (panorama.ImageLink == null && panorama.ImageLink.Length == 0) continue;

                var keyName = GetNameFromPath(panorama.ImageLink);

                if (panorams.TryGetValue(keyName, out var existedPanorama))
                {
                    MergePanorama(existedPanorama, panorama);
                }
                else
                {
                    panorams.Add(keyName, panorama);
                }
            }

            mapData = panoramsData.MapData;
            mapLayoutName = panoramsData.MapLayoutImage;
        }
    }

    public static void ParseDataFromAsset(PanoramsData data, out Dictionary<string, Panorama> panorams, out List<MapItemData> mapData, out string mapLayoutName)
    {
        panorams = new Dictionary<string, Panorama>();
        mapData = new List<MapItemData>();
        mapLayoutName = null;

        List<PanoramaData> panoramsData = data.Data;

        if (panoramsData != null)
        {
            foreach (var panoramaData in panoramsData)
            {
                var panorama = new Panorama(panoramaData);

                if (panorama.ImageLink == null && panorama.ImageLink.Length == 0) continue;

                var keyName = GetNameFromPath(panorama.ImageLink);

                panorams.Add(keyName, panorama);
            }

            mapData = data.MapData;
            mapLayoutName = data.MapLayoutImage;
        }

    }

    public static bool ReadData(out Dictionary<string, Panorama> panorams, out List<MapItemData> mapData, out string mapLayoutName, string expansion)
    {
        var path = OpenFilePanel(expansion);

        string data = "";
        panorams = new Dictionary<string, Panorama>();
        mapData = new List<MapItemData>();
        mapLayoutName = null;

        if (path != null && path.Length > 0)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                data = reader.ReadToEnd();
            }

            ParseDataFromJson(data, out panorams, out mapData, out mapLayoutName);

            if(panorams != null && panorams.Count > 0)
            { 
                if (ReadBindingsData(out var bindings))
                {
                    TrenObjectsManager.RegistrateObjects(bindings);
                    AddBindingsDataToPanorams(bindings, ref panorams);
                }

                return true;
            }
            else
            {
                Debug.LogError("Reading panorama data error.");
            }
        }

        return false;
    }

    public static Texture LoadTexture(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        texture.LoadImage(bytes);

        return texture;
    }

    public static string GetNameFromPath(string path)
    {
        var splited = path.Split(separator);
        return splited[splited.Length - 1];
    }

    public static string GetNextImage(string path)
    {
        var directory = path.TrimEnd(separator).Remove(path.LastIndexOf(separator) + 1);

        var currentFileIndex = 0;
        var currentFileDetected = false;
        var files = Directory.GetFiles(directory);
        files = files.OrderBy(f => Regex.Replace(f, @"\d+", fn => fn.Value.PadLeft(5, '0'))).ToArray();
        
        for(int i = 0; i < files.Length; i++)
        {
            if (files[i] == path && i < files.Length - 1)
            {
                currentFileIndex = i;
                currentFileDetected = true;
            }

            if(currentFileDetected && i > currentFileIndex && Path.GetExtension(files[i]).ToLower() == ".jpg")
            {
                return files[i];
            }
        }

        return null;
    }

    public static bool CheckStreamingAssetsPath()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            return true;
        }
        else
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        return false;
    }
}