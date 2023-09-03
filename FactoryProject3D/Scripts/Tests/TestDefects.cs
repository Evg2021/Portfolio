using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class TestDefects : MonoBehaviour
{
    private const string pipesDataPointer = "#####";
    private const string pipesDataPattern = @"\[.*?\]";
    private const char rowsSpliter = ';';

    public List<PipeData> PipesData;

    private void Reset()
    {
        //CollectPipes(Directory.GetCurrentDirectory() + "\\visio\\");
    }

    //Информация о потоках начинается после строки #####
    //Там есть имена потоков
    //public void CollectListPipes()
    //{
    //    Debug.Log("start CollectListPipes");

    //    string pathDir = Directory.GetCurrentDirectory() + "\\visio";

    //    string[] fileNames = Directory.GetFiles(pathDir, "*.csv");
    //    Debug.Log("start CollectListPipes in" + pathDir);

    //    for (int i = 0; i < fileNames.Length; i++)
    //    {
    //        Debug.Log("start CollectListPipes in" + fileNames[i].ToString());

    //        //System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    //        var win1251 = System.Text.Encoding.GetEncoding("windows-1251");
    //        var rows = File.ReadLines(fileNames[i], win1251);
    //        bool alert = false;
    //        foreach (var row in rows)
    //        {
    //            if (alert)
    //            {
    //                var partsOfRow = Regex.Matches(row, @"\[.*?\]", RegexOptions.Singleline);  //row.Split(new char[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
    //                if (partsOfRow == null || partsOfRow.Count == 0)
    //                    continue;

    //                string pipeName = partsOfRow[0].ToString();
    //                pipeName = pipeName.Substring(2, pipeName.Length - 4);

    //                List<string> pipesIncludeFittings = new List<string>();
    //                for (int j = 1; j < partsOfRow.Count; j++)
    //                {
    //                    //if (FittingComponentsSetterProp.IsExistingTrenName(partsOfRow[j].ToString().Substring(1, partsOfRow[j].ToString().Length - 2)))
    //                    //    pipesIncludeFittings.Add(partsOfRow[j].ToString().Substring(1, partsOfRow[j].ToString().Length - 2));
    //                }
    //                if (pipesIncludeFittings.Count > 0)
    //                {
    //                    //if (!pipesItems.ContainsKey(pipeName))
    //                    //{
    //                    //    pipesItems.Add(pipeName, pipesIncludeFittings);
    //                    //}
    //                    //else
    //                    //    Debug.Log(pipeName + " double in " + fileNames[i]);
    //                }
    //            }

    //            if (row.Contains("########"))
    //                alert = true;
    //        }
    //        //Debug.Log("pipesItems has: " + pipesItems.Count + " items");
    //    }
    //}

    private void CollectPipes(string path, TrenObject[] trenObjects)
    {
        string[] fileNames = Directory.GetFiles(path, "*.csv");
        if (fileNames != null)
        {
            PipesData = new List<PipeData>();
            foreach (var fileName in fileNames)
            {
                var rows = File.ReadAllLines(fileName, Encoding.GetEncoding("windows-1251"));
                bool isPipesData = false;
                for (int i = 0; i < rows.Length; i++)
                {
                    if (isPipesData)
                    {
                        var rowsData = Regex.Matches(rows[i], pipesDataPattern);
                        var pipeData = new PipeData();
                        var inObjects = new List<Transform>();
                        var outObjects = new List<Transform>();

                        for (int j = 0; j < rowsData.Count; j++)
                        {
                            string clearedData = rowsData[j].Value.Replace("[", string.Empty)
                                                                  .Replace("]", string.Empty)
                                                                  .Replace("\"", string.Empty);

                            if (j == 0)
                            {
                                pipeData.PipeName = clearedData;
                            }
                            else if(j % 2 == 1)
                            {
                                var trenObjectOnScene = trenObjects.FirstOrDefault(h => h.TrenName == clearedData);

                                if (j < rowsData.Count - 1 && trenObjectOnScene)
                                {
                                    string enterType = rowsData[j+1].Value.Replace("[", string.Empty)
                                                                          .Replace("]", string.Empty)
                                                                          .Replace("\"", string.Empty);
                                    if (enterType.Contains('o'))
                                    {
                                        outObjects.Add(trenObjectOnScene.transform);
                                    }
                                    else
                                    {
                                        inObjects.Add(trenObjectOnScene.transform);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(pipeData.PipeName) && (outObjects.Count > 0 || inObjects.Count > 0))
                        {
                            pipeData.inTrenObjects = inObjects.ToArray();
                            pipeData.outTrenObject = outObjects.ToArray();
                            PipesData.Add(pipeData);
                        }
                    }

                    if (rows[i].Contains(pipesDataPointer))
                    {
                        isPipesData = true;
                    }
                }
            }
        }
    }

    private void Initbindigs()
    {
        var trenObjectsBindings = new List<TrenObjectData>();

        if (Utilities.CheckStreamingAssetsPath())
        {
            if (File.Exists(Application.streamingAssetsPath + '\\' + "Bindings.json"))
            {
                using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + '\\' + "Bindings.json"))
                {
                    try
                    {
                        var data = JsonUtility.FromJson<TrenObjectDataList>(reader.ReadToEnd());
                        trenObjectsBindings = data.Data;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }
    }
}

[Serializable]
public struct PipeData
{
    public string PipeName;
    public Transform[] inTrenObjects;
    public Transform[] outTrenObject;
}

[Serializable]
public struct Row
{
    public List<string> point;

    public Row(List<string> value)
    {
        point = value;
    }
}