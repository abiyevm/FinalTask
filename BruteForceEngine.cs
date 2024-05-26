using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordBruteForce
{
    public class BruteForceEngine
    {
        private readonly string _encryptedPassword;
        private readonly int _maxThreads;
        private readonly Action<string> _onPasswordFound;

        public BruteForceEngine(string encryptedPassword, int maxThreads, Action<string> onPasswordFound)
        {
            _encryptedPassword = encryptedPassword;
            _maxThreads = maxThreads;
            _onPasswordFound = onPasswordFound;
        }

        public void Start()
        {
            var startTime = DateTime.Now;
            Parallel.ForEach(GenerateCombinations(), new ParallelOptions { MaxDegreeOfParallelism = _maxThreads }, (password, state) =>
            {
                if (PasswordManager.VerifyPassword(password, _encryptedPassword))
                {
                    _onPasswordFound(password);
                    state.Break();
                }
            });
            var endTime = DateTime.Now;
            Console.WriteLine($"Time elapsed: {endTime - startTime}");
        }

        private IEnumerable<string> GenerateCombinations()
        {
            char[] charset = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            int maxLength = 4; // Set a reasonable max length for demonstration purposes

            for (int length = 1; length <= maxLength; length++)
            {
                var current = new char[length];
                foreach (var combination in GenerateCombinationsRecursive(current, charset, 0))
                {
                    yield return new string(combination);
                }
            }
        }

        private IEnumerable<char[]> GenerateCombinationsRecursive(char[] current, char[] charset, int position)
        {
            if (position == current.Length)
            {
                yield return current.ToArray();
            }
            else
            {
                foreach (var c in charset)
                {
                    current[position] = c;
                    foreach (var combination in GenerateCombinationsRecursive(current, charset, position + 1))
                    {
                        yield return combination;
                    }
                }
            }
        }
    }
}