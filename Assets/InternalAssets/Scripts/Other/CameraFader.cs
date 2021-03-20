using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFader : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera camera2Fade;
    [SerializeField] private float timeToFade = 1; // Время
    [SerializeField] private RawImage camera2RenderImage;
    private RenderTexture Camera2Render_t;

    private void Awake()
    {
        Camera2Render_t = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, (int)mainCamera.depth);
        camera2Fade.targetTexture = Camera2Render_t;
        camera2RenderImage.texture = Camera2Render_t;
        camera2RenderImage.enabled = true;
    }

    private void Update()
    {
        if (Camera2Render_t.width != mainCamera.pixelWidth || Camera2Render_t.height != mainCamera.pixelHeight)
        {
            Camera2Render_t = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, (int)mainCamera.depth);
            camera2Fade.targetTexture = Camera2Render_t;
            camera2RenderImage.texture = Camera2Render_t;
        }
    }

    public IEnumerator _IFade()
    {
        Color c = camera2RenderImage.color;
        while (c.a > 0)
        {
            c = camera2RenderImage.color;
            c.a -= Time.deltaTime / timeToFade;
            camera2RenderImage.color = c;
            yield return null;
        }
        camera2RenderImage.color = Color.clear;
        yield return new WaitForSeconds(timeToFade);
    }

}
