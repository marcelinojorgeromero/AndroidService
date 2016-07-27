using System;
using System.Threading.Tasks;
using Android.Util;

namespace Client
{
    public class DataOperations<T> where T: ITable
    {
        private readonly T _table;

        public DataOperations(T table)
        {
            _table = table;
        }
        public async Task DoSomethingLong()
        {
            Log.Debug("AppDebuggingFlow",  "[StorageService][DoSomethingLong] Entered");
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLong] Starting execution at {GetNowTime()}... Value of _table.Name: {_table.Name}, Id: {_table.Id}");
            await Task.Delay(4000);
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLong] Finished at {GetNowTime()}.");
        }

        public async Task DoSomethingLonger(string andPrintSomethingBefore, string andPrintSomethingAfter)
        {
            Log.Debug("AppDebuggingFlow",  "[StorageService][DoSomethingLonger] Entered");
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLonger] andPrintSomethingBefore: {andPrintSomethingBefore}");
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLonger] Starting execution at {GetNowTime()}... Value of _table.Name: {_table.Name}, Id: {_table.Id}");
            await Task.Delay(5000);
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLonger] Finished at {GetNowTime()}.");
            Log.Debug("AppDebuggingFlow", $"[StorageService][DoSomethingLonger] andPrintSomethingAfter: {andPrintSomethingAfter}");
        }

        private string GetNowTime()
        {
            return DateTime.Now.ToString("T");
        }
    }
}