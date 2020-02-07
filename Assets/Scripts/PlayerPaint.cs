using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaint : MonoBehaviour
{
    [SerializeField]
    private bool canPaint;
    
    [SerializeField]
    private LayerMask canvasLayerMask;

    [SerializeField]
    GameObject paintObject;

    [SerializeField]
    GameObject eraseObject;

    [SerializeField]
    Transform paintParent;
    // Start is called before the first frame update
    private Vector3 lastPos;
    void Start()
    {
        lastPos = transform.position;   
    }

    // Update is called once per frame
    void Update()
    {
        CheckCanPaint();
        CheckInput();
    }

    void CheckCanPaint()
    {
        Vector3 boxScale = transform.localScale/2.0f;
        boxScale.z = 3f;
        canPaint = (Physics.OverlapBox(transform.position,boxScale,transform.rotation,canvasLayerMask).Length != 0); 
    }

    void CheckInput()
    {
        if(Input.GetKey(KeyCode.LeftShift) && canPaint )
        {
            TryPaint();
        }
    }

    void TryPaint()
    {
        if( !PaintManager.instance.CanPaint )
            return;

        Vector3 deltaPos = transform.position - lastPos;
            lastPos = transform.position;

            if ((deltaPos != Vector3.zero) && !PaintManager.instance.EraseMode)
            {
                GameObject obj = Instantiate(paintObject,new Vector3(transform.position.x,transform.position.y,2.0f),transform.rotation);
                obj.transform.parent = paintParent;
            } 
            else {
                GameObject obj = Instantiate(eraseObject,new Vector3(transform.position.x,transform.position.y,2.0f),transform.rotation);
                obj.transform.parent = paintParent;
            }
        PaintManager.instance.Paint();
    }
}
