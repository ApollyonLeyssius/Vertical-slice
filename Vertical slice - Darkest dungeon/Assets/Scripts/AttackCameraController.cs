using UnityEngine;
using System.Collections;

public class AttackCameraController : MonoBehaviour
{
    public static AttackCameraController Instance;

    [Header("Camera")]
    [SerializeField] float camLungeX = 0.32f;
    [SerializeField] float camZoom = 0.035f;
    [SerializeField] float camRotation = 1.2f;
    [SerializeField] float camLungeTime = 0.06f;
    [SerializeField] float camReturnTime = 0.12f;
    [SerializeField] float camOvershoot = 0.15f;

    [Header("Impact Shake")]
    [SerializeField] float shakeStrength = 0.05f;
    [SerializeField] float shakeTime = 0.06f;

    [Header("Character Layer")]
    [SerializeField] Transform characterLayer;
    [SerializeField] float globalCharPullX = 0.05f;
    [SerializeField] float globalCharPullZ = -0.25f;

    [Header("Attacker / Target Compression")]
    [SerializeField] float attackerPullX = 0.12f;
    [SerializeField] float targetPullX = -0.12f;
    [SerializeField] float pairPullZ = -0.35f;

    [Header("Background Cross-Fade")]
    [SerializeField] Renderer backgroundRenderer;   // uses BackgroundCrossFade shader
    [SerializeField] float blurPeak = 0.8f;         // max _Blend value

    [Header("Test Keys")]
    [SerializeField] bool enableTestKeys = true;
    [SerializeField] Transform testAttacker;
    [SerializeField] Transform testTarget;

    Camera cam;
    Vector3 baseCamPos;
    Quaternion baseCamRot;
    float baseCamSize;

    Vector3 baseCharLayerPos;
    Vector3 baseAttackerPos;
    Vector3 baseTargetPos;

    Material bgMat;
    static readonly int BlendID = Shader.PropertyToID("_Blend");

    Coroutine routine;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();

        baseCamPos = transform.localPosition;
        baseCamRot = transform.localRotation;
        baseCamSize = cam.orthographicSize;

        if (characterLayer)
            baseCharLayerPos = characterLayer.localPosition;

        if (backgroundRenderer)
        {
            bgMat = backgroundRenderer.material; // instance
            bgMat.SetFloat(BlendID, 0f);
        }
    }

    void Update()
    {
        if (!enableTestKeys || testAttacker == null || testTarget == null)
            return;

        if (Input.GetKeyDown(KeyCode.I))
            PlayAttack(testAttacker, testTarget);

        if (Input.GetKeyDown(KeyCode.P))
            PlayAttack(testTarget, testAttacker);
    }

    public void PlayAttack(Transform attacker, Transform target)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(AttackRoutine(attacker, target));
    }

    IEnumerator AttackRoutine(Transform attacker, Transform target)
    {
        Vector3 dir = GetAttackDirection(attacker, target);

        baseAttackerPos = attacker.localPosition;
        baseTargetPos = target.localPosition;

        Vector3 camAttackPos = baseCamPos + dir * camLungeX;
        Quaternion camAttackRot = Quaternion.Euler(0f, 0f, -dir.x * camRotation);
        float camZoomTarget = baseCamSize * (1f - camZoom);

        Vector3 charLayerAttackPos =
            baseCharLayerPos +
            dir * globalCharPullX +
            Vector3.forward * globalCharPullZ;

        Vector3 attackerAttackPos =
            baseAttackerPos +
            dir * attackerPullX +
            Vector3.forward * pairPullZ;

        Vector3 targetAttackPos =
            baseTargetPos -
            dir * targetPullX +
            Vector3.forward * pairPullZ;

        // LUNGE (blur in)
        yield return LerpAll(
            baseCamPos, camAttackPos,
            baseCamRot, camAttackRot,
            baseCamSize, camZoomTarget,
            baseCharLayerPos, charLayerAttackPos,
            attacker, baseAttackerPos, attackerAttackPos,
            target, baseTargetPos, targetAttackPos,
            0f, blurPeak,
            camLungeTime
        );

        // IMPACT SHAKE (hold blur)
        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            transform.localPosition = camAttackPos + (Vector3)Random.insideUnitCircle * shakeStrength;
            SetBGBlend(blurPeak);
            yield return null;
        }

        // RETURN (blur out, synced to return)
        Vector3 camOvershootPos = baseCamPos - dir * camLungeX * camOvershoot;

        yield return LerpAll(
            transform.localPosition, camOvershootPos,
            transform.localRotation, baseCamRot,
            cam.orthographicSize, baseCamSize,
            characterLayer.localPosition, baseCharLayerPos,
            attacker, attacker.localPosition, baseAttackerPos,
            target, target.localPosition, baseTargetPos,
            blurPeak, blurPeak * 0.4f,
            camReturnTime * 0.6f
        );

        yield return LerpAll(
            camOvershootPos, baseCamPos,
            transform.localRotation, baseCamRot,
            cam.orthographicSize, baseCamSize,
            characterLayer.localPosition, baseCharLayerPos,
            attacker, attacker.localPosition, baseAttackerPos,
            target, target.localPosition, baseTargetPos,
            blurPeak * 0.4f, 0f,
            camReturnTime * 0.4f
        );

        // HARD RESET
        transform.localPosition = baseCamPos;
        transform.localRotation = baseCamRot;
        cam.orthographicSize = baseCamSize;

        if (characterLayer)
            characterLayer.localPosition = baseCharLayerPos;

        attacker.localPosition = baseAttackerPos;
        target.localPosition = baseTargetPos;

        SetBGBlend(0f);
    }

    IEnumerator LerpAll(
        Vector3 camFrom, Vector3 camTo,
        Quaternion rotFrom, Quaternion rotTo,
        float sizeFrom, float sizeTo,
        Vector3 charFrom, Vector3 charTo,
        Transform attacker, Vector3 attFrom, Vector3 attTo,
        Transform target, Vector3 tarFrom, Vector3 tarTo,
        float blendFrom, float blendTo,
        float time
    )
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float e = EaseOutCubic(t);

            transform.localPosition = Vector3.Lerp(camFrom, camTo, e);
            transform.localRotation = Quaternion.Lerp(rotFrom, rotTo, e);
            cam.orthographicSize = Mathf.Lerp(sizeFrom, sizeTo, e);

            if (characterLayer)
                characterLayer.localPosition = Vector3.Lerp(charFrom, charTo, e);

            attacker.localPosition = Vector3.Lerp(attFrom, attTo, e);
            target.localPosition = Vector3.Lerp(tarFrom, tarTo, e);

            SetBGBlend(Mathf.Lerp(blendFrom, blendTo, e));
            yield return null;
        }
    }

    void SetBGBlend(float v)
    {
        if (bgMat != null)
            bgMat.SetFloat(BlendID, v);
    }

    Vector3 GetAttackDirection(Transform attacker, Transform target)
    {
        float dir = Mathf.Sign(
            cam.WorldToViewportPoint(target.position).x -
            cam.WorldToViewportPoint(attacker.position).x
        );
        return Vector3.right * dir;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
