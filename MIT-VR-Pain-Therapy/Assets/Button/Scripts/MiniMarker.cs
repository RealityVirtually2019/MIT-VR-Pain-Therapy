using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMarker : MonoBehaviour {


    [SerializeField]
    private LeapProvider _provider;

    Renderer _renderer;

    public Color hoverColor;
    public Color selectedColor;

    public Step step;

    public int index;

    public bool selected = false;
    public Transform location;

    public Sprite sprite;


    public Sprite dicom;

    private void Awake()
    {
        if (this._provider == null)
        {
            this._provider = Hands.Provider;
        }
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (selected)
        {
            return;
        }
        float distance = 999f;
        foreach (Hand current in this._provider.CurrentFrame.Hands)
        {
            _renderer.material.color = Color.white;
            foreach (Finger current2 in current.Fingers)
            {
                Vector3 vector = current2.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
                Vector3 localPoint = base.transform.InverseTransformPoint(vector);
                distance = Mathf.Min(distance, localPoint.magnitude);
            }
        }

        if (distance < 0.02f / transform.localScale.x)
        {
            _renderer.material.color = selectedColor;
            selected = true;
            step.SelectLesion(index);

        } else
        {
            _renderer.material.color = Color.Lerp(hoverColor, Color.white, distance / 2f);
        }

    }
}
