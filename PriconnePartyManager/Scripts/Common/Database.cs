﻿using System;
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

        private int[] m_UnlockRarity6UnitIds;
        private int[] m_PlayableUnitIds;
        private UnitProfile[] m_UnitProfiles;
        private UnitData[] m_UnitData;

        public event Action<UserParty> OnAddUserParty;
        public event Action<UserParty> OnRemoveUserParty;
            
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
            
            Units = units.ToArray();
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
        
    }
}