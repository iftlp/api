using System.Text.Json;
using System.Text.Json.Serialization;
using SoundFingerprinting.Data;

public class AVHashesJsonConverter : JsonConverter<AVHashes>
{
    public override AVHashes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;

            // Read audio hashes
            var audioHashes = new List<HashedFingerprint>();
            if (root.TryGetProperty("Audio", out JsonElement audioElement))
            {
                foreach (JsonElement item in audioElement.EnumerateArray())
                {
                    var hashBins = new int[25]; // SoundFingerprinting uses 25 hash bins
                    var hashBinsArray = item.GetProperty("HashBins").EnumerateArray().ToArray();

                    for (int i = 0; i < Math.Min(25, hashBinsArray.Length); i++)
                    {
                        hashBins[i] = hashBinsArray[i].GetInt32();
                    }

                    var sequenceNumber = (uint)item.GetProperty("SequenceNumber").GetInt32();
                    var startsAt = (float)item.GetProperty("StartsAt").GetDouble();
                    var originalPoint = item.GetProperty("OriginalPoint").GetBytesFromBase64();

                    var hashedFingerprint = new HashedFingerprint(
                        hashBins,
                        sequenceNumber,
                        startsAt,
                        originalPoint
                    );

                    audioHashes.Add(hashedFingerprint);
                }
            }

            var audioMilliseconds = 0;
            if (root.TryGetProperty("FingerprintingTime", out JsonElement timeElement))
            {
                audioMilliseconds = timeElement.GetProperty("AudioMilliseconds").GetInt32();
            }

            var audio = new Hashes(audioHashes, audioMilliseconds, MediaType.Audio);
            return new AVHashes(audio, null);
        }
    }

    public override void Write(Utf8JsonWriter writer, AVHashes value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Write audio hashes
        if (value.Audio?.Any() == true)
        {
            writer.WritePropertyName("Audio");
            writer.WriteStartArray();

            foreach (var hash in value.Audio)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("HashBins");
                writer.WriteStartArray();
                foreach (var bin in hash.HashBins)
                {
                    writer.WriteNumberValue(bin);
                }
                writer.WriteEndArray();

                writer.WritePropertyName("SequenceNumber");
                writer.WriteNumberValue(hash.SequenceNumber);

                writer.WritePropertyName("StartsAt");
                writer.WriteNumberValue(hash.StartsAt);

                writer.WritePropertyName("OriginalPoint");
                writer.WriteStringValue(hash.OriginalPoint?.ToString() ?? "");

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        // Write video hashes
        if (value.Video?.Any() == true)
        {
            writer.WritePropertyName("Video");
            writer.WriteStartArray();

            foreach (var hash in value.Video)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("HashBins");
                writer.WriteStartArray();
                foreach (var bin in hash.HashBins)
                {
                    writer.WriteNumberValue(bin);
                }
                writer.WriteEndArray();

                writer.WritePropertyName("SequenceNumber");
                writer.WriteNumberValue(hash.SequenceNumber);

                writer.WritePropertyName("StartsAt");
                writer.WriteNumberValue(hash.StartsAt);

                writer.WritePropertyName("OriginalPoint");
                writer.WriteStringValue(hash.OriginalPoint?.ToString() ?? "");

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}

// Helper class for serialization/deserialization
public static class AVHashesSerializer
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        Converters = { new AVHashesJsonConverter() },
        WriteIndented = false
    };

    public static string Serialize(AVHashes avHashes)
    {
        return JsonSerializer.Serialize(avHashes, Options);
    }

    public static AVHashes Deserialize(string json)
    {
        return JsonSerializer.Deserialize<AVHashes>(json, Options);
    }

    public static byte[] SerializeToBytes(AVHashes avHashes)
    {
        return JsonSerializer.SerializeToUtf8Bytes(avHashes, Options);
    }

    public static AVHashes DeserializeFromBytes(byte[] bytes)
    {
        return JsonSerializer.Deserialize<AVHashes>(bytes, Options);
    }
}
