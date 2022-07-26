using UnityEngine;
using System.Collections;

using Pico.Platform.Models;

namespace Pico.Platform.Samples.Game
{
    public class GameDebugLog
    {
        public static string GetRoomLogData(Models.Room room)
        {
            string str = "Room[\nDataStore:\n";
            foreach (var item in room.DataStore)
            {
                str += $"DataStore key: {item.Key}, value: {item.Value}\n";
            }
            str += $"Description: {room.Description}, ID: {room.RoomId}, IsMembershipLocked: {room.IsMembershipLocked}, JoinPolicy: {room.RoomJoinPolicy}, " +
                $"Joinability: {room.RoomJoinability}, MaxUsers: {room.MaxUsers}, Type: {room.RoomType}\n";
            if (room.OwnerOptional == null)
            {
                str += "OwnerOptional is null\n";
            }
            else
            {
                str += $"{GetUserLogData(room.OwnerOptional)}\n";
            }
            if (room.UsersOptional == null)
            {
                str += "UsersOptional is null\n";
            }
            else
            {
                str += $"{GetUserListLogData(room.UsersOptional)}\n";
            }
            return str + "\n]Room";
        }

        public static string GetMatchmakingAdminSnapshotLogData(MatchmakingAdminSnapshot obj)
        {
            return $"MatchmakingAdminSnapshot[\nCandidates: {GetMatchmakingAdminSnapshotCandidateListLogData(obj.CandidateList)}\nMyCurrentThreshold: {obj.MyCurrentThreshold}\n]MatchmakingAdminSnapshot";
        }
        public static string GetMatchmakingAdminSnapshotCandidateLogData(MatchmakingAdminSnapshotCandidate obj)
        {
            return $"MatchmakingAdminSnapshotCandidate[\nCanMatch: {obj.CanMatch}, MyTotalScore: {obj.MyTotalScore}, TheirCurrentThreshold: {obj.TheirCurrentThreshold}\n]MatchmakingAdminSnapshotCandidate";
        }

        public static string GetMatchmakingAdminSnapshotCandidateListLogData(MatchmakingAdminSnapshotCandidateList obj)
        {
            string log = $"MatchmakingAdminSnapshotCandidateList[\nCount: {obj.Count}";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetMatchmakingAdminSnapshotCandidateLogData(item)}\n";
            }
            return log + "]MatchmakingAdminSnapshotCandidateList";
        }

        public static string GetMatchmakingEnqueueResultLogData(MatchmakingEnqueueResult obj)
        {
            string log = "MatchmakingEnqueueResult[\n";
            if (obj.AdminSnapshotOptional == null)
            {
                log += "AdminSnapshotOptional: null\n";
            }
            else
            {
                log += $"AdminSnapshotOptional: {GetMatchmakingAdminSnapshotLogData(obj.AdminSnapshotOptional)}\n";
            }
            log += $"AverageWait: {obj.AverageWait}, MatchesInLastHourCount: {obj.MatchesInLastHourCount}, MaxExpectedWait: {obj.MaxExpectedWait}, " +
                $"Pool: {obj.Pool}, RecentMatchPercentage: {obj.RecentMatchPercentage}";
            return log + "\n]MatchmakingEnqueueResult";
        }
        public static string GetMatchmakingEnqueueResultAndRoomLogData(MatchmakingEnqueueResultAndRoom obj)
        {
            return $"MatchmakingEnqueueResultAndRoom[\nMatchmakingEnqueueResult: {GetMatchmakingEnqueueResultLogData(obj.MatchmakingEnqueueResult)}\nRoom: {GetRoomLogData(obj.Room)}\n]MatchmakingEnqueueResultAndRoom";
        }

        public static string GetMatchmakingRoomLogData(MatchmakingRoom obj)
        {
            return $"MatchmakingRoom[\nRoom: {GetRoomLogData(obj.Room)}\nPingTime: {obj.PingTime}\nHasPingTime: {obj.HasPingTime}\n]MatchmakingRoom";
        }
        public static string GetMatchmakingRoomListLogData(MatchmakingRoomList obj)
        {
            string log = $"MatchmakingRoomList[\nCount: {obj.Count}";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetMatchmakingRoomLogData(item)}\n";
            }
            log += "]MatchmakingRoomList";
            return log;
        }
        public static string GetMatchmakingStatsLogData(MatchmakingStats obj)
        {
            return $"MatchmakingStats[\nDrawCount: {obj.DrawCount}, LossCount: {obj.LossCount}, SkillLevel: {obj.SkillLevel}, SkillMean: {obj.SkillMean}, " +
                $"SkillStandardDeviation: {obj.SkillStandardDeviation}, WinCount: {obj.WinCount}\n]MatchmakingStats";
        }
        public static string GetRoomListLogData(RoomList obj)
        {
            string log = $"RoomList[\nCurIndex: {obj.CurIndex}, PageSize: {obj.PageSize}, Count: {obj.Count}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetRoomLogData(item)}\n";
            }
            return log + "\n]RoomList";
        }
        public static string GetUserLogData(Models.User obj)
        {
            string log =
                $"User[DisplayName: {obj.DisplayName}, ID: {obj.ID}, ImageURL: {obj.ImageUrl}, " +
                $"PresenceStatus: {obj.PresenceStatus}, Gender: {obj.Gender}]User";
            Debug.Log(log);
            return log;
        }
        public static string GetUserListLogData(UserList obj)
        {
            string log = $"UserList[Count: {obj.Count}\n";
            var list = obj.GetEnumerator();
            while (list.MoveNext())
            {
                var item = list.Current;
                log += $"{GetUserLogData(item)}\n";
            }

            return log + "]UserList";
        }
    }
}
