using ETModel;
namespace ETHotfix
{
	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	public partial class G2C_TestHotfixMessage : IMessage {}

	[Message(HotfixOpcode.C2M_TestActorRequest)]
	public partial class C2M_TestActorRequest : IActorLocationRequest {}

	[Message(HotfixOpcode.M2C_TestActorResponse)]
	public partial class M2C_TestActorResponse : IActorLocationResponse {}

}
namespace ETHotfix
{
	public static partial class HotfixOpcode
	{
		 public const ushort G2C_TestHotfixMessage = 10001;
		 public const ushort C2M_TestActorRequest = 10002;
		 public const ushort M2C_TestActorResponse = 10003;
	}
}
