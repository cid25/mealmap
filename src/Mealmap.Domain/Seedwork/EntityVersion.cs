namespace Mealmap.Domain.Common
{
    public record EntityVersion
    {
        private byte[] _version;

        public EntityVersion(byte[] version)
        {
            _version = version;
        }

        public EntityVersion(string version)
        {
            _version = ConvertFromString(version);
        }

        public byte[] AsBytes()
        {
            return _version;
        }

        public string AsString()
        {
            return Convert.ToBase64String(_version);
        }

        public void Set(byte[] version)
        {
            _version = version;
        }

        public void Set(string version)
        {
            _version = ConvertFromString(version);
        }

        private static byte[] ConvertFromString(string version)
        {
            return Convert.FromBase64String(version);
        }
    }
}
