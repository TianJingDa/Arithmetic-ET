namespace ETModel
{
    [ObjectSystem]
    public class AccountInfoAwakeSystem : AwakeSystem<AccountInfo, string, string>
    {
        public override void Awake(AccountInfo self, string account, string password)
        {
            self.Awake(account, password);
        }
    }

    public class AccountInfo : Entity
    {
        public string Account { get; private set; }

        public string Password { get; private set; }

        public void Awake(string account, string password)
        {
            Account = account;
            Password = password;
        }
    }
}
