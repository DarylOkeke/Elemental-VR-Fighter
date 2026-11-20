using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

// local movement and health test fixture, not the shared arena
public class PracticeArena : MonoBehaviour
{
    public Health health;
    public DashController dash;
    public Transform rig;
    public Camera view;

    [Header("PC movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float headHeight = 1.65f;
    [SerializeField] float turnSpeed = 65f;
    [Header("Practice dummy")]
    [SerializeField] float hitDamage = 25f;
    [SerializeField] float warningTime = 1.2f;
    [SerializeField] float shotSpeed = 6f;
    [SerializeField] bool showHurtbox = true;

    CapsuleCollider bodyCollider;
    Hurtbox bodyHurtbox;
    Camera observer;
    Renderer dummy;
    Transform shot;
    LineRenderer outline;
    bool attacking;
    float hitUntil;
    float previousHealth;
    string hitText;
    string dummyStatus = "Ready";

    void Start()
    {
        Application.runInBackground = true;
        var tracking = view.GetComponent<TrackedPoseDriver>();
        if (tracking != null) tracking.enabled = false;
        view.transform.localPosition = new Vector3(0, 1.65f, 0);
        view.transform.localRotation = Quaternion.identity;
        view.enabled = false;
        var oldHud = view.transform.Find("Health HUD");
        if (oldHud != null) oldHud.gameObject.SetActive(false);

        bodyCollider = rig.GetComponentInChildren<CapsuleCollider>();
        bodyHurtbox = bodyCollider.GetComponent<Hurtbox>();
        previousHealth = health.CurrentHealth;
        health.OnHealthChanged.AddListener(OnHealthChanged);

        observer = new GameObject("Practice observer").AddComponent<Camera>();
        observer.transform.position = new Vector3(6, 5, -7);
        observer.transform.LookAt(new Vector3(0, 1, 0));
        observer.clearFlags = CameraClearFlags.SolidColor;
        observer.backgroundColor = new Color(.025f, .04f, .07f);
        observer.fieldOfView = 48;
        RenderSettings.ambientLight = new Color(.55f, .6f, .7f);
        CreateBlock("Test floor", new Vector3(0, -.12f, 0), new Vector3(6, .2f, 11), new Color(.09f, .14f, .19f));
        for (int z = -4; z <= 4; z++)
            CreateBlock("Distance mark", new Vector3(0, .005f, z), new Vector3(4, .015f, .035f), new Color(.2f, .38f, .44f));

        var dummyObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dummyObject.name = "Practice dummy";
        dummyObject.transform.position = new Vector3(0, 1, 4);
        dummy = dummyObject.GetComponent<Renderer>();
        dummy.material = MakeMaterial(new Color(.95f, .48f, .14f));
        Destroy(dummyObject.GetComponent<Collider>());

        var shotObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shotObject.name = "Practice shot";
        shotObject.transform.localScale = Vector3.one * .18f;
        shotObject.GetComponent<Renderer>().material = MakeMaterial(new Color(1, .65f, .2f));
        Destroy(shotObject.GetComponent<Collider>());
        shot = shotObject.transform;
        shot.gameObject.SetActive(false);

        outline = new GameObject("Body hurtbox outline").AddComponent<LineRenderer>();
        outline.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        outline.material.color = new Color(.2f, .95f, .85f);
        outline.startWidth = .025f;
        outline.endWidth = .025f;
        outline.positionCount = 33;
    }

    void Update()
    {
        // stand in for headset tracking during the desktop test
        view.transform.position = rig.position + Vector3.up * headHeight;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.rKey.wasPressedThisFrame) Restart();
        if (keyboard.hKey.wasPressedThisFrame && !attacking) StartCoroutine(FireShot());
        if (keyboard.bKey.wasPressedThisFrame) showHurtbox = !showHurtbox;
        if (dash.IsDashing) return;
        float movement = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
        Vector3 forward = Vector3.ProjectOnPlane(view.transform.forward, Vector3.up).normalized;
        rig.position += forward * movement * moveSpeed * Time.deltaTime;
        float turn = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
        view.transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime, Space.World);
    }

    void LateUpdate()
    {
        outline.enabled = showHurtbox;
        // draw the capsule's actual radius and height in the observer-facing plane
        Vector3 center = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float radius = bodyCollider.radius;
        float straight = bodyCollider.height * .5f - radius;
        Vector3 right = observer.transform.right;
        for (int i = 0; i <= 32; i++)
        {
            float angle = i * Mathf.PI * 2 / 32;
            float y = Mathf.Sin(angle);
            Vector3 point = right * (Mathf.Cos(angle) * radius);
            point += Vector3.up * (y * radius + (y >= 0 ? straight : -straight));
            outline.SetPosition(i, center + point);
        }
    }

    public void Restart()
    {
        StopAllCoroutines();
        attacking = false;
        shot.gameObject.SetActive(false);
        dummyStatus = "Ready";
        dummy.material.color = new Color(.95f, .48f, .14f);
        rig.position = new Vector3(0, 0, -3);
        view.transform.localRotation = Quaternion.identity;
        health.ResetHealth();
        hitUntil = 0;
    }

