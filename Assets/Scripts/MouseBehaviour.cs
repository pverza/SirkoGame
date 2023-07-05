using Unity.Netcode;
using UnityEngine;

public class MouseBehaviour : NetworkBehaviour
{
    [SerializeField]
    private Piece grabbedPiece;
    [SerializeField]
    private NetworkVariable<GrabbedType> isGrabbed = new NetworkVariable<GrabbedType>(GrabbedType.NOT_GRABBED);
    public LayerMask LayerMask;

    private Vector3 offset;

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer || !NetworkManager.Singleton.IsConnectedClient)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PerformMovementAndLogic();
            }
            //movement a bith hacky but I need to see if it works
            Move();
        }
        else 
        {
            if (Input.GetMouseButtonDown(0))
            {
                SendServerMessageServerRpc(Camera.main.ScreenPointToRay(Input.mousePosition));
            }
            if (isGrabbed.Value == GrabbedType.GRABBED_CLIENT) //for the movement
            {
                Vector3 targetPosition = GetMouseWorldPosition() + offset;
                SendServerMessageForMovementServerRpc(targetPosition.x, targetPosition.z);
            }
        }

            
    }

    //private void NetworkUpdate()
    //{
    //    if (isGrabbed && !NetworkManager.Singleton.IsServer) //for the movement
    //    {
    //        Vector3 targetPosition = GetMouseWorldPosition() + offset;
    //        SendServerMessageForMovementServerRpc(targetPosition.x, targetPosition.z);
    //    }
    //}


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }


    private NetworkClient networkClient;

    private void Start()
    {
        networkClient = NetworkManager.Singleton.LocalClient;
    }

    //this is the client
    [ServerRpc(RequireOwnership = false)]
    private void SendServerMessageServerRpc(Ray ray)
    {
        if (isGrabbed.Value == GrabbedType.NOT_GRABBED)
        {
            // Raycast to check if we hit a GameObject
            //Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePositionX, mousePositionY, mousePositionZ));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask))
            {
                Piece piece = hit.collider.gameObject.GetComponent<Piece>();
                if (piece != null && GameManager.instance.IsTeamsTurn(piece.pieceTeam) && !piece.isAlreadyMoved && !GameManager.instance.isGameover)
                {
                    Piece grabbedPieceExtracted = GameManager.instance.getPieceWithPieceID(piece.pieceId);
                    if (grabbedPieceExtracted == null) { return; }
                    grabbedPiece = grabbedPieceExtracted;

                    grabbedPiece.Clicked();
                    isGrabbed.Value = GrabbedType.GRABBED_CLIENT;
                    offset = grabbedPiece.transform.position - GetMouseWorldPosition(); // move thisone outside

                    UpdateHiglightedClientRpc(GameManager.instance.GetHiglightedStatus());
                }
            }

        }
        else
        {
            // Release the grabbed GameObject
            grabbedPiece.DeClicked();
            grabbedPiece = null;
            isGrabbed.Value = GrabbedType.NOT_GRABBED;

            UpdateClientCellsAfterDeclickedClientRpc(GameManager.instance.GetBlockedPositions(), GameManager.instance.GetBlockedPositions1());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendServerMessageForMovementServerRpc(float xPosition, float zPosition)
    {
        //Debug.Log(xPosition + " " + zPosition);
        if (grabbedPiece != null)
        {
            grabbedPiece.transform.position = new Vector3(xPosition, grabbedPiece.transform.position.y, zPosition);
        }
    }

    //this is for server
    void PerformMovementAndLogic() {
        if (isGrabbed.Value == GrabbedType.NOT_GRABBED)
        {
            // Raycast to check if we hit a GameObject
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask))
            {
                Piece piece = hit.collider.gameObject.GetComponent<Piece>();
                if (piece != null && GameManager.instance.IsTeamsTurn(piece.pieceTeam) && !piece.isAlreadyMoved && !GameManager.instance.isGameover)
                {
                    grabbedPiece = piece;
                    grabbedPiece.Clicked();
                    isGrabbed.Value = GrabbedType.GRABBED_SERVER;
                    offset = grabbedPiece.transform.position - GetMouseWorldPosition();

                    UpdateHiglightedClientRpc(GameManager.instance.GetHiglightedStatus());
                }
            }

        }
        else
        {
            // Release the grabbed GameObject
            grabbedPiece.DeClicked();
            grabbedPiece = null;
            isGrabbed.Value = GrabbedType.NOT_GRABBED;

            UpdateClientCellsAfterDeclickedClientRpc(GameManager.instance.GetBlockedPositions(), GameManager.instance.GetBlockedPositions1());
        }
    }

    private void Move()
    {
        if (isGrabbed.Value == GrabbedType.GRABBED_SERVER) //for the movement
        {
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            grabbedPiece.transform.position = new Vector3(targetPosition.x, grabbedPiece.transform.position.y, targetPosition.z);
        }
    }

    [ClientRpc]
    private void UpdateHiglightedClientRpc(int [] cellsToUpdate)
    {
        //Debug.Log($"Received message from server: {message}");
        // Process the received message on the client
        GameManager.instance.SetHiglightedCells(cellsToUpdate);
    }

    [ClientRpc]
    private void UpdateClientCellsAfterDeclickedClientRpc(int [] blockerStatus, int [] blocker1status)
    {
        //Debug.Log($"Received message from server: {message}");
        // Process the received message on the client
        GameManager.instance.ResetHighlightedCells();
        GameManager.instance.ResetBlocked();
        GameManager.instance.SetBlockedPositions(blockerStatus);
        GameManager.instance.SetBlockedPositions1(blocker1status);
    }
}

public enum GrabbedType{
    NOT_GRABBED,
    GRABBED_SERVER,
    GRABBED_CLIENT
}
