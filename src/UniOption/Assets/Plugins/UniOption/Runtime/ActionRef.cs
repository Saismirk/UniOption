using Cysharp.Threading.Tasks;

namespace UniOption {
    public delegate void    ActionRef<in T, TContext>(T value, ref TContext context);
    public delegate void    ActionRef<TContext>(ref TContext context);
    public delegate UniTask FuncRef<in T, TContext>(T value, ref TContext context);
    public delegate UniTask FuncRef<TContext>(ref TContext context);
}