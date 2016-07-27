namespace Client
{
    public interface IActivityConnection<TBinder>
    {
        TBinder Binder { get; set; }
        bool IsBound { get; set; }
    }
}