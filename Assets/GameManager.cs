using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Model;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    /*public List<Sprite> sprites;
    public Image image;

    public static GameManager Instance { get; private set; }

    private bool starting, updating;
    private List<Collectables> collectibles;
    private List<Collectables> generatedCollectibles;

    public bool canStartGenerating;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        starting = true;
        updating = false;
        collectibles = sprites.Select(s => {
            return new Collectables {
                image = s
            };
        }).ToList();
        StartCoroutine(InitCoroutine());
        
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(5);
        var currentLevelCollectibles = DataManager.Instance.GetCurrentLevelCollectables();

        if (currentLevelCollectibles.Count == 0)
        {
            yield return DisplaySpritesCoroutine(collectibles.Select(s => s.image).ToList());
            yield return DataManager.Instance.UploadSelectedCollectables(collectibles);
            currentLevelCollectibles = DataManager.Instance.GetCurrentLevelCollectables();
        }           
        else
        {
            yield return DisplaySpritesCoroutine(currentLevelCollectibles.Select(s => s.image).ToList());
        }

        generatedCollectibles = currentLevelCollectibles;
        starting = !starting;
        canStartGenerating = true;
}

    IEnumerator RefreshCoroutine()
    {
        updating = true;
        yield return new WaitForSeconds(5);

        var currentLevelCollectables = new List<Collectables>();

        if(Random.value < 0.5)
        {
            yield return DataManager.Instance.UploadSelectedCollectables(collectibles);
            collectibles = DataManager.Instance.GetCurrentLevelCollectables();
            currentLevelCollectables = collectibles;
        }  
        else
        {
            yield return DataManager.Instance.UploadSelectedCollectables(generatedCollectibles);
            generatedCollectibles = DataManager.Instance.GetCurrentLevelCollectables();
            currentLevelCollectables = generatedCollectibles;
        }

 
        yield return DisplaySpritesCoroutine(currentLevelCollectables.Select(s => s.image).ToList());

        updating = false;
    }

    IEnumerator DisplaySpritesCoroutine(List<Sprite> images)
    {
        foreach (var img in images)
        {
            //image.sprite = img;
            yield return new WaitForSeconds(1);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!starting)
        {
            if (!updating)
            {
                StartCoroutine(RefreshCoroutine());
            }
        }
    }*/
}
