using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesion : MonoBehaviour {

    float time = 0;

    private void OnEnable()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update () {
        if(Time.time - time < 2)
        {
            return;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);


        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);

    }

}
