using UnityEngine;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// Tipos de ítems disponibles en la tienda.
    /// </summary>
    public enum ShopItemType
    {
        PowerUp,
        Card
    }

    /// <summary>
    /// ScriptableObject que define un ítem de la tienda.
    /// Crear desde: Assets → Create → TravesiaColombia → ShopItem
    /// </summary>
    [CreateAssetMenu(fileName = "NewShopItem",
                     menuName = "TravesiaColombia/ShopItem")]
    public class ShopItem : ScriptableObject
    {
        [Header("Identificación")]
        [SerializeField] private string _itemId;
        [SerializeField] private string _itemName;
        [SerializeField] private ShopItemType _itemType;

        [Header("Descripción")]
        [SerializeField][TextArea(2, 3)] private string _description;
        [SerializeField] private Sprite _icon;

        [Header("Precio")]
        [SerializeField] private int _price = 30;

        [Header("PowerUp — solo si ItemType es PowerUp")]
        [SerializeField] private PowerUpType _powerUpType;
        [SerializeField] private float _powerUpDuration = 5f;

        [Header("Carta — solo si ItemType es Card")]
        [SerializeField] private CardData _cardData;

        [Header("Disponibilidad")]
        [SerializeField] private RegionType _availableInRegion = RegionType.Selva;
        [SerializeField] private bool _isAvailable = true;

        // ── Propiedades públicas ──────────────────────────────────────────────
        public string ItemId => _itemId;
        public string ItemName => _itemName;
        public ShopItemType ItemType => _itemType;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int Price => _price;
        public PowerUpType PowerUpType => _powerUpType;
        public float PowerUpDuration => _powerUpDuration;
        public CardData CardData => _cardData;
        public RegionType AvailableInRegion => _availableInRegion;
        public bool IsAvailable => _isAvailable;
    }
}