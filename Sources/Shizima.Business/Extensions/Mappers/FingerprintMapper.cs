using Shizima.CoreLib.Entities;
using Shizima.Dtos;
using Shizima.Entities;

namespace Shizima.Business.Extensions.Mappers
{
    public static class FingerprintMapper
    {

        public static Fingerprint Harmonize(this DatabaseFingerprint fingerprint)
        {
            return new Fingerprint
            {
                Hash = fingerprint.Hash,
                TimeOffset = fingerprint.TimeOffset
            };
        }

        public static Dictionary<long, List<Fingerprint>> ToDict(this List<DatabaseFingerprint> fingerprints)
        {
            return fingerprints.GroupBy(f => f.MusicId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.Harmonize()).ToList()
                );
        }

        public static DatabaseFingerprint ToDatabase(this Fingerprint fingerprint, long musicId)
        {
            return new DatabaseFingerprint
            {
                Hash = fingerprint.Hash,
                TimeOffset = fingerprint.TimeOffset,
                MusicId = musicId
            };
        }

        public static List<DatabaseFingerprint> ToDatabase(this List<Fingerprint> fingerprints, long musicId)
        {
            return fingerprints.Select(f => f.ToDatabase(musicId)).ToList();
        }
    }
}
