/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Leap.Unity;
using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This simple script changes the color of an InteractionBehaviour as
/// a function of its distance to the palm of the closest hand that is
/// hovering nearby.
/// </summary>
[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]
public class InteractionZoom : MonoBehaviour {

  [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
  public bool useHover = true;

  [Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
  public bool usePrimaryHover = false;

  [Header("InteractionBehaviour Colors")]
  public Color defaultColor = Color.Lerp(Color.black, Color.white, 0.1F);
  public Color suspendedColor = Color.red;
  public Color hoverColor = Color.Lerp(Color.black, Color.white, 0.7F);
  public Color primaryHoverColor = Color.Lerp(Color.black, Color.white, 0.8F);

  [Header("InteractionButton Colors")]
  [Tooltip("This color only applies if the object is an InteractionButton or InteractionSlider.")]
  public Color pressedColor = Color.white;

  private SpriteRenderer _renderer;

  private InteractionBehaviour _intObj;


    private InteractionSlider _sliObj;


    public Transform ring;

    public AudioSource buzz;


    public AudioSource[] boops;
    int cursor = 0;


    private String _previous = "";

    void Start() {
    _intObj = GetComponent<InteractionBehaviour>();
        _sliObj = GetComponent<InteractionSlider>();

        _renderer  = GetComponentInChildren<SpriteRenderer>();

  }

  void Update() {

      // The target color for the Interaction object will be determined by various simple state checks.
      Color targetColor = defaultColor;

        float distance = 0f;

      // "Primary hover" is a special kind of hover state that an InteractionBehaviour can
      // only have if an InteractionHand's thumb, index, or middle finger is closer to it
      // than any other interaction object.
      if (_intObj.isPrimaryHovered && usePrimaryHover) {
        targetColor = primaryHoverColor;
      }
      else {
        // Of course, any number of objects can be hovered by any number of InteractionHands.
        // InteractionBehaviour provides an API for accessing various interaction-related
        // state information such as the closest hand that is hovering nearby, if the object
        // is hovered at all.
        if (_intObj.isHovered && useHover) {
          float glow = _intObj.closestHoveringControllerDistance.Map(0F, 0.2F, 1F, 0.0F);
                distance = glow;
                targetColor = Color.Lerp(defaultColor, hoverColor, glow);
        }
      }

        float scale = Mathf.Min(Mathf.Lerp(0.013f, 0.050f, distance), 0.024f);
        ring.localScale = new Vector3(scale, scale, scale);


        buzz.volume = Mathf.Min(Mathf.Lerp(0, 0.25f, distance), 0.15f);

        if (_intObj.isSuspended) {
        // If the object is held by only one hand and that holding hand stops tracking, the
        // object is "suspended." InteractionBehaviour provides suspension callbacks if you'd
        // like the object to, for example, disappear, when the object is suspended.
        // Alternatively you can check "isSuspended" at any time.
        targetColor = suspendedColor;
      }

      // We can also check the depressed-or-not-depressed state of InteractionButton objects
      // and assign them a unique color in that case.
      if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed) {
        targetColor = pressedColor;
      }

        // Lerp actual material color to the target color.
        _renderer.color = Color.Lerp(_renderer.color, targetColor, 30F * Time.deltaTime);
    
  }

    public void step()
    {
        
         String currentRot = (_sliObj.VerticalSliderValue * 2f).ToString("F1");
        if (currentRot != _previous)
        {
            boops[cursor].pitch = Mathf.Lerp(0.8f, 1.1f, _sliObj.VerticalSliderValue);
            boops[cursor].Play();
            cursor = (cursor + 1) % boops.Length;
            _previous = currentRot;
        }
    }

}
