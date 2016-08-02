namespace Fortuna.Generator
{
    public interface IGenerator
    {
        void GenerateBytes(byte[] data);

        void Reseed(byte[] seed);
    }
}