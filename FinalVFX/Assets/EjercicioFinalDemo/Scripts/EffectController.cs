using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EffectController : MonoBehaviour
{
    private Coroutine coneEffectCoroutine;
    private Animator animator;
    private static readonly int effectStateHash = Animator.StringToHash("EffectState");
    private Coroutine resetCoroutine;
    private int lastEffectState = 0;
private float lastEffectChangeTime = 0f;
private float safetyTimeout = 5f; // Tiempo máximo permitido sin volver a estado 0

    private bool isPaused = false;
    private float remainingWaitTime = 0f;

    [SerializeField] private Button shieldButton;
    [SerializeField] private Button powerRayButton;
    [SerializeField] private Button healButton;
    [SerializeField] private Button orbButton;

    [SerializeField] private GameObject shieldEffectObject;
    [SerializeField] private GameObject powerRayEffectObject;
    [SerializeField] private GameObject LineEffectObject;

    [SerializeField] private GameObject healEffectObject;
    [SerializeField] private GameObject ConeEffectObject;
    [SerializeField] private GameObject orbEffectObject;
    [SerializeField] private GameObject TorusEffectObject;

    // 👇 Nuevas referencias de partículas
    [SerializeField] private ParticleSystem shieldParticles;
    [SerializeField] private ParticleSystem powerRayParticles;
    [SerializeField] private ParticleSystem healParticles;
    //[SerializeField] private ParticleSystem orbParticles;

    private ParticleSystem[] allParticles;

    // 👇 Materiales con shader que usa _CustomTime
    [SerializeField] private Material[] effectMaterials;
    private float customTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[EffectController] No se encontró Animator en {gameObject.name}");
        }

        DisableAllEffectObjects();

        // Inicializamos array de partículas
        allParticles = new ParticleSystem[]
        {
            shieldParticles, powerRayParticles, healParticles
        };
    }

    private void Update()
    {
        // Actualiza _CustomTime solo si no está en pausa
        if (!isPaused)
        {
            customTime += Time.deltaTime;

            foreach (var mat in effectMaterials)
            {
                if (mat != null)
                {
                    mat.SetFloat("_CustomTime", customTime);
                }
            }
                    if ( orbEffectObject != null && orbEffectObject.activeSelf && TorusEffectObject != null)
        {
            TorusEffectObject.transform.Rotate(2f, 2f, 20f, Space.Self);
        }
        }
        // ⏱ Seguridad: Si pasan más de 5 segundos en una animación ≠ 0, forzamos reset
if (!isPaused && animator.GetInteger(effectStateHash) != 0)
{
   
    if (Time.time - lastEffectChangeTime > safetyTimeout)
    {

        Debug.LogWarning("[EffectController] Se forzó regreso a estado 0 por timeout.");
        animator.SetInteger(effectStateHash, 0);
        SetButtonsInteractable(true);
        DisableAllEffectObjects();
        lastEffectState = 0;
    }
}

    }

    public void PlayShield() => TriggerEffect(1);
    public void PlayPowerRay() => TriggerEffect(2);
    public void PlayHeal() => TriggerEffect(3);
    public void PlayOrb() => TriggerEffect(4);

  private void TriggerEffect(int state)
{
    if (animator == null) return;

    SetButtonsInteractable(false);
    lastEffectState = state;

    animator.SetInteger(effectStateHash, state);
    ActivateEffectObject(state);

    // 🔄 Marca el tiempo del último cambio de animación
    if(!isPaused){
    lastEffectChangeTime = Time.time;
    }else{
         lastEffectChangeTime = lastEffectChangeTime;
    }

    if (resetCoroutine != null)
    {
        StopCoroutine(resetCoroutine);
        resetCoroutine = null;
    }

    resetCoroutine = StartCoroutine(ResetToIdle());
}


    private System.Collections.IEnumerator ResetToIdle()
    {
        yield return null;
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = stateInfo.length / stateInfo.speed;

        if (lastEffectState == 4) waitTime *= 1.6f;
        else if (lastEffectState == 1) waitTime *= 1.5f;
        else if (lastEffectState == 2) waitTime *= 1.4f;

        remainingWaitTime = waitTime;

        while (remainingWaitTime > 0f)
        {
            if (!isPaused)
            {
                remainingWaitTime -= Time.deltaTime;
            }
            yield return null;
        }

        animator.SetInteger(effectStateHash, 0);
        resetCoroutine = null;
        SetButtonsInteractable(true);
        DisableAllEffectObjects();
    }

    private void SetButtonsInteractable(bool value)
    {
        shieldButton.interactable = value;
        powerRayButton.interactable = value;
        healButton.interactable = value;
        orbButton.interactable = value;
    }

