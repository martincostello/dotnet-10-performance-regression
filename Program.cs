using System.Drawing;
using System.Globalization;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Benchmarks>(args: args);

[MemoryDiagnoser]
public class Benchmarks
{
    private string[] Input;

    [GlobalSetup]
    public void GlobalSetup()
    {
        Input = File.ReadAllLines(Environment.GetEnvironmentVariable("PUZZLE_INPUT"));
    }

    [Benchmark]
    public (int ViableNodes, int StepsToExtract) Solve() => Puzzle.Solve(Input);
}

public sealed class Puzzle
{
    public static (int ViableNodes, int StepsToExtract) Solve(IEnumerable<string> output)
    {
        var nodes = output
            .Skip(2)
            .Select(Node.Parse)
            .ToList();

        int viableCount = 0;
        int nodeCount = nodes.Count;

        for (int i = 0; i < nodeCount; i++)
        {
            Node node = nodes[i];

            for (int j = 0; j < nodeCount; j++)
            {
                if (i == j)
                {
                    continue;
                }

                Node other = nodes[j];

                if (node.Used > 0 && node.Used <= other.Available)
                {
                    viableCount++;
                }
            }
        }

        int width = nodes.Max((p) => p.Location.X) + 1;
        int height = nodes.Max((p) => p.Location.Y) + 1;

        char[,] grid = new char[width, height];

        Node goalNode = nodes
            .Where((p) => p.Location.Y == 0)
            .OrderByDescending((p) => p.Location.X)
            .First();

        Node emptyNode = nodes.First((p) => p.Used == 0);

        int minimumSize = nodes.Min((p) => p.Size);

        const char Available = '.';
        const char Empty = '_';
        const char Full = '#';
        const char Goal = 'G';

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodes.First((p) => p.Location.X == x && p.Location.Y == y);

                if (node == goalNode)
                {
                    grid[x, y] = Goal;
                }
                else if (node == emptyNode)
                {
                    grid[x, y] = Empty;
                }
                else if (node.Used > minimumSize)
                {
                    grid[x, y] = Full;
                }
                else
                {
                    grid[x, y] = Available;
                }
            }
        }

        Point empty = emptyNode.Location;

        int steps = 0;

        while (grid[empty.X, empty.Y - 1] == Available)
        {
            empty += Directions.Up;
            steps++;
        }

        while (grid[empty.X, empty.Y - 1] == Full)
        {
            empty += Directions.Left;
            steps++;
        }

        while (empty.Y > 0)
        {
            empty += Directions.Up;
            steps++;
        }

        while (grid[empty.X, empty.Y] != Goal)
        {
            empty += Directions.Right;
            steps++;
        }

        while (empty.X != 1)
        {
            empty += Directions.Right;
            empty += Directions.Down;
            empty += Directions.Left;
            empty += Directions.Left;
            empty += Directions.Up;
            steps += 5;
        }

        return (viableCount, steps);
    }

    private static class Directions
    {
        public static readonly Size Up = new(0, -1);
        public static readonly Size Down = new(0, 1);
        public static readonly Size Left = new(-1, 0);
        public static readonly Size Right = new(1, 0);
    }

    private sealed class Node
    {
        private Node(string name, int size, int used, Point location)
        {
            Name = name;
            Size = size;
            Used = used;
            Location = location;
        }

        public string Name { get; }

        public int Size { get; }

        public int Used { get; private set; }

        public int Available => Size - Used;

        public Point Location { get; private set; }

        public static Node Parse(string value)
        {
            string[] split = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var first = split[0].AsSpan();
            int indexX = first.IndexOf('x');
            int indexY = first.IndexOf('y');

            int x = Parse<int>(first.Slice(indexX + 1, indexY - indexX - 2));
            int y = Parse<int>(first[(indexY + 1)..]);

            return new Node(
                split[0],
                Parse<int>(split[1].TrimEnd('T')),
                Parse<int>(split[2].TrimEnd('T')),
                new(x, y));
        }

        public bool Move(Node other)
        {
            bool moved = false;

            if (other.Available >= Used)
            {
                other.Used += Used;
                Used = 0;
                moved = true;
            }

            return moved;
        }

        private static T Parse<T>(ReadOnlySpan<char> s)
            where T : INumber<T>
            => Parse<T>(s, NumberStyles.Integer);

        private static T Parse<T>(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer)
            where T : INumber<T>
            => T.Parse(s, style, CultureInfo.InvariantCulture);
    }
}
