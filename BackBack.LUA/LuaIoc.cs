using RF.WPF;

namespace BackBack.LUA
{
    public class LuaIoc : IocBase
    {
        protected override void Setup() => Bind<Lua>().ToSelf();
    }
}
