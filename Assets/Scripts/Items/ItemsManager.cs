using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour
{
    GameObject[] itemButton;
    GameObject itemToBuy;
    IInventoryItem itemToSell;
    [SerializeField] GameObject buyButton, sellButton;
    int priceItem; int indexItem; int indexSlot;
    NetworkManager networkManager;
    [SerializeField] Transform backGroundShopItem;
    public TextMeshProUGUI priceValue;
    [SerializeField] TextMeshProUGUI itemInfoText;
    
    private void Awake()
    {
        networkManager=FindObjectOfType<NetworkManager>();
        LoadStatItemButton();
    }
    private void OnEnable()
    {
        itemToBuy = null;
        priceItem = 0;
        priceValue.text = "0";
        itemInfoText.text = "";
        buyButton.SetActive(true); sellButton.SetActive(false);
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
        priceItem = itemToBuy.GetComponent<InventoryItemBase>().Price;
        priceValue.text= priceItem.ToString();
        indexItem=index;
        ShowInfoItem(itemToBuy.GetComponent<InventoryItemBase>());
        buyButton.SetActive(true); sellButton.SetActive(false);  
    }

    public void BuyItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        if (player.playerStat.coinsValue< priceItem)
        {

        }
        else
        {
            player.playerStat.coinsValue-= priceItem;
            Singleton<Inventory>.Instance.AddItem(itemToBuy.GetComponent<InventoryItemBase>(), out int indexItemSlot);
            
        }
    }
    public void SellItem()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        Singleton<Inventory>.Instance.RemoveItem(itemToSell, indexSlot);
        player.playerStat.coinsValue += priceItem;
    }
    public void ShowInfoItem(IInventoryItem item)
    {
        itemInfoText.text = item.Info;
    }
    public void CheckInfoToSell(IInventoryItem item, int indexSlot)
    {
        ShowInfoItem(item);
        buyButton.SetActive(false); sellButton.SetActive(true);
        itemToSell = item;
        this.indexSlot=indexSlot;
        priceItem = (int)(item.Price * 0.7);
        priceValue.text= priceItem.ToString();
    }
}
