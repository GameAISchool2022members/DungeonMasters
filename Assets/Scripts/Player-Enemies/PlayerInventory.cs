using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject removeProject;

    public GameObject mergeButton;

    public GameObject[] slots;      //Inventory slots
    public GameObject selected1; //Selected to combine slot 1
    public GameObject selected2; //Selected to combine slot 2

    public GameObject[] results;
    public GameObject[] resultBorder;

    public Sprite defaultSprite;

    public int[] ids;
    public Sprite[] images;



    private int invCounter;
    private int select1;
    private int select2;
    private int selectFinal;

    private bool invOpen;

    void Start()
    {
        inventory.SetActive(false);
        removeProject.SetActive(false);
        invOpen = false;

        ids = new int[5];
        images = new Sprite[5];

        invCounter = 0;
        select1 = -1;
        select2 = -1;
        selectFinal = -1;

        for (int i = 0; i < resultBorder.Length; i++)
        {
            resultBorder[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I))
        {
            //Pause and open inventory
            if (invOpen)
            {
                if (!removeProject.active)
                {
                    inventory.SetActive(false);
                    invOpen = false;
                }
            }
            else
            {
                inventory.SetActive(true);
                invOpen = true;
            }
        }

        if (invOpen)
        {
            if (select1 >= 0 && select2 >= 0)
            {
                mergeButton.SetActive(true);
            }
            else
            {
                mergeButton.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "collectable")
        {
            if (invCounter < 5)
            {
                ids[invCounter] = other.GetComponent<Collectables>().ID;
                images[invCounter] = other.GetComponent<Collectables>().image;

                slots[invCounter].GetComponent<Image>().sprite = images[invCounter];

                invCounter++;

                Destroy(other.gameObject);
            }
        }
    }

    public void SelectSlot1()
    {
        if (invCounter > 0)
        {
            if (select1 < 0)
            {
                select1 = 0;
                selected1.GetComponent<Image>().sprite = images[0];
            }
            else if (select2 < 0 && select1 != 0)
            {
                select2 = 0;
                selected2.GetComponent<Image>().sprite = images[0];
            }
        }
    }

    public void SelectSlot2()
    {
        if (invCounter > 1)
        {
            if (select1 < 0)
            {
                select1 = 1;
                selected1.GetComponent<Image>().sprite = images[1];
            }
            else if (select2 < 0 && select1 != 1)
            {
                select2 = 1;
                selected2.GetComponent<Image>().sprite = images[1];
            }
        }
    }
    public void SelectSlot3()
    {
        if (invCounter > 2)
        {
            if (select1 < 0)
            {
                select1 = 2;
                selected1.GetComponent<Image>().sprite = images[2];
            }
            else if (select2 < 0 && select1 != 2)
            {
                select2 = 2;
                selected2.GetComponent<Image>().sprite = images[2];
            }
        }
    }
    public void SelectSlot4()
    {
        if (invCounter > 3)
        {
            if (select1 < 0)
            {
                select1 = 3;
                selected1.GetComponent<Image>().sprite = images[3];
            }
            else if (select2 < 0 && select1 != 3)
            {
                select2 = 3;
                selected2.GetComponent<Image>().sprite = images[3];
            }
        }
    }
    public void SelectSlot5()
    {
        if (invCounter > 4)
        {
            if (select1 < 0)
            {
                select1 = 4;
                selected1.GetComponent<Image>().sprite = images[4];
            }
            else if (select2 < 0 && select1 != 4)
            {
                select2 = 4;
                selected2.GetComponent<Image>().sprite = images[4];
            }
        }
    }

    public void OutputSlot1()
    {
        if (select2 >= 0)
        {
            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[0].SetActive(true);
            selectFinal = 0;
        }
    }
    public void OutputSlot2()
    {
        if (select2 >= 0)
        {
            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[1].SetActive(true);
            selectFinal = 1;

        }
    }
    public void OutputSlot3()
    {
        if (select2 >= 0)
        {
            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[2].SetActive(true);
            selectFinal = 2;
        }
    }
    public void OutputSlot4()
    {
        if (select2 >= 0)
        {
            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[3].SetActive(true);
            selectFinal = 3;
        }
    }
    public void OutputSlot5()
    {
        if (select2 >= 0)
        {

            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[4].SetActive(true);
            selectFinal = 4;
        }
    }
    public void OutputSlot6()
    {
        if (select2 >= 0)
        {

            for (int i = 0; i < resultBorder.Length; i++)
            {
                resultBorder[i].SetActive(false);
            }

            resultBorder[5].SetActive(true);
            selectFinal = 5;
        }
    }

    /*public void DeselectCombine1()
    {
        //Ask if you are sure to remove
        if (select1 >= 0)
        {
            removeProject.SetActive(true);
        }
        
    }*/
    public void DeselectCombine1()
    {
        selected1.GetComponent<Image>().sprite = defaultSprite;
        select1 = -1;
        for (int i = 0; i < resultBorder.Length; i++)
        {
            resultBorder[i].SetActive(false);
        }
    }

    public void DeselectCombine2()
    {
        selected2.GetComponent<Image>().sprite = defaultSprite;
        select2 = -1;
        for (int i = 0; i < resultBorder.Length; i++)
        {
            resultBorder[i].SetActive(false);
        }
    }

    public void Combine()
    {
        //Will require getting new image
        if (select1 >= 0 && select2 >= 0)
        {
            //Reset Inventory
            for (int i = 0; i < 5; i++)
            {
                slots[i].GetComponent<Image>().sprite = defaultSprite;
                invCounter = 0;
            }

            slots[select1].GetComponent<Collectables>().ID = ids[select1];
            slots[select2].GetComponent<Collectables>().ID = ids[select2];

            slots[select1].GetComponent<Collectables>().image = images[select1];
            slots[select2].GetComponent<Collectables>().image = images[select2];

            //ResetPopulation for next level
            //List<Collectables> population = new List<Collectables>();
            //population.Add(slots[select1].GetComponent<Collectables>());
            //population.Add(slots[select2].GetComponent<Collectables>());

            //DataManager.Instance.UploadSelectedCollectables(population); //Reset the population (next level, the Generator will spawn the new pop)
           // StartCoroutine(NextPop(population));
            //Reset combination slots
            selected1.GetComponent<Image>().sprite = defaultSprite;
            select1 = -1;
            selected2.GetComponent<Image>().sprite = defaultSprite;
            select2 = -1;

            if(slots[select1].GetComponent<Collectables>().image != slots[select2].GetComponent<Collectables>().image)
            {
                GetComponent<Player>().artCount+=2;
            }
            else
            {
                GetComponent<Player>().artCount++;
            }

            
        }
    }

    //IEnumerator NextPop(List<Collectables> population)
    //{
        //yield return DataManager.Instance.canStartGenerating;
        //yield return DataManager.Instance.UploadSelectedCollectables(population); //Initialising population (with arbitary images)
   // }

    public void YesRemove()
    {
        if (select1 >= 0)
        {
            removeProject.SetActive(false);
            selected1.GetComponent<Image>().sprite = defaultSprite;
            select1 = -1;
            selectFinal = -1;
        }
    }

    public void NoKeep()
    {
        if (select1 >= 0)
        {
            removeProject.SetActive(false);
        }
    }
}
