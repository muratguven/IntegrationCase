using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Backend
{
    public class DistributedLock : IDisposable
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly object disposalLock = new object(); // Used for thread-safe disposal

        public async Task AcquireLockAsync(string lockKey)
        {
            var semaphore = locks.GetOrAdd(lockKey, key => new SemaphoreSlim(1, 1));

            try
            {
                await semaphore.WaitAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Error acquiring lock for key {lockKey}: {ex.Message}");
                throw; // Rethrow the exception if necessary
            }
        }
        public void AcquireLock(string lockKey)
        {
            var semaphore = locks.GetOrAdd(lockKey, key => new SemaphoreSlim(1, 1));
            semaphore.Wait();
        }

        public void ReleaseLock(string lockKey)
        {
            if (locks.TryGetValue(lockKey, out var semaphore))
            {
                semaphore.Release();
            }
        }

   

        public bool IsLocked(string lockKey)
        {
            return locks.ContainsKey(lockKey) && locks[lockKey].CurrentCount == 0;
        }

        public void Dispose()
        {
            lock (disposalLock)
            {
                foreach (var semaphore in locks.Values)
                {
                    semaphore.Dispose();
                }
                locks.Clear();
            }
        }
    }
}
