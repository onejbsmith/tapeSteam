using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CLRConsole = System.Console;



public static class ObjectExtensions
{
    public static void Dump<T>(this T data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        Console.WriteLine(json);
        Debug.WriteLine(json);
    }
    public static string Dumps<T>(this T data)
    {
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }
}

public static class JSRuntimeExtensions
{
    public static async Task<string> ConfirmAsync(this Microsoft.JSInterop.IJSRuntime jSRuntime, string message)
    {
        return await jSRuntime.InvokeAsync<string>("Confirm", new object[] { message });
    }
    public static void Confirm(this Microsoft.JSInterop.IJSRuntime jSRuntime, string message, bool isForced = false)
    {
        //if (!isForced)
        //    return;
        jSRuntime.InvokeAsync<string>("Confirm", new object[] { message });
    }

    public static async Task GroupTableAsync<T>(this Microsoft.JSInterop.IJSRuntime jSRuntime, T data, string label)
    {
        await jSRuntime.InvokeAsync<T>("GroupTable", new object[] { data.Dumps(), label });
    }

    public static void GroupTable<T>(this Microsoft.JSInterop.IJSRuntime jSRuntime, T data, string label)
    {
        jSRuntime.InvokeAsync<T>("GroupTable", new object[] { data.Dumps(), label });
    }

    public static async Task DumpAsync<T>(this Microsoft.JSInterop.IJSRuntime jSRuntime, T data, string label)
    {
        await jSRuntime.InvokeAsync<T>("Dump", new object[] { data.Dumps(), label });
    }

    public static void Dump<T>(this Microsoft.JSInterop.IJSRuntime jSRuntime, T data, string label)
    {
        jSRuntime.InvokeAsync<T>("Dump", new object[] { data.Dumps(), label });
    }
}


namespace JS
{
    public static class Console
    {
        public static void WriteLine(string value)
        {
            CLRConsole.WriteLine(value);
        }

        public static void WriteBlueLine(string value)
        {
            System.ConsoleColor currentColor = CLRConsole.ForegroundColor;

            CLRConsole.ForegroundColor = System.ConsoleColor.Blue;
            CLRConsole.WriteLine(value);

            CLRConsole.ForegroundColor = currentColor;
        }

        public static System.ConsoleKeyInfo ReadKey(bool intercept)
        {
            return CLRConsole.ReadKey(intercept);
        }

        public static void GroupTable()
        {

        }
    }
}


public static class IntExtensions
{
    public static long GetEpochSeconds(this DateTime date)
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        return (int)t.TotalSeconds;
    }
        
    public static DateTime FromEpochSeconds(this DateTime date, long EpochSeconds)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(EpochSeconds);

    }

    public static DateTime FromUnixTime(this long unixTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(unixTime);
    }

    public static long ToUnixTime(this DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date - epoch).TotalSeconds);
    }
}