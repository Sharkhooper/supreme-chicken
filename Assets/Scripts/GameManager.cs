using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Current { get; private set; }

    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private float fadeTime;

    public bool AllowReload { get; set; }

    public bool HotStart { get; private set; } = false;

    private bool reloading = false;

    public void Start() {
        if (Current == null) {
            Current = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        overlay.alpha = 1;
        overlay.DOFade(0, fadeTime);
        AllowReload = false;
    }

    public void RestartLevel(bool force = false) {
        if (!(force || AllowReload)) return;
        if (reloading) return;
        reloading = true;
        HotStart = true;
        StartCoroutine(Reload());
    }

    public void GotoMenu(bool force = false) {
        if (!(force || AllowReload)) return;
        if (reloading) return;
        reloading = true;
        HotStart = false;
        StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        Tween t = overlay.DOFade(1, fadeTime).SetEase(Ease.InOutSine);
        yield return t.WaitForCompletion();
        AllowReload = false;
        int index = SceneManager.GetActiveScene().buildIndex;
        var op = SceneManager.UnloadSceneAsync(index);
        if (op != null) {
            while (!op.isDone) {
                yield return null;
            }

        }
        op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        if (op != null) {
            while (!op.isDone) {
                yield return null;
            }
        }
        t = overlay.DOFade(0, fadeTime).SetEase(Ease.InOutSine);
        yield return t.WaitForCompletion();
        reloading = false;
    }
}
