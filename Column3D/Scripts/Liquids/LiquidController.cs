using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidController : MonoBehaviour
{
    public float fullPlatesLiquid = -0.05f;
    public float emptyPlatesLiquid = 0.05f;
    public float fullBoardsLiquid = -1.0f;
    public float emptyBoardsLiquid = 0.06f;
    public Vector3 emptyPlateLiquidOffset;
    public Vector3 emptyBoardsLiquidOffset;

    public float SpeedFilling = 1.0f;
    [Range(0.0f, 1.0f)] public float PartForShow = 1.0f;

    private LiquidManager liquidManager;
    private static string fullnessShaderName = "_Fullness";

    private Material mainMaterial;
    private List<Material> boardMaterials;
    private Vector3 startPosition;
    private Vector3[] boardsStartPositions;
    private Coroutine currentPlateRoutine;
    private Coroutine currentBoardsRoutine;

    public bool plateIsFull { get; private set; }
    public bool boardsIsFull { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeMaterials();
        InitializePosition();
        InitializeLiquidManager();
        HidePlateNBoards();
    }
    private void InitializeMaterials()
    {
        if (transform.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            mainMaterial = meshRenderer.material;
            
            if (transform.childCount > 0)
            {
                boardMaterials = new List<Material>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i) != null)
                    {
                        if (transform.GetChild(i).TryGetComponent<MeshRenderer>(out var childMeshRenderer))
                        {
                            var childMaterial = childMeshRenderer.material;
                            boardMaterials.Add(childMaterial);
                        }
                    }
                }
            }

            plateIsFull = true;
            boardsIsFull = true;
        }
    }
    private void InitializePosition()
    {
        startPosition = transform.position;

        boardsStartPositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            boardsStartPositions[i] = child.position;          
        }
    }
    private void InitializeLiquidManager()
    {
        if (liquidManager == null)
        {
            if (transform.parent != null)
            {
                liquidManager = transform.parent.GetComponent<LiquidManager>();
            }
        }
    }

    public void ShowPlate()
    {
        StopPlateRoutine();

        if (!plateIsFull && mainMaterial != null)
        {
            var diffPosition = transform.position - startPosition;
            mainMaterial.SetFloat(fullnessShaderName, fullPlatesLiquid);
            transform.position = startPosition;

            foreach (Transform child in transform)
            {
                child.position += diffPosition;
            }
        }

        plateIsFull = true;
    }
    public void HidePlate()
    {
        if ((plateIsFull || currentPlateRoutine != null) && mainMaterial != null)
        {
            StopPlateRoutine();

            var diffPosition = transform.position - (startPosition + emptyPlateLiquidOffset);
            mainMaterial.SetFloat(fullnessShaderName, emptyPlatesLiquid);
            transform.position = startPosition + emptyPlateLiquidOffset;

            foreach (Transform child in transform)
            {
                child.position += diffPosition;
            }
        }

        plateIsFull = false;
    }
    public void ShowBoards()
    {
        StopBoardsRoutine();

        if (!boardsIsFull)
        {
            if (boardMaterials != null && boardsStartPositions != null)
            {
                foreach (var board in boardMaterials)
                {
                    if (board != null)
                    {
                        board.SetFloat(fullnessShaderName, fullBoardsLiquid);
                    }
                }
            }

            if (boardsStartPositions != null && boardsStartPositions.Length == transform.childCount)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position = boardsStartPositions[i];
                }
            }

            boardsIsFull = true;
        }
    }
    public void HideBoards()
    {
        if ((boardsIsFull || currentBoardsRoutine != null))
        {
            StopBoardsRoutine();

            if (boardMaterials != null)
            {
                foreach (var board in boardMaterials)
                {
                    if (board != null)
                    {
                        board.SetFloat(fullnessShaderName, emptyBoardsLiquid);
                    }
                }
            }

            if (boardsStartPositions != null && boardsStartPositions.Length == transform.childCount)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).position = boardsStartPositions[i] + emptyBoardsLiquidOffset;
                }
            }
        }

        boardsIsFull = false;
    }
    public void StartFillingPlate(Action actionAfter = null)
    {
        StopPlateRoutine();

        currentPlateRoutine = StartCoroutine(FillPlate(actionAfter));
    }
    public void StartFillingBoards(Action actionAfter = null)
    {
        StopBoardsRoutine();

        currentBoardsRoutine = StartCoroutine(FillBoards(actionAfter));
    }
    public void StartDrainagePlate(Action actionAfter = null)
    {
        StopPlateRoutine();

        currentPlateRoutine = StartCoroutine(DrainPlate(actionAfter));
    }
    public void StartDrainageBoards(Action actionAfter = null)
    {
        StopBoardsRoutine();

        currentBoardsRoutine = StartCoroutine(DrainBoards(actionAfter));
    }
    public void ShowPlatesNBoardsFromMe()
    {
        if (liquidManager != null)
        {
            liquidManager.ShowPlatesNBoardsFrom(this);
        }
    }
    public void ShowOnlyPlatesFromMe()
    {
        if (liquidManager != null)
        {
            liquidManager.ShowPlatesFrom(this);
        }
    }
    public void StartFillingFromMe()
    {
        if (liquidManager != null)
        {
            StopPlateRoutine();
            StopBoardsRoutine();

            liquidManager.StartFillingFrom(this);
        }
    }
    public void StartFilingOnlyMe()
    {
        StartFillingPlate(() => StartFillingBoards());
    }
    public void StartDrainageFromMe()
    {
        if (liquidManager != null)
        {
            StopPlateRoutine();
            StopBoardsRoutine();

            liquidManager.StartDrainageFrom(this);
        }
    }
    public void StartDrainageOnlyMe()
    {
        StartDrainagePlate(() => StartDrainageBoards());
    }
    public void ShowPlateNBoards()
    {
        ShowPlate();
        ShowBoards();
    }
    public void HidePlateNBoards()
    {
        HidePlate();
        HideBoards();
    }
    public void ShowPartPlate()
    {
        StopPlateRoutine();

        if (mainMaterial != null)
        {
            mainMaterial.SetFloat(fullnessShaderName, fullPlatesLiquid * PartForShow - (1 - PartForShow) * emptyPlatesLiquid);
            transform.position = Vector3.Lerp(startPosition + emptyPlateLiquidOffset, startPosition, PartForShow);
            plateIsFull = true;
        }
    }

    private IEnumerator FillPlate(Action actionAfter = null)
    {
        plateIsFull = false;

        if (mainMaterial != null)
        {
            float timeValue = 0.0f;
            var newPosition = startPosition;
            var oldPosition = transform.position;
            var oldLiquidValue = mainMaterial.GetFloat(fullnessShaderName);

            while (timeValue < 1.0f)
            {
                timeValue += Time.deltaTime * SpeedFilling;
                var previousPosition = transform.position;
                transform.position = Vector3.Lerp(oldPosition, newPosition, timeValue);
                mainMaterial.SetFloat(fullnessShaderName, Mathf.Lerp(oldLiquidValue, fullPlatesLiquid, timeValue));

                var diffPosition = previousPosition - transform.position;
                foreach (Transform child in transform)
                {
                    child.position += diffPosition;
                }

                yield return new WaitForEndOfFrame();
            }
        }
        plateIsFull = true;

        actionAfter?.Invoke();
        currentPlateRoutine = null;
    }
    private IEnumerator FillBoards(Action actionAfter = null)
    {
        float timeValue = 0.0f;
        boardsIsFull = false;

        if (boardMaterials != null)
        {
            while (timeValue < 1.0f)
            {
                timeValue += Time.deltaTime * SpeedFilling;
                foreach (var boardMaterial in boardMaterials)
                {
                    boardMaterial.SetFloat(fullnessShaderName, Mathf.Lerp(emptyBoardsLiquid, fullBoardsLiquid, timeValue));
                }

                if (boardsStartPositions != null && boardsStartPositions.Length == transform.childCount)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(boardsStartPositions[i] + emptyBoardsLiquidOffset, boardsStartPositions[i], timeValue);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        boardsIsFull = true;

        actionAfter?.Invoke();
        currentBoardsRoutine = null;
    }
    private IEnumerator DrainPlate(Action actionAfter = null)
    {
        plateIsFull = true;
        if (mainMaterial != null)
        { 
            float timeValue = 0.0f;
            var newPosition = transform.position + emptyPlateLiquidOffset;
            var oldPosition = transform.position;
            var oldLiquidValue = mainMaterial.GetFloat(fullnessShaderName);

            while (timeValue < 1.0f)
            {
                timeValue += Time.deltaTime * SpeedFilling;
                var previousPosition = transform.position;
                transform.position = Vector3.Lerp(oldPosition, newPosition, timeValue);
                mainMaterial.SetFloat(fullnessShaderName, Mathf.Lerp(oldLiquidValue, emptyPlatesLiquid, timeValue));

                var diffPosition = previousPosition - transform.position;
                foreach (Transform child in transform)
                {
                    child.position += diffPosition;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        plateIsFull = false;
        actionAfter?.Invoke();
        currentPlateRoutine = null;
    }
    private IEnumerator DrainBoards(Action actionAfter = null)
    {
        boardsIsFull = true;
        float timeValue = 0.0f;

        if (boardMaterials != null)
        {
            while (timeValue < 1.0f)
            {
                timeValue += Time.deltaTime * SpeedFilling;
                foreach (var boardMaterial in boardMaterials)
                {
                    boardMaterial.SetFloat(fullnessShaderName, Mathf.Lerp(fullBoardsLiquid, emptyBoardsLiquid, timeValue));
                }

                if (boardsStartPositions != null && boardsStartPositions.Length == transform.childCount)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(boardsStartPositions[i], boardsStartPositions[i] + emptyBoardsLiquidOffset, timeValue);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
        boardsIsFull = false;

        actionAfter?.Invoke();
        currentBoardsRoutine = null;
    }

    private void StopPlateRoutine()
    {
        if (currentPlateRoutine != null)
        {
            StopCoroutine(currentPlateRoutine);
            currentPlateRoutine = null;
        }
    }
    private void StopBoardsRoutine()
    {
        if (currentBoardsRoutine != null)
        {
            StopCoroutine(currentBoardsRoutine);
            currentBoardsRoutine = null;
        }
    }
}
