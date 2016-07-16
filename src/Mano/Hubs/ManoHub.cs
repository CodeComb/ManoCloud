using Microsoft.AspNetCore.SignalR;

namespace Mano.Hubs
{
    public class ManoHub : Hub
    {
        public void JoinGroup(string name)
        {
            Groups.Add(Context.ConnectionId, name);
        }
    }
}
