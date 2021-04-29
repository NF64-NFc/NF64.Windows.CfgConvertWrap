using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


namespace NF64.Windows.CfgConvertWrap
{
    internal enum CfgConvertMode
    {
        BinToCpp,
        CppToBin,
    }


    internal sealed class CfgConvertParams
    {
        public string ExePath  { get; }

        public CfgConvertMode Mode { get; }

        public string SourcePath { get; }


        public string DestinationExt { get; }

        public string DestinationPath { get; }


        public CfgConvertParams(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (args.Length != 3)
                throw new ArgumentException("Need 3 Parameters. Ex: [ExePath] [Mode ('-b2c', '-c2b')] [SourcePath]");

            this.ExePath = LoadPath(args[0]);
            this.Mode = LoadMode(args[1]);
            this.SourcePath = LoadPath(args[2]);
            this.DestinationExt = LoadDestExt(this.Mode);
            this.DestinationPath = this.SourcePath.Replace(Path.GetExtension(this.SourcePath), this.DestinationExt);
            this.ValidateArgs();
        }


        private void ValidateArgs()
        {
            var srcExt = Path.GetExtension(this.SourcePath).ToLower();
            switch (this.Mode)
            {
            case CfgConvertMode.BinToCpp:
                if (srcExt != ".bin")
                    throw new ArgumentException($"Source was not '.bin'");
                break;
            case CfgConvertMode.CppToBin:
                if (srcExt != ".cpp")
                    throw new ArgumentException($"Source was not '.cpp'");
                break;
            default: throw new ArgumentException($"unknown value : '{this.Mode}'");
            }
        }


        public string GetModeOption()
        {
            switch (this.Mode)
            {
            case CfgConvertMode.BinToCpp: return "-txt";
            case CfgConvertMode.CppToBin: return "-bin";
            default: throw new ArgumentException($"unknown value : '{this.Mode}'");
            }
        }


        private static string LoadPath(string path)
        {
            var fullPath = Path.GetFullPath(path.Replace("\"", ""));
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"'{fullPath}' was not found.", fullPath);
            return fullPath;
        }


        private static CfgConvertMode LoadMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
                throw new ArgumentException($@"Invalid args. Mode = empty");

            switch (mode.ToLower())
            {
            case "-b2c": return CfgConvertMode.BinToCpp;
            case "-c2b": return CfgConvertMode.CppToBin;
            default: throw new ArgumentException($"Invalid args. Mode = '{mode}'");
            }
        }


        private static string LoadDestExt(CfgConvertMode mode)
        {
            switch (mode)
            {
            case CfgConvertMode.BinToCpp: return ".cpp";
            case CfgConvertMode.CppToBin: return ".bin";
            default: throw new ArgumentException($"unknown value : '{mode}'");
            }
        }


        public override string ToString() => $"{nameof(ExePath)} = '{ExePath}', {nameof(Mode)} = '{Mode}', {nameof(SourcePath)} = '{SourcePath}', {nameof(DestinationExt)} = '{DestinationExt}', {nameof(DestinationPath)} = '{DestinationPath}'";
    }


    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var p = new CfgConvertParams(args);
                Process.Start(p.ExePath, $"{p.GetModeOption()} -dst \"{p.DestinationPath}\" \"{p.SourcePath}\"");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ExecutablePath, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
