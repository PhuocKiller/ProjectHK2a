using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    public virtual string Name
    {
        get;
    }
    public ItemTypes _ItemType;
    public ItemTypes ItemType
    {
        get
        {
            return _ItemType;
        }
    }
    public Sprite _Image;
    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }
    public int _Price;
    public int Price
    {
        get
        {
            return _Price;
        }
    }
    public bool isDecreaseAlpha, isIncreaseAlpha;
    public virtual ItemTypes itemTypes { get; set; }
    public InventorySlot Slot
    {
        get; set;
    }
    public SkillName skillName;
    public SkillTypes skillType;
    public float timerTrigger;
    public float cooldownTime;
    public int[] levelManaCosts;
    public Sprite skillIcon;
    public AudioClip triggerSoundFX;
    public int[] levelDamages;
    public bool isPhysicDamage;
    public bool isMakeStun;
    public bool isMakeSlow;
    public bool isMakeSilen;
    public float timeEffect;
    public NetworkObject VfxEffect;
    public virtual string Info {  get; set; }
    public void Start()
    {
      //  StartCoroutine(FadeItemNoPick());
    }
    public void FixedUpdate()
    {
        if (isDecreaseAlpha)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GetComponent<SpriteRenderer>().color.a - 0.1f);
            if (GetComponent<SpriteRenderer>().color.a < 0.2f)
            {
                isDecreaseAlpha = false;
                isIncreaseAlpha = true;
            }
        }
        if (isIncreaseAlpha)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GetComponent<SpriteRenderer>().color.a + 0.1f);
            if (GetComponent<SpriteRenderer>().color.a > 1)
            {
                isDecreaseAlpha = true;
                isIncreaseAlpha = false;
            }
        }
    }
    public IEnumerator FadeItemNoPick()
    {
        yield return new WaitForSeconds(2);
        isDecreaseAlpha = true;
        StartCoroutine(DestroyItemNoPick());
    }
    public IEnumerator DestroyItemNoPick()
    {
        yield return new WaitForSeconds(2);

        Destroy(gameObject);
    }
   
    public virtual void OnPickUp()
    {
        Destroy(gameObject);
    }
    
    public virtual void OnDrop()
    {
        /*Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector3(pos.x, pos.y, 0);*/
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo,500))
        {
            Singleton<Inventory>.Instance.CreateNewItem(hitInfo.point, GetItemTypes());
        }
        
    }
    public virtual ItemTypes GetItemTypes()
    {
        return itemTypes;
    }
    public virtual void OnUse()
    {

    }
}
