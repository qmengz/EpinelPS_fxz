using EpinelPS.Database;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Inventory
{
    [PacketPath("/inventory/equipment/awakening")]
    public class AwakeningEquipment : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqEquipmentAwakening req = await ReadData<ReqEquipmentAwakening>();
            User user = GetUser();

            ResEquipmentAwakening response = new();

            await WriteDataAsync(response);
        }
    }
}
