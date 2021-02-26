using System;
using System.Collections.Generic;
using System.Net;

namespace ETModel
{
	[MessageHandler(AppType.Realm)]
	public class C2R_RegisterHandler : AMRpcHandler<C2R_Register, R2C_Register>
	{
		protected override async ETTask Run(Session session, C2R_Register request, R2C_Register response, Action reply)
		{
            try
            {
                DBProxyComponent dbProxy = Game.Scene.GetComponent<DBProxyComponent>();
                List<ComponentWithId> result = await dbProxy.Query<PlayerInfo>($"{{Account:'{request.Account}'}}");
                if (result.Count > 0)
                {
                    response.Error = ErrorCode.ERR_AccountRepeat;
                    reply();
                    return;
                }

                AccountInfo newAccount = ComponentFactory.CreateWithId<AccountInfo, string ,string>(IdGenerater.GenerateId(), request.Account, request.Password);
                await dbProxy.Save(newAccount);

                PlayerInfo newPlayer = ComponentFactory.CreateWithId<PlayerInfo, string>(newAccount.Id, request.Name);
                await dbProxy.Save(newPlayer);

                reply();
            }
            catch (Exception e)
            {
                response.Error = ErrorCode.ERR_RpcFail;
                response.Message = e.ToString();
                reply();
            }
        }
    }
}