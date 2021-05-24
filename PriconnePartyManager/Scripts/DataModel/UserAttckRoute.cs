// ReSharper disable UnusedAutoPropertyAccessor.Global
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PriconnePartyManager.Scripts.DataModel
{
    public class UserAttackRoute
    {
        public string Id { get; set; }

        public IEnumerable<UserParty> RouteParties { get; set; }

        public string Comment { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime UpdateTime { get; set; }

        public UserAttackRoute()
        {
        }

        public UserAttackRoute(UserAttackRoute route)
        {
            Id = route.Id;
            RouteParties = route.RouteParties.Select(x => new UserParty(x));
            Comment = route.Comment;
            CreateTime = route.CreateTime;
            UpdateTime = route.UpdateTime;
        }

        public UserAttackRoute(IEnumerable<UserParty> parties, string comment)
        {
            RouteParties = parties.Select(x => new UserParty(x));
            Comment = comment;
            CreateTime = DateTime.Now;
            UpdateTime = CreateTime;
            Id = GenerateId();
        }

        /// <summary>
        /// 上書き保存
        /// </summary>
        public void Save(IEnumerable<UserParty> parties, string comment)
        {
            RouteParties = parties.Select(x => new UserParty(x));
            Comment = comment;
            UpdateTime = DateTime.Now;
        }

        private string GenerateId()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Serialize(RouteParties, options);
            var unixTime = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var data = $"{json}{unixTime}";
            var hashProvider = new SHA256CryptoServiceProvider();
            var hash = string.Join("", hashProvider.ComputeHash(Encoding.UTF8.GetBytes(data)).Select(x => $"{x:x2}"));
            return hash;
        }
    }
}