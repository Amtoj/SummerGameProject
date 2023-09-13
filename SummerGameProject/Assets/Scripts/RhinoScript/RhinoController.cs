using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RhinoController : MonoBehaviour
{
    [Serializable] public class RhinoAction
    {
        [SerializeField] public string name;

        [SerializeField] public float radius;
        [HideInInspector] public bool ready;

        [SerializeField] public float coolDown;
        [HideInInspector] public bool isOnCoolDown;

        [SerializeField] public Color color;
    }
    [SerializeField] List<RhinoAction> rhinoActions;

    /*
    rhinoActions[0] is the "agro state"
    rhinoActions[1] is the "body slam attack"
    rhinoActions[2] is the "charge attack"
     */

    Animator animator;
    Rigidbody body;

    Transform player;

    bool isFacingPlayer = true;//This is to toggle the facing player functionality depending on whether the rhino is alive or defeated.

    Vector3 groundedNormal;

    [SerializeField] float movementSpeed;
    [SerializeField] float health = 10;

    private bool isDead = false;//If the enemy is dead boolean
    private float deathAnimationDuration = 2.0f;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();

        player = FindObjectOfType<GameManager>().sceneObjects.player.transform;
    }

    private void Update()
    {
        //For each action, check if the Rhino is in range of the player
        foreach (RhinoAction action in rhinoActions)
            action.ready = Vector3.Distance(transform.position, player.position) <= action.radius;

        //If the Rhino is in range of the player, check if the Rhino is on cooldown, and then perform the action
        //If the player is in the agro range, perform the agro state actions
        if (rhinoActions[0].ready) AgroState();

        animator.SetBool("Agro", rhinoActions[0].ready);

        //If the Body Slam attack is not in cooldown and the player is in range, Body Slam them
        if (!rhinoActions[1].isOnCoolDown && rhinoActions[1].ready) StartCoroutine(BodySlam());

        //If the Body Slam is out of range of the player, but the charge attack is ready and not on cooldown, CHARGE!
        if (!rhinoActions[2].isOnCoolDown && rhinoActions[2].ready && !rhinoActions[1].ready) StartCoroutine(ChargeAttack());

        //If charge attack is finished after the player reaches a certain border, then there's a horn sweep attack!
        if (!rhinoActions[3].isOnCoolDown && rhinoActions[3].ready && !rhinoActions[2].ready) StartCoroutine(HornSweep());
    }

    private void AgroState()
    {
        if (isFacingPlayer)
        {
            //Face towards the player immediately when triggered
            Vector3 direction = transform.position - player.position;
            direction.y = 0f; //Keep enemy's rotation level
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);

            //Check if the Rhino is too close to the player, and if not, move them towards the player
            bool isTooCloseToPlayer = Vector3.Distance(transform.position, player.position) <= rhinoActions[1].radius / 1.5f;
            if (!isTooCloseToPlayer) body.velocity = -transform.forward * movementSpeed + new Vector3(0, body.velocity.y, 0);
        }
    }

    private IEnumerator BodySlam()
    {
        //Set cooldown
        rhinoActions[1].isOnCoolDown = true;

        //Set animation
        animator.SetTrigger("BodySlam");
        Debug.Log("Body Slam!");

        movementSpeed = 10;

        //Count Down seconds until the action can be used again
        yield return new WaitForSeconds(rhinoActions[1].coolDown);

        //Reset the action
        rhinoActions[1].isOnCoolDown = false;
    }

    private IEnumerator ChargeAttack()
    {
        //Set cooldown
        rhinoActions[2].isOnCoolDown = true;

        //Set animation
        animator.SetTrigger("ChargeAttack");
        Debug.Log("Charge Attack!");

        //Count Down seconds until the action can be used again
        yield return new WaitForSeconds(rhinoActions[2].coolDown);

        //Reset the action
        rhinoActions[2].isOnCoolDown = false;
    }

    private IEnumerator HornSweep()
    {
        //Set cooldown
        rhinoActions[3].isOnCoolDown = true;

        //Set animation
        animator.SetTrigger("HornSweep");
        Debug.Log("Horn Sweep!");

        //Count down seconds until the action can be used again
        yield return new WaitForSeconds(rhinoActions[3].coolDown);

        //Reset the action
        rhinoActions[3].isOnCoolDown = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        //Check if the collision is with the ground, and collect the ground normal
        if (collision.collider.CompareTag("Ground")) groundedNormal = collision.contacts[0].normal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }

        if (other.gameObject.CompareTag("Weapon"))//If the rhino is hit by the player's weapon
        {
            health -= 1;
            if (health <= 0)
            {
                health = 0;
                animator.SetBool("isDead", true);
                StartCoroutine(FreezeAfterDeath());
            }
        }
    }

    private IEnumerator FreezeAfterDeath()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        movementSpeed = 0;
        isFacingPlayer = false;
        isDead = true;
    }

    private void OnDrawGizmos()
    {
        //For each action, draw a sphere around the enemy to show the radius of the action
        foreach (RhinoAction action in rhinoActions)
        {
            Gizmos.color = action.color;
            Gizmos.DrawWireSphere(transform.position, action.radius);
        }

        //Draw the cardinal directions of the enemy
        //This is for those who can't see the rhino model at times...
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.right * 5);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 5);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + -transform.forward * 5);

    }
}
