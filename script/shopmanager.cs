using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;        // Confirm popup
    public GameObject shop;             // Main shop panel
    public GameObject badaiHo;          // Success popup
    public GameObject needMoreCoin;     // Not enough coins popup

    public TMP_Text messageText;
    public TMP_Text priceText;
    public TMP_Text cointext;

    public List<GameObject> itemPurchaseList;
    public List<int> itemPriceList;

    private int activePurchasePrice;
    private int activePurchaseIndex;

    void Start()
    {
        int totalCoins = PlayerPrefs.GetInt("coin");
        cointext.text = "Gold: " + totalCoins;

        for (int i = 0; i < itemPurchaseList.Count; i++)
        {
            if (PlayerPrefs.GetString("item" + i) == "1")
            {
                itemPurchaseList[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void ThisItem(int index)
    {
        if (PlayerPrefs.GetString("item" + index) == "1")
        {
            itemPurchaseList[index].transform.GetChild(1).gameObject.SetActive(false);
            PlayerPrefs.SetInt("activecolour", index);
        }
        else
        {
            activePurchaseIndex = index;
            activePurchasePrice = itemPriceList[index];
            priceText.text = activePurchasePrice + " Gold";
            shopPanel.SetActive(true);
        }
    }

    public void ConfirmPurchase()
    {
        int totalCoins = PlayerPrefs.GetInt("coin");

        if (totalCoins >= activePurchasePrice)
        {
            totalCoins -= activePurchasePrice;
            PlayerPrefs.SetInt("coin", totalCoins);
            cointext.text = "Gold: " + totalCoins;

            PlayerPrefs.SetString("item" + activePurchaseIndex, "1");
            PlayerPrefs.SetInt("activecolour", activePurchaseIndex);

            itemPurchaseList[activePurchaseIndex].transform.GetChild(1).gameObject.SetActive(false);

            shopPanel.SetActive(false);
            shop.SetActive(true);

            // ✅ Show success
            badaiHo.SetActive(true);
        }
        else
        {
            shopPanel.SetActive(false);
            shop.SetActive(true);

            // ❌ Show "need more coin"
            needMoreCoin.SetActive(true);
        }
    }

    public void CloseBadaiHo()
    {
        badaiHo.SetActive(false);
    }

    public void CloseNeedMoreCoin()
    {
        needMoreCoin.SetActive(false);
    }

    public void onclick()
    {
        SceneManager.LoadScene(1);
    }
}
