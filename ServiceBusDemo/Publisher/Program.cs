using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

// KLISTRA IN DIN CONNECTION STRING HÄR!
var connectionString = "Endpoint=sb://vilikestandinginline.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=+lloHOeOE3O6JTDRJzohbYJ3W+Qgg29vT+ASbC9nUrw=";
var queueName = "orders";

// Skapa Service Bus client
await using var client = new ServiceBusClient(connectionString);

// Skapa queue (gör detta första gången)
try
{
    var adminClient = new ServiceBusAdministrationClient(connectionString);
    if (!await adminClient.QueueExistsAsync(queueName))
    {
        await adminClient.CreateQueueAsync(queueName);
        Console.WriteLine($" Queue '{queueName}' skapad!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Obs: {ex.Message}");
}

// Skapa sender
var sender = client.CreateSender(queueName);

// Skicka 5 testmeddelanden
Console.WriteLine("\n Skickar meddelanden...\n");

for (int i = 1; i <= 5; i++)
{
    var order = new
    {
        OrderId = i,
        Customer = $"Kund {i}",
        Amount = 100 * i,
        Timestamp = DateTime.Now
    };

    var message = new ServiceBusMessage(
        System.Text.Json.JsonSerializer.Serialize(order)
    );

    await sender.SendMessageAsync(message);
    Console.WriteLine($" Skickade: Order #{order.OrderId} - {order.Amount} kr");

    await Task.Delay(500); // Vänta lite mellan varje
}

Console.WriteLine("\n Klart! Alla meddelanden skickade.");
Console.WriteLine("Tryck Enter för att avsluta...");
Console.ReadLine();