using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Arrow
{
    public bool stairsIsUp;
    public bool IsStairsUp => stairsIsUp;

    protected override void Awake()
    {
        base.Awake();

        var panoramaController = PanoramaController.Instance;
        if(panoramaController != null)
        {
            //panoramaController.TransitionAdded += PanoramaController_TransitionAdded;
        }    
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var panoramaController = PanoramaController.Instance;
        if (panoramaController != null)
        {
            //panoramaController.TransitionAdded -= PanoramaController_TransitionAdded;
        }
    }

    private void PanoramaController_TransitionAdded(Panorama arg1, ArrowInfo arg2)
    {
        
        var adapter = AdapterPanelController.Instance;
        if(adapter != null)
        {
            if (arg2.NextPanoramaFilename == NextPanorama.ImageLink)
            {
                stairsIsUp = adapter.IsStairsUp();
            }
        }
    }

    public override void OnPressUp()
    {
        base.OnPressUp();

        if (NextPanorama != null && !ui.AdapterController.gameObject.activeSelf && timer > 0)
        {
            /*int nextIndex = MapController.Instance.CurrentLevel + (stairsIsUp ? 1 : -1);
            MapController.Instance.ShowLevel(nextIndex);*/
        }
    }

    public override void OnClickRight()
    {
        base.OnClickRight();

        if (!canMove)
        {
            ui.AdapterController.SetStateStairs(stairsIsUp);
        }
    }
}
