// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PriconnePartyManager.Scripts.DataModel
{
    public class UserParty
    {
        public string Id { get; set; }
        public IEnumerable<UserUnit> UserUnits { get; set; }
        
        /// <summary>　補足情報やTLなど　</summary>
        public string Comment { get; set; }
        
        /// <summary> ユーザーが任意につけれるタグ </summary>
        public int[] Tags { get; set; }
        
        /// <summary> 推定ダメージ </summary>
        public string EstimateDamage { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime UpdateTime { get; set; }

        public UserParty()
        {
        }

        public UserParty(UserParty party)
        {
            Id = party.Id;
            UserUnits = party.UserUnits.Select(x => new UserUnit(x));
            Comment = party.Comment;
            Tags = party.Tags?.ToArray();
            EstimateDamage = party.EstimateDamage;
            CreateTime = party.CreateTime;
            UpdateTime = party.UpdateTime;
        }

        public UserParty(IEnumerable<UserUnit> userUnits, string comment = null, string estimateDamage = null, int[] tags = null)
        {
            UserUnits = userUnits.Select(x => new UserUnit(x));
            Comment = comment;
            EstimateDamage = estimateDamage;
            Id = GenerateId();
            Tags = tags;
            CreateTime = DateTime.Now;
            UpdateTime = CreateTime;
        }

        public void UpdateData(IEnumerable<UserUnit> userUnits, string comment = null, string estimateDamage = null, int[] tags = null)
        {
            UserUnits = userUnits;
            Comment = comment;
            EstimateDamage = estimateDamage;
            Tags = tags;
            UpdateTime = DateTime.Now;
        }

        /// <summary>
        /// ユニークなID生成
        /// UserUnitの配列とパーティーの作成日時から生成
        /// </summary>
        private string GenerateId()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Serialize(UserUnits, options);
            var unixTime = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var data = $"{json}{unixTime}";
            var hashProvider = new SHA256CryptoServiceProvider();
            var hash = string.Join("", hashProvider.ComputeHash(Encoding.UTF8.GetBytes(data)).Select(x => $"{x:x2}"));
            return hash;
        }
    }
}