using EpinelPS.Database;
using EpinelPS.Utils;
using EpinelPS.Data;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;

namespace EpinelPS.LobbyServer.Inventory
{
    [PacketPath("/inventory/getharmonycube")]
    public class GetHarmonyCube : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqGetHarmonyCube req = await ReadData<ReqGetHarmonyCube>();
            User user = GetUser();

            ResGetHarmonyCube response = new();
            List<ItemData> harmonyCubes = [.. user.Items.Where(item =>
                GameData.Instance.ItemHarmonyCubeTable.ContainsKey(item.ItemType))];

            // 获取数据 AttractiveCounselCharacterRecord

            Dictionary<int, CharacterRecord> accts = GameData.Instance.CharacterTable;
            List<string> corporation = [];
            List<int> name_code = [];
            foreach (KeyValuePair<int, CharacterRecord> data in accts)
            {
                corporation.Add(data.Value.corporation);
                name_code.Add(data.Value.name_code);
                Logging.WriteLine($"{data.Value} = {GameData.Instance.ObjectToJson(data.Value)}", LogType.Debug);
            }
            Logging.WriteLine($"corporation = {GameData.Instance.ObjectToJson(corporation.Distinct().ToList())}", LogType.Debug);
            Logging.WriteLine($"name_code = {GameData.Instance.ObjectToJson(name_code.Distinct().ToList())}", LogType.Debug);
            // using ProgressBar progress = new();
            // var data = await GameData.Instance.LoadZip2<Dictionary<int, string>>("CharacterTable.json", progress);
            // Logging.WriteLine($"{ObjectToJson(data)}", LogType.Debug);

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
                }
            }


            await WriteDataAsync(response);
        }
        // public static string ObjectToJson(object obj)
        // {
        //     DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        //     MemoryStream stream = new MemoryStream();
        //     serializer.WriteObject(stream, obj);
        //     byte[] dataBytes = new byte[stream.Length];
        //     stream.Position = 0;
        //     stream.Read(dataBytes, 0, (int)stream.Length);
        //     return Encoding.UTF8.GetString(dataBytes);
        // }

    }
}
