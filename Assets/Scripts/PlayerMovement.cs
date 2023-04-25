using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform topBoundary;
    public Transform rightBoundary;
    public Transform bottomBoundary;
    public Transform leftBoundary;
    public int playerHealth = 3;
    public Material yellowMaterial;
    public Material redMaterial;

    public Transform triangleHeadModel;
    private Renderer renderer;
    public Transform playerModel;
    private float damageOffset = 0.0f;

    public float xySpeed = 2;

    public MultiAudioClip collisionAudio;
    
    //i dont think yaw makes sense
    //if we are copying SF, the ship animates into a roll and pitch
    //but you're just holding down left right
    //not sure how that translates to physics

    private float pitch;
    private float yaw;

    private float v;
    private float h;

    // Start is called before the first frame update
    void Start()
    {
        //triangleHeadModel = transform.Find("triangle-head");
        renderer = triangleHeadModel.GetComponent<Renderer>();
        
    }

    Rigidbody rb;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void HandleInputs(){
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        
        HandleInputs();
        LocalMove(h, v, xySpeed);
        ClampPosition();
        //playerModel.localPosition = damageOffset * (-Vector3.forward);

    }

    void FixedUpdate(){
        damageOffset *= 0.9f; //note: non-linear needs a percentage
        //losing 10% each update
        //this requires fixed update
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log(playerHealth);
            playerHealth--;

            if (playerHealth == 2)
            {
                //renderer.material = yellowMaterial;
                renderer.material.SetColor("_Color", Color.yellow);
            }
            else if (playerHealth == 1)
            {
                //renderer.material = redMaterial;
                renderer.material.SetColor("_Color", Color.red);
            }
            else if (playerHealth == 0)
            {
                SceneManager.LoadScene("GameOver");
            }        
            //SceneManager.LoadScene("GameOver");
            //transform.localPosition -= new Vector3(0, 0, 5);
            damageOffset = 5.0f; //max severity
            audioSource.PlayOneShot(collisionAudio);
        }
    }

    /*
    void OnTriggerEnter(Collision collision)
    {
        //Debug.Log(collision);
    }
    */

    void LocalMove(float x, float y, float speed)
    {
        
        //this takes HV input, h, then v
        //transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        Vector3 movement = new Vector3(x, v, 0f);
        transform.position = Vector3.Lerp(transform.position, transform.position + movement * speed, Time.fixedDeltaTime * 10f);
        //^ this might do the trick is LocalMove is called every Update
    }

    /*void ClampPosition(){
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        //pos.z = Mathf.Clamp01(pos.z);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }*/

    void ClampPosition(){

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.2f, 0.8f);
        pos.y = Mathf.Clamp(pos.y, 0.2f, 0.8f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
        
        /*
        Vector3 pos = transform.position; //have to make a copy first
        pos.x = Mathf.Clamp(pos.x, leftBoundary.position.x, rightBoundary.position.x);
        pos.y = Mathf.Clamp(pos.y, topBoundary.position.y, bottomBoundary.position.y);
        transform.position = pos;
        */
        

    }


    
}
