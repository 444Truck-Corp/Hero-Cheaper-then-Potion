using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotPotion : MonoBehaviour
{
    public Toggle toggle;
    public int slotNum => transform.GetSiblingIndex();

    [SerializeField] private Image potionImg;
    [SerializeField] private TextMeshProUGUI potionNameTxt;
    [SerializeField] private TextMeshProUGUI potionCountTxt;
    public ItemData ItemData { get; private set; }

    public void SetSlot(ItemData data)
    {
        ItemData = data;
        potionImg.sprite = ResourceManager.Instance.LoadAsset<Sprite>(ResourceManager.textureDir, data.icon);
        potionNameTxt.text = data.name;

        int count = SaveManager.Instance.MySaveData.items[data.id];
        potionCountTxt.text = count.ToString();
    }

    public void SetPotionCount(int newCount)
    {
        potionCountTxt.text = newCount.ToString();
    }
}
