using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    [SerializeField] Transform[] checkPoints;
    [SerializeField] int reachedCheckPointNo = 0;
    [SerializeField] int UNDOcheckPointNo = 0;
    [SerializeField] Transform startPoint;
    [SerializeField] EndPoint[] allEndPoint;
    [SerializeField] GameObject BlockExplodeParticle;
    EndPoint selectedEndPoint;
    public Transform target;
    public float speed = 5f;
    [SerializeField] bool move = false;
    public PlayerType playerType;
    Color startColor;
    public Color EndColor;
    bool failed = false;


    bool canCollide = true;
    bool canUNDO = false;
    bool inputWork = true;

    private void Start()
    {
        startPoint.transform.position = transform.position;
        startColor = gameObject.GetComponent<SpriteRenderer>().color;
        UNDOcheckPointNo = checkPoints.Length;
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

    private void OnMouseDown()
    {
        if(inputWork && !failed)
        {
            if (canCollide)
            {
                if (!canUNDO)
                    CheckForNewCheckPoint();
            }
            else
            {
                if (canUNDO)
                {
                    ReverseNewCheckPoint();
                    if (selectedEndPoint != null)
                        selectedEndPoint.isFilled = false;
                    if (LevelManager.instance != null)
                        LevelManager.instance.noOfCheckReached--;
                    if(UiManager.instance != null)
                        UiManager.instance.UpdatePowerUpCount(1);
                }
                if ((UiManager.instance != null))
                    UiManager.instance.DeactivateUNDOUI();
            }

        }
    }

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
    }

    public void ReverseOn()
    {
        canCollide = false;
    }

    public void ReverseOff()
    {
        canCollide = true;
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

                if (BlockExplodeParticle != null)
                {
                    ParticleSystem _paricle = Instantiate(BlockExplodeParticle, collision.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule _main = _paricle.main;
                    _main.startColor = GetComponent<SpriteRenderer>().color;
                }
                Destroy(collision.gameObject);
            }

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
