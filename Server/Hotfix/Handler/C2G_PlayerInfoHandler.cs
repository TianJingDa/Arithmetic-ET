using System;
using ETModel;

namespace ETHotfix
{
	// 用来测试消息包含复杂类型，是否产生gc
	[MessageHandler(AppType.Gate)]
	public class C2G_PlayerInfoHandler : AMRpcHandler<C2G_PlayerInfo, G2C_PlayerInfo>
	{
		protected override async ETTask Run(Session session, C2G_PlayerInfo request, G2C_PlayerInfo response, Action reply)
		{

			reply();
			await ETTask.CompletedTask;
		}
	}
}
