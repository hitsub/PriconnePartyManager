using System;
using System.Collections.Generic;
using System.Linq;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Sql;
using PriconnePartyManager.Scripts.Sql.Model;

namespace PriconnePartyManager.Scripts.Common
{
    /// <summary>
    /// 各所で使うデータの管理を行うクラス
    /// </summary>
    public class Database : Singleton<Database>
    {
        public Unit[] Units { get; private set; }
        public List<UserParty> UserParties { get; private set; }
        
        public List<UserAttackRoute> UserAttackRoutes { get; private set; }

        private int[] m_UnlockRarity6UnitIds;
        private int[] m_PlayableUnitIds;
        private UnitProfile[] m_UnitProfiles;
        private UnitData[] m_UnitData;

        public event Action<UserParty> OnAddUserParty;
        public event Action<UserParty> OnRemoveUserParty;
        public event Action<UserParty> OnChangeUserParty;

        public event Action<UserAttackRoute> OnAddUserAttackRoute;
        public event Action<UserAttackRoute> OnRemoveUserAttackRoute;
        public event Action<UserAttackRoute> OnChangeUserAttackRoute;
            
        public Database()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var sqlConnector = new SqlConnector();

            m_UnitProfiles = sqlConnector.UnitProfiles.ToArray();
            m_UnitData = sqlConnector.UnitData.ToArray();
            m_UnlockRarity6UnitIds = sqlConnector.UnlockRarity6
                .Where(x => x.UnlockLevel > 0)
                .Select(x => x.UnitId)
                .Distinct().ToArray();
            m_PlayableUnitIds = sqlConnector.UnitPlayables
                .Select(x => x.UnitId)
                .Distinct().ToArray();
            
            sqlConnector.Dispose();
            
            CreateUnits();
            LoadParties();
            LoadAttackRoutes();
        }

        private void CreateUnits()
        {
            var iconLoader = new IconLoader();
            var units = new List<Unit>();
            foreach (var profile in m_UnitProfiles)
            {
                if (!m_PlayableUnitIds.Contains(profile.UnitId))
                {
                    continue;
                }
                var isUnlock6 = m_UnlockRarity6UnitIds.Contains(profile.UnitId);
                var data = m_UnitData.SingleOrDefault(x => x.UnitId == profile.UnitId);
                var unit = new Unit(profile, data, iconLoader.LoadIcon(profile.UnitId, isUnlock6), isUnlock6);
                units.Add(unit);
            }
            
            Units = units.OrderBy(x => x.Order).ToArray();
        }

        private void LoadParties()
        {
            UserParties = new List<UserParty>();
            var exists = FileManager.I.LoadJson<UserParty[]>();
            if (exists?.Length > 0)
            {
                UserParties.AddRange(exists);
            }
        }

        private void LoadAttackRoutes()
        {
            UserAttackRoutes = new List<UserAttackRoute>();
            var exists = FileManager.I.LoadJson<UserAttackRoute[]>();
            if (exists?.Length > 0)
            {
                UserAttackRoutes.AddRange(exists);
            }
        }

        public void SaveParty(UserParty party)
        {
            if (UserParties.Contains(party))
            {
                var index = UserParties.FindIndex(x => x.Id == party.Id);
                UserParties[index] = party;
                OnChangeUserParty?.Invoke(party);
                FileManager.I.SaveJson(UserParties.ToArray());
            }
            else
            {
                AddParty(party);
            }
        }

        public void AddParty(UserParty party)
        {
            UserParties.Add(party);
            OnAddUserParty?.Invoke(party);
            FileManager.I.SaveJson(UserParties.ToArray());
        }

        public void RemoveParty(UserParty party)
        {
            UserParties.Remove(party);
            OnRemoveUserParty?.Invoke(party);
            FileManager.I.SaveJson(UserParties.ToArray());
        }

        public void AddAttackRoute(UserAttackRoute route)
        {
            UserAttackRoutes.Add(route);
            OnAddUserAttackRoute?.Invoke(route);
            FileManager.I.SaveJson(UserAttackRoutes.ToArray());
        }

        public void RemoveAttackRoute(UserAttackRoute route)
        {
            UserAttackRoutes.Remove(route);
            OnRemoveUserAttackRoute?.Invoke(route);
            FileManager.I.SaveJson(UserAttackRoutes.ToArray());
        }

        public void SaveAttackRoute(UserAttackRoute route)
        {
            if (UserAttackRoutes.Contains(route))
            {
                var index = UserAttackRoutes.FindIndex(x => x.Id == route.Id);
                UserAttackRoutes[index] = route;
                OnChangeUserAttackRoute?.Invoke(route);
                FileManager.I.SaveJson(UserAttackRoutes.ToArray());
            }
            else
            {
                AddAttackRoute(route);
            }
        }
        
    }
}