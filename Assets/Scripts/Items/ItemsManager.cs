using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsManager : MonoBehaviour
{
    GameObject[] itemButton;
    GameObject itemToBuy;
    InventoryItemBase itemToSell;
    [SerializeField] GameObject buyBtn, sellBtn, basicBtn,shieldBtn, rangeBtn, meleeBtn;
    int priceItem; int indexItem; int indexSlot;
    NetworkManager networkManager;
    [SerializeField] Transform basicBG,shieldBG,rangeBG,meleeBG;
    public TextMeshProUGUI priceValue;
    [SerializeField] TextMeshProUGUI itemInfoText;
    
    private void Awake()
    {
        networkManager=FindObjectOfType<NetworkManager>();
        LoadStatItems();
        BasicButton();
    }
    private void OnEnable()
    {
        itemToBuy = null;
        priceItem = 0;
        priceValue.text = "0";
        itemInfoText.text = "";
        buyBtn.SetActive(true); sellBtn.SetActive(false);
    }
    void LoadStatItems()
    {
        LoadStatBasicItems();
        LoadStatShieldItems();
        LoadStatRangeItems();
        LoadStatMeleeItems();
    }
    void LoadStatBasicItems()
    {
        int index = -1;
        foreach (Transform item in basicBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.basicItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatShieldItems()
    {
        int index = -1;
        foreach (Transform item in shieldBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.shieldItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatRangeItems()
    {
        int index = -1;
        foreach (Transform item in rangeBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.rangeItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    void LoadStatMeleeItems()
    {
        int index = -1;
        foreach (Transform item in meleeBG)
        {
            index++;
            Image imageItem = item.GetChild(0).GetComponent<Image>();
            imageItem.sprite = networkManager.MeleeItems[index].GetComponent<IInventoryItem>().Image;
        }
    }
    public void UpdatePrice(Transform thisBtn)
    {
        string parentName=thisBtn.parent.name;
        switch (parentName)
        {
            case "BasicBG": { itemToBuy = networkManager.basicItems[thisBtn.GetSiblingIndex()];  break; }
            case "ShieldBG": { itemToBuy = networkManager.shieldItems[thisBtn.GetSiblingIndex()]; break; }
            case "RangeBG": { itemToBuy = networkManager.rangeItems[thisBtn.GetSiblingIndex()]; break; }
            case "MeleeBG": { itemToBuy = networkManager.MeleeItems[thisBtn.GetSiblingIndex()]; break; }
        }
        priceItem = itemToBuy.GetComponent<InventoryItemBase>().Price;
        priceValue.text= priceItem.ToString();
        ShowInfoItem(itemToBuy.GetComponent<InventoryItemBase>());
        buyBtn.SetActive(true); sellBtn.SetActive(false);  
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
    public void ShowInfoItem(InventoryItemBase item)
    {
        itemInfoText.text = item.Info;
    }
    public void CheckInfoToSell(InventoryItemBase item, int indexSlot)
    {
        ShowInfoItem(item);
        buyBtn.SetActive(false); sellBtn.SetActive(true);
        itemToSell = item;
        this.indexSlot=indexSlot;
        priceItem = (int)(item.Price * 0.7);
        priceValue.text= priceItem.ToString();
    }
    public void BasicButton()
    {
        basicBtn.GetComponent<Image>().color= Color.green;
        shieldBtn.GetComponent<Image>().color = Color.white;
        rangeBtn.GetComponent<Image>().color = Color.white;
        meleeBtn.GetComponent<Image>().color = Color.white;
    }
    public void ShieldButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.green;
        rangeBtn.GetComponent<Image>().color = Color.white;
        meleeBtn.GetComponent<Image>().color = Color.white;
    }
    public void RangeButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.white;
        rangeBtn.GetComponent<Image>().color = Color.green;
        meleeBtn.GetComponent<Image>().color = Color.white;
    }
    public void MeleeButton()
    {
        basicBtn.GetComponent<Image>().color = Color.white;
        shieldBtn.GetComponent<Image>().color = Color.white;
        rangeBtn.GetComponent<Image>().color = Color.white;
        meleeBtn.GetComponent<Image>().color = Color.green;
    }
}
