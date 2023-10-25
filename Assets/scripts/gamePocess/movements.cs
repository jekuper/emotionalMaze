using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class movements : MonoBehaviour {
    public Joystick joystick;
    //public fieldOfView fov;
    
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI transition;
    public TipsTrigger tipsTrigger;

    public UnityEngine.Rendering.Universal.Light2D lightPoint;
    
    public float speed = 3f;
    public float lightTimerInit = 5f;
    public float dummyTimerInit = 25f;
    public float deathTimeInit = 10f;
    public float lightDecriment = 0.8f;
    public float lightMinRadius = 2.3f;
    public float bulletDecriment = 3f;
    private float lightTimer;
    private float dummyTimer;
    private float deathTimer;
    private float lightRadiusInit;
    
    public GameObject key, arrow, ultimateButton, winnerParticle;
    
    public Transform whiteLightEffect;
    public Transform eye;
    
    public cameraShake cam;
    
    public secondMovements second;
    
    public int found = 0;
    public int curStage = 1;

    public SpriteRenderer background;
    public SpriteRenderer line;

    public Sprite newBg;
    public Sprite newLine;


    private Rigidbody2D rb;

    private DistanceJoint2D dj;

    private mazeManager maze;

    private bool isInsideCamp = false;
    private bool isAnimating = false;

    private void Awake() {
        Globals.player = gameObject;
        lightTimer = lightTimerInit;
        deathTimer = deathTimeInit;
        lightRadiusInit = lightPoint.pointLightOuterRadius;
        if (Globals.mainSettings.starSearchHelp == false)
            dummyTimerInit = -1;
        dummyTimer = dummyTimerInit;
    }
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        dj = GetComponent<DistanceJoint2D>();
        maze = GetComponent<mazeManager>();
        second.enabled = false;
        Globals.audioManager.FadeIn("firstStageTheme", 2);
    }

    void FixedUpdate() {
        if (!isAnimating)
        {
            Vector2 move = Vector2.zero;
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                move = new Vector2(joystick.Horizontal, joystick.Vertical);
            }
            else
            {
                move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            rb.MovePosition((Vector2)transform.position + move * Time.deltaTime * speed);
            eye.localPosition = Vector2.MoveTowards(eye.localPosition, move.normalized / 3, 2f * Time.deltaTime);
        }
    }
    private void Update() {
        
        if (isAnimating)
            return;
        if (curStage == 1)
        {
            if (found != maze.starCount) {
                if (dummyTimer >= 0 && dummyTimer <= Time.deltaTime) {
                    int res = 0;
                    GameObject[] stars = GameObject.FindGameObjectsWithTag("star");
                    for(int i = 0; i < stars.Length; i++) {
                        Vector2Int ind1 = Node.getIndexes(stars[i].transform.position);
                        Vector2Int ind2 = Node.getIndexes(stars[res].transform.position);
                        if (maze.mx[ind1.x][ind1.y] < maze.mx[ind2.x][ind2.y]) {
                            res = i;
                        }
                    }
                    Vector2Int ind = Node.getIndexes(stars[res].transform.position);
                    if ((maze.mx[ind.x][ind.y] <= dj.distance)) {
                        arrow.SetActive(true);
                        arrow.GetComponent<arrowManager>().target = stars[res].transform;
                        arrow.GetComponent<arrowManager>().isStartTarget = true;
                    }
                    dummyTimer = -1;
                }
                else if (dummyTimer > Time.deltaTime) {
                    dummyTimer -= Time.deltaTime;
                }
            }


            if (isInsideCamp) {
                lightPoint.pointLightOuterRadius = Mathf.Min(lightRadiusInit, lightPoint.pointLightOuterRadius + lightDecriment * 5 * Time.deltaTime);
                deathTimer = deathTimeInit;
            }
            else {
                if (lightTimer >= Time.deltaTime) {
                    lightTimer -= Time.deltaTime;
                } else {
                    if (lightPoint.pointLightOuterRadius <= lightMinRadius + 0.1f) {
                        deathTimer -= Time.deltaTime;
                    }
                    if (deathTimer <= 0) {
                        StartCoroutine(Die());
                    }
                    lightPoint.pointLightOuterRadius = Mathf.Max(lightPoint.pointLightOuterRadius - lightDecriment * Time.deltaTime, lightMinRadius);
                    if (lightPoint.pointLightOuterRadius < lightRadiusInit / 2) {
                        tipsTrigger.Trigger ("lightShrinks");
                    }
                }
            }
        }
        else
        {
            if (maze.enemiesCount + maze.enemiesCount / 2 == found)
            {
                StartCoroutine(Win());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "star") {
            tipsTrigger.Trigger ("starGot");
            dj.distance += Globals.distIncrement;
            lightTimerInit += 2.5f;
            deathTimeInit += 2.5f;
            found++;
            Destroy(collision.gameObject);
            UpdateScore();
            arrow.SetActive(false);
            if (found == maze.starCount) {
                key.SetActive(true);
                ultimateButton.SetActive(true);
                if (Globals.mainSettings.keySearchHelp) {
                    arrow.SetActive(true);
                    arrow.GetComponent<arrowManager>().isStartTarget = false;
                    arrow.GetComponent<arrowManager>().target = key.transform;
                }
                else {
                    arrow.SetActive(false);
                }
            }
            dummyTimer = dummyTimerInit;
        }
        if (collision.tag == "camp") {
            tipsTrigger.Trigger ("campFound");
            lightTimer = lightTimerInit;
            deathTimer = deathTimeInit;
            isInsideCamp = true;
        }
        if(collision.tag == "key") {
            Destroy(collision.gameObject);
            StartCoroutine(StartSecondLevel());
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "camp") {
            lightTimer = lightTimerInit;
            isInsideCamp = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (isAnimating)
            return;
        if(curStage == 1)
        {
            if (collision.transform.tag == "bullet") {
                tipsTrigger.Trigger ("gotHit");
                lightPoint.pointLightOuterRadius = Mathf.Max(lightPoint.pointLightOuterRadius - bulletDecriment, lightMinRadius);
                StartCoroutine(cam.Shake(0.07f, 0.1f));
                Destroy(collision.gameObject);
            }
            if (collision.transform.tag == "enemy") {
                lightPoint.pointLightOuterRadius = Mathf.Max(lightPoint.pointLightOuterRadius - bulletDecriment, lightMinRadius);
                StartCoroutine(cam.Shake(0.07f, 0.1f));
            }
        }
        else
        {
            if (collision.transform.tag == "bullet")
            {
                StartCoroutine(Die());
            }
        }
    }
    public void UpdateScore() {
        if (curStage == 1)
        {
            scoreUI.text = "Found: " + found + "/" + maze.starCount;
        }
        else
        {
            scoreUI.text = "Destroyed: " + found + "/" + (maze.enemiesCount + maze.enemiesCount / 2);
        }
    }

    public IEnumerator Die()
    {
        if (isAnimating)
            yield break;
        isAnimating = true;
        if (ultimateButton != null && ultimateButton.activeSelf)
            StartCoroutine(ultimateButton.GetComponent<ultimateButton>().Dissapear());
        if (curStage == 1)
        {
            Globals.audioManager.FadeOut("firstStageTheme", 3);
            yield return StartCoroutine(cEffector.resizeLight2(lightPoint, 0, 3f));
            Destroy(joystick.gameObject);
            Globals.GoToScene("gameOver");
            yield return new WaitForSeconds(3f);
        }
        else
        {
            Globals.audioManager.Stop("secondStageTheme");
            Globals.GoToScene("gameOver");
        }
    }
    public IEnumerator Win()
    {
        isAnimating = true;
        yield return new WaitForSeconds(1f);
        Instantiate(winnerParticle, transform);
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(cEffector.SpriteScale2(whiteLightEffect, 6.5f, new Vector3(80, 80, 1)));
        isAnimating = false;
        string encrypted = JsonConvert.SerializeObject(new Score(Globals.mainSettings));
        fileManager.SaveId("scores", Encription.AESEncryption(encrypted), true);
        Globals.GoToScene("winnerScreen");
    }

    public void ultimate() {
        maze.spawnCamp(transform.position);
    }
    public IEnumerator StartSecondLevel()
    {
        if (isAnimating)
            yield break;
        isAnimating = true;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("enemy");
        List<Vector2> enemiesPos = new List<Vector2>();
        foreach (var enemy in objects)
        {
            enemiesPos.Add(enemy.transform.position);
            Destroy(enemy);
        }
        objects = GameObject.FindGameObjectsWithTag("bullet");
        foreach (var bullet in objects)
        {
            Destroy(bullet);
        }
        objects = GameObject.FindGameObjectsWithTag("camp");
        foreach (var camp in objects)
        {
            Destroy(camp);
        }
        curStage = 2;

        if (ultimateButton != null)
            StartCoroutine(ultimateButton.GetComponent<ultimateButton>().Dissapear());

        arrow.SetActive(false);
        Globals.audioManager.FadeOut("firstStageTheme", 2);
        yield return StartCoroutine(cEffector.resizeLight2(lightPoint, 0, 2));
        background.sprite = newBg;
        line.sprite = newLine;
        objects = GameObject.FindGameObjectsWithTag("wall");
        foreach (var wall in objects){
            if (wall.transform.localScale.x == maze.mazeSize.x * mazeManager.cellSize ||
                wall.transform.localScale.y == maze.mazeSize.x * mazeManager.cellSize)
                continue;
            Destroy(wall);
        }

        yield return new WaitForSeconds(1);
        yield return StartCoroutine(cEffector.UiOpacity2(transition, 3, 1));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Globals.audioManager.FadeInC("secondStageTheme", 1));

        dj.distance = 5;
        found = 0;
        second.enabled = true;
        UpdateScore();
        transform.position = new Vector3(second.transform.position.x - 5, second.transform.position.y, transform.position.z);
        foreach (var pos in enemiesPos)
        {
            targetingEnemy te;
            GameObject enemy2 = maze.SpawnTargetingEnemy(pos, 2);
            te = enemy2.GetComponent<targetingEnemy>();
            te.target = transform;
            te.movingSpeed = Random.Range(1, 3);
            te.timeoutInit = Random.Range(1.5f, 2);
        }
        isAnimating = false;
        StartCoroutine(cEffector.UiOpacity2(transition, 1, 0));
        yield return StartCoroutine(cEffector.resizeLight2(lightPoint, 50, 3, true));
    }
}
