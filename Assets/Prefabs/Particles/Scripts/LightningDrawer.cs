using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool isReady = false;
    private Vector3 previousTransform;
    private Vector3 normal;
    private Vector3 binormal;
    private Vector3 distance;

    [SerializeField]
    // Time taken before next lightning is generated
    private float delay = 0.2f;
    [SerializeField]
    // Number of nodes
    private int size = 20;
    /* The offsets in the normal and binormal directions are calculated 
    using z_n = smoothness * z_(n-1) + a_n + damping * a_(n-1)
    where a_n = noiseFactor * Random.insideUnitCircle */
    [SerializeField]
    /* Values between -1 to 0 makes the bolt less chaotic; any other 
    values aggravates the effect of the previous noise value. */
    private float damping = 0f;
    [SerializeField]
    // Scales the effect of the random perturbations
    private float noiseFactor = 1f;
    [SerializeField]
    /* Values close to 0 makes the bolt look like random noise while 
    values close to 1 makes the bolt follow an arc more often. 
    Any values outside 0 to 1 creates funky behaviour. */
    private float smoothness = 1f;

    public Transform target;

    // Start is called before the first frame update
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = size;
        isReady = true;
    }

    private void Update()
    {
        if (target.position != previousTransform)
        {
            previousTransform = target.position;
            UpdateNormals();
        }
    }

    private void OnEnable()
    {
        if (target.position != previousTransform)
        {
            previousTransform = target.position;
            UpdateNormals();
        }
        StartCoroutine(HandleLightning());
        StartCoroutine(LightningGlow());
    }

    private void OnDisable()
    {
        StopCoroutine(HandleLightning());
        StopCoroutine(LightningGlow());
    }

    private void UpdateNormals()
    {
        distance = target.position - transform.position;
        Vector3 direction = distance.normalized;
        if (direction == Vector3.up || direction == -Vector3.up)
        {
            normal = Vector3.forward;
        }
        else
        {
            normal = Vector3.up;
        }
        Vector3.OrthoNormalize(ref direction, ref normal);
        if (direction == Vector3.right || direction == -Vector3.right)
        {
            binormal = Vector3.forward;
        }
        else
        {
            binormal = Vector3.right;
        }
        Vector3.OrthoNormalize(ref direction, ref binormal);
    }

    private Vector2[] GenerateNoise()
    {
        Vector2[] output = new Vector2[size];
        Vector2[] noises = new Vector2[size];
        output[0] = Vector2.zero;
        noises[0] = Vector2.zero;
        for (int i = 1; i < size - 1; i++)
        {
            noises[i] = Random.insideUnitCircle * noiseFactor;
            output[i] = smoothness * output[i - 1] + noises[i] + noises[i - 1] * damping;
        }
        output[size - 1] = Vector2.zero;
        return output;
    }

    // Update is called once per frame
    private IEnumerator HandleLightning()
    {
        while (true)
        {
            if (!enabled)
            {
                yield break;
            }
            if (!isReady)
            {
                yield return new WaitForEndOfFrame();
            }
            Vector2[] offsets = GenerateNoise();
            for (int i = 0; i < size; i++)
            {
                Vector3 finalPosition = transform.position;
                finalPosition += distance * i / (size - 1);
                finalPosition += normal * offsets[i].x;
                finalPosition += binormal * offsets[i].y;
                lineRenderer.SetPosition(i, finalPosition);
            }

            if (delay == 0f)
            {
                yield break;
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator LightningGlow()
    {
        while (true)
        {
            if (!enabled)
            {
                yield break;
            }
            if (!isReady)
            {
                yield return new WaitForEndOfFrame();
            } 
            else
            {
                break;
            }
        }
        for (int i = 5; i > 0; i--)
        {
            if (!enabled)
            {
                yield break;
            }
            lineRenderer.startColor = new Color(i / 5f, i / 5f, i / 5f);
            lineRenderer.endColor = lineRenderer.startColor;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
