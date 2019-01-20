using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour {

    public List<Step> allSteps;

    bool reportOpen = false;

    public GameObject report;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HideSteps(Step step)
    {
        foreach (var aStep in allSteps)
        {
            if (step != aStep)
            {
                aStep.Hide();
            }
        }
        reportOpen = false;
        report.SetActive(false);
    }

    public void ResetSteps()
    {
        foreach (var aStep in allSteps)
        {
            aStep.Reset();
        }
        reportOpen = false;
        report.SetActive(false);
    }

    public void OpenReport()
    {
        if (reportOpen)
        {

            reportOpen = false;
            report.SetActive(false);
        } else
        {
            ResetSteps();
            reportOpen = true;
            report.SetActive(true);
        }
        
    }
}
