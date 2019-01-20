using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour {

    public GameObject infoObject;
    public GameObject controlsObject;
    public StepManager stepManager;

    public GameObject lineObject;
    public GameObject platformObject;
    public LineRenderer bottomLineObject;


    public GameObject turntableObject;
    public GameObject BodyObject;
    public GameObject headAnchorObject;
    public GameObject titleObject;


    public GameObject skinObject;
    public GameObject skeletonObject;
    public GameObject lesionsObject;



    Vector3 titlePosition;

    float targetScale = 1f;

    float targetDistance = -2.032f;

    bool selected = false;

    float targetAngle = 0f;



    float targetSkin = 1.5f;
    float targetSkeleton = 2f;

    float targetLesion = 1.5f;

    float currentSkin = 1.5f;
    float currentSkeleton = 2f;
    float currentLesion = 1.5f;

    Renderer _bodyRenderer;
    Renderer _skeletonRenderer;
    Renderer _lesionRenderer;


    public List<Renderer> _rings;
    public List<LineRenderer> _lines;
    public List<MiniMarker> _lesions;

    public Color lesionColor;

    string target = "_progress";
    string targetColor = "_MainColor";

    bool hasSelection = false;

    public LineRenderer lesionLine;
    MiniMarker selectedMarker;

    public SpriteRenderer _lesionInfo;


    public InteractionSlider _lesionSlider;

    public InteractionColor _button;
    public Color selectedColor;


    public List<Lesion> _lesionObjects;

    public SpriteRenderer _dicom;
    public SpriteRenderer _bodyBack;
    public SpriteRenderer _info;

    bool bodyShown = true;

    bool dicomShown = false;

    public SpriteRenderer _dicomTextSprite;
    public Sprite _showDicomSprite;
    public Sprite _hideDicomSprite;

    // Use this for initialization
    void Start () {
        _bodyRenderer = skinObject.GetComponent<Renderer>();
        _skeletonRenderer = skeletonObject.GetComponent<Renderer>();
        _lesionRenderer = lesionsObject.GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {

        if (selected)
        {
            targetAngle = turntableObject.transform.localRotation.eulerAngles.y;
        } 
        Vector3 angle = new Vector3(0, Mathf.LerpAngle(BodyObject.transform.localRotation.eulerAngles.y, targetAngle, Time.deltaTime * 4f), 0);

        if (!selected)
        {
            turntableObject.transform.localRotation = Quaternion.Euler(angle);
        }

        BodyObject.transform.localRotation = Quaternion.Euler(angle);

        titlePosition = headAnchorObject.transform.position;
        titlePosition.y += 0.3f;
        titleObject.transform.position = titlePosition;

        // body scale
        float scale = Mathf.Lerp(BodyObject.transform.localScale.x, targetScale, Time.deltaTime * 4f);

        BodyObject.transform.localScale = new Vector3(scale, scale, scale);

        // platform move
        float position = Mathf.Lerp(platformObject.transform.localPosition.z, targetDistance , Time.deltaTime * 4f);

        platformObject.transform.localPosition = new Vector3(0, -(scale - 1), position);

        bottomLineObject.SetPosition(1, new Vector3(0, -(scale - 1), position + 0.032f));

        // skin stuff

        currentSkin = Mathf.Lerp(currentSkin, targetSkin, Time.deltaTime);
        currentSkeleton = Mathf.Lerp(currentSkeleton, targetSkeleton, Time.deltaTime);
        currentLesion = Mathf.Lerp(currentLesion, targetLesion, Time.deltaTime);

        lesionColor.a = currentLesion;

        _bodyRenderer.material.SetFloat(target, currentSkin);
        _skeletonRenderer.material.SetFloat(target, currentSkeleton);
       // _lesionRenderer.material.color = lesionColor;

        var cola = Mathf.Clamp01(currentLesion + 1.5f) + .07f;
        var col = new Vector4(cola, cola, cola, 1);
        foreach (var ring in _rings) {
            ring.material.color = col;
        }

        var cola2 = Mathf.Clamp01(currentLesion + 2f) + .07f;
        var col2 = new Vector4(cola2, cola2, cola2, 1);

        foreach (var line in _lines)
        {
            line.material.color = col2;
        }

        if(selectedMarker != null){
            lesionLine.SetPosition(0, lesionLine.transform.InverseTransformPoint(selectedMarker.location.position));
        }

    }
    public void Show()
    {
        //lineObject.SetActive(true);
        //platformObject.SetActive(true);
        selected = true;

        targetSkin = 1.5f;
        targetSkeleton = 2f;

        targetLesion = 1.5f;

        _button.defaultColor = selectedColor;

    }
    public void ShowInfo()
    {
        //infoObject.SetActive(true);
        controlsObject.SetActive(true);
        //lineObject.SetActive(true);
        //platformObject.SetActive(true);

        BodyObject.SetActive(true);

        targetDistance = -1.032f;
    }

    public void Hide()
    {
        controlsObject.SetActive(false);
        //lineObject.SetActive(false);
        //platformObject.SetActive(false);

        //BodyObject.SetActive(false);

        targetDistance = -2.032f;

        targetScale = 1f;
        selected = false;

        targetAngle = 0f;


        targetSkin = -1f;
        targetSkeleton = -.5f;
        targetLesion = -3f;
        hasSelection = false;
        SelectLesion(-1);
        infoObject.SetActive(false);
        selectedMarker = null;

        _lesionSlider.VerticalSliderPercent = 0;

        _button.defaultColor = Color.white;
    }

    public void Reset()
    {
        controlsObject.SetActive(false);
        //lineObject.SetActive(false);
        //platformObject.SetActive(false);

        //BodyObject.SetActive(false);

        targetDistance = -2.032f;

        targetScale = 1f;
        selected = false;

        targetAngle = 0f;


        targetSkin = 1.5f;
        targetSkeleton = 2f;
        targetLesion = 1.5f;
        hasSelection = false;
        SelectLesion(-1);
        infoObject.SetActive(false);
        selectedMarker = null;

        _lesionSlider.VerticalSliderPercent = 0;

        _button.defaultColor = Color.white;
        _dicom.gameObject.SetActive(false);

        bodyShown = true;
        dicomShown = false;
        _info.gameObject.SetActive(true);
        lesionLine.gameObject.SetActive(true);
    }


    public void Select()
    {
        if (selected)
        {
            stepManager.ResetSteps();
            return;
        }
        stepManager.HideSteps(this);
        Show();
        ShowInfo();
    }

    public void SetZoom(float e)
    {
        /*float position = - 1f - 3f * e;

        platformObject.transform.localPosition = new Vector3(0, 0, position - 0.032f);

        bottomLineObject.SetPosition(1, new Vector3(0, 0, position));*/

        targetScale = 1f + e * 1.5f;

    }

    public void SelectLesion(int index)
    {
        foreach(var lesion in _lesions)
        {
            if (lesion.index != index) {
                lesion.selected = false;
                _lesionObjects[lesion.index].gameObject.SetActive(false);
            } else
            {
                selectedMarker = lesion;
                _lesionObjects[lesion.index].gameObject.SetActive(true);

                _lesionObjects[lesion.index].transform.localPosition = Vector3.zero;
                _lesionObjects[lesion.index].transform.localRotation = Quaternion.identity;

            }
        }
        if(selectedMarker != null)
        {
            _lesionInfo.sprite = selectedMarker.sprite;
            hasSelection = true;
            infoObject.SetActive(true);
            _dicom.sprite = selectedMarker.dicom;
        }
    }

    public void ShowDicom()
    {
        if(dicomShown)
        {
            dicomShown = false;
            _dicom.gameObject.SetActive(false);
            _dicomTextSprite.sprite = _showDicomSprite;
            _info.gameObject.SetActive(true);
            lesionLine.gameObject.SetActive(true);
        } else
        {
            dicomShown = true;
            _dicom.gameObject.SetActive(true);
            _dicomTextSprite.sprite = _hideDicomSprite;
            _info.gameObject.SetActive(false);
            lesionLine.gameObject.SetActive(false);
        }
    }

    public void ToggleBody()
    {
        _dicom.gameObject.SetActive(true);
        bodyShown = !bodyShown;
        _bodyBack.flipX = bodyShown;

        if (bodyShown)
        {

            targetSkin = 1.5f;
            targetSkeleton = 2f;

        }
        else { 
            targetSkin = -1f;
            targetSkeleton = -.5f;
        }
    }
}
