using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
    public Texture2D CursorTexture;
    public Vector2 CursorHotspot = new Vector2(16, 16); // Set the hotspot to the center of the texture

    private GameObject treeItself;
    private GameObject treeIcon;
    private GameObject treeRings;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        treeItself = transform.Find("TreeItself").gameObject;
        treeIcon = transform.Find("TreeIcon").gameObject;
        treeRings = transform.Find("TreeRings").gameObject;

        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component if needed
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(CursorTexture, CursorHotspot, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Reset cursor to default
    }

    private void OnMouseDown()
    {
        treeItself.SetActive(false); // Toggle visibility of the tree itself
        treeIcon.SetActive(false); // Toggle visibility of the tree icon
        treeRings.SetActive(true); // Toggle visibility of the tree rings

        rb.detectCollisions = false; // Disable collision detection so that the player can walk through the tree
    }
}
