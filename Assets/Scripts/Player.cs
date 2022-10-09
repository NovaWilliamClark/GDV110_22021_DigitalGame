using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    public float getSanity => sanity;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sanityLossRate = 0.01f;
    private bool isInLight = false;
    private float sanity = 100f;

    private void Start()
    {
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        Light.onLightEnter += Light_OnLightEnter; 
        Light.onLightExit += Light_OnLightExit;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontal, 0, 0);
        transform.position += movement * Time.deltaTime * movementSpeed;

        if (!isInLight)
        {
            sanity -= sanityLossRate;
        }

        if (sanity <= 1f)
        {
            sanity = 1f;
        }
    }

    private void Light_OnLightEnter(Collider2D collider)
    {
        isInLight = true; 
        Debug.Log("Light Entered");
    }

    private void Light_OnLightExit(Collider2D collider)
    {
        isInLight = false; 
        Debug.Log("Light Exited");
    }
    private void OnDestroy()
    {
        Light.onLightEnter -= Light_OnLightEnter;
        Light.onLightExit -= Light_OnLightExit;
    }
}