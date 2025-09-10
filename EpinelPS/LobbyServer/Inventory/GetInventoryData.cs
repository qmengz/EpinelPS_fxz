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

            // HarmonyCubes
            List<ItemData> harmonyCubes = user.Items.Where(item =>
                GameData.Instance.ItemHarmonyCubeTable.ContainsKey(item.ItemType)).ToList();

            foreach (ItemData item in user.Items)
            {
                foreach (ItemData harmonyCube in harmonyCubes)
                {
                    if (GameData.Instance.ItemHarmonyCubeTable.TryGetValue(harmonyCube.ItemType, out ItemHarmonyCubeRecord? harmonyCubeData))
                    {
                        if (item.ItemType != harmonyCube.ItemType)
                        {
                            response.Items.Add(new NetUserItemData() { Count = item.Count, Tid = item.ItemType, Csn = item.Csn, Lv = item.Level, Exp = item.Exp, Corporation = item.Corp, Isn = item.Isn, Position = item.Position });
                        }
                    }
                }
            }

            

            foreach (ItemData harmonyCube in harmonyCubes)
                {
                    if (GameData.Instance.ItemHarmonyCubeTable.TryGetValue(harmonyCube.ItemType, out ItemHarmonyCubeRecord? harmonyCubeData))
                    {
                        NetUserHarmonyCubeData netHarmonyCube = new()
                        {
                            Isn = harmonyCube.Isn,
                            Tid = harmonyCube.ItemType,
                            Lv = harmonyCube.Level
                        };

                        foreach (long csn in harmonyCube.CsnList)
                        {
                            netHarmonyCube.CsnList.Add(csn);
                        }

                        if (harmonyCube.Csn > 0 && !harmonyCube.CsnList.Contains(harmonyCube.Csn))
                        {
                            netHarmonyCube.CsnList.Add(harmonyCube.Csn);
                        }

                        response.HarmonyCubes.Add(netHarmonyCube);
                        response.ArenaHarmonyCubes.Add(netHarmonyCube);
                        
                    }
                }

            // TODO: HarmonyCubes, RunAwakeningIsnList, UserRedeems

            await WriteDataAsync(response);
        }
    }
}
