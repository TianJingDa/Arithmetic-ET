/// <summary>
/// 所有控制器的基类
/// </summary>
public abstract class Controller
{
    protected ControllerID id;
    protected virtual void InitController() { }//初始化控制器
}
