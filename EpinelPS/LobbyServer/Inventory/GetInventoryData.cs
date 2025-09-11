using EpinelPS.Data;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Inventory
{
    [PacketPath("/inventory/get")]
    public class GetInventoryData : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqGetInventoryData req = await ReadData<ReqGetInventoryData>();
            User user = GetUser();

            ResGetInventoryData response = new();

            foreach (ItemData item in user.Items)
            {
                response.Items.Add(new NetUserItemData() { Count = item.Count, Tid = item.ItemType, Csn = item.Csn, Lv = item.Level, Exp = item.Exp, Corporation = item.Corp, Isn = item.Isn, Position = item.Position });
                
                if (GameData.Instance.ItemHarmonyCubeTable.TryGetValue(item.ItemType, out _))
                {
                    NetUserHarmonyCubeData netHarmonyCube = new()
                    {
                        Isn = item.Isn,
                        Tid = item.ItemType,
                        Lv = item.Level
                    };

                    foreach (long csn in item.CsnList)
                    {
                        netHarmonyCube.CsnList.Add(csn);
                    }

                    if (item.Csn > 0 && !item.CsnList.Contains(item.Csn))
                    {
                        netHarmonyCube.CsnList.Add(item.Csn);
                    }

                    response.HarmonyCubes.Add(netHarmonyCube);
                    response.ArenaHarmonyCubes.Add(netHarmonyCube);
                }
            }

            // TODO: RunAwakeningIsnList, UserRedeems

            await WriteDataAsync(response);
        }
    }
}
