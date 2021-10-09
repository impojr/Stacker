using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Block : MonoBehaviour
{
    private Material _material;
    private Image _uiImage;
    private Color _originalGlowColor;
    private Color _originalMainColor; 

    void Awake()
    {
        _uiImage = GetComponent<Image>();
        _material = new Material(_uiImage.material);
        _originalGlowColor = _material.GetColor("_GlowColor");
        _originalMainColor = _material.color;
        ResetMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        #if DEBUG
           if (Input.GetKeyDown(KeyCode.LeftArrow))
           {
               GlowAndFade();
           }
           else if (Input.GetKeyDown(KeyCode.RightArrow))
           {
               Shine();
           }
           else if (Input.GetKeyDown(KeyCode.DownArrow))
           {
               ResetMaterial();
           }
        #endif
    }

    public void Shine()
    {
        _material.DOFloat(1f, "_ShineLocation", 0.5f);
    }

    public void GlowAndFade()
    {
        // we want this to be roughly 1 second to match the sound
        _material.DOFloat(1f, "_HitEffectBlend", 0.1f).OnComplete(() =>
        {
            _material.DOFloat(0f, "_HitEffectBlend", 0.9f);
        });
        //Sequence mySequence = DOTween.Sequence();
        //mySequence.Append(_material.DOColor(Color.red, "_GlowColor", 0.2f)).OnStart(() =>
        //{
        //    _material.color = Color.red;
        //    _material.DOFloat(1f, "_Glow", 0.2f);
        //}).OnComplete(() =>
        //{
        //    _material.DOFloat(1f, "_FadeAmount", 0.8f);
        //});
    }

    public void ResetMaterial()
    {
        _material.SetColor("_GlowColor", _originalGlowColor);
        _material.SetFloat("_Glow", 0);
        _material.SetFloat("_ShineLocation", 0);
        _material.SetFloat("_FadeAmount", 0);
        _material.SetFloat("_HitEffectBlend", 0);
        _material.color = _originalMainColor;
        _uiImage.material = _material;
    }
}
