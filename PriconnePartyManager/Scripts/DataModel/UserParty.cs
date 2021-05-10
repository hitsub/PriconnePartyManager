namespace PriconnePartyManager.Scripts.DataModel
{
    public class UserParty
    {
        public UserUnit[] UserUnits { get; set; }
        
        /// <summary> ユーザーが任意につけれるタグ </summary>
        public string[] Tags { get; set; }
        
        /// <summary> 推定ダメージ </summary>
        public int EstimateDamage { get; set; }

        public UserParty()
        {
        }

        public UserParty(UserUnit[] userUnits)
        {
            UserUnits = userUnits;
        }
    }
}