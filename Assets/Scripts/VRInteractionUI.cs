using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.VR;

public class VRInteractionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject reticle;

    [SerializeField]
    private Transform reticleSelectionIndicator;

    [SerializeField]
    private float focusedReticleSize = 2f;

    [SerializeField]
    private float unfocusedReticleSize = 1f;

    [SerializeField]
    private float focusingSpeed = 10f;

    [SerializeField]
    private bool testMode = false;

    private Vector3 originalReticleScale;

    private GraphicRaycaster graphicRaycaster;

    private PointerEventData pointerEventData;

    private Button buttonToInteract;

    private void Start()
    {
        if (!VRSettings.isDeviceActive && !testMode)
        {
            reticle.gameObject.SetActive(false);
            Destroy(this);
        }

        originalReticleScale = reticleSelectionIndicator.localScale;

        pointerEventData = new PointerEventData(null);
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);

        GetGraphicsRaycaster();
    }

    private void Update()
    {
        if (graphicRaycaster == null)
        {
            GetGraphicsRaycaster();

            return;
        }

        buttonToInteract = null;

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            buttonToInteract = results[0].gameObject.GetComponent<Button>();

            if (buttonToInteract != null)
            {
                buttonToInteract.Select();

                if (Input.GetMouseButtonDown(0))
                {
                    buttonToInteract.onClick.Invoke();

                    return;
                }
            }
        }

        float size = buttonToInteract != null ? focusedReticleSize : unfocusedReticleSize;

        if (reticleSelectionIndicator != null)
            reticleSelectionIndicator.localScale = Vector3.Lerp(reticleSelectionIndicator.localScale, originalReticleScale * size, Time.deltaTime * focusingSpeed);
    }

    private void GetGraphicsRaycaster()
    {
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
    }
}
