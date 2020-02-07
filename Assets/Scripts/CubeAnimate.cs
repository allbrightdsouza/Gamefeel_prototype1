using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimate : MonoBehaviour
{
    [SerializeField]
    private float contractTime;

    [SerializeField]
    private float expandTime;

    private float lerpStartTime;
    bool startContractLerp;
    bool startExpandLerp;

    [SerializeField]
    private float squishRangeMin = 11f;
    [SerializeField]
    private float squishRangeMax = 24f;

    private Vector3 finalScale;
    private Vector3 startScale;
    
    public void CubeSquish(float val)
    {
        val = -val;
        val = Mathf.Clamp(val,squishRangeMin,squishRangeMax);

        float percent =  (val - squishRangeMin) / (squishRangeMax - squishRangeMin);
        Debug.Log(val + " percent "+ percent );
        
        startContractLerp = true;
        lerpStartTime = Time.time;
        finalScale = startScale = transform.localScale;
        finalScale.y = 1.0f - percent;
        finalScale.x += percent/2.0f;
    }

    void Update()
    {
        if(startContractLerp)
        {
            float timeSinceStart = Time.time - lerpStartTime;
            float percent = timeSinceStart/contractTime;
            transform.localScale = Vector3.Lerp(startScale,finalScale,percent);

            if(percent >= 1.0f)
            {
                StartExpandLerp();
            }
        }

        if(startExpandLerp)
        {
            float timeSinceStart = Time.time - lerpStartTime;
            float percent = timeSinceStart/expandTime;
            transform.localScale = Vector3.Lerp(startScale,finalScale,percent);

            if(percent >= 1.0f)
            {
                startExpandLerp = false;
            }
        }
    }

    void StartExpandLerp()
    {
        startContractLerp = false;
        startExpandLerp = true;
        lerpStartTime = Time.time;
        startScale = transform.localScale;
        finalScale = new Vector3(1.0f,1.0f,1.0f);
    }
}
