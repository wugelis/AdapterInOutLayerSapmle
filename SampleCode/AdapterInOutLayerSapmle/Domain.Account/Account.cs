using System.Text.RegularExpressions;

namespace Domain
{
    /// <summary>
    /// 帳號 Entity
    /// </summary>
    public class Account
    {
        private string _accountId;
        private string _name;

        public void SetAccountId(string accountId)
        {
            _accountId = accountId;
        }

        public void SetName(string name)
        {
            _name = name;
        }
        /// <summary>
        /// 檢核 AID 邏輯（假設 accountId = AID）
        /// </summary>
        /// <returns></returns>
        public bool CheckIsAID()
        {
            return new Regex("^[A-Z]{1}[1-2]{1}[0-9]{8}$").IsMatch(_accountId);
        }
    }
}