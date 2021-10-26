using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
	public static GameManager instance; 
	public GameObject loadingScreen;
	public ProgressBar bar;
	public TextMeshProUGUI textField;
	public Sprite[] backgrounds;
	public Image backgroundImage;
	public TextMeshProUGUI tipsText;
	public CanvasGroup alphaCanvas;
	public string[] tips;
	
	private void Awake()
	{
		instance = this;
		SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
	}
	
	List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
	
	public void LoadGame()
	{
		backgroundImage.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
		loadingScreen.gameObject.SetActive(true);
		
		StartCorountine(GenerateTips());
		
		scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
		scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MAP, LoadSceneMode.Additive));
		scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.RACETRACK, LoadSceneMode.Additive));
		
		StartCorountine(GetSceneLoadProgress());
	}
	
	float totalSceneProgress;
	public IEnumerator GetSceneLoadProgress()
	{
		for(int i=0; i<scenesLoading.Count; i++)
		{
			while (!scenesLoading[i].isDone)
			{
				totalSceneProgress = 0;
				
				foreach(AsyncOperation operation in scenesLoading)
				{
					totalSceneProgress += operation.progress;
				}
				
				totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;
				
				bar.current = Mathf.RoundToInt(totalSceneProgress);
				
				textField.text = string.Format("Loading Level : {0}%", totalSceneProgress);
				
				yield return null;
			}
		}
		
		loadingScreen.gameObject.SetActive(false);
	}
	
	public int tipCount;
	public IEnumerator GenerateTips()
	{
		tipCount = Random.Range(0, tips.Length);
		tipsText.text = tips[tipCount];
		while (loadingScreen.activeInHierarchy)
		{
			yield return new WaitForSeconds(3f);
			
			LeanTween.alphaCanvas(alphaCanvas, 0, 0.5f);
			
			yield return new WaitForSeconds(0.5f);
			
			tipCount++;
			if (tipCount >= tips.Length)
			{
				tipCount = 0;
			}
			
			tipsText.text = tips[tipCount];
			
			LeanTween.alphaCanvas(alphaCanvas, 1, 0.5f);
		}
	}
	
}
