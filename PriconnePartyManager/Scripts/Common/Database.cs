using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
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
        private const string RediveUrl = "https://redive.estertion.win/";
        private const string RediveJsonName = "last_version_jp.json";
        private const string RediveDatabaseName = "redive_jp.db";
        
        public Unit[] Units { get; private set; }
        public List<UserParty> UserParties { get; private set; }
        
        public List<UserAttackRoute> UserAttackRoutes { get; private set; }
        
        public Tag[] Tags { get; private set; }
            
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

        private IconLoader m_IconLoader;
            
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
            LoadTags();
        }

        private void CreateUnits()
        {
            if (m_IconLoader == null)
            {
                m_IconLoader= new IconLoader();
            }
            var units = new List<Unit>();
            foreach (var profile in m_UnitProfiles)
            {
                if (!m_PlayableUnitIds.Contains(profile.UnitId))
                {
                    continue;
                }
                var isUnlock6 = m_UnlockRarity6UnitIds.Contains(profile.UnitId);
                var data = m_UnitData.SingleOrDefault(x => x.UnitId == profile.UnitId);
                var unit = new Unit(profile, data, m_IconLoader.LoadIcon(profile.UnitId, isUnlock6), isUnlock6);
                units.Add(unit);
            }

            if (units.Any(x => x.Icon == null))
            {
                MessageBox.Show("画像が無いキャラがあります。\nメニュー>データベース>キャラアイコン更新 を実行してください。");
            }
            
            Units = units.OrderBy(x => x.Order).ToArray();
        }

        private void LoadParties()
        {
            UserParties = new List<UserParty>();
            var exists = FileManager.I.LoadJsonFromFile<UserParty[]>();
            if (exists?.Length > 0)
            {
                UserParties.AddRange(exists);
            }
        }

        private void LoadAttackRoutes()
        {
            UserAttackRoutes = new List<UserAttackRoute>();
            var exists = FileManager.I.LoadJsonFromFile<UserAttackRoute[]>();
            if (exists?.Length > 0)
            {
                UserAttackRoutes.AddRange(exists);
            }
        }

        private void LoadTags()
        {
            Tags = FileManager.I.LoadJsonFromFile<Tag[]>();
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

        public void UpdateDatabase()
        {
            const string path = "./data/last_version_jp.json";
            RediveVersionData currentVersion = null;
            if (File.Exists(path))
            {
                currentVersion = FileManager.I.LoadJsonFromFile<RediveVersionData>(path);
            }
            
            var webClient = new WebClient();
            string updateRediveString;
            
            try
            {
                updateRediveString = webClient.DownloadString(RediveUrl + RediveJsonName);
            }
            catch
            {
                MessageBox.Show("Databaseのバージョン情報の取得に失敗しました。");
                return;
            }
            if (string.IsNullOrEmpty(updateRediveString))
            {
                MessageBox.Show("Databaseのバージョン情報の取得に失敗しました。");
                return;
            }

            var latestVersion = FileManager.I.LoadJson<RediveVersionData>(updateRediveString);

            if (latestVersion.TruthVersion == currentVersion?.TruthVersion)
            {
                MessageBox.Show("Databaseは最新版です。");
                return;
            }
            
            var databaseUrl = RediveUrl + "db/" + RediveDatabaseName + ".br";
            var databaseBrotliSavePath = Path.Combine("data", RediveDatabaseName + ".br");
            var databaseSavePath = Path.Combine("data", RediveDatabaseName);

            try
            {
                webClient.DownloadFile(databaseUrl, databaseBrotliSavePath);
            }
            catch
            {
                MessageBox.Show("Databaseの取得に失敗しました。");
                return;
            }
            FileManager.I.DecompressBrotli(databaseBrotliSavePath, databaseSavePath, true);
            File.WriteAllText(path, updateRediveString);
            
            InitializeDatabase();
        }

        public void RefreshUnitIcons()
        {
            foreach (var unit in Units)
            {
                unit.Icon = m_IconLoader.LoadIcon(unit.Id, unit.IsUnlockRarity6);
            }
        }
        
    }
}