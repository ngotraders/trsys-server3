﻿namespace Trsys.CopyTrading
{
    public interface IEaSessionTokenProvider
    {
        string GenerateToken(string id, string key, string keyType);
    }
}