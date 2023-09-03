using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapItem : MovableObject
{
    public List<StairsInfo> Stairs;

    public int Level;
    public bool AllowEdit = true;

    public List<Transform> TransitionsInside { get; private set; }
    public Dictionary<Transform, Transform> TransitionsOutside { get; private set; }

    private string panoramaPath;

    private float speedRotation = 5.0f;
    private float timeToHoldForMove = 0.25f;

    private bool isDestroyed;
    private Vector3 savedPosition;

    public string PanoramaPath
    {
        get
        {
            return panoramaPath;
        }
        set
        {
            panoramaPath = value;
            InitializeTexture();
        }
    }

    public Vector3 Position
    {
        get
        {
            if (this != null)
            {
                return transform.localPosition;
            }
            else
            {
                return savedPosition;
            }
        }
        set
        {
            transform.localPosition = value;
        }
    }

    public void AddTransitionInside(Transform transition)
    {
        if (TransitionsInside != null)
            TransitionsInside.Add(transition);
    }

    public void AddTransitionOutside(Transform transition, Transform nextMapItem)
    {
        if (TransitionsOutside != null)
            TransitionsOutside.Add(transition, nextMapItem);
    }

    public void RemoveTransitionInside(Transform transition)
    {
        if (TransitionsInside != null && TransitionsInside.Contains(transition))
            TransitionsInside.Remove(transition);
    }

    public void RemoveTransitionOutside(Transform transition)
    {
        if (TransitionsOutside != null && TransitionsOutside.ContainsKey(transition))
            TransitionsOutside.Remove(transition);
    }

    public void AddStairs(StairsInfo stairs)
    {
        var oldStairs = Stairs.Where(h => h.NextPanoramaFilename == stairs.NextPanoramaFilename).FirstOrDefault();
        if (oldStairs == null)
        {
            Stairs.Add(stairs);
        }
        else
        {
            oldStairs = stairs;
        }
    }

    public void RemoveStairs(StairsInfo stairs)
    {
        Stairs.Remove(stairs);
    }

    public void Initialize()
    {
        TransitionsInside  = new List<Transform>();
        TransitionsOutside = new Dictionary<Transform, Transform>();
        Stairs = new List<StairsInfo>();
        Level = 0;
    }

    public void InitializeTexture()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", Utilities.LoadTexture(panoramaPath));
    }

    public override void OnPressDown()
    {
        if (!AllowEdit) return;

        base.OnPressDown();
    }

    public override void OnHold()
    {
        if (!AllowEdit) return;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (timer <= 0 && startCursor == input.CursorPosition() / Screen.dpi)
        {
            EnableHighlight();
        }
        else if (timer <= 0 && startCursor != input.CursorPosition() / Screen.dpi)
        {
            timer = timeToHoldForMove;
            startCursor = input.CursorPosition() / Screen.dpi;
        }

        if (allowToChangePosition)
        {
            MoveByCursor();
            MapController.Instance.UpdateMapItem(this);
            MapController.Instance.DisableMoving();
        }
    }

    private void Start()
    {
        renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        ownMaterial = renderer.sharedMaterial;
    }

    public override void CancelPress()
    {
        if (!AllowEdit) return;

        base.CancelPress();

        MapController.Instance.EnableMoving();
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.0f);
    }

    public override void OnPressUp()
    {
        base.OnPressUp();

        var controller = PanoramaController.Instance;
        var panorama = controller.GetPanorama(panoramaPath);
        UIController.Instance.HideMap();
        controller.ChangePanorama(panorama);

        MapController.Instance.EnableMoving();
        MapController.Instance.ShowLevel(Level);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.0f);
    }

    private void Update()
    {
        if(!input.GetFirstKey())
        {
            transform.Rotate(Vector3.up, Time.deltaTime * speedRotation);
        }
    }

    private void OnGUI()
    {
        var screenPoint = Camera.main.WorldToScreenPoint(transform.position - transform.up * transform.localScale.y * 0.5f);

        if(screenPoint.z > 0)
        {
            var keyName = Utilities.GetNameFromPath(panoramaPath);

            var rect = new Rect(screenPoint.x, Screen.height - screenPoint.y, 100.0f, 20.0f);
            GUI.Label(rect, keyName);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        savedPosition = Position;
    }
}
