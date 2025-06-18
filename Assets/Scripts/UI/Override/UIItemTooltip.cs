using TMPro;
using UnityEngine;

public class UIItemTooltip : UIBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Vector3 _offset = new(10, -10);

    private RectTransform _parent;
    private Camera _camera;

    protected override void Awake()
    {
        base.Awake();
        _parent = GetComponentInParent<RectTransform>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    _parent.transform as RectTransform,
            //    Input.mousePosition,
            //    _camera,
            //    out Vector2 pos
            //);
            var pos = Input.mousePosition;
            transform.position = (Vector3)pos + _offset;
        }
    }

    public override void Opened(object[] param)
    {
        transform.position = Input.mousePosition;
        //int amount = (item.category != EItemCategory.Equipment) ? SaveManager.Instance.MySaveData.items[item.id] : 1;
        ItemData item = param[0] as ItemData;
        int amount = param.Length > 1 ? (int)param[1] : 0;
        _nameText.text = item.name + (amount > 1 ? $"x {amount}" : "");
        _descriptionText.text = item.description;
    }
}