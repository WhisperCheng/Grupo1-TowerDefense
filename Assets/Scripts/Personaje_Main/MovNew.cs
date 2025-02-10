using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovNew : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    int VelocityZHash;
    int VelocityXHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //aumentar performance
        VelocityZHash = Animator.StringToHash("VelZ");
        VelocityXHash = Animator.StringToHash("VelX");
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool backwardsPressed = Input.GetKey(KeyCode.S);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        velocityX = Mathf.Lerp(velocityX * (backwardsPressed ? -1 : 1),
            horizontal * currentMaxVelocity, Time.deltaTime * acceleration);
        velocityZ = Mathf.Lerp(velocityZ * (leftPressed ? -1 : 1),
            vertical * currentMaxVelocity, Time.deltaTime * acceleration);

        if (velocityX < 0.001f && velocityX > -0.001f)
        {
            velocityX = 0f;
        }

        if (velocityZ < 0.001f && velocityZ > -0.001f)
        {
            velocityZ = 0f;
        }

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }
}