    IEnumerator FireShot()
    {
        attacking = true;
        dummyStatus = "Preparing hit";
        dummy.material.color = new Color(1, .75f, .3f);
        yield return new WaitForSeconds(warningTime);
        shot.position = dummy.transform.position;
        Vector3 direction = (bodyCollider.bounds.center - shot.position).normalized;
        shot.gameObject.SetActive(true);
        dummyStatus = "Shot fired";
        float distance = 0;
        while (distance < 12)
        {
            float step = shotSpeed * Time.deltaTime;
            // check the real hurtbox before advancing the test shot
            Physics.SyncTransforms();
            if (bodyCollider.Raycast(new Ray(shot.position, direction), out RaycastHit hit, step))
            {
                bodyHurtbox.ApplyHit(hitDamage, hit.point);
                dummyStatus = "Hit connected";
                break;
            }
            shot.position += direction * step;
            distance += step;
            yield return null;
        }
        shot.gameObject.SetActive(false);
        dummy.material.color = new Color(.95f, .48f, .14f);
        if (distance >= 12) dummyStatus = "Missed";
        attacking = false;
    }

    void OnHealthChanged()
    {
        float damage = previousHealth - health.CurrentHealth;
        if (damage > 0)
        {
            hitText = "HIT  -" + damage.ToString("0");
            hitUntil = Time.time + .6f;
        }
        previousHealth = health.CurrentHealth;
    }

    void OnGUI()
    {
        if (health == null) return;
        GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.width / 1280f));
        float height = Screen.height * 1280f / Screen.width;
        Panel(24, 24, 300, 116);
        Label(40, 34, "MOVEMENT + HEALTH", 16, new Color(.6f, .85f, .86f));
        Label(40, 63, "Health   " + health.CurrentHealth.ToString("0") + " / " + health.MaxHealth.ToString("0"), 25, Color.white);
        Bar(40, 108, 268, 10, health.CurrentHealth / health.MaxHealth, new Color(.18f, .85f, .65f));
        Panel(24, height - 100, 640, 76);
        if (GUI.Button(new Rect(700, height - 90, 140, 44), "Dash")) dash.TryDash();
        if (GUI.Button(new Rect(850, height - 90, 140, 44), "Test shot") && !attacking) StartCoroutine(FireShot());
        if (GUI.Button(new Rect(1000, height - 90, 110, 44), "Reset")) Restart();
        string dashStatus = dash.IsDashing ? "Dashing" : dash.CooldownRemaining > 0 ? "Dash ready in " + dash.CooldownRemaining.ToString("0.0") + "s" : "Dash ready";
        Label(40, height - 93, dashStatus + "    |    Dummy: " + dummyStatus, 18, Color.white);
        Label(40, height - 59, "SPACE dash   H test shot   W/S move   A/D turn   B hurtbox   R reset", 15, new Color(.6f, .85f, .86f));
        if (showHurtbox && bodyCollider != null)
        {
            Vector3 labelPoint = observer.WorldToScreenPoint(bodyCollider.bounds.center + Vector3.up * .8f);
            Label(labelPoint.x * 1280f / Screen.width - 55, (Screen.height - labelPoint.y) * 1280f / Screen.width, "BODY HURTBOX", 14, new Color(.3f, 1, .85f));
        }
        if (Time.time < hitUntil)
        {
            float fade = (hitUntil - Time.time) / .6f;
            GUI.color = new Color(1, .3f, .1f, fade * .35f);
            GUI.DrawTexture(new Rect(0, 0, 1280, 8), Texture2D.whiteTexture);
            GUI.color = Color.white;
            Label(550, 36, hitText, 24, new Color(1, .65f, .3f, fade));
        }
    }

    void Panel(float x, float y, float width, float height)
    {
        GUI.color = new Color(.025f, .045f, .07f, .95f);
        GUI.DrawTexture(new Rect(x, y, width, height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void Bar(float x, float y, float width, float height, float value, Color color)
    {
        GUI.color = new Color(.18f, .25f, .3f);
        GUI.DrawTexture(new Rect(x, y, width, height), Texture2D.whiteTexture);
        GUI.color = color;
        GUI.DrawTexture(new Rect(x, y, width * Mathf.Clamp01(value), height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void Label(float x, float y, string text, int size, Color color)
    {
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = size;
        style.normal.textColor = color;
        GUI.Label(new Rect(x, y, 900, 36), text, style);
    }

    Material MakeMaterial(Color color)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        return material;
    }

    void CreateBlock(string name, Vector3 position, Vector3 size, Color color)
    {
        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.SetParent(transform);
        block.transform.position = position;
        block.transform.localScale = size;
        block.GetComponent<Renderer>().material = MakeMaterial(color);
    }

    void OnDestroy()
    {
        if (health != null) health.OnHealthChanged.RemoveListener(OnHealthChanged);
    }
}


