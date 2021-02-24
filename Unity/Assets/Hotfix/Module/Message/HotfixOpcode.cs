using ETModel;
namespace ETHotfix
{
	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	public partial class G2C_TestHotfixMessage : IMessage {}

	[Message(HotfixOpcode.C2M_TestActorRequest)]
	public partial class C2M_TestActorRequest : IActorLocationRequest {}

	[Message(HotfixOpcode.M2C_TestActorResponse)]
	public partial class M2C_TestActorResponse : IActorLocationResponse {}

	[Message(HotfixOpcode.PlayerInfo)]
	public partial class PlayerInfo : IMessage {}

	[Message(HotfixOpcode.C2G_PlayerInfo)]
	public partial class C2G_PlayerInfo : IRequest {}

	[Message(HotfixOpcode.G2C_PlayerInfo)]
	public partial class G2C_PlayerInfo : IResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixOpcode
	{
		 public const ushort G2C_TestHotfixMessage = 10001;
		 public const ushort C2M_TestActorRequest = 10002;
		 public const ushort M2C_TestActorResponse = 10003;
		 public const ushort PlayerInfo = 10004;
		 public const ushort C2G_PlayerInfo = 10005;
		 public const ushort G2C_PlayerInfo = 10006;
	}
}
