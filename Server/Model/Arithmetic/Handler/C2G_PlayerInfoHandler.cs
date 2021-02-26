using System;

namespace ETModel
{
	[MessageHandler(AppType.Gate)]
	public class C2G_PlayerInfoHandler : AMRpcHandler<C2G_PlayerInfo, G2C_PlayerInfo>
	{
		protected override async ETTask Run(Session session, C2G_PlayerInfo request, G2C_PlayerInfo response, Action reply)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;
			DBProxyComponent dbProxyComponent = Game.Scene.GetComponent<DBProxyComponent>();
			PlayerInfo playerInfo = await dbProxyComponent.Query<PlayerInfo>(player.Id);

			response.Name = playerInfo.Name;
			response.PlayerId = player.Id;
			reply();
			await ETTask.CompletedTask;
		}
	}
}
