using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables
    public Vector2 rotationSpeed;
    public Player player;
    public Transform cameraTF;
    public float maxRotation;
    public float minRotation;
    public float cameraZoomSpeed;
    #endregion Variables

    void Start()
    {
        cameraTF = transform.GetChild(0);
    }

    void Update()
    {
        transform.position = player.transform.position;
      
	    //Left or Right Mouse
	    if (Input.GetMouseButton(0)||Input.GetMouseButton(1))
        {
            transform.eulerAngles += new Vector3 (0,Mathf.Clamp (Input.GetAxis("Mouse X"),-.5f,.5f) * rotationSpeed.x,0);
            transform.eulerAngles += new Vector3 (Mathf.Clamp (Input.GetAxis("Mouse Y"),-.5f,.5f) * rotationSpeed.y,0,0);
            transform.eulerAngles = new Vector3 (Mathf.Clamp (transform.eulerAngles.x, minRotation, maxRotation),transform.eulerAngles.y,transform.eulerAngles.z);

            if(Input.GetMouseButton(1) && ! player.IsDead)
            { 
                player.transform.eulerAngles = new Vector3 (0,cameraTF.eulerAngles.y,0);
            }
        }

        //Zoom
        cameraTF.localPosition += new Vector3 (0,0,Input.mouseScrollDelta.y * cameraZoomSpeed);
        cameraTF.localPosition = new Vector3 (0,0,Mathf.Clamp(cameraTF.localPosition.z,-100,0));
    }
}