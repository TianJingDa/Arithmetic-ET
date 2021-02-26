using System;

namespace ETModel
{
	[MessageHandler(AppType.Gate)]
	public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate>
	{
		protected override async ETTask Run(Session session, C2G_LoginGate request, G2C_LoginGate response, Action reply)
		{
			long playerId = Game.Scene.GetComponent<GateSessionKeyComponent>().Get(request.Key);
			if (playerId == 0)
			{
				response.Error = ErrorCode.ERR_ConnectGateKeyError;
				reply();
				return;
			}
			Player player = ComponentFactory.CreateWithId<Player>(playerId);
			Game.Scene.GetComponent<PlayerComponent>().Add(player);
			session.AddComponent<SessionPlayerComponent>().Player = player;
			session.AddComponent<MailBoxComponent, string>(MailboxType.GateSession);

			response.PlayerId = player.Id;
			reply();

			await ETTask.CompletedTask;
		}
	}
}