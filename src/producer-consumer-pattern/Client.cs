using System.Text.Json.Serialization;

namespace producer_consumer_pattern;

/// <summary>
/// Order is a json-friendly representation of an order.
/// </summary>
/// <param name="Id">order id</param>
/// <param name="Name">food name</param>
/// <param name="Temp">ideal temperature</param>
/// <param name="Freshness">freshness in seconds</param>
record Order(string Id, string Name, string Temp, long Freshness);

record Problem(List<Order> Orders);


/// <summary>
/// Action is a json-friendly representation of an action.
/// </summary>
/// <param name="timestamp">action timestamp</param>
/// <param name="id">order id</param>
/// <param name="action">place, move, pickup or discard</param>
class Action(DateTime timestamp, string id, string action)
{
    public static readonly string Place = "place";
    public static readonly string Move = "move";
    public static readonly string Pickup = "pickup";
    public static readonly string Discard = "discard";

    public DateTime DateTime { get; init; } = timestamp;
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; } = (long)timestamp.Subtract(DateTime.UnixEpoch).TotalMicroseconds;
    [JsonPropertyName("id")]
    public string Id { get; init; } = id;
    [JsonPropertyName("action")]
    public string Action_ { get; init; } = action;
};

class Client()
{
    public async Task<Problem> NewProblemAsync()
    {
        return new Problem([
            new("kc3yd", "Chicken Nuggets", "hot", 109),
            new("k6ie1", "Pressed Juice", "cold", 166),
            new("oozhn", "Tuna Sandwich", "cold", 76),
            new("ye5h4", "Stale Bread", "room", 84),
            new("97kyd", "Coconut", "room", 68),
            new("6xt88", "French Fries", "hot", 70),
            new("pqi6y", "Tuna Sandwich", "cold", 142),
            new("k33xb", "Apple", "room", 154),
            new("cnhek", "Mixed Greens", "cold", 155),
            new("b8dmo", "Pressed Juice", "cold", 114),
            new("8foed", "Vanilla Ice Cream", "cold", 92),
            new("jqwjk", "Banana", "room", 117),
            new("qd6ci", "Chocolate Gelato", "cold", 152),
            new("p7jc7", "Sushi", "cold", 132),
            new("99xpj", "Tuna Sandwich", "cold", 171),
            new("ytwba", "Chocolate Gelato", "cold", 132),
            new("3f13a", "Danish Pastry", "room", 172),
            new("q6jf6", "Banana", "room", 133),
            new("cskmf", "Sushi", "cold", 109),
            new("r7o5a", "Italian Meatballs", "hot", 95),
            new("jw6m4", "Tomato Soup", "hot", 89),
            new("c4h6o", "Lasagna", "hot", 77),
            new("g6ze6", "Cheeseburger", "hot", 155),
            new("1sdcq", "Tuna Sandwich", "cold", 65),
            new("53qyj", "Vanilla Ice Cream", "cold", 157),
            new("w8cq3", "Pad Thai", "hot", 76),
            new("fdsim", "Chicken Tacos", "hot", 150),
            new("z97b6", "Kale Salad", "cold", 133),
            new("rh7ip", "Pad See Ew", "hot", 104),
            new("utu8w", "Apple", "room", 63),
            new("84qq5", "Vanilla Ice Cream", "cold", 161),
            new("1yiey", "Mac & Cheese", "hot", 84),
            new("osxjk", "Bacon Burger", "hot", 106),
            new("kc88d", "Stale Bread", "room", 160),
            new("mad8w", "Cheeseburger", "hot", 178),
            new("5swzx", "Mac & Cheese", "hot", 162),
            new("e33hj", "Burrito", "hot", 106),
            new("cj1do", "BBQ Pizza", "hot", 95),
            new("x9h4b", "Gas Station Sushi", "room", 127),
            new("wscrq", "Turkey Sandwich", "cold", 92),
            new("o7dtq", "Chicken Nuggets", "hot", 107),
            new("k6jqj", "Fizzed-Out Pepsi", "room", 109),
            new("3xido", "Pad Thai", "hot", 114),
            new("tjnrp", "Vegetarian Pizza", "hot", 139),
            new("4rrjc", "Lasagna", "hot", 141),
            new("qi5wo", "Cheese Pizza", "hot", 176),
            new("zdpz6", "Danish Pastry", "room", 121),
            new("bu5d4", "Tuna Sandwich", "cold", 64)
        ]);
    }
}
