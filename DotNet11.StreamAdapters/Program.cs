// .NET 11 Libraries - Stream memory and text without copying.
// Introduced: Preview 6.
//
// Four new Stream types wrap in-memory data so it can be handed to any API that expects a
// Stream, without first copying into a MemoryStream:
//   * ReadOnlyMemoryStream   - exposes a ReadOnlyMemory<byte> as a read-only stream.
//   * WritableMemoryStream   - exposes a writable Memory<byte> as a fixed-size stream.
//   * ReadOnlySequenceStream - streams over ReadOnlySequence<byte> segments without flattening.
//   * StringStream           - reads a string / ReadOnlyMemory<char> using a given encoding.
//
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md

using System.Buffers;
using System.Text;

Console.WriteLine("== StringStream ==");
const string Yaml = "name: dotnet-dev-days\nyear: 2026\n";
using (Stream config = new StringStream(Yaml, Encoding.UTF8))
using (var reader = new StreamReader(config, Encoding.UTF8))
{
    Console.WriteLine($"length (bytes): {config.Length}");
    Console.WriteLine(await reader.ReadToEndAsync());
}

Console.WriteLine("== ReadOnlyMemoryStream ==");
ReadOnlyMemory<byte> payload = Encoding.UTF8.GetBytes("{\"ok\":true}");
using (Stream body = new ReadOnlyMemoryStream(payload))
{
    var sink = new MemoryStream();
    await body.CopyToAsync(sink);
    Console.WriteLine($"copied: {Encoding.UTF8.GetString(sink.ToArray())}");
    Console.WriteLine($"writable: {body.CanWrite}");
}

Console.WriteLine();
Console.WriteLine("== WritableMemoryStream ==");
Memory<byte> buffer = new byte[16];
using (Stream writable = new WritableMemoryStream(buffer))
{
    await writable.WriteAsync(Encoding.UTF8.GetBytes("fixed-size!"));
    Console.WriteLine($"written {writable.Position} byte(s) into a caller-owned buffer");
    Console.WriteLine($"buffer : {Encoding.UTF8.GetString(buffer.Span[..(int)writable.Position])}");
}

Console.WriteLine();
Console.WriteLine("== ReadOnlySequenceStream (multi-segment, nothing is flattened) ==");
ReadOnlySequence<byte> sequence = CreateSegmentedSequence("Pipelines ", "segments ", "stay split.");
using (Stream segmented = new ReadOnlySequenceStream(sequence))
using (var reader = new StreamReader(segmented, Encoding.UTF8))
{
    Console.WriteLine($"segments: {CountSegments(sequence)}");
    Console.WriteLine(await reader.ReadToEndAsync());
}

static ReadOnlySequence<byte> CreateSegmentedSequence(params string[] parts)
{
    Segment? first = null;
    Segment? last = null;

    foreach (string part in parts)
    {
        ReadOnlyMemory<byte> bytes = Encoding.UTF8.GetBytes(part);
        if (first is null)
        {
            first = new Segment(bytes, 0);
            last = first;
        }
        else
        {
            last = last!.Append(bytes);
        }
    }

    return new ReadOnlySequence<byte>(first!, 0, last!, last!.Memory.Length);
}

static int CountSegments(ReadOnlySequence<byte> sequence)
{
    int count = 0;
    foreach (ReadOnlyMemory<byte> _ in sequence)
    {
        count++;
    }

    return count;
}

internal sealed class Segment : ReadOnlySequenceSegment<byte>
{
    public Segment(ReadOnlyMemory<byte> memory, long runningIndex)
    {
        Memory = memory;
        RunningIndex = runningIndex;
    }

    public Segment Append(ReadOnlyMemory<byte> memory)
    {
        var next = new Segment(memory, RunningIndex + Memory.Length);
        Next = next;
        return next;
    }
}
