using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField]
    private float maxAmount = 100f;

    [SerializeField]
    private float curAmount;

    [SerializeField]
    private float amountDrop = 10f;

    [SerializeField]

    private float eraseAmountDrop = 10f;

    [SerializeField]
    private Transform colorPivot;

    [SerializeField]
    private MeshRenderer paintRenderer;
    private bool eraseMode  = false;
    public bool EraseMode {get {return eraseMode;} set {eraseMode = value;}}
    public bool CanPaint {get;set;}
    public static PaintManager instance;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        instance = this;
        curAmount = maxAmount;
        CanPaint = true;
    }

    public void Paint()
    {
        if(!eraseMode) 
        {
            curAmount -= amountDrop;
            if(curAmount <= 0)
            {
                curAmount = 0f;
                eraseMode = true;
                StartCoroutine(PaintResetDelay());
            }
        } 
        else {
            curAmount += eraseAmountDrop;
            if(curAmount >= 100.0f)
            {
                curAmount = 100.0f;
                eraseMode = false;
                StartCoroutine(PaintResetDelay());
            }
        }

        float percent = curAmount/maxAmount;
        colorPivot.localScale = new Vector3(colorPivot.localScale.x,percent,colorPivot.localScale.x);
    }
    IEnumerator PaintResetDelay()
    {
        CanPaint = false;
        yield return new WaitForSeconds(1f);
        CanPaint = true;
    }
}
