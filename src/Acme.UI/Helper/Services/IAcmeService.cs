namespace Acme.UI.Helper.Services
{
    public interface IAcmeService<TInput, TReturn>
    {
        TReturn Process(TInput arg);
    }
}