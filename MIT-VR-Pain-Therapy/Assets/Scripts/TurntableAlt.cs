

using Leap;
using Leap.Unity;
using Leap.Unity.Attributes;
using Leap.Unity.RuntimeGizmos;
using Leap.Unity.Swizzle;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurntableAlt : MonoBehaviour, IRuntimeGizmoComponent
{
    private struct FingerPointKey : IEquatable<TurntableAlt.FingerPointKey>
    {
        public int handId;

        public Finger.FingerType fingerType;

        public override bool Equals(object obj)
        {
            return obj is TurntableAlt.FingerPointKey && this.Equals((TurntableAlt.FingerPointKey)obj);
        }

        public bool Equals(TurntableAlt.FingerPointKey other)
        {
            return this.handId == other.handId && this.fingerType == other.fingerType;
        }
    }

    [SerializeField]
    private LeapProvider _provider;

    [SerializeField]
    private bool _drawGizmos;

    [Header("Turntable Shape"), SerializeField, Tooltip("The local height of the upper section of the turntable.")]
    private float _tableHeight;

    [MinValue(0f), SerializeField, Tooltip("The radius of the upper section of the turntable.")]
    private float _tableRadius;

    [MinValue(0f), SerializeField, Tooltip("The length of the edge that connects the upper and lower sections of the turntable.")]
    private float _edgeLength;

    [Range(0f, 90f), SerializeField, Tooltip("The angle the edge forms with the upper section of the turntable.")]
    private float _edgeAngle = 45f;

    [MinValue(0f), Header("Turntable Motion"), SerializeField, Tooltip("How much to scale the rotational motion by.  A value of 1 causes no extra scale.")]
    private float _rotationScale = 1.5f;

    [MinValue(1E-05f), SerializeField, Tooltip("How much to smooth the velocity while the user is touching the turntable.")]
    private float _rotationSmoothing = 0.1f;

    [Range(0f, 1f), SerializeField, Tooltip("The damping factor to use to damp the rotational velocity of the turntable.")]
    private float _rotationDamping = 0.95f;

    [MinValue(0f), SerializeField, Tooltip("The speed under which the turntable will stop completely.")]
    private float _minimumSpeed = 0.01f;

    private Dictionary<TurntableAlt.FingerPointKey, Vector3> _currTipPoints = new Dictionary<TurntableAlt.FingerPointKey, Vector3>();

    private Dictionary<TurntableAlt.FingerPointKey, Vector3> _prevTipPoints = new Dictionary<TurntableAlt.FingerPointKey, Vector3>();

    private SmoothedFloat _smoothedVelocity;

    private float _rotationalVelocity;


    public AudioSource[] boops;
    int cursor = 0;

    private String _previous = "";

    private float _lowerLevelHeight
    {
        get
        {
            return this._tableHeight - this._edgeLength * Mathf.Sin(this._edgeAngle * 0.0174532924f);
        }
    }

    private float _lowerLevelRadius
    {
        get
        {
            return this._tableRadius + this._edgeLength * Mathf.Cos(this._edgeAngle * 0.0174532924f);
        }
    }

    private void Awake()
    {
        if (this._provider == null)
        {
            this._provider = Hands.Provider;
        }
        this._smoothedVelocity = new SmoothedFloat();
        this._smoothedVelocity.delay = this._rotationSmoothing;
        
    }

    private void Update()
    {
        Utils.Swap<Dictionary<TurntableAlt.FingerPointKey, Vector3>>(ref this._currTipPoints, ref this._prevTipPoints);
        this._currTipPoints.Clear();
        foreach (Hand current in this._provider.CurrentFrame.Hands)
        {
            foreach (Finger current2 in current.Fingers)
            {
                TurntableAlt.FingerPointKey key = new TurntableAlt.FingerPointKey
                {
                    handId = current.Id,
                    fingerType = current2.Type
                };
                Vector3 vector = current2.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
                Vector3 localPoint = base.transform.InverseTransformPoint(vector);
                
                if (this.isPointInsideTurntable(localPoint))
                {
                    this._currTipPoints[key] = vector;

                }
            }
        }
        float num = 0f;
        float num2 = 0f;
        foreach (KeyValuePair<TurntableAlt.FingerPointKey, Vector3> current3 in this._currTipPoints)
        {
            Vector3 value = current3.Value;
            Vector3 position;
            if (!this._prevTipPoints.TryGetValue(current3.Key, out position))
            {
                return;
            }
            Vector3 vector2 = base.transform.InverseTransformPoint(value);
            Vector3 vector3 = base.transform.InverseTransformPoint(position);
            num += Vector2.SignedAngle(vector3.xz(), vector2.xz()) * this._rotationScale * -1f;
            num2 += 1f;
        }
        if (num2 > 0f)
        {
            float num3 = num / num2;
            Vector3 localEulerAngles = base.transform.localEulerAngles;
            localEulerAngles.y += num3;
            base.transform.localEulerAngles = localEulerAngles;
            this._smoothedVelocity.Update(num3 / Time.deltaTime, Time.deltaTime);
            this._rotationalVelocity = this._smoothedVelocity.value;
        }
        else
        {
            this._rotationalVelocity *= this._rotationDamping;
            if (Mathf.Abs(this._rotationalVelocity) < this._minimumSpeed)
            {
                this._rotationalVelocity = 0f;
            }
            Vector3 localEulerAngles2 = base.transform.localEulerAngles;
            localEulerAngles2.y += this._rotationalVelocity * Time.deltaTime;
            base.transform.localEulerAngles = localEulerAngles2;
        }
        String currentRot = (base.transform.localEulerAngles.y/36f * 2).ToString("F0");
        if (currentRot != _previous)
        {
            boops[cursor].Play();
            cursor = (cursor + 1) % boops.Length;
            _previous = currentRot;
        }
    }

    private bool isPointInsideTurntable(Vector3 localPoint)
    {
        if (localPoint.y > this._tableHeight)
        {
            return false;
        }
        float t = Mathf.Clamp01(Mathf.InverseLerp(this._tableHeight, this._lowerLevelHeight, localPoint.y));

        float num = Mathf.Lerp(this._tableRadius, this._lowerLevelRadius, t);
        
        Vector2 vector = new Vector2(localPoint.x, localPoint.z);
        float magnitude = vector.magnitude;

        return magnitude <= num && magnitude >= num - 0.05f;
    }

    public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer)
    {
        if (!this._drawGizmos)
        {
            return;
        }
        drawer.color = Color.blue;
        drawer.RelativeTo(base.transform);
        for (int i = 0; i < 16; i++)
        {
            float f = (float)i / 16f * 3.14159274f * 2f;
            Vector3 a = new Vector3(Mathf.Cos(f) * this._tableRadius, this._tableHeight, Mathf.Sin(f) * this._tableRadius);
            Vector3 b = new Vector3(Mathf.Cos(f) * this._lowerLevelRadius, this._lowerLevelHeight, Mathf.Sin(f) * this._lowerLevelRadius);
            drawer.DrawLine(a, b);
        }
    }
}