using System.Threading.Tasks;
using OlimpBack.Application.DTO.Realtime;

namespace OlimpBack.Hubs;

public interface IEncryptedChatClient
{
    Task ReceiveEncryptedMessage(RealtimeEncryptedMessageDto message);
    Task MessageDelivered(DeliveryAckDto ack);
    Task MessageRead(ReadAckDto ack);
    Task UserConnected(ConnectionEventDto evt);
    Task UserDisconnected(ConnectionEventDto evt);
}
