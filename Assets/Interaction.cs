using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/*public class Interaction : MonoBehaviour
{
    private IInteractable curInteractable;
    public GameObject curInteractGameObject;

    //public Transform rayStartPoint;
    public float interDistance = 3f;
    public LayerMask itemLayer;
    //public GameObject potionInfoUI;
    public TextMeshProUGUI promptText;

    void Update()
    {
        CheckForItem();
    }

    void CheckForItem()
    {
        Camera mainCam = Camera.main;
        if(promptText == null || mainCam == null) return;

        RaycastHit rayHit;
        Vector3 origin = mainCam.transform.position;
        Vector3 direction = mainCam.transform.forward;
        Vector3 boxExtent = new Vector3(0.7f, 0.7f, 0.7f);
        Quaternion orientation = mainCam.transform.rotation;
        if (Physics.BoxCast(origin, boxExtent, direction, out rayHit, orientation, interDistance, itemLayer)) // BoxCast 실행: 상자(boxExtents)를 방향(direction)으로 쏘기
        {
            if (rayHit.collider.gameObject != curInteractGameObject)
            {
                // 감지됨
                curInteractGameObject = rayHit.collider.gameObject;
                curInteractable = rayHit.collider.GetComponent<IInteractable>();

                SetPromptText();
            }
        }
        else
        {
            //감지 X
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
        Debug.DrawRay(origin, direction * interDistance, Color.yellow);
    }
    private void SetPromptText()
    {
        if (curInteractable != null && promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = curInteractable.GetInteractPrompt();
        }
        else
        {
            // BoxCast는 성공했으나 IInteractable이 없는 경우
            promptText.gameObject.SetActive(false);
        }
    }
}*/
