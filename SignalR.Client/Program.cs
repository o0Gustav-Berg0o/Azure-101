using Microsoft.AspNetCore.SignalR.Client;


string username = Environment.UserName;

var hubUrl = "https://signalrapi20260114081342-fxbnfscea2bjb2am.swedencentral-01.azurewebsites.net/chathub";
var hubUrl2 = "http://localhost:5149/chathub";

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

ConsoleColor myColor = ConsoleColor.Green;
ConsoleColor othersColor = ConsoleColor.DarkRed;
int windowWidth = Console.WindowWidth;


connection.On<string>("ReceiveMessage", message =>
{
    if (!message.Contains("bob"))
    {
        Console.WriteLine($"Received: {message}");
    }
});


connection.On<string>("UsersUpdated", user =>
{
    Console.WriteLine(user + " has entered the chat");
});

await connection.StartAsync();
Console.WriteLine("Connected to SignalR Hub");
await connection.InvokeAsync("Register", username);

while (true)
{
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;
    var now = DateTime.Now.ToShortTimeString();
    await connection.InvokeAsync("SendMessage", now +"\n"+username +": " + input);
}
