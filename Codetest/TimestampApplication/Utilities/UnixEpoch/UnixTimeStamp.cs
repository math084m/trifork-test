using System;

namespace Utilities.UnixEpoch
{
    public class UnixTimeStamp : IUnixTimeStamp
    {
        public int GetUnixEpochNow()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}