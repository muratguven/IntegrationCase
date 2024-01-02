using Integration.Common;
using Integration.Backend;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
    private HashSet<string> uniqueContents = new HashSet<string>();

    // This is called externally and can be called multithreaded, in parallel.
    // More than one item with the same content should not be saved. However,
    // calling this with different contents at the same time is OK, and should
    // be allowed for performance reasons.
    public Result SaveItem(string itemContent)
    {
        lock (uniqueContents)
        {
            // Check the backend to see if the content is already saved.
            //|| ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0
            if (uniqueContents.Contains(itemContent))
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = ItemIntegrationBackend.SaveItem(itemContent);

            // Add the content to the HashSet to mark it as processed
            uniqueContents.Add(itemContent);
            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
        }
      
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}