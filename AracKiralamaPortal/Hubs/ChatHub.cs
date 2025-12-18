using Microsoft.AspNetCore.SignalR;

namespace AracKiralamaPortal.Hubs
{


    public class ChatHub : Hub
    {
        // İstemciden mesaj alındığında çağrılacak metot
        public async Task SendMessage(string user, string message)
        {
            // Tüm istemcilere mesaj gönderme
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

}