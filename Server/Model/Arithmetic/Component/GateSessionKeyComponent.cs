using System.Collections.Generic;

namespace ETModel
{
	public class GateSessionKeyComponent : Component
	{
		private readonly Dictionary<long, long> sessionKey = new Dictionary<long, long>();
		
		public void Add(long key, long playerId)
		{
			this.sessionKey.Add(key, playerId);
			this.TimeoutRemoveKey(key).Coroutine();
		}

		public long Get(long key)
		{
			if(sessionKey.TryGetValue(key, out long playerId))
            {
				return playerId;
			}
			return 0;
		}

		public void Remove(long key)
		{
			this.sessionKey.Remove(key);
		}

		private async ETVoid TimeoutRemoveKey(long key)
		{
			await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000);
			this.sessionKey.Remove(key);
		}
	}
}
