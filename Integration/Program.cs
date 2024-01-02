using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();
        var distributedService = new DistributedItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(5000);

        var task1 = Task.Run(() => service.SaveItem("a"));
        var task2 = Task.Run(() => service.SaveItem("b"));
        var task3 = Task.Run(() => service.SaveItem("c"));

        Task.WaitAll(task1, task2, task3);

        //Distributed
        //var distTask1 = Task.Run(async () => await distributedService.SaveItem("a"));
        //var distTask2 = Task.Run(async() => await distributedService.SaveItem("b"));
        //var distTask3 = Task.Run(async () => await distributedService.SaveItem("c"));

        //Task.WaitAll(distTask1, distTask2, distTask3);
        Thread.Sleep(500);
        ThreadPool.QueueUserWorkItem(async _ =>await distributedService.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(async _ => await distributedService.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(async _ => await distributedService.SaveItem("c"));
        Thread.Sleep(500);
        ThreadPool.QueueUserWorkItem(async _ => await distributedService.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(async _ => await distributedService.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(async _ => await distributedService.SaveItem("c"));
        Thread.Sleep(5000);


        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);
        Console.WriteLine("Distributed records:");
        distributedService.GetAllItems().ForEach(Console.WriteLine);
        Console.ReadLine();
    }
}