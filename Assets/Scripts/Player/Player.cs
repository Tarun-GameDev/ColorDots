using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using DG.Tweening;
using static UnityEngine.ParticleSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Transform[] checkPoints;
    int reachedCheckPointNo = 0;
    int UNDOcheckPointNo = 0;
    [SerializeField] Transform startPoint;
    [SerializeField] EndPoint[] allEndPoint;
    [SerializeField] GameObject BlockExplodeParticle;
    EndPoint selectedEndPoint;
    Transform target;
    public float speed = 5f;
    bool move = false;
    public PlayerType playerType;
    Color startColor;
    public Color EndColor;
    bool failed = false;


    bool inUndoMode = false;
    bool canCollide = true;
    bool canUNDO = false;
    bool inEraseMode = false;
    bool inPhaontomMode = false;
    bool usingPhantom = false;
    bool inputWork = true;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject phantomColliderObj;
    [SerializeField] GameObject eraseAnimObj;
    Collider2D collider2d;
    [SerializeField] GameObject lightiningEffect;
    GameObject lightingRef;
    [SerializeField] ParticleSystem[] allPartEff;

    private void Start()
    {
        startPoint.transform.position = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        UNDOcheckPointNo = checkPoints.Length;
        collider2d = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (transform.position == target.position)
            {
                move = false;
                if (!canUNDO)
                {
                    reachedCheckPointNo++;
                    CheckForNewCheckPoint();
                }
                else
                {
                    UNDOcheckPointNo--;
                    ReverseNewCheckPoint();
                }
            }
        }
    }

    #region Input

    private void OnMouseDown()
    {
        if (inputWork && !failed)
        {
            //Normal Movement
            if (canCollide)
            {
                if (!canUNDO)
                    CheckForNewCheckPoint();

                if (inPhaontomMode && !canUNDO) //Phantom Mode ON
                {
                    EnablePhantomEffect();
                    CheckForNewCheckPoint();
                    if ((UiManager.instance != null))
                        UiManager.instance.DeactivatePhantom();
                }
            }
            else if (inUndoMode) //Undo Mode ON
            {
                if (canUNDO)
                {
                    ReverseNewCheckPoint();
                    if (selectedEndPoint != null)
                        selectedEndPoint.isFilled = false;
                    if (LevelManager.instance != null)
                        LevelManager.instance.noOfCheckReached--;
                    if (UiManager.instance != null)
                        UiManager.instance.UpdatePowerUpCount(1);
                }
                if ((UiManager.instance != null))
                    UiManager.instance.DeactivateUNDOUI();
            }

            if (inEraseMode)//Erase Mode On 
            {
                inputWork = false;
                move = false;
                if (UiManager.instance != null)
                    UiManager.instance.updateEraseCount(1);
                eraseObj();
            }


        }
    }
    #endregion

    #region Normal Movement CheckPoints and EndpPoints

    void CheckForNewCheckPoint()
    {
        if (reachedCheckPointNo < checkPoints.Length + 1)
        {
            if (reachedCheckPointNo == checkPoints.Length)
            {
                if (allEndPoint.Length > 0)
                    CheckForEndPoint();
            }
            else
            {
                target = checkPoints[reachedCheckPointNo];
                move = true;
                inputWork = false;
            }
        }
        else
        {
            EndPointReached();
        }
    }
    void EndPointReached()
    {
        move = false;
        inputWork = true;
        canUNDO = true;
        if (selectedEndPoint != null)
            selectedEndPoint.EndPointReached();
        if (LevelManager.instance != null)
            LevelManager.instance.PlayerCheckPointReached();
        gameObject.GetComponent<SpriteRenderer>().color = EndColor;
        if (UiManager.instance != null)
            UiManager.instance.PlayHaptic();
        if (usingPhantom)
            DisablePhantomEffect();
        reachedCheckPointNo = 0;
    }

    void CheckForEndPoint()
    {
        for (int i = 0; i < allEndPoint.Length; i++)
        {
            if (allEndPoint[i].isFilled == false)
            {
                target = allEndPoint[i].transform;
                selectedEndPoint = allEndPoint[i];
                selectedEndPoint.isFilled = true;
                move = true;
                inputWork = false;
                UNDOcheckPointNo = checkPoints.Length;
                break;
            }
        }
    }
    #endregion


    #region UNDO Funtionality
    void ReverseNewCheckPoint()
    {
        if (UNDOcheckPointNo > -1)
        {
            if (UNDOcheckPointNo == 0)
            {
                if (startPoint != null)
                {
                    target = startPoint;
                    move = true;
                    inputWork = false;
                }
            }
            else
            {
                target = checkPoints[UNDOcheckPointNo - 1];
                move = true;
                inputWork = false;
            }
        }
        else
        {
            ReverseEndPointReached();
        }
    }

    void ReverseEndPointReached()
    {
        move = false;
        inputWork = true;
        canUNDO = false;
        gameObject.GetComponent<SpriteRenderer>().color = startColor;
        if (UiManager.instance != null)
            UiManager.instance.PlayHaptic();
        if (usingPhantom)
            DisablePhantomEffect();
    }

    public void ReverseOn()
    {
        inUndoMode = true;
        canCollide = false;
    }

    public void ReverseOff()
    {
        inUndoMode = true;
        canCollide = true;
    }

    #endregion


    #region Erase PowerUp
    public void EraseOn()
    {
        inEraseMode = true;
    }

    public void EraseOff()
    {
        inEraseMode = false;
    }

    void eraseObj()
    {
        GameObject obj = Instantiate(eraseAnimObj, transform.position, Quaternion.identity);
        if ((UiManager.instance != null))
            UiManager.instance.DeactivateErasePowerUp();
        spriteRenderer.DOFade(0, 1f).OnComplete(()=>
        {
            CheckForEndPoint();
            if (LevelManager.instance != null && !canUNDO)
                LevelManager.instance.PlayerCheckPointReached();
            Destroy(obj);
            if (target != null)
                Destroy(target.gameObject);
            Destroy(this.gameObject);
        });

    }
    #endregion

    #region Phantom Power Up

    public void PhantomOn()
    {
        inPhaontomMode = true;
    }

    public void PhantomOff()
    {
        inPhaontomMode = false;
    }

    void EnablePhantomEffect()
    {
        usingPhantom = true;
        collider2d.enabled = false;
        if (phantomColliderObj != null)
            phantomColliderObj.SetActive(true);
        if(lightiningEffect != null)
        {
            lightingRef = Instantiate(lightiningEffect,transform.position, Quaternion.identity,this.transform);
            allPartEff = lightingRef.transform.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem _eff in allPartEff)
            {
                ParticleSystem.MainModule _main01 = _eff.main;
                _main01.startColor = spriteRenderer.color;
            }

        }
        if (UiManager.instance != null)
            UiManager.instance.UpdatePhantomCount(1);
    }

    void DisablePhantomEffect()
    {
        usingPhantom = false;
        collider2d.enabled = true;
        if (phantomColliderObj != null)
            phantomColliderObj.SetActive(false);
        if (lightingRef != null)
            Destroy(lightingRef.gameObject);
    }
    #endregion

    #region BallShake
    public void StartShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;

    }
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (failed || !canCollide)
            return;

        if (collision.CompareTag("Player"))
        {
            LevelFailed();
        }
        else if(collision.CompareTag("Block"))
        {
            if (collision.GetComponent<Block>().PlayerType != playerType)
            {
                LevelFailed();
            }
            else
            {
                SpawnBlockExplodeParticel(collision);
                Destroy(collision.gameObject);
            }
        }    
    }

    public void SpawnBlockExplodeParticel(Collider2D _collision)
    {
        if (BlockExplodeParticle != null)
        {
            ParticleSystem _paricle = Instantiate(BlockExplodeParticle, _collision.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            ParticleSystem.MainModule _main = _paricle.main;
            _main.startColor = spriteRenderer.color;
        }
    }

    public void LevelFailed()
    {
        move = false;
        inputWork = false;
        failed = true;
        StartShake(.5f, .035f);
        if(UiManager.instance != null)
        {
            UiManager.instance.LevelFailed();
        } 
    }
}
