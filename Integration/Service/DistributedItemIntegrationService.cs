using Integration.Backend;
using Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Service
{
    public sealed class DistributedItemIntegrationService :IDisposable
    {
        private DistributedLock lockProvider; // You can use Redis distributed lock interface instead of this.
        private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

        public DistributedItemIntegrationService()
        {
            this.lockProvider = new DistributedLock();
        }

        public  async Task<Result> SaveItem(string itemContent)
        {

            // Distributed lock acquired, now proceed to save the item
            try
            {
                if (lockProvider.IsLocked(itemContent))
                {
                    return new Result(false, $"Duplicate item received with content {itemContent}.");
                }

                await lockProvider.AcquireLockAsync(itemContent);

                var item = await  ItemIntegrationBackend.SaveItemAsync(itemContent);

                lockProvider.ReleaseLock(itemContent);
                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log, and return an appropriate result
                return new Result(false, $"Error saving item: {ex.Message}");
            }

        }

        public List<Item> GetAllItems()
        {
            return ItemIntegrationBackend.GetAllItems();
        }

        public void Dispose()
        {
            // Dispose of any disposable resources
            lockProvider.Dispose();
        }
    }
}
