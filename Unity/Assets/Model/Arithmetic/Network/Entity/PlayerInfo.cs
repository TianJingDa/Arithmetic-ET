namespace ETModel
{
    [ObjectSystem]
    public class PlayerInfoAwakeSystem1 : AwakeSystem<PlayerInfo, string>
    {
        public override void Awake(PlayerInfo self, string name)
        {
            self.Awake(name);
        }
    }

    [ObjectSystem]
    public class PlayerInfoAwakeSystem2 : AwakeSystem<PlayerInfo, G2C_PlayerInfo>
    {
        public override void Awake(PlayerInfo self, G2C_PlayerInfo info)
        {
            self.Awake(info);
        }
    }

    public class PlayerInfo : Entity
    {
        public string Name { get; set; } = "";

        public void Awake(string name)
        {
            Name = name;
        }

        public void Awake(G2C_PlayerInfo info)
        {
            Name = info.Name;
        }
    }
}
