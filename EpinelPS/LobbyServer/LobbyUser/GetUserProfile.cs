﻿using EpinelPS.Data;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.LobbyUser
{
    [PacketPath("/User/GetProfile")]
    public class GetUserProfile : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            ReqGetProfileData req = await ReadData<ReqGetProfileData>();
            User callingUser = GetUser();
            User? user = GetUser((ulong)req.TargetUsn);
            ResGetProfileData response = new();

            if (user != null)
            {
                response.Data = new NetProfileData
                {
                    User = LobbyHandler.CreateWholeUserDataFromDbUser(user),
                    LastActionAt = DateTimeOffset.UtcNow.Ticks,
                };
                response.Data.CharacterCount.Add(new NetCharacterCount() { Count = user.Characters.Count });
                response.Data.InfraCoreLv = user.InfraCoreLvl;
                response.Data.LastCampaignNormalStageId = user.LastNormalStageCleared;
                response.Data.LastCampaignHardStageId = user.LastHardStageCleared;
                response.Data.OutpostOpenState = user.MainQuestData.ContainsKey(25);
                if (user.Currency.TryGetValue(CurrencyType.UserExp, out long exp))
                {
                    response.Data.Exp = (int)exp;
                }
                response.Data.SynchroLv = user.SynchroDeviceLevel;
                response.Data.SynchroSlotCount = user.SynchroSlots.Count(x => x.CharacterSerialNumber > 0);

                foreach (var rrrp in user.ResearchProgress)
                {
                    response.Data.Recycle.Add(new NetUserRecycleRoomData()
                    {
                        Tid = rrrp.Key,
                        Lv = rrrp.Value.Level,
                        Exp = rrrp.Value.Exp
                    });
                }

                response.Data.LastTacticAcademyClass = user.CompletedTacticAcademyLessons.Max();
                response.Data.LastTacticAcademyLesson = user.CompletedTacticAcademyLessons.Max();
                response.Data.JukeboxCount = user.JukeboxBgm.Count;
                response.Data.CostumeLv = 1;
                response.Data.CostumeCount = 2;
                response.Data.ProfileFrameHistoryType = NetProfileFrameHistoryType.Representative;
                // response.Data.ProfileFrames.Add(new NetProfileFrameData() { });
                // response.Data.RepresentativeProfileFrames.Add(new NetProfileRepresentativeFrame() { });
                response.Data.RecentAcquireFilterTypes.Add(1);
                response.Data.SimRoomOverclockHighScoreLevel = 1;
                // response.Data.SimRoomOverclockHighScoreData.Add(new NetProfileSimRoomOverclockHighScoreData() { });
                response.Data.Desc = "这就是个测试";

                int slot = 0;
                foreach (long csn in user.RepresentationTeamDataNew)
                {
                    CharacterModel? c = user.GetCharacterBySerialNumber(csn);

                    if (c != null)
                    {
                        slot++;
                        response.Data.ProfileTeam.Add(new NetProfileTeamSlot() { Slot = slot, Default = new() { CostumeId = c.CostumeId, Csn = c.Csn, Grade = c.Grade, Lv = c.Level, Skill1Lv = c.Skill1Lvl, Skill2Lv = c.Skill2Lvl, Tid = c.Tid, UltiSkillLv = c.UltimateLevel } });
                    }
                }
            }

            await WriteDataAsync(response);
        }
    }
}
