using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PipesStreamsController : SingletonMonoBehaviour<PipesStreamsController>
{
    public List<Stream> Streams;

    private static CanvasController canvas2D;
    private Dictionary<int, GameObject> ownDescriptions;
    private PipeKeyObject[] pipesList;
    private List<Stream> selectedStreams;

    protected override void Awake()
    {
        base.Awake();

        InitializeCanvas();
        InitializePipesKeyObjects();
        InitializeVariables();
    }

    private void InitializePipesKeyObjects()
    {
        pipesList = GetComponentsInChildren<PipeKeyObject>();
    }
    private void InitializeCanvas()
    {
        var canvasObject = Utility.GetCanvas2D();
        if (canvasObject != null)
        {
            if (!canvasObject.TryGetComponent(out canvas2D))
            {
                Debug.LogError("Canvas2D has no CanvasController component.");
            }
        }
    }
    private void InitializeVariables()
    {
        ownDescriptions = new Dictionary<int, GameObject>();
        selectedStreams = new List<Stream>();
    }

    public void CollectStreams()
    {
        var streamsDictionary = new Dictionary<int, List<PipeKeyObject>>();

        var pipeKeyObjects = FindObjectsOfType<PipeKeyObject>();
        foreach (var pipe in pipeKeyObjects)
        {
            var pipeStreams = pipe.StreamsNumbers;
            if (pipeStreams != null)
            {
                foreach (var pipeStream in pipeStreams)
                {
                    if (streamsDictionary.TryGetValue(pipeStream, out var pipes))
                    {
                        if (!pipes.Contains(pipe))
                        {
                            pipes.Add(pipe);
                        }
                    }
                    else
                    {
                        var newPipes = new List<PipeKeyObject>();
                        newPipes.Add(pipe);
                        streamsDictionary.Add(pipeStream, newPipes);
                    }
                }
            }
        }

        if (streamsDictionary.Count > 0)
        {
            if (Streams == null || Streams.Count == 0)
            {
                Streams = new List<Stream>();

                foreach (var stream in streamsDictionary)
                {
                    Streams.Add(new Stream()
                    {
                        StreamNumber = stream.Key,
                        PipeKeyObjects = stream.Value
                    });
                }
            }
            else if (Streams != null && Streams.Count > 0)
            {
                foreach (var stream in streamsDictionary)
                {
                    var originStream = Streams.FirstOrDefault(h => h.StreamNumber == stream.Key);
                    if (originStream.StreamNumber != stream.Key || string.IsNullOrEmpty(originStream.Description))
                    {
                        Streams.Add(new Stream()
                        {
                            StreamNumber = stream.Key,
                            PipeKeyObjects = stream.Value
                        });
                    }
                    else
                    {
                        Streams.Remove(originStream);
                        originStream.PipeKeyObjects = stream.Value;
                        Streams.Add(originStream);
                    }
                }
            }
        }


        Debug.Log("Pipes collected. Streams is " + streamsDictionary.Count + '.');
    }

    public void ClearNullStreams()
    {
        if (Streams != null)
        {
            foreach (var stream in Streams)
            {
                var oldPipes = stream.PipeKeyObjects;
                stream.PipeKeyObjects.Clear();
                foreach (var pipeKeyObject in oldPipes)
                {
                    if (pipeKeyObject != null && pipeKeyObject.gameObject != null)
                    {
                        stream.PipeKeyObjects.Add(pipeKeyObject);
                    }
                }
            }
        }
    }

    public void IndicatorStreamOn(int key)
    {
        if (Streams != null)
        {
            var stream = Streams.FirstOrDefault(h => h.StreamNumber == key);
            if (stream.StreamNumber == key)
            {
                if (!string.IsNullOrEmpty(stream.Description))
                {
                    foreach (var pipes in stream.PipeKeyObjects)
                    {
                        pipes.IndicatorMaterialOn();
                    }
                }
            }
        }
    }
    public void IndicatorStreamOff(int key)
    {
        if (Streams != null)
        {
            var stream = Streams.FirstOrDefault(h => h.StreamNumber == key);
            if (stream.StreamNumber == key)
            {
                foreach (var pipes in stream.PipeKeyObjects)
                {
                    pipes.IndicatorMaterialOff();
                }
            }
        }
    }
    public void SelectedStreamOn(int key)
    {
        if (Streams != null)
        {
            var stream = Streams.FirstOrDefault(h => h.StreamNumber == key);
            if (stream.StreamNumber == key)
            {
                if (!string.IsNullOrEmpty(stream.Description))
                {
                    foreach (var pipes in stream.PipeKeyObjects)
                    {
                        pipes.SelectedMaterialOn();
                    }
                    selectedStreams.Add(stream);
                }
            }
        }
    }
    public void SelectedStreamOff(int key)
    {
        if (Streams != null)
        {
            var stream = Streams.FirstOrDefault(h => h.StreamNumber == key);
            if (stream.StreamNumber == key)
            {
                var otherSelectedStreams = selectedStreams.Where(h => h.StreamNumber != key);
                foreach (var pipes in stream.PipeKeyObjects)
                {
                    if (otherSelectedStreams.Count() > 0)
                    {
                        if (!otherSelectedStreams.Any(h => h.PipeKeyObjects.Any(j => j == pipes)))
                        {
                            pipes.SelectedMaterialOff();
                        }
                    }
                    else
                    {
                        pipes.SelectedMaterialOff();
                    }
                }

                if (selectedStreams.Count > 0)
                {
                    selectedStreams.Remove(stream);
                }
            }
        }
    }
    
    public void ShowStreamDescription(int key)
    {
        if (canvas2D != null && ownDescriptions != null && Streams != null)
        {
            var stream = Streams.FirstOrDefault(h => h.StreamNumber == key);
            if (stream.StreamNumber == key)
            {
                if (!ownDescriptions.TryGetValue(key, out _) && !string.IsNullOrEmpty(stream.Description))
                {
                    var ownWindowDescription = canvas2D.InstantiateDescriptionWindow();
                    if (ownWindowDescription.TryGetComponent<DescriptionController>(out var controller))
                    {
                        controller.Initialize(stream.Description, stream.Title, null, null, delegate () 
                        {
                            ownDescriptions.Remove(key);
                            SelectedStreamOff(key);
                        });
                        ownDescriptions.Add(key, ownWindowDescription);
                    }
                }
            }
        }
    }
    public bool ContainsAnyStream(int[] keys)
    {
        if (Streams != null)
        {
            foreach (int key in keys)
            {
                return ContainsStream(key);
            }
        }

        return false;
    }
    public bool ContainsStream(int key)
    {
        if (Streams != null)
        {
            return Streams.Any(h => h.StreamNumber == key);
        }

        return false;
    }

    public void MakePipesNonClickable()
    {
        SetClickable(false);
    }
    public void MakePipesClickable()
    {
        SetClickable(true);
    }
    private void SetClickable(bool value)
    {
        if (pipesList != null)
        {
            foreach (var pipe in pipesList)
            {
                pipe.SetClickable(value);
            }

        }
    }

    [Serializable]
    public struct Stream
    {
        public int StreamNumber;
        public List<PipeKeyObject> PipeKeyObjects;
        public string Title;
        [TextArea]
        public string Description;
    }
}
