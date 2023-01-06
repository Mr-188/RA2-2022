using System;
using System.Linq;

namespace ClientCore.CnCNet5
{
    public static class NameValidator
    {
        /// <summary>
        /// Checks if the player's nickname is valid for CnCNet.
        /// </summary>
        /// <returns>Null if the nickname is valid, otherwise a string that tells
        /// what is wrong with the name.</returns>
        public static string IsNameValid(string name)
        {
            var profanityFilter = new ProfanityFilter();

            if (string.IsNullOrEmpty(name))
                return "请输入一个名称.";

            if (profanityFilter.IsOffensive(name))
                return "请输入一个不那么冒犯的名称.";

            if (int.TryParse(name.Substring(0, 1), out _))
                return "玩家名的第一个字符不能是数字.";

            if (name[0] == '-')
                return "玩家名的第一个字符不能是破折号(-).";

            // Check that there are no invalid chars
            char[] allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_[]|\\{}^`".ToCharArray();
            char[] nicknameChars = name.ToCharArray();

            foreach (char nickChar in nicknameChars)
            {
                if (!allowedCharacters.Contains(nickChar))
                {
                    return "你的玩家名中有无效字符." + Environment.NewLine +
                     "允许的字符是从A到Z和数字的任何字符.";
                }
            }

            if (name.Length > ClientConfiguration.Instance.MaxNameLength)
                return "你的昵称太长了.";

            return null;
        }

        /// <summary>
        /// Returns player nickname constrained to maximum allowed length and with invalid characters for offline nicknames removed.
        /// Does not check for offensive words or invalid characters for CnCNet.
        /// </summary>
        /// <param name="name">Player nickname.</param>
        /// <returns>Player nickname with invalid offline nickname characters removed and constrained to maximum name length.</returns>
        public static string GetValidOfflineName(string name)
        {
            char[] disallowedCharacters = ",;".ToCharArray();

            string validName = new string(name.Trim().Where(c => !disallowedCharacters.Contains(c)).ToArray());

            if (validName.Length > ClientConfiguration.Instance.MaxNameLength)
                return validName.Substring(0, ClientConfiguration.Instance.MaxNameLength);

            return validName;
        }
    }
}
