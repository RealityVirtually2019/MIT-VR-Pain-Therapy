using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour {

    Vector3 rotation;

    // Use this for initialization
    void Start () {
        rotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
        rotation.y = Mathf.Sin(Time.time * 3.0f) * 6.0f;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
