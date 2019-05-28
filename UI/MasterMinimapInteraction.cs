// ======================================================================
//    Land Forgotten : Master Minimap Interaction UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MasterMinimapInteraction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TopDownMinimap _MasterMinimap;
    [SerializeField] private Image _MapImage;

    private void Start()
    {
        _MapImage.alphaHitTestMinimumThreshold = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        // Debug.Log("LocalCursor:" + localCursor);
        AkSoundEngine.PostEvent("Play_Select", gameObject);
        _MasterMinimap.ChangeMasterPosisiton(localCursor);

    }


}
