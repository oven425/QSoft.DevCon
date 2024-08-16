// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;


//https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/coreclr/System.Private.CoreLib/src/System/Collections/Generic/EqualityComparer.CoreCLR.cs
Console.WriteLine("Hello, World!");

var t1 = Enumerable.Range(0, 10).Select(x => new {index = x, name=$"name_{x}" });
var t2 = Enumerable.Range(5, 10).Select(x => new { index = x, count =x });
var tt = t1.GroupJoin(t2, x => x.index, y => y.index, (x, y) => new { x, y })
    .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x, y });

foreach(var oo in tt)
{

}
var a1 = Enumerable.Range(0, 5).Select(x => $"test{x}");
var a2 = Enumerable.Range(1, 6).Select(x => $"{x}");
var aa1 = a1.LeftJoin(a2, x=>x, y=>y, (x, y) => y.Contains(x));
foreach (var oo in aa1)
{

}

Console.ReadLine();


public static partial class LeftJoinExtension
{
    public static IEnumerable<(T1, T2?)> LeftJoin<T1, T2, TKey>(this IEnumerable<T1> t1, IEnumerable<T2> t2, Func<T1, TKey> key1, Func<T2, TKey> key2)
    {
        return t1.GroupJoin(t2, key1, key2, (x, y) => new { x, y })
            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => (x.x, y));
    }

    public static IEnumerable<(T1, T2?)> LeftJoin<T1, T2, TKey>(this IEnumerable<T1> t1, IEnumerable<T2> t2, Func<T1, TKey> key1, Func<T2, TKey> key2, IEqualityComparer<TKey> compare)
    {
        return t1.GroupJoin(t2, key1, key2, (x, y) => new { x, y }, compare)
            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => (x.x, y));
    }

    public static IEnumerable<(T1, T2?)> LeftJoin<T1, T2, TKey>(this IEnumerable<T1> t1, IEnumerable<T2> t2, Func<T1, TKey> key1, Func<T2, TKey> key2, Func<TKey, TKey, bool> compare)
    {
        return t1.GroupJoin(t2, key1, key2, (x, y) => new { x, y }, new EqualityComparerLambda<TKey>(compare))
            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => (x.x, y));
    }
}

public class EqualityComparerLambda<T>(Func<T, T, bool> func) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        return func(x, y);
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return 0;
    }
}

public class EqualityComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        var bb = y.Contains(x);
        System.Diagnostics.Trace.WriteLine($"x:{x} y:{y} bb:{bb}");
        return bb;
    }

    public int GetHashCode(string obj)
    {
        //return obj.GetHashCode();
        return 0;
        return obj.GetHashCode()^ obj.GetHashCode();
    }
}