using System.Threading.Channels;

namespace producer_consumer_pattern;

static class ProducerConsumerPattern
{
    private static readonly string hot = "hot", cold = "cold", room = "room";
    private static readonly int amountTotalHeater = 6, amountTotalCooler = 6, amountTotalShelf = 12;
    private static readonly List<Order> itemsCold = [];
    private static readonly List<Order> itemsHot = [];
    private static readonly List<Order> itemsShelf = [];

    /// <summary>
    /// Challenge harness
    /// </summary>
    /// <param name="min">Minimum pickup time (in seconds)</param>
    /// <param name="max">Maximum pickup time (in seconds)</param>
    static async Task Main(int min = 4, int max = 8)
    {
        try
        {
            var client = new Client();
            var problem = await client.NewProblemAsync();

            var channel = Channel.CreateBounded<Action>(new BoundedChannelOptions(problem.Orders.Count)
            {
                FullMode = BoundedChannelFullMode.Wait,
                AllowSynchronousContinuations = true,
                SingleWriter = false
            });

            var actions = new List<Action>();

            var producerTask = Producer(channel.Writer, problem.Orders, actions);
            var consumerTask = Consumer(channel.Reader, actions, min, max);

            await Task.WhenAll(producerTask, consumerTask);

            Console.WriteLine($"End.");

        }
        catch (Exception e)
        {
            Console.WriteLine($"Simulation failed: {e}");
        }
    }

    static async Task Producer(ChannelWriter<Action> writer, List<Order> orders, List<Action> actions)
    {
        var countList = orders.Count;
        var itensPerSecond = 2;

        for (int i = 0; i < countList; i++)
        {
            var order = orders[i];

            if (i != 0 && i % itensPerSecond == 0)
            {
                await Task.Delay(2000);
            }

            if (order.Temp.Equals(hot) && itemsHot.Count < amountTotalHeater)
            {
                var action = new Action(DateTime.Now, order.Id, Action.Place);
                actions.Add(action);
                await writer.WriteAsync(action);

                Console.WriteLine($"Produced: {order.Id} - {DateTime.Now} - {order.Temp} - HOT");

                itemsHot.Add(order);
                continue;
            }

            if (order.Temp.Equals(cold) && itemsCold.Count < amountTotalCooler)
            {
                var action = new Action(DateTime.Now, order.Id, Action.Place);
                actions.Add(action);
                await writer.WriteAsync(action);

                Console.WriteLine($"Produced: {order.Id} - {DateTime.Now} - {order.Temp} - COLD");

                itemsCold.Add(order);
                continue;
            }

            if (itemsShelf.Count < amountTotalShelf)
            {
                var action = new Action(DateTime.Now, order.Id, Action.Place);
                actions.Add(action);
                await writer.WriteAsync(action);

                Console.WriteLine($"Produced: {order.Id} - {DateTime.Now} - {order.Temp} - SHELF");

                itemsShelf.Add(order);
                continue;
            } 
            else
            {
                var coldItem = itemsShelf.Find(x => x.Temp.Equals(cold, StringComparison.OrdinalIgnoreCase));
                if (coldItem is not null && itemsCold.Count < amountTotalCooler)
                {
                    itemsCold.Add(coldItem);
                    itemsShelf.Remove(coldItem);
                    actions.Add(new Action(DateTime.Now, coldItem.Id, Action.Move));
                    Console.WriteLine($"Move {order.Name} - {order.Id} - {order.Temp} - {DateTime.Now.ToLongTimeString()}");
                }

                var hotItem = itemsShelf.Find(x => x.Temp.Equals(hot, StringComparison.OrdinalIgnoreCase));
                if (hotItem is not null && itemsHot.Count < amountTotalHeater)
                {
                    itemsHot.Add(hotItem);
                    itemsShelf.Remove(hotItem);
                    actions.Add(new Action(DateTime.Now, hotItem.Id, Action.Move));
                    Console.WriteLine($"Move {order.Name} - {order.Id} - {order.Temp} - {DateTime.Now.ToLongTimeString()}");
                }

                if (itemsShelf.Count < amountTotalShelf)
                {
                    var action = new Action(DateTime.Now, order.Id, Action.Place);
                    actions.Add(action);
                    await writer.WriteAsync(action);

                    Console.WriteLine($"Produced: {order.Id} - {DateTime.Now} - {order.Temp} - SHELF");

                    itemsShelf.Add(order);
                }
                else
                {
                    itemsShelf.Remove(itemsShelf.First());
                    itemsShelf.Add(order);
                    actions.Add(new Action(DateTime.Now, order.Id, Action.Discard));

                    var action = new Action(DateTime.Now, order.Id, Action.Place);
                    actions.Add(action);
                    await writer.WriteAsync(action);

                    Console.WriteLine($"Discard:  {itemsShelf.First().Id} - {itemsShelf.First().Temp} - {DateTime.Now.ToLongTimeString()}");
                    Console.WriteLine($"Produced: {order.Id} - {DateTime.Now} - {order.Temp} - SHELF");
                }
            }
        }

        writer.Complete();

    }

    static async Task Consumer(ChannelReader<Action> reader, List<Action> actions, int min, int max)
    {
        while (await reader.WaitToReadAsync())
        {
            while (reader.TryRead(out var item))
            {
                if (actions.Any(x => x.Id.Equals(item.Id) && x.Action_.Equals("discard")))
                    continue;

                var delayTime = (long)TimeSpan.FromSeconds(new Random().Next(min, max)).TotalMilliseconds;

                var elapsedTime = DateTime.Now.Subtract(item.DateTime).TotalMilliseconds;

                var timeToWait = delayTime - elapsedTime;

                if (timeToWait > 0) 
                { 
                    await Task.Delay((int)timeToWait);
                }

                actions.Add(new Action(DateTime.Now, item.Id, Action.Pickup));
                Console.WriteLine($"Consumed: {item.Id} - {DateTime.Now} - random {delayTime}");

                var hotItem = itemsHot.Find(x => x.Id.Equals(item.Id));
                if (hotItem is not null)
                    itemsHot.Remove(hotItem);

                var coldItem = itemsCold.Find(x => x.Id.Equals(item.Id));
                if (coldItem is not null)
                    itemsCold.Remove(coldItem);

                var roomItem = itemsShelf.Find(x => x.Id.Equals(item.Id));
                if (roomItem is not null)
                    itemsShelf.Remove(roomItem);
            }
        }
    }
}