private void ActivateEffectObject(int effectState)
{
    DisableAllEffectObjects();

    switch (effectState)
    {
        case 1:
            shieldEffectObject?.SetActive(true);
            shieldEffectObject.transform.localScale = Vector3.zero;

            if (shieldScaleCoroutine != null)
                StopCoroutine(shieldScaleCoroutine);

            shieldScaleCoroutine = StartCoroutine(ScaleOverTime(shieldEffectObject, Vector3.one * 1.5f, 2f));
            break;

        case 2:
            powerRayEffectObject?.SetActive(true);

            Vector3 currentScale = LineEffectObject.transform.localScale;
            LineEffectObject.transform.localScale = new Vector3(currentScale.x, currentScale.y, 0f);

            if (powerRayScaleCoroutine != null)
                StopCoroutine(powerRayScaleCoroutine);

            powerRayScaleCoroutine = StartCoroutine(DelayedScaleZ(LineEffectObject, 5f, 1.5f, 1.5f));
            break;

        case 3:
            healEffectObject?.SetActive(true);
            
            if (coneEffectCoroutine != null)
                StopCoroutine(coneEffectCoroutine);
            
            coneEffectCoroutine = StartCoroutine(ActivateConeEffectAfterDelay(1.2f));
            break;

        case 4:
            orbEffectObject?.SetActive(true);
            break;
    }
}


private void DisableAllEffectObjects()
{
    if (shieldEffectObject != null)
    {
        shieldEffectObject.SetActive(false);
        shieldEffectObject.transform.localScale = Vector3.zero;
    }

    if (powerRayEffectObject != null)
    {
        powerRayEffectObject.SetActive(false);
        Vector3 resetScale = LineEffectObject.transform.localScale;
        LineEffectObject.transform.localScale = new Vector3(resetScale.x, resetScale.y, 0f);
    }

    healEffectObject?.SetActive(false);
    orbEffectObject?.SetActive(false);

    // 👉 Cancelar corutina si está corriendo
    if (coneEffectCoroutine != null)
    {
        StopCoroutine(coneEffectCoroutine);
        coneEffectCoroutine = null;
    }

    ConeEffectObject?.SetActive(false);
}


    public void TogglePause()
    {
        if (!animator) return;

        isPaused = !isPaused;
        animator.speed = isPaused ? 0f : 1f;

        SetParticlesPaused(isPaused);

        if (!isPaused && resetCoroutine == null && remainingWaitTime > 0f)
        {
            resetCoroutine = StartCoroutine(WaitThenReset(remainingWaitTime));
        }
    }

    private void SetParticlesPaused(bool paused)
    {
        foreach (var ps in allParticles)
        {
            if (ps == null) continue;

            if (paused)
                ps.Pause();
            else
                ps.Play();
        }
    }

    private System.Collections.IEnumerator WaitThenReset(float time)
    {
        while (time > 0f)
        {
            if (!isPaused)
            {
                time -= Time.deltaTime;
            }
            yield return null;
        }

        animator.SetInteger(effectStateHash, 0);
        resetCoroutine = null;
        SetButtonsInteractable(true);
        DisableAllEffectObjects();
    }
    private Coroutine shieldScaleCoroutine;

private IEnumerator ScaleOverTime(GameObject obj, Vector3 targetScale, float duration)
{
    float time = 0f;
    Vector3 initialScale = obj.transform.localScale;

    while (time < duration)
    {
        if (!isPaused)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.Clamp01(t);

            // 👇 Función explosiva: t elevado a una potencia alta para acelerar al final
            float easedT = Mathf.Pow(t, 10); // Cuanto mayor el exponente, más lento inicia y más rápido explota

            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, easedT);
        }
        yield return null;
    }

    obj.transform.localScale = targetScale;
}
private Coroutine powerRayScaleCoroutine;

private IEnumerator ScaleZOverTime(GameObject obj, float targetZ, float duration)
{
    float time = 0f;
    Vector3 initialScale = obj.transform.localScale;
    Vector3 finalScale = new Vector3(initialScale.x, initialScale.y, targetZ);

    while (time < duration)
    {
        if (!isPaused)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // Suavizado explosivo
            float easedT = Mathf.Pow(t, 1f);

            float z = Mathf.Lerp(0f, targetZ, easedT);
            obj.transform.localScale = new Vector3(initialScale.x, initialScale.y, z);
        }
        yield return null;
    }

    obj.transform.localScale = finalScale;
}
private IEnumerator DelayedScaleZ(GameObject obj, float targetZ, float duration, float delay)
{
    yield return new WaitForSeconds(delay);

    yield return StartCoroutine(ScaleZOverTime(obj, targetZ, duration));
}

private IEnumerator ActivateConeEffectAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    ConeEffectObject?.SetActive(true);
}


}
