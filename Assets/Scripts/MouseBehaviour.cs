using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    private Piece grabbedPiece;
    private bool isGrabbed = false;
    public LayerMask LayerMask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isGrabbed)
            {
                // Raycast to check if we hit a GameObject
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask))
                {
                    Piece piece = hit.collider.gameObject.GetComponent<Piece>();
                    if (piece != null)
                    {
                        grabbedPiece = piece;
                        grabbedPiece.ClickedFull();
                        isGrabbed = true;
                    }
                }
            }
            else
            {
                // Release the grabbed GameObject
                grabbedPiece.DeClickedFull();
                grabbedPiece = null;
                isGrabbed = false;
            }
        }


    }

}
