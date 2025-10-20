namespace UI.Inventory.Items.Domain {
    /// <summary>
    /// Logiczny model przedmiotu w ekwipunku.
    /// </summary>
    public class ItemData
    {
        public string Id { get; }
        public string DisplayName { get; }
        public ItemShape Shape { get; }
        public string IconPath { get; } // opcjonalnie

        public ItemData(string id, string displayName, ItemShape shape, string iconId = null)
        {
            Id = id;
            DisplayName = displayName;
            Shape = shape;
            IconPath = iconId;
        }
    }
}