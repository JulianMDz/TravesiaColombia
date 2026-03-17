using UnityEngine;

namespace TravesiaColombia.Core
{
    /// <summary>
    /// ScriptableObject que define una carta cultural coleccionable.
    /// Crear desde: Assets → Create → TravesiaColombia → CardData
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard",
                     menuName = "TravesiaColombia/CardData")]
    public class CardData : ScriptableObject
    {
        [Header("Identificación")]
        [SerializeField] private string _cardId;
        [SerializeField] private string _cardName;
        [SerializeField] private CardRarity _rarity;
        [SerializeField] private RegionType _region;

        [Header("Contenido cultural")]
        [SerializeField][TextArea(2, 4)] private string _culturalInfo;
        [SerializeField][TextArea(2, 4)] private string _curiousFact;
        [SerializeField][TextArea(2, 4)] private string _environmentalIssue;

        [Header("Visual")]
        [SerializeField] private Sprite _cardArt;

        [Header("Tienda")]
        [SerializeField] private int _shopPrice = 50;
        [SerializeField] private bool _availableInShop = true;

        // ── Propiedades públicas ──────────────────────────────────────────────
        public string CardId => _cardId;
        public string CardName => _cardName;
        public CardRarity Rarity => _rarity;
        public RegionType Region => _region;
        public string CulturalInfo => _culturalInfo;
        public string CuriousFact => _curiousFact;
        public string EnvironmentalIssue => _environmentalIssue;
        public Sprite CardArt => _cardArt;
        public int ShopPrice => _shopPrice;
        public bool AvailableInShop => _availableInShop;
    }
}