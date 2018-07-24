using System;
using System.IO;
using Harmony;

namespace WeaponRealizer
{
    public static class Logger
    {
        private static string LogFilePath => $"{Core.ModDirectory}/{Core.ModName}.log";

        public static void Error(Exception ex)
        {
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"Message: {ex.Message}");
                writer.WriteLine($"StackTrace: {ex.StackTrace}");
                WriteLogFooter(writer);
            }
        }

        public static void Debug(String line)
        {
            FileLog.Log($"{LogFilePath}");
            if (!Core.ModSettings.debug) return;
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(line);
                WriteLogFooter(writer);
            }
        }

        private static void WriteLogFooter(StreamWriter writer)
        {
            writer.WriteLine($"Date: {DateTime.Now}");
            writer.WriteLine(new string(c: '-', count: 80));
        }

//        public static void ListTheStack(StringBuilder sb, List<CodeInstruction> codes)
//        {
//            sb.AppendLine(new string(c: '-', count: 80));
//            for (var i = 0; i < codes.Count(); i++)
//            {
//                sb.Append($"{codes[i].opcode}\t\t");
//                if (codes[i].operand != null)
//                {
//                    sb.Append($"{codes[i].operand}");
//                }
//
//                sb.Append(Environment.NewLine);
//            }
//            sb.AppendLine(new string(c: '-', count: 80));
//        }
//
//        public static void LogStringBuilder(StringBuilder sb)
//        {
//            if (sb.Length > 0)
//            {
//                FileLog.Log(sb.ToString());
//                sb.Remove(0, sb.Length);
//            }
//        }
    }
}