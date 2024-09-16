public class InventoryItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty; // Can store which region or location the item is in
}
