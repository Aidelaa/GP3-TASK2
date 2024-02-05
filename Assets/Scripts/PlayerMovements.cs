using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float raycastDistance = 10f;
    [SerializeField] Transform headPos;

    PlayerInputs playerInputs;

    Vector2 runAction;

    bool isPickUp = false;
    bool isPutDown = false;

    [SerializeField] bool isHoldingAnimal = false;
    private GameObject heldAnimal;

    public TextMeshProUGUI pickUPTxt;
    public TextMeshProUGUI putDownTxt;

    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            playerInputs = new PlayerInputs();

            playerInputs.movementInputs.Move.performed += i => runAction = i.ReadValue<Vector2>();
            playerInputs.movementInputs.Move.canceled += i => runAction = Vector2.zero;

            playerInputs.Interact.pickUp.performed += i => isPickUp = true;
            playerInputs.Interact.pickUp.canceled += i => isPickUp = false;

            playerInputs.Interact.putDown.performed += i => isPutDown = true;
            playerInputs.Interact.putDown.canceled += i => isPutDown = false;
        }
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        if (playerInputs != null)
        {
            playerInputs.movementInputs.Move.performed -= i => runAction = i.ReadValue<Vector2>();

            playerInputs.Interact.pickUp.performed -= i => isPickUp = true;
            playerInputs.Interact.pickUp.canceled -= i => isPickUp = false;

            playerInputs.Interact.putDown.performed -= i => isPutDown = true;
            playerInputs.Interact.putDown.canceled -= i => isPutDown = false;
        }
        playerInputs.Disable();
    }

    private void Update()
    {
        MovePlayer();
        handlepickupandputdown();

        if (isHoldingAnimal)
        {
            heldAnimal.transform.position = headPos.position + Camera.main.transform.forward * 3.5f;
            Debug.Log("Currently holding: " + heldAnimal.name);
        }
    }

    void MovePlayer()
    {
        Vector3 movement = new Vector3(runAction.x, 0f, runAction.y);

        Vector3 moveDir = Camera.main.transform.forward * movement.z + Camera.main.transform.right * movement.x;
        moveDir.y = 0; 

        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
    }

    private void handlepickupandputdown()
    {
        Ray ray = new Ray(headPos.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            
            if (hit.collider.CompareTag("Fruit") && !isHoldingAnimal)
            {
                pickUPTxt.gameObject.SetActive(true);
                putDownTxt.gameObject.SetActive(false);

                if (isPickUp)
                {
                    Debug.Log("Animal picked up: " + hit.collider.gameObject.name);

                    isHoldingAnimal = true;
                    heldAnimal = hit.collider.gameObject;

                    heldAnimal.GetComponent<Rigidbody>().isKinematic = true;

                    pickUPTxt.gameObject.SetActive(false);
                    putDownTxt.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.green);
            pickUPTxt.gameObject.SetActive(false);
            putDownTxt.gameObject.SetActive(false);
        }

        if (isPutDown)
        {
            if (isHoldingAnimal)
            {
                isHoldingAnimal = false;
                heldAnimal.GetComponent<Rigidbody>().isKinematic = false;
                Debug.Log("Fruit put down");

                heldAnimal = null;

                pickUPTxt.gameObject.SetActive(false);
                putDownTxt.gameObject.SetActive(true);
            }
            else
            {
                putDownTxt.gameObject.SetActive(false);
                Debug.Log("Not holding an item.");
            }
        }
    }

}
