using System.Net;
using System.Net.Http.Headers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

//VARIABLES

//Bot created by Token 

HttpClient client = new HttpClient();
Console.WriteLine("Ingrese el token del bot con formato #########:*****************");
string botToken = Console.ReadLine() + "";


Console.WriteLine("Ingrese la url del webhook que responderá al usuario en Telegram");
string webHookUrl = Console.ReadLine() + "";
TelegramBotClient botClient;

//Time
int year;
int month;
int day;
int hour;
int minute;
int second;

//Messages and user Info
long chatId = 0;
string messageText;
int messageId;
string firstName;
string lastName;
long id;
Message sentMessage;
if (!string.IsNullOrEmpty(botToken))
{

    botClient = new TelegramBotClient(botToken);


    //Read time and save variables
    year = int.Parse(DateTime.UtcNow.Year.ToString());
    month = int.Parse(DateTime.UtcNow.Month.ToString());
    day = int.Parse(DateTime.UtcNow.Day.ToString());
    hour = int.Parse(DateTime.UtcNow.Hour.ToString());
    minute = int.Parse(DateTime.UtcNow.Minute.ToString());
    second = int.Parse(DateTime.UtcNow.Second.ToString());
    Console.WriteLine("Fecha y hora de Inicio: " + year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second);

    using var cts = new CancellationTokenSource();

    var receiverOptions = new ReceiverOptions() { AllowedUpdates = { } };

    botClient.StartReceiving(
        HandleUpdateAsync,
        HandleErrorAsync,
        receiverOptions,
        cancellationToken: cts.Token);

    var me = await botClient.GetMeAsync();

    Console.WriteLine($"\nEl Bot {me.Username} se ha inicializado correctamente \n");
    Console.ReadKey();
    cts.Cancel();
}
else
{
    Console.WriteLine("El token no puede estar vacio");
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message)
    {
        return;
    }
    if (update.Message!.Type != MessageType.Text)
    {
        return;
    }
    chatId = update.Message.Chat.Id;
    messageText = update.Message.Text;
    messageId = update.Message.MessageId;
    firstName = update.Message.From.FirstName;
    lastName = update.Message.From.LastName;
    id = update.Message.From.Id;
    year = update.Message.Date.Year;
    month = update.Message.Date.Month;
    day = update.Message.Date.Day;
    hour = update.Message.Date.Hour;
    minute = update.Message.Date.Minute;
    second = update.Message.Date.Second;

    Console.WriteLine("\nMensaje entrante Fecha y hora: " + year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second);
    Console.WriteLine($"El usuario {firstName} {lastName} dice: \n{messageText}\n\n");

    if (messageText != null && int.Parse(day.ToString()) >= day && int.Parse(hour.ToString()) >= hour && int.Parse(minute.ToString()) >= minute && int.Parse(second.ToString()) >= second - 10)
    {
        //REQUEST A TU API 
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        var content = new FormUrlEncodedContent(new[]
        {
                    new KeyValuePair<string, string>("chatId", chatId.ToString()),
                    new KeyValuePair<string, string>("chatName", $"{firstName} {lastName}"),
                    new KeyValuePair<string, string>("messageId", $"{messageId}"),
                    new KeyValuePair<string, string>("messageText", $"{messageText}"),
                    new KeyValuePair<string, string>("messageDate", $"{day}/{month}/{year} {hour}:{minute}:{second}" )
            });
        client.PostAsync(webHookUrl, content).GetAwaiter();
    }
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    return Task.CompletedTask;
}