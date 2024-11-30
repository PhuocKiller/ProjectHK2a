using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemsManager : MonoBehaviour
{
    GameObject[] itemButton;
    GameObject itemToBuy;
    int priceToBuy;
    NetworkManager networkManager;
    [SerializeField] Transform backGroundShopItem;
    public TextMeshProUGUI priceValue;
    
    private void Awake()
    {
        networkManager=FindObjectOfType<NetworkManager>();
        LoadStatItemButton();
    }
    private void OnEnable()
    {
        itemToBuy = null;
        priceToBuy = 0;
        priceValue.text = "0";
    }
    void LoadStatItemButton()
    {
        int index = -1;
        foreach (Transform item in backGroundShopItem)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.shopItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    public void UpdatePrice(int index)
    {
        itemToBuy = networkManager.shopItems[index];
        priceToBuy = itemToBuy.GetComponent<InventoryItemBase>().Price;
        priceValue.text= priceToBuy.ToString();
    }
    public void BuyItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (player.playerStat.coinsValue< priceToBuy)
        {

        }
        else
        {
            player.playerStat.coinsValue-= priceToBuy;
            Singleton<Inventory>.Instance.AddItem(itemToBuy.GetComponent<InventoryItemBase>());
        }
    }
}
