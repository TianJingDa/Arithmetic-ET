namespace ETModel
{
	[ObjectSystem]
	public class SessionPlayerComponentDestroySystem : DestroySystem<SessionPlayerComponent>
	{
		public override void Destroy(SessionPlayerComponent self)
		{
			DestroyAsync(self).Coroutine();
		}

		private static async ETVoid DestroyAsync(SessionPlayerComponent self)
        {
			// 发送断线消息
			if(self.Player.UnitId != 0)
            {
				ActorLocationSender actorLocationSender = await Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(self.Player.UnitId);
				actorLocationSender.Send(new G2M_SessionDisconnect()).Coroutine();
			}
			Game.Scene.GetComponent<PlayerComponent>()?.Remove(self.Player.Id);
		}
	}
}