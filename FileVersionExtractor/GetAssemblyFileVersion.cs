using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace FileVersionExtractor
{
    public class GetAssemblyFileVersion : ITask
    {
        private const string Pattern = @"(?:AssemblyFileVersion\("")(?<ver>(\d*)\.(\d*)(\.(\d*)(\.(\d*))?)?)(?:""\))";

        [Required]
        public string FilePathAssemblyInfo { get; set; }

        [Output]
        public string AssemblyFileVersion { get; set; }

        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            StreamReader streamreaderAssemblyInfo = null;
            AssemblyFileVersion = String.Empty;
            try
            {
                streamreaderAssemblyInfo = new StreamReader(FilePathAssemblyInfo);
                string strLine;
                while ((strLine = streamreaderAssemblyInfo.ReadLine()) != null)
                {
                    Match matchVersion = Regex.Match(
                        strLine,
                        Pattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                    if (!matchVersion.Success)
                    {
                        continue;
                    }

                    var groupVersion = matchVersion.Groups["ver"];
                    if ((!groupVersion.Success) || (String.IsNullOrEmpty(groupVersion.Value)))
                    {
                        continue;
                    }
                    
                    AssemblyFileVersion = groupVersion.Value;
                    break;
                }
            }
            catch (Exception e)
            {
                var args = new BuildMessageEventArgs(e.Message, string.Empty, "GetAssemblyFileVersion", MessageImportance.High);
                BuildEngine.LogMessageEvent(args);
            }
            finally
            {
                if (streamreaderAssemblyInfo != null)
                {
                    streamreaderAssemblyInfo.Close();
                }
            }

            return (true);
        }
    }
}