using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackCameraController : MonoBehaviour
{
    public static AttackCameraController Instance;

    public bool IsAttacking { get; private set; }

    [SerializeField] float camLungeX = 0.32f;
    [SerializeField] float camRotation = 1.2f;
    [SerializeField] float camLungeTime = 0.06f;
    [SerializeField] float camReturnTime = 0.12f;
    [SerializeField] float camOvershoot = 0.15f;

    [SerializeField] float maxAttackFOV = 65f;
    [SerializeField] float fovInSpeed = 120f;
    [SerializeField] float fovOutSpeed = 160f;

    [SerializeField] float shakeStrength = 0.05f;
    [SerializeField] float shakeTime = 0.06f;

    [SerializeField] Transform characterLayer;
    [SerializeField] float globalCharPullX = 0.05f;
    [SerializeField] float globalCharPullZ = -0.25f;

    [SerializeField] float attackerPullX = 0.12f;
    [SerializeField] float targetPullX = -0.12f;
    [SerializeField] float pairPullZ = -0.35f;

    [SerializeField] List<GameObject> allies = new List<GameObject>(4);
    [SerializeField] List<GameObject> enemies = new List<GameObject>(4);

    [SerializeField] List<GameObject> allyIdleSprites = new List<GameObject>(4);
    [SerializeField] List<GameObject> allyAttackSprites = new List<GameObject>(4);

    [SerializeField] List<GameObject> enemyIdleSprites = new List<GameObject>(4);
    [SerializeField] List<GameObject> enemyHitSprites = new List<GameObject>(4);

    [SerializeField] List<GameObject> fadeDuringAttack = new List<GameObject>();
    [SerializeField] float fadedAlpha = 0.25f;
    [SerializeField] float fadeTime = 0.08f;

    [SerializeField] Renderer backgroundRenderer;
    [SerializeField] float blurOnAttack = 1f;
    [SerializeField] float blurOffAttack = 0f;
    [SerializeField] float blurFadeTime = 0.08f;

    struct FadeTarget
    {
        public CanvasGroup canvas;
        public SpriteRenderer sprite;
        public float originalAlpha;
    }

    List<FadeTarget> fadeTargets = new List<FadeTarget>();

    Camera cam;
    Vector3 baseCamPos;
    Quaternion baseCamRot;
    float baseFOV;
    Vector3 baseCharLayerPos;

    Material backgroundMat;

    Coroutine routine;
    Coroutine fadeRoutine;
    Coroutine blurRoutine;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();

        baseCamPos = transform.localPosition;
        baseCamRot = transform.localRotation;
        baseFOV = cam.fieldOfView;

        if (characterLayer)
            baseCharLayerPos = characterLayer.localPosition;

        if (backgroundRenderer)
            backgroundMat = backgroundRenderer.material;

        CacheFadeTargets();
    }

    void CacheFadeTargets()
    {
        fadeTargets.Clear();

        foreach (GameObject go in fadeDuringAttack)
        {
            if (!go) continue;

            CanvasGroup cg = go.GetComponent<CanvasGroup>();
            if (cg)
            {
                fadeTargets.Add(new FadeTarget
                {
                    canvas = cg,
                    sprite = null,
                    originalAlpha = cg.alpha
                });
                continue;
            }

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr)
            {
                fadeTargets.Add(new FadeTarget
                {
                    canvas = null,
                    sprite = sr,
                    originalAlpha = sr.color.a
                });
            }
        }
    }

    public void PlayAttackByIndex(int allyIndex, int enemyIndex)
    {
        if (allyIndex < 0 || allyIndex >= allies.Count) return;
        if (enemyIndex < 0 || enemyIndex >= enemies.Count) return;

        if (routine != null)
            StopCoroutine(routine);

        IsAttacking = false;

        routine = StartCoroutine(
            AllyAttacksEnemyRoutine(
                allies[allyIndex].transform,
                enemies[enemyIndex].transform,
                allyIndex,
                enemyIndex
            )
        );
    }

    public void PlayEnemyAttackByIndex(int enemyIndex, int allyIndex)
    {
        if (allyIndex < 0 || allyIndex >= allies.Count) return;
        if (enemyIndex < 0 || enemyIndex >= enemies.Count) return;

        if (routine != null)
            StopCoroutine(routine);

        IsAttacking = false;

        routine = StartCoroutine(
            EnemyAttacksAllyRoutine(
                enemies[enemyIndex].transform,
                allies[allyIndex].transform,
                allyIndex,
                enemyIndex
            )
        );
    }

    IEnumerator AllyAttacksEnemyRoutine(
        Transform attacker,
        Transform target,
        int allyIndex,
        int enemyIndex
    )
    {
        IsAttacking = true;

        DisableNonParticipants(allyIndex, enemyIndex);

        Transform attackerMove = GetMoveTransformForAlly(allyIndex, attacker);
        Transform targetMove = GetMoveTransformForEnemy(enemyIndex, target);

        BeginAttackSprites(allyIndex, enemyIndex);

        StartFade(false);
        StartBlur(true);

        Vector3 startAttackerPos = attackerMove.localPosition;
        Vector3 startTargetPos = targetMove.localPosition;

        Vector3 dir = GetAttackDirection(attacker, target);

        Vector3 camAttackPos = baseCamPos + dir * camLungeX;
        Quaternion camAttackRot = Quaternion.Euler(0f, 0f, -dir.x * camRotation);

        Vector3 attackerAttackPos =
            startAttackerPos + dir * attackerPullX + Vector3.forward * pairPullZ;

        Vector3 targetHitPos =
            startTargetPos - dir * targetPullX + Vector3.forward * pairPullZ;

        yield return LerpAttack(
            camAttackPos,
            camAttackRot,
            attackerMove,
            attackerAttackPos,
            targetMove,
            targetHitPos,
            maxAttackFOV,
            camLungeTime
        );

        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            transform.localPosition =
                camAttackPos + (Vector3)Random.insideUnitCircle * shakeStrength;

            cam.fieldOfView = Mathf.MoveTowards(
                cam.fieldOfView,
                maxAttackFOV,
                fovInSpeed * Time.deltaTime
            );

            yield return null;
        }

        yield return LerpAttack(
            baseCamPos,
            baseCamRot,
            attackerMove,
            startAttackerPos,
            targetMove,
            startTargetPos,
            baseFOV,
            camReturnTime
        );

        transform.localPosition = baseCamPos;
        transform.localRotation = baseCamRot;
        cam.fieldOfView = baseFOV;

        attackerMove.localPosition = startAttackerPos;
        targetMove.localPosition = startTargetPos;

        EndAttackSprites(allyIndex, enemyIndex);
        StartFade(true);
        StartBlur(false);
        EnableAllCharacters();

        IsAttacking = false;
    }

    IEnumerator EnemyAttacksAllyRoutine(
        Transform attacker,
        Transform target,
        int allyIndex,
        int enemyIndex
    )
    {
        IsAttacking = true;

        DisableNonParticipants(allyIndex, enemyIndex);

        StartFade(false);
        StartBlur(true);

        Vector3 startAttackerPos = attacker.localPosition;
        Vector3 startTargetPos = target.localPosition;

        Vector3 dir = GetAttackDirection(attacker, target);

        Vector3 camAttackPos = baseCamPos + dir * camLungeX;
        Quaternion camAttackRot = Quaternion.Euler(0f, 0f, -dir.x * camRotation);

        Vector3 attackerAttackPos =
            startAttackerPos + dir * attackerPullX + Vector3.forward * pairPullZ;

        Vector3 targetHitPos =
            startTargetPos - dir * targetPullX + Vector3.forward * pairPullZ;

        yield return LerpAttack(
            camAttackPos,
            camAttackRot,
            attacker,
            attackerAttackPos,
            target,
            targetHitPos,
            maxAttackFOV,
            camLungeTime
        );

        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            transform.localPosition =
                camAttackPos + (Vector3)Random.insideUnitCircle * shakeStrength;

            cam.fieldOfView = Mathf.MoveTowards(
                cam.fieldOfView,
                maxAttackFOV,
                fovInSpeed * Time.deltaTime
            );

            yield return null;
        }

        yield return LerpAttack(
            baseCamPos,
            baseCamRot,
            attacker,
            startAttackerPos,
            target,
            startTargetPos,
            baseFOV,
            camReturnTime
        );

        transform.localPosition = baseCamPos;
        transform.localRotation = baseCamRot;
        cam.fieldOfView = baseFOV;

        attacker.localPosition = startAttackerPos;
        target.localPosition = startTargetPos;

        StartFade(true);
        StartBlur(false);
        EnableAllCharacters();

        IsAttacking = false;
    }

    Transform GetMoveTransformForAlly(int allyIndex, Transform fallback)
    {
        if (allyIndex >= 0 && allyIndex < allyAttackSprites.Count)
        {
            GameObject atk = allyAttackSprites[allyIndex];
            if (atk) return atk.transform;
        }
        return fallback;
    }

    Transform GetMoveTransformForEnemy(int enemyIndex, Transform fallback)
    {
        if (enemyIndex >= 0 && enemyIndex < enemyHitSprites.Count)
        {
            GameObject hit = enemyHitSprites[enemyIndex];
            if (hit) return hit.transform;
        }
        return fallback;
    }

    void DisableNonParticipants(int allyIndex, int enemyIndex)
    {
        for (int i = 0; i < allies.Count; i++)
            if (i != allyIndex && allies[i])
                allies[i].SetActive(false);

        for (int i = 0; i < enemies.Count; i++)
            if (i != enemyIndex && enemies[i])
                enemies[i].SetActive(false);
    }

    void EnableAllCharacters()
    {
        foreach (var a in allies)
            if (a) a.SetActive(true);

        foreach (var e in enemies)
            if (e) e.SetActive(true);
    }

    void BeginAttackSprites(int allyIndex, int enemyIndex)
    {
        GameObject allyAttack = (allyIndex >= 0 && allyIndex < allyAttackSprites.Count) ? allyAttackSprites[allyIndex] : null;
        GameObject enemyHit = (enemyIndex >= 0 && enemyIndex < enemyHitSprites.Count) ? enemyHitSprites[enemyIndex] : null;

        if (allyAttack)
        {
            if (allyIndex >= 0 && allyIndex < allyIdleSprites.Count && allyIdleSprites[allyIndex] && allyIdleSprites[allyIndex] != allies[allyIndex])
                allyIdleSprites[allyIndex].SetActive(false);

            allyAttack.SetActive(true);
        }

        if (enemyHit)
        {
            if (enemyIndex >= 0 && enemyIndex < enemyIdleSprites.Count && enemyIdleSprites[enemyIndex] && enemyIdleSprites[enemyIndex] != enemies[enemyIndex])
                enemyIdleSprites[enemyIndex].SetActive(false);

            enemyHit.SetActive(true);
        }
    }

    void EndAttackSprites(int allyIndex, int enemyIndex)
    {
        SetActiveSafe(allyAttackSprites, allyIndex, false);

        if (allyIndex >= 0 && allyIndex < allyIdleSprites.Count && allyIdleSprites[allyIndex] && allyIdleSprites[allyIndex] != allies[allyIndex])
            allyIdleSprites[allyIndex].SetActive(true);

        SetActiveSafe(enemyHitSprites, enemyIndex, false);

        if (enemyIndex >= 0 && enemyIndex < enemyIdleSprites.Count && enemyIdleSprites[enemyIndex] && enemyIdleSprites[enemyIndex] != enemies[enemyIndex])
            enemyIdleSprites[enemyIndex].SetActive(true);
    }

    void SetActiveSafe(List<GameObject> list, int index, bool value)
    {
        if (index >= 0 && index < list.Count && list[index])
            list[index].SetActive(value);
    }

    void StartFade(bool restore)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(restore));
    }

    IEnumerator FadeRoutine(bool restore)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;

            foreach (var f in fadeTargets)
            {
                float target = restore ? f.originalAlpha : fadedAlpha;

                if (f.canvas)
                    f.canvas.alpha = Mathf.Lerp(f.canvas.alpha, target, t);
                else if (f.sprite)
                {
                    Color c = f.sprite.color;
                    c.a = Mathf.Lerp(c.a, target, t);
                    f.sprite.color = c;
                }
            }

            yield return null;
        }

        foreach (var f in fadeTargets)
        {
            float a = restore ? f.originalAlpha : fadedAlpha;

            if (f.canvas)
                f.canvas.alpha = a;
            else if (f.sprite)
            {
                Color c = f.sprite.color;
                c.a = a;
                f.sprite.color = c;
            }
        }
    }

    void StartBlur(bool enable)
    {
        if (!backgroundMat) return;

        if (blurRoutine != null)
            StopCoroutine(blurRoutine);

        blurRoutine = StartCoroutine(
            BlurRoutine(enable ? blurOnAttack : blurOffAttack)
        );
    }

    IEnumerator BlurRoutine(float target)
    {
        float start = backgroundMat.GetFloat("_Blend");
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / blurFadeTime;
            backgroundMat.SetFloat("_Blend", Mathf.Lerp(start, target, t));
            yield return null;
        }

        backgroundMat.SetFloat("_Blend", target);
    }

    IEnumerator LerpAttack(
        Vector3 camTargetPos,
        Quaternion camTargetRot,
        Transform attacker,
        Vector3 attackerTargetPos,
        Transform target,
        Vector3 targetTargetPos,
        float targetFOV,
        float time
    )
    {
        float t = 0f;

        Vector3 camStartPos = transform.localPosition;
        Quaternion camStartRot = transform.localRotation;

        Vector3 attackerStart = attacker.localPosition;
        Vector3 targetStart = target.localPosition;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float e = EaseOutCubic(t);

            transform.localPosition = Vector3.Lerp(camStartPos, camTargetPos, e);
            transform.localRotation = Quaternion.Lerp(camStartRot, camTargetRot, e);

            cam.fieldOfView = Mathf.MoveTowards(
                cam.fieldOfView,
                targetFOV,
                (targetFOV > cam.fieldOfView ? fovInSpeed : fovOutSpeed)
                * Time.deltaTime
            );

            attacker.localPosition = Vector3.Lerp(attackerStart, attackerTargetPos, e);
            target.localPosition = Vector3.Lerp(targetStart, targetTargetPos, e);

            yield return null;
        }
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
