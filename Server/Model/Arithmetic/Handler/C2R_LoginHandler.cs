using System;
using System.Collections.Generic;
using System.Net;

namespace ETModel
{
	[MessageHandler(AppType.Realm)]
	public class C2R_LoginHandler : AMRpcHandler<C2R_Login, R2C_Login>
	{
		protected override async ETTask Run(Session session, C2R_Login request, R2C_Login response, Action reply)
		{
			DBProxyComponent dbProxy = Game.Scene.GetComponent<DBProxyComponent>();
			List<ComponentWithId> result = await dbProxy.Query<AccountInfo>($"{{Account:'{request.Account}',Password:'{request.Password}'}}");
			if (result.Count <= 0)
			{
				response.Error = ErrorCode.ERR_AccountOrPasswordError;
				reply();
				return;
			}

			//已验证通过，可能存在其它地方有登录，要先踢下线
			AccountInfo account = (AccountInfo)result[0];
			//await RealmHelper.KickOutPlayer(account.Id);

			// 随机分配一个Gate
			StartConfig config = Game.Scene.GetComponent<RealmGateAddressComponent>().GetAddress();
			IPEndPoint innerAddress = config.GetComponent<InnerConfig>().IPEndPoint;
			Session gateSession = Game.Scene.GetComponent<NetInnerComponent>().Get(innerAddress);

			// 向gate请求一个key,客户端可以拿着这个key连接gate
			G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await gateSession.Call(new R2G_GetLoginKey() {UserId = account.Id});

			string outerAddress = config.GetComponent<OuterConfig>().Address2;

			response.Address = outerAddress;
			response.Key = g2RGetLoginKey.Key;
			reply();
		}
	}
}