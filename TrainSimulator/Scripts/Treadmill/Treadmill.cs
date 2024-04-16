using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    [Header("Spawn settings")] [SerializeField]
    private List<Chunk> _types = new();

    [SerializeField] private float _speedMultiplier = 0.25f;
    [SerializeField] private List<OrderChunk> _chunksOrder = new();
    [SerializeField] private Transform _container;
    [SerializeField] private int _count;

    [Header("Movement settings")] [Dropdown("GetVectorValues")] [SerializeField]
    private Vector3 _direction;

    private ChunkPool _pool;

    public ReactiveProperty<float> Speed { get; } = new();
    private DropdownList<Vector3> GetVectorValues()
    {
        return new DropdownList<Vector3>()
        {
            { "Right", Vector3.right },
            { "Left", Vector3.left },
            { "Up", Vector3.up },
            { "Down", Vector3.down },
            { "Forward", Vector3.forward },
            { "Back", Vector3.back }
        };
    }

    private List<Chunk> _activeChunks = new();
    private float _maxDistance;
    private int _lastSpawnedIndex = -1;
    private int _lastInsertedInOrderChunksIndex = -1;

    private void OnValidate()
    {
        if (_chunksOrder.Count > 0)
            foreach (var chunk in _chunksOrder)
                chunk.Init(_types);

        _count = Mathf.Min(_count, _chunksOrder.Count);
    }

    private void Start()
    {
        CreatePool();
        SpawnChunksOnStart();
    }

    private void CreatePool()
    {
        var chunksCollection = _chunksOrder.Select(element => element.Chunk).ToList();
        _pool = new(chunksCollection, _container);
    }

    private void SpawnChunksOnStart()
    {
        List<Chunk> initialChunks = _chunksOrder.Select(element => element.Chunk).Take(_count).ToList();
        CountMaxDistance(initialChunks);

        for (int i = 0; i < _count; i++)
            Spawn();
    }

    private Chunk Spawn()
    {
        _lastSpawnedIndex = GetNextIndex();
        Chunk chunk = _pool.Get(_lastSpawnedIndex);

        chunk.transform.position = (_activeChunks.Count == 0)
            ? Vector3.zero
            : _activeChunks.Last().transform.position - ((_activeChunks.Last().Size + chunk.Size) * 0.5f * _direction);

        _activeChunks.Add(chunk);

        return chunk;
    }

    private int GetNextIndex() => (_lastSpawnedIndex + 1) % _chunksOrder.Count;

    private void CountMaxDistance(List<Chunk> chunks)
    {
        float totalChunksLength = 0;

        foreach (var chunk in chunks)
            totalChunksLength += chunk.Size;

        _maxDistance = totalChunksLength * 0.5f;
    }

    private void Update()
    {
        MoveChunks();
        CheckAndHandleFarChunks();
    }

    private void MoveChunks()
    {
        Vector3 directionSpeed = Speed.Value * _speedMultiplier * Time.deltaTime * _direction;

        foreach (var chunk in _activeChunks)
            chunk.transform.position += directionSpeed;
    }

    private void CheckAndHandleFarChunks()
    {
        Chunk farChunk = _activeChunks[0];

        if (IsFar(farChunk))
            RemoveAndSpawnChunks();
    }

    private void RemoveAndSpawnChunks()
    {
        Chunk farChunk = _activeChunks[0];
        _activeChunks.RemoveAt(0);
        _pool.Release(farChunk);

        Spawn();

        CountMaxDistance(_activeChunks);
    }

    private bool IsFar(Chunk chunk)
    {
        float positionAxisValue = chunk.transform.position.magnitude;

        return Mathf.Abs(positionAxisValue) > _maxDistance;
    }

    public void SetSpeed(float value, float duration, Ease ease = Ease.Linear)
    {
        DOTween.To(() => Speed.Value, x => Speed.Value = x, value, duration).SetEase(ease);
    }

    public void InsertChunk(Chunk chunkPrefab)
    {
        if (_types.Count(x => x.Name == chunkPrefab.Name) == 0)
            _types.Add(chunkPrefab);

        OrderChunk orderChunk = AddOrderChunk(chunkPrefab, _chunksOrder.Count - 1);

        _pool.Add(orderChunk.Chunk, _container);
    }

    public void InsertChunk(Chunk chunkPrefab, int positionIndex)
    {
        int nearestActiveIndex = 0;

        for (int i = 0; i < _activeChunks.Count; i++)
            if (GetChunkDistance(i, _activeChunks) < GetChunkDistance(nearestActiveIndex, _activeChunks))
                nearestActiveIndex = i;

        int activeIndex = nearestActiveIndex + positionIndex;

        if (activeIndex < 0 || activeIndex > _activeChunks.Count - 1) return;

        int orderIndex = _pool.Chunks.IndexOf(_activeChunks[activeIndex]);

        var orderChunk = AddOrderChunk(chunkPrefab, orderIndex);

        _pool.Add(orderChunk.Chunk, _container, orderIndex);
        var chunk = _pool.Get(orderIndex);
        chunk.transform.position = _activeChunks[activeIndex].transform.position;
        _activeChunks.Insert(activeIndex, chunk);
            
        if (_lastSpawnedIndex >= activeIndex)
            _lastSpawnedIndex = GetNextIndex();

        CountMaxDistance(_activeChunks);

        _lastInsertedInOrderChunksIndex = orderIndex;

        if (activeIndex >= _activeChunks.Count - 1) return;

        for (int i = activeIndex + 1; i < _activeChunks.Count; i++)
            _activeChunks[i].transform.position -= chunk.Size * _direction;

        static float GetChunkDistance(int index, List<Chunk> chunks) => 
            (Vector3.Distance(Vector3.zero, chunks[index].transform.position));
    }

    private OrderChunk AddOrderChunk(Chunk chunkPrefab, int index)
    {
        if (_types.Count(x => x.Name == chunkPrefab.Name) == 0)
            _types.Add(chunkPrefab);

        OrderChunk orderChunk = new();
        orderChunk.Create(chunkPrefab);
        _chunksOrder.Insert(index, orderChunk);
        _chunksOrder[index].Init(_types);

        return _chunksOrder[index];
    }

    public void StopAtLastInsertedChunk(Vector3 stopAt, out float stopDuration) => 
        StopAt(stopAt, out stopDuration, _chunksOrder.Count - 1);

    public void StopAtLastChunk(Vector3 stopAt, out float stopDuration)
    {
        if (_lastInsertedInOrderChunksIndex == -1)
        {
            stopDuration = 0;

            return;
        }

        StopAt(stopAt, out stopDuration, _lastInsertedInOrderChunksIndex);
    }

    public void StopAtIndex(Vector3 stopAt, out float stopDuration, int index) =>
        StopAt(stopAt, out stopDuration, index);

    private void StopAt(Vector3 stopAt, out float stopDuration, int index)
    {
        DOTween.Kill(this, false);

        float distance = GetDistance(index, stopAt);
        float duration = 2f * distance /  Speed.Value; //need to multiply to _speedMultiply

        stopDuration = duration;

        SetSpeed(0, duration);

        float GetDistance(int chunkIndex, Vector3 stopAt)
        {
            float distance = 0;
            var chunkInQueue = _pool.Chunks[chunkIndex];

            if (_activeChunks.Contains(chunkInQueue))
            {
                distance = Vector3.Distance(stopAt, chunkInQueue.transform.position);
            }
            else
            {
                Chunk lastInActiveChunk = _activeChunks[^1];
                distance += Vector3.Distance(stopAt, lastInActiveChunk.transform.position + 0.5f * lastInActiveChunk.Size * -_direction);

                int lastInActivePoolIndex = _pool.Chunks.IndexOf(lastInActiveChunk);

                List<Chunk> chunks = GetDifferenceElements(_pool.Chunks, lastInActivePoolIndex, chunkIndex);

                for (int i = 0; i < chunks.Count; i++)
                {
                    float additiveSize = (i < chunks.Count - 1) ? chunks[i].Size : chunks[i].Size * 0.5f;
                    distance += additiveSize;
                }

                static List<Chunk> GetDifferenceElements(List<Chunk> collection, int startIndex, int endIndex)
                {
                    List<Chunk> chunks = new();

                    int i = startIndex;

                    while (i != endIndex)
                    {
                        i++;

                        if (i > collection.Count - 1)
                            i = 0;

                        chunks.Add(collection[i]);
                    }

                    return chunks;
                }
            }

            return distance;
        }
    }

    [Serializable]
    public class OrderChunk
    {
        public Chunk Chunk => _chunk;
        private DropdownList<Chunk> _types = new();

        public void Init(List<Chunk> types)
        {
            _types.Clear();

            for (int i = 0; i < types.Count; i++)
                _types.Add(types[i].Name, types[i]);
        }

        public void Create(Chunk chunk) => _chunk = chunk;

        [Dropdown("_types")] [SerializeField] private Chunk _chunk;
    }

    public class ChunkPool
    {
        public List<Chunk> Chunks => _chunks;
        private List<Chunk> _chunks;

        public ChunkPool(List<Chunk> prefabs, Transform container)
        {
            _chunks = new();

            foreach (var prefab in prefabs)
            {
                var chunk = Instantiate(prefab);
                chunk.transform.SetParent(container);

                _chunks.Add(chunk);
            }
        }

        public void Add(Chunk prefab, Transform container, int index = -1)
        {
            if (prefab == null) return;

            var chunk = Instantiate(prefab);
            chunk.gameObject.SetActive(false);
            chunk.transform.SetParent(container);

            if (index == -1)
                _chunks.Add(chunk);
            else
                _chunks.Insert(index, chunk);
        }

        public Chunk Get(int index)
        {
            if (index > _chunks.Count) return null;

            _chunks[index].gameObject.SetActive(true);

            return _chunks[index];
        }

        public void Release(int chunk)
        {
            _chunks[chunk].gameObject.SetActive(false);
        }

        public void Release(Chunk chunk)
        {
            _chunks[_chunks.IndexOf(chunk)].gameObject.SetActive(false);
        }
    }
}