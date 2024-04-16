using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class InteractableDescription : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriprionText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _interactableTypeImage;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Start()
    {
        _signalBus.Subscribe<EnterPointerEntitySignal>(OnShowEntityDescription);
        _signalBus.Subscribe<MoveInteractSignal>(UpdateDescriptionInfo);
        _signalBus.Subscribe<ExitPointerEntitySignal>(OnHideWhenExitCursor);
        _signalBus.Subscribe<StartZoomAtSignal>(ZoomAtLaunch);

        SetActive(false);
    }
    private void OnDestroy()
    {
        _signalBus.Unsubscribe<EnterPointerEntitySignal>(OnShowEntityDescription);
        _signalBus.Unsubscribe<MoveInteractSignal>(UpdateDescriptionInfo);
        _signalBus.Unsubscribe<ExitPointerEntitySignal>(OnHideWhenExitCursor);
        _signalBus.Unsubscribe<StartZoomAtSignal>(ZoomAtLaunch);
    }

    private void OnShowEntityDescription(EnterPointerEntitySignal signal)
    {
        if (signal.Value.Description == null)
        {
            print("Description is missing!!!");
            return;
        }

        SetActive(true);
        SetInfo(signal.Value);
    }

    private void SetActive(bool value)
    {
        _nameText.enabled = value;
        _descriprionText.enabled = value;
        _image.enabled = value;
        _interactableTypeImage.enabled = value;
    }

    private void UpdateDescriptionInfo(MoveInteractSignal entity)
    {
        string value = entity.Value.Interactable.GetDisplayInfo();
        _nameText.text = $"{entity.Value.Description.Name} <style=\"Bold\">{value}</style>";
    }

    private void SetInfo(RaycastEntity entity)
    {
        string value = "";
        if (entity.Interactable != null)
            value = entity.Interactable.GetDisplayInfo();

        _nameText.text = $"{entity.Description.Name} <style=\"Bold\">{value}</style>";
        _descriprionText.text = entity.Description.Description;
        _image.sprite = entity.Description.Sprite;

        SetPositionToPointer();
    }

    private void OnHideWhenExitCursor()
    {
        SetActive(false);
    }

    private void ZoomAtLaunch(StartZoomAtSignal signal)
    {
        if (!signal.Value.ZoomAt.ZoomDescription)
            SetActive(false);
    }

    private void SetPositionToPointer()
    {
        Vector3 pointerPosition = Pointer.current.position.ReadValue();
        transform.position = pointerPosition;
    }
}