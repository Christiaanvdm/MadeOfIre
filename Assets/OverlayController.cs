using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class OverlayController : MonoBehaviour
{

    private RawImage overlayImage;
    // Start is called before the first frame update
    void Start()
    {
        overlayImage = transform.Find("Canvas").Find("OverlayImage").GetComponent<RawImage>();
        StartCoroutine(hideOverlayCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator hideOverlayCoroutine() {
        float alpha = overlayImage.color.a;
        while (alpha > 0)
        {
            alpha -= hidingRate * Time.deltaTime;
            yield return new WaitForFixedUpdate();
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
        }
        yield return null;
    }

    IEnumerator showOverlayCoroutine()
    {
        float alpha = overlayImage.color.a;
        while (alpha < 1)
        {
            alpha += hidingRate * Time.deltaTime * 5;
            yield return new WaitForFixedUpdate();
            overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, alpha);
        }
        yield return null;
    }

    public void showOverlay() {
        StartCoroutine(showOverlayCoroutine());
    }

    public void hideOverlay()
    {
        StartCoroutine(hideOverlayCoroutine());
    }


    private float hidingRate = 1f;
}
