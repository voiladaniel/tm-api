using System;
using System.Collections.Generic;
using System.Text;

namespace TravianManager.Core
{
    public interface IHelpers
    {
        string Encrypt(string clearPassword);

        string Decrypt(string encryptedPassword);
    }
}
