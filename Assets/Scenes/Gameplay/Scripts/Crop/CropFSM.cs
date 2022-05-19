using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropFSM : MonoBehaviour
{
    [SerializeField] private float timeFirstStage;
    [SerializeField] private float timeSecondStage;
    [SerializeField] private float timeThirdStage;
    [SerializeField] private float timeFourthStage;
    [SerializeField] private float timeFiveStage;
    [SerializeField] private CropSlot cropSlot;
    private enum CropState
    {
        first,
        second,
        third,
        fourth,
        five
    };
    private float timerCropFSM;
    private bool isStarted;
    void Start()
    {
        timerCropFSM = 0.0f;
        cropSlot.ActivatedSlot += SetCycle;
    }

    private void OnDisable()
    {
        cropSlot.ActivatedSlot -= SetCycle;
    }

    void Update()
    {
        StartCylce();
    }

    private void SetCycle(bool state)
    {
        isStarted = state;
    }

    private void StartCylce()
    {
        if (isStarted)
        {
            timerCropFSM += Time.deltaTime;
            if (timerCropFSM >= timeFirstStage)
            {
                cropSlot.NextStage(1);
                if(timerCropFSM >= timeSecondStage)
                {
                    cropSlot.NextStage(2);
                }
            }

        }
    }

}
