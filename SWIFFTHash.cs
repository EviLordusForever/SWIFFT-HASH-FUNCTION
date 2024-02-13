using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1
{
    public class SWIFFTHash
    {
        private const int ROUNDS = 3;
        private const int BLOCK_SIZE = 64;
        private const int NUM_WORDS = 16;

        private ulong[] M;
        private ulong[] H;

        public SWIFFTHash()
        {
            M = new ulong[NUM_WORDS];
            H = new ulong[NUM_WORDS];

            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < NUM_WORDS; i++)
            {
                H[i] = 0;
            }
        }

        private static ulong RotateLeft(ulong x, int n)
        {
            return ((x << n) | (x >> (BLOCK_SIZE - n)));
        }

        private static void Swap(ref ulong a, ref ulong b)
        {
            ulong temp = a;
            a = b;
            b = temp;
        }

        private void RoundA()
        {
            for (int i = 0; i < NUM_WORDS; i++)
            {
                M[i] ^= H[i];
            }
            for (int i = 0; i < ROUNDS; i++)
            {
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] ^= M[(j + 2) % NUM_WORDS];
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    M[j] = RotateLeft(M[j] + H[(j + 1) % NUM_WORDS], (int)(H[j] & 0x3F));
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] = RotateLeft(H[j], 0x1B);
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] += M[(j + 1) % NUM_WORDS];
                }
                Swap(ref H[0], ref H[1]);
            }
        }

        private void RoundB()
        {
            for (int i = 0; i < NUM_WORDS; i++)
            {
                M[i] ^= H[(i + 1) % NUM_WORDS];
            }
            for (int i = 0; i < ROUNDS; i++)
            {
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] ^= M[(j + 2) % NUM_WORDS];
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    M[j] = RotateLeft(M[j] ^ H[(j + 1) % NUM_WORDS], (int)(H[j] & 0x3F));
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] = RotateLeft(H[j], 0x1B);
                }
                for (int j = 0; j < NUM_WORDS; j++)
                {
                    H[j] ^= M[(j + 1) % NUM_WORDS];
                }
                Swap(ref H[0], ref H[2]);
            }
        }

        public byte[] ComputeHash(byte[] data)
        {
            Initialize();

            int numBlocks = data.Length / BLOCK_SIZE;
            for (int block = 0; block < numBlocks; block++)
            {
                for (int i = 0; i < NUM_WORDS; i++)
                {
                    M[i] = BitConverter.ToUInt64(data, block * BLOCK_SIZE + i * sizeof(ulong));
                }

                RoundA();
                RoundB();
            }

            int remainingBytes = data.Length % BLOCK_SIZE;
            int remainingWords = remainingBytes / sizeof(ulong);
            byte[] paddedBytes = new byte[BLOCK_SIZE];
            Buffer.BlockCopy(data, numBlocks * BLOCK_SIZE, paddedBytes, 0, remainingBytes);

            for (int i = 0; i < remainingWords; i++)
            {
                M[i] = BitConverter.ToUInt64(paddedBytes, i * sizeof(ulong));
            }

            M[remainingWords] = (ulong)remainingBytes;

            RoundA();
            RoundB();

            byte[] hashBytes = new byte[NUM_WORDS * sizeof(ulong)];
            for (int i = 0; i < NUM_WORDS; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(H[i]), 0, hashBytes, i * sizeof(ulong), sizeof(ulong));
            }

            return hashBytes;
        }
    }
}
