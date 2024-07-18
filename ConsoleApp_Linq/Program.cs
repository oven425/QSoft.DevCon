// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;


//https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/coreclr/System.Private.CoreLib/src/System/Collections/Generic/EqualityComparer.CoreCLR.cs
Console.WriteLine("Hello, World!");

var t1 = Enumerable.Range(0, 10).Select(x => new {index = x, name=$"name_{x}" });
var t2 = Enumerable.Range(5, 10).Select(x => new { index = x, count =x });
var tt = t1.GroupJoin(t2, x => x.index, y => y.index, (x, y) => new { x, y })
    .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => new { x, y });

foreach(var oo in tt)
{

}
var tt2 = t1.LeftJoin(t2, x => x.index, y => y.index);
foreach (var oo in tt2)
{

}
static class LinqEx
{
    public static IEnumerable<(T1, T2?)> LeftJoin<T1, T2, TKey>(this IEnumerable<T1> t1, IEnumerable<T2> t2, Func<T1, TKey> key1, Func<T2, TKey> key2)
    {
        return t1.GroupJoin(t2, key1, key2, (x, y) => new { x, y })
            .SelectMany(x => x.y.DefaultIfEmpty(), (x, y) => (x.x, y));
    }
}