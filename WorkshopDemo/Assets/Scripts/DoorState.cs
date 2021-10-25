using UnityEngine;

public class DoorState : MonoBehaviour
{
    void OnEnable()
    {
        Close();
    }

    void OnDisable() 
    {
        Open();
    }
    
    public void Open()
    {
        gameObject.SetActive(false);

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    void Close()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}
