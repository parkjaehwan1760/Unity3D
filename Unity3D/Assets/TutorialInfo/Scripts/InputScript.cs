using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // Assign a default value for clarity
    [SerializeField] Animator Anicon_Santa; // Renamed for consistency to Anicon_Santa

    // It's good practice to initialize components in Awake or Start
    void Awake()
    {
        // If Anicon_Santa is not assigned in the Inspector, try to get it from the GameObject
        if (Anicon_Santa == null)
        {
            Anicon_Santa = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // 1. Input Handling
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = new Vector3(moveX, 0, moveZ).normalized; // Use a more descriptive name

        // 2. Movement
        // Directly move the character's position
        transform.position += moveInput * moveSpeed * Time.deltaTime;

        // 3. Rotation
        // Only rotate if there's significant input to avoid snapping to a default direction
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            // Smoothly interpolate the rotation for a more natural turn
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 4. Animation Control
        // Set ISWALK based on whether there's any movement input
        bool isWalking = moveInput.magnitude > 0.01f; // Use a small threshold to avoid floating point issues
        if (Anicon_Santa != null) // Always check if the Animator is assigned
        {
            Anicon_Santa.SetBool("ISWALK", isWalking);

            // Handle attack input
            if (Input.GetKeyDown(KeyCode.C))
            {
                Anicon_Santa.SetTrigger("ISATTACK");
            }
            // IMPORTANT: Remove the duplicate Anicon_Santa.SetTrigger("ISATTACK"); here
            // This was likely causing the attack animation to trigger constantly.
        }
    }
}