using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ProyectG.Toolbox.Singletons;

public class SceneLoader : GenericSingleton<SceneLoader>
{
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private float delayLoadFake;
    [SerializeField] private int waitForActivateScene;
    private float target;

    void Start()
    {
        loaderCanvas.SetActive(false);
        target = 0;
        progressBar.fillAmount = 0;
    }
    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, delayLoadFake * Time.deltaTime);
    }

    private void LoadSceneAsync(string value)
    {
        IEnumerator AsyncLoad()
        {
            AsyncOperation val = SceneManager.LoadSceneAsync(value, LoadSceneMode.Additive);

            while (!val.isDone)
                yield return new WaitForEndOfFrame();

            if (val.allowSceneActivation)
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
        StartCoroutine(AsyncLoad());
    }

    public async void LoadSceneAsyncWithLoadScreen(string value)
    {
        target = 0;
        progressBar.fillAmount = 0;

        var loadOp = SceneManager.LoadSceneAsync(value);
        loadOp.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            target = loadOp.progress;

        } while (loadOp.progress < 0.9f);

        await Task.Delay(waitForActivateScene);

        loadOp.allowSceneActivation = true;
        loaderCanvas.SetActive(false);
    }

    private void ReloadGameplay(int secondsToWait)
    {
        IEnumerator WaitFewSecondsUtilLoadScene()
        {
            float t = 0;

            while (t < secondsToWait)
            {
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            SceneManager.LoadScene("Gameplay");
            yield break;
        }

        StartCoroutine(WaitFewSecondsUtilLoadScene());
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
