using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedHolder : MonoBehaviour
{
    [SerializeField, Tooltip("Flag specifying the condition: Real or Virt.")]
    public string realCondition = "Virt";
    [SerializeField, Tooltip("Flag specifying if the calibration target is displayed.\nNote that, if not displayed, calibration doesn't work.")]
    private bool _cal = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCondition()
    {
        realCondition = "Real";
    }

    public string getCondition()
    {
        return realCondition;
    }

    public void setCalibrationTargetVisibility(bool input)
    {
        _cal = input;
    }

    public bool getCalibrationTargetVisibility()
    {
        return _cal;
    }
}
