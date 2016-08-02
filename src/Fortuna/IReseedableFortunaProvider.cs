namespace Fortuna
{
    internal interface IReseedableFortunaProvider : IPRNGFortunaProvider
    {
        void Reseed(byte[] seed);
    }
}
