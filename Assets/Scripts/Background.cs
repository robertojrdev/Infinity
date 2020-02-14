using System.Collections;
using UnityEngine;
using static Easing;

public class Background : MonoBehaviour
{
    public float transitionTime = .7f;
    public Ease easingFunction = Ease.Linear;
    public Gradient[] colors; 

    private int index = 0;
    private MeshRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    [ContextMenu("Update Color")]
    public void UpdateColor()
    {
        if(colors.Length == 0)
        {
            Debug.LogError("No colors");
            return;
        }

        if(colors[index].colorKeys.Length < 2)
        {
            Debug.LogError("Gradients must have two color keys at least");
            return;
        }

        var a = colors[index].colorKeys[0].color;
        var b = colors[index].colorKeys[1].color;
        StopAllCoroutines();
        StartCoroutine(UpdateColorRoutine(a, b, transitionTime));
        index = (index +1) % colors.Length;
    }

    private IEnumerator UpdateColorRoutine(Color a, Color b, float time)
    {
        var initA =  renderer.material.GetColor("_Color");
        var initB =  renderer.material.GetColor("_Color2");

        var easeFunc = GetEasingFunction(easingFunction);

        var counter = 0f;
        while (counter <= time)
        {
            var t = Mathf.InverseLerp(0, time, counter);
            t = easeFunc(0, 1, t);
            renderer.material.SetColor("_Color", Color.Lerp(initA, a, t));
            renderer.material.SetColor("_Color2", Color.Lerp(initB, b, t));

            counter += Time.deltaTime;
            yield return null;
        }
    }
}