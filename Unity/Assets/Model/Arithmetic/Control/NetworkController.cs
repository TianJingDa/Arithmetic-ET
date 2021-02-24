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
			Game.Scene.AddComponent<ResourcesComponent>();
			Game.Scene.AddComponent<PlayerComponent>();
			Game.Scene.AddComponent<UnitComponent>();

			// 加载配置
			Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
			Game.Scene.AddComponent<ConfigComponent>();
			Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");
			Game.Scene.AddComponent<OpcodeTypeComponent>();
			Game.Scene.AddComponent<MessageDispatcherComponent>();
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

	public async ETVoid OnLoginAsync(string account)
	{
		try
		{
			// 创建一个ETModel层的Session
			Session realmSession = Game.Scene.GetComponent<NetOuterComponent>().Create(GlobalConfigComponent.Instance.GlobalProto.Address);

			R2C_Login r2CLogin = (R2C_Login)await realmSession.Call(new C2R_Login() { Account = account, Password = "111111" });
			realmSession.Dispose();

            // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
            Session gateSession = Game.Scene.GetComponent<NetOuterComponent>().Create(r2CLogin.Address);
            Game.Scene.AddComponent<SessionComponent>().Session = gateSession;

            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await SessionComponent.Instance.Session.Call(new C2G_LoginGate() { Key = r2CLogin.Key });

            Log.Info("登陆gate成功!");
		}
		catch (Exception e)
		{
			Log.Error(e);
		}
	}

}
