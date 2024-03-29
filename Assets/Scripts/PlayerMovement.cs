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

    public float xySpeed = 2.0f;

    public MultiAudioClip collisionAudio;
    public AudioClip deathAudio;

    public GameObject DeathExplosion;

    private float recoverySeconds = 0.0f;
    private float timeToRecovery = 1.0f;
    
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
        
        if(recoverySeconds > 0.0f){
            recoverySeconds -= Time.deltaTime; //this isn't machine depedant, deltaTime is time between frames
            if(recoverySeconds < 0.0f){
                hidePlayer(false);
            } else {
                int flickerScaledNumber = (int)(recoverySeconds * 10.0f); //tuning //10ths of a second
                hidePlayer(flickerScaledNumber % 2 == 1);
            }
        } 
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
        CheckCollision(other);
    }

    public bool CheckCollision(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if(recoverySeconds > 0.0f){
                return false;
            }
            playerHealth--;

            if (playerHealth == 2)
            {
                //renderer.material = yellowMaterial;
                renderer.material.SetColor("_Color", Color.yellow);
                //^ changing the material might have slight advantages over chaging the material's property
            }
            else if (playerHealth == 1)
            {
                //renderer.material = redMaterial;
                renderer.material.SetColor("_Color", Color.red);
            }
            else if (playerHealth == 0)
            {
                //SceneManager.LoadScene("GameOver");
                StartCoroutine(WaitThenGameOver());
                //^ set the above to a timer 3s
                //set planeSpeed to 0 (will stop game movement)
                //death animation
                //
                
            }

            if(playerHealth > 0){
                recoverySeconds = timeToRecovery;
                audioSource.PlayOneShot(collisionAudio);
            } else {
                audioSource.PlayOneShot(deathAudio);
            }

            //SceneManager.LoadScene("GameOver");
            //transform.localPosition -= new Vector3(0, 0, 5);
            damageOffset = 5.0f; //max severity
            //audioSource.PlayOneShot(collisionAudio);   
            return true;
        }
        return false;
    }

    /*
    void OnTriggerEnter(Collision collision)
    {
        //Debug.Log(collision);
    }
    */

    public void hidePlayer(bool toHide){
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(toHide == false);
        }        
    }

    IEnumerator WaitThenGameOver(){
        planeMovement pmScript = gameObject.GetComponentInParent<planeMovement>();
        pmScript.enabled = false;
        hidePlayer(true);
        GameObject.Instantiate(DeathExplosion, transform.position, Quaternion.identity); //keeps the explosion upright
        yield return new WaitForSeconds(3.0f);
        //Debug.Log("gameover");
        //add the if level1, else level2 script over here
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Level 1"){
            Debug.Log("game over reached");
            SceneManager.LoadScene("GameOverLevel1");
        } else if (currentScene.name == "Level 2"){
            SceneManager.LoadScene("GameOverLevel2");
        }
        //SceneManager.LoadScene("GameOver");
    }

    void LocalMove(float x, float y, float speed)
    {
        
        //this takes HV input, h, then v
        //transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime;
        Vector3 movement = new Vector3(x, v, 0f);
        transform.position = Vector3.Lerp(transform.position, transform.position + movement * speed, Time.deltaTime * 10f);
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

        /*
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.2f, 0.8f);
        pos.y = Mathf.Clamp(pos.y, 0.2f, 0.8f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
        */
        
        
        Vector3 pos = transform.position; //have to make a copy first
        pos.x = Mathf.Clamp(pos.x, leftBoundary.position.x, rightBoundary.position.x);
        pos.y = Mathf.Clamp(pos.y, bottomBoundary.position.y, topBoundary.position.y);
        transform.position = pos;
        
        
        

    }


    
}
