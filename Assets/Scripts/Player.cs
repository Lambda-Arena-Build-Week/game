using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Player : MonoBehaviour
{
    public int id;
    public bool isControlled = false;
    public float moveSpeed = 5.0f;
    public float turnSpeed = 10.0f;
    public string playerName;
    public int health = 100;
    public Color pantsColor = new Color(0.0f / 255.0f, 12.0f / 255.0f, 178.0f / 255.0f);
    public Color hairColor = new Color(95.0f / 255.0f, 2.0f / 255.0f, 2.0f / 255.0f);
    public Color shirtColor = new Color(217.0f / 255.0f, 39.0f / 255.0f, 0.0f / 255.0f);
    public Color shoesColor = new Color(101.0f / 255.0f, 17.0f / 255.0f, 6.0f / 255.0f);
    public Color skinColor = new Color(219.0f / 255.0f, 171.0f / 255.0f, 125.0f / 255.0f);
    public Animator animator;
    public RagdollController ragdollController;
    public bool dead = false;
    public Collider capsuleCollider;
    public bool isMenu = false;
    public GameObject model;
    public Rigidbody rigid;
    public GameObject weapon;
    public Weapon weaponScript;
    public Transform weaponMount;
    public Vector3 position;
    public Quaternion rotation;
    public float animSpeed = 0.0f;
    public float destroyTrigger = 30.0f;
    public bool isRagdoll = false;
    public Renderer rend;

    private Dictionary<string, Material> materials = new Dictionary<string, Material>();
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 resolution;
    private bool usedController = false;
    private Vector3 targetPos;
    private int floorLayer;
    private float destroyTimer = 0.0f;
    
    private void Start()
    {
        if (!isMenu)
        {
            this.weapon = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/Handgun"));
            this.weaponScript = this.weapon.GetComponent<Weapon>();
            this.weaponScript.playerId = this.id;
            this.weapon.transform.position = this.weaponMount.position;
            this.weapon.transform.rotation = this.weaponMount.rotation;
            this.weapon.transform.parent = this.weaponMount;
        }
     }

    // Get all the materials in the renderer for use in changing player's colors
    private void GetMaterials()
    {
        Renderer playerRenderer = this.model.GetComponent<Renderer>();
        if (this.materials.Count == 0)
        {
            Material[] mats =  playerRenderer.materials;
            char[] spearator = { ' ' };
            for (int i = 0; i < mats.Length; i++)
            {
                string[] nameKey = mats[i].name.Split(spearator);
                this.materials.Add(nameKey[0], mats[i]);
            }
        }
    }

    private void OnEnable()
    {
        this.floorLayer = LayerMask.GetMask("Floor");
        this.capsuleCollider.enabled = true;
        this.animator.enabled = true;
        this.animator.SetFloat("speed", 0.0f);
        this.animSpeed = 0.0f;
        this.ragdollController.TurnRagdollOff();
        this.GetMaterials();            

        this.ChangeColors();
    }

    // Sets player's colors
    public void ChangeColors()
    {
        this.materials["pants"].color = this.pantsColor;
        this.materials["hair"].color = this.hairColor;
        this.materials["shirt"].color = this.shirtColor;
        this.materials["shoes"].color = this.shoesColor;
        this.materials["skin"].color = this.skinColor;

        Message message = new Message();
        message.id = this.id;
        message.message = "setcolor";
        message.pantsColor = this.pantsColor;
        message.hairColor = this.hairColor;
        message.shirtColor = this.shirtColor;
        message.shoesColor = this.shoesColor;
        message.skinColor = this.skinColor;

        Multiplayer.instance.Send(JsonUtility.ToJson(message));
    }

    // Poll input and update player's movement vector
    private void DoMovement()
    {
        moveDirection = Vector3.zero;

        Vector2 stickMove = Vector2.zero;

        if (Gamepad.current != null)
            stickMove = Gamepad.current.leftStick.ReadValue();

        if (stickMove != Vector2.zero)
            this.moveDirection = new Vector3(stickMove.x, 0.0f, stickMove.y);
        else
        {
            if (Input.GetKey(KeyCode.A))
                this.moveDirection += Vector3.left;

            if (Input.GetKey(KeyCode.D))
                this.moveDirection += Vector3.right;

            if (Input.GetKey(KeyCode.W))
                this.moveDirection += Vector3.forward;

            if (Input.GetKey(KeyCode.S))
                this.moveDirection += Vector3.back;
        }
        this.moveDirection.Normalize();
    }

    // Poll input and update player's look direction
    private void DoLook()
    {
        Vector2 stickLook = Vector2.zero;

        if (Gamepad.current != null)
            stickLook = Gamepad.current.rightStick.ReadValue();

        if (stickLook == Vector2.zero && !usedController)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 200.0f, this.floorLayer))
            {
                Vector3 currentPos = this.rigid.transform.position;
                Vector3 hitPoint = hit.point;
                hitPoint.y = currentPos.y;

                targetPos = Vector3.Normalize(hitPoint - currentPos) + this.rigid.position;
            }
        }
        else
        {
            usedController = true;
            targetPos = this.rigid.position + new Vector3(stickLook.x, 0.0f, stickLook.y);
        }
    }

    // Poll input and fire weapon
    private void DoWeapon()
    {
        if (this.weaponScript)
        {
            bool gamepadFire = false;
            if (Gamepad.current != null)
                gamepadFire = Gamepad.current.rightTrigger.isPressed;

            if (gamepadFire || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
                this.weaponScript.Fire(true);
            else
                this.weaponScript.Fire(false);
        }
    }

    // Kills player. And creates ragdoll physics object
    public void KillPlayer()
    {
        this.animSpeed = 0.0f;
        this.animator.SetFloat("speed", 0.0f);
        this.dead = true;
        this.capsuleCollider.enabled = false;
        this.weaponScript.Fire(false);
        this.health = 0;

        GameObject ragdoll = Instantiate(this.gameObject);
        Player script = ragdoll.GetComponent<Player>();
        script.isRagdoll = true;
        
        Vector3 pos = script.rigid.position;
        pos.y += 0.1f;
        script.rigid.position = pos;
        script.capsuleCollider.enabled = false;
        script.health = 0;
        script.animator.enabled = false;
        this.rigid.MovePosition(new Vector3(-10000.0f, -10000.0f, -10000.0f));
    }

    public void Spawn(Vector3 pos)
    {
        this.rigid.MovePosition(pos);
        this.rigid.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        this.health = 100;
        this.capsuleCollider.enabled = true;
        this.dead = false;
        this.animator.enabled = true;
        this.ragdollController.TurnRagdollOff();
    }

    public void Update()
    {

        if (this.isRagdoll)
        {
            this.ragdollController.TurnRagdollOn();

            this.destroyTimer += Time.deltaTime;

            if (this.destroyTimer >= this.destroyTrigger)
                Destroy(this.gameObject);

            return;
        }
        if (!this.dead && !this.isMenu && this.isControlled)
        {
            this.DoMovement();
            this.DoLook();
            this.DoWeapon();
        }

        if (this.isControlled)
            Health.instance.health = Mathf.Clamp(this.health, 0, 100);
    }

    // Applies players movement and look direction
    private void ApplyMovement()
    {
        this.resolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        this.resolution *= 0.5f;

        this.rigid.MovePosition(this.rigid.position + ((moveDirection * moveSpeed) * Time.deltaTime));

        this.rigid.transform.LookAt(targetPos);
    }

    // Applies animation based on player movement
    private void ApplyAnimation()
    {
        if (this.moveDirection != Vector3.zero)
        {
            this.animSpeed = 1.0f;
            this.animator.SetFloat("speed", 1.0f);
        }
        else
        {
            this.animSpeed = 0.0f;
            this.animator.SetFloat("speed", 0.0f);
        }
    }

    // Clears rigid body velocites since they are not needed
    private void ClearVelocities()
    {
        this.rigid.velocity = Vector3.zero;
        this.rigid.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (this.isRagdoll)
            return;

        if (!this.dead && !this.isMenu && this.isControlled)
        {
            this.ApplyMovement();
            this.ApplyAnimation();
        }

        this.ClearVelocities();

        this.position = this.rigid.position;
        this.rotation = this.rigid.rotation;
    }

    public void SwitchWeapon(string gun)
    {
        if (this.weapon)
            Destroy(this.weapon);

        this.weapon = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + gun));
        this.weaponScript = this.weapon.GetComponent<Weapon>();
        this.weaponScript.playerId = this.id;
        this.weapon.transform.position = this.weaponMount.position;
        this.weapon.transform.rotation = this.weaponMount.rotation;
        this.weapon.transform.parent = this.weaponMount;

        Message message = new Message();
        message.id = this.id;
        message.message = "switchweapon";
        message.weapon = gun;
        Multiplayer.instance.Send(JsonUtility.ToJson(message));
    }

    public void OnTriggerEnter(Collider trigger)
    {
        string tag = trigger.gameObject.tag;

        if (tag.Equals("HealthPack") && this.health < 100)
        {
            this.health = 100;
            trigger.gameObject.SetActive(false);
        }
        else
        if (tag.Equals("Shotgun") && !this.weapon.tag.Equals("Shotgun"))
        {
            this.health = 100;
            trigger.gameObject.SetActive(false);
            this.SwitchWeapon("Shotgun");
        }
        else
        if (tag.Equals("Rifle") && !this.weapon.tag.Equals("Rifle"))
        {
            this.health = 100;
            trigger.gameObject.SetActive(false);
            this.SwitchWeapon("Rifle");
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.DisableProjectile();

            if (this.isControlled)
            { 
                this.health -= projectile.damageDone;

                if (this.health <= 0 && !this.dead)
                {
                    Message message = new Message();
                    message.id = this.id;
                    message.message = "killplayer";
                    message.targetid = this.id;
                    message.shooterid = projectile.playerId;
                    this.dead = true;
                    Multiplayer.instance.Send(JsonUtility.ToJson(message));      
                }
            }
        }
    }
}
