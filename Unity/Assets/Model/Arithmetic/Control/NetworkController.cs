using System;
using ETModel;
using System.Threading;

public sealed class NetworkController : Controller
{
    #region C#单例
    private static NetworkController instance = null;
    private NetworkController()
    {
        base.id = ControllerID.NetworkController;
        MyDebug.LogWhite("Loading Controller:" + id.ToString());
    }
    public static NetworkController Instance
    {
        get { return instance ?? (instance = new NetworkController()); }
    }
	#endregion

	public void Start()
	{
		try
		{
			SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
			Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

			Game.Scene.AddComponent<TimerComponent>();
			Game.Scene.AddComponent<GlobalConfigComponent>();
			Game.Scene.AddComponent<NetOuterComponent>();
			//Game.Scene.AddComponent<ResourcesComponent>();
			Game.Scene.AddComponent<PlayerComponent>();
			Game.Scene.AddComponent<UnitComponent>();

			// 加载配置
			//Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
			//Game.Scene.AddComponent<ConfigComponent>();
			//Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
			Game.Scene.AddComponent<OpcodeTypeComponent>();
			Game.Scene.AddComponent<MessageDispatcherComponent>();
			Game.Scene.AddComponent<SessionComponent>();
		}
		catch (Exception e)
		{
			Log.Error(e);
		}
	}

	public void Update()
	{
		OneThreadSynchronizationContext.Instance.Update();
		Game.EventSystem.Update();
	}

	public void LateUpdate()
	{
		Game.EventSystem.LateUpdate();
	}

	public void OnApplicationQuit()
	{
		Game.Close();
	}

	public async ETTask<int> LoginAsync(string account, string password)
	{
		Session realmSession = Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

		R2C_Login r2CLogin = (R2C_Login)await realmSession.Call(new C2R_Login() { Account = account, Password = password });
		realmSession.Dispose();
		if(r2CLogin.Error != ErrorCode.ERR_Success)
        {
			return r2CLogin.Error;
        }
		MyDebug.LogGreen("Realm Success");

		Session gateSession = Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);

		G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
		if(g2CLoginGate.Error != ErrorCode.ERR_Success)
        {
			gateSession.Dispose();
			return g2CLoginGate.Error;
        }
		SessionComponent.Instance.Session = gateSession;
		MyDebug.LogGreen("Gate Success");

		Player player = ComponentFactory.CreateWithId<Player>(g2CLoginGate.PlayerId);
        PlayerComponent playerComponent = Game.Scene.GetComponent<PlayerComponent>();
        playerComponent.MyPlayer = player;

		return ErrorCode.ERR_Success;
	}

	public async ETTask<PlayerInfo> GetPlayerInfo()
    {
		G2C_PlayerInfo g2CPlayerInfo = (G2C_PlayerInfo) await SessionComponent.Instance.Session.Call(new C2G_PlayerInfo());
		PlayerInfo playerInfo = ComponentFactory.CreateWithId<PlayerInfo, G2C_PlayerInfo>(g2CPlayerInfo.PlayerId, g2CPlayerInfo);
		return playerInfo;
	}

	public async ETTask<int> RegisterAsync(string playerName, string account, string password)
    {
        Session session = Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);
		R2C_Register r2C_Register = (R2C_Register)await session.Call(new C2R_Register() {Name = playerName, Account = account, Password = password });
        session.Dispose();

        return r2C_Register.Error;
	}
}
