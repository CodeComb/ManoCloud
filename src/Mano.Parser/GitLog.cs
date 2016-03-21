using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Mano.Parser.Models;
using CodeComb.Package;

namespace Mano.Parser
{
    public class GitLog
    {
        private static List<Commit> Parse(string src)
        {
            var ret = new List<Commit>();
            src = src.Replace("\r\n", "\n");
            var tmp = src.Split('\n');
            for (var i =0; i < tmp.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(tmp[i]))
                    continue;
                var splited = tmp[i].Split('\t');
                if (splited.Count() == 1)
                {
                    var commit = new Commit();
                    commit.Id = tmp[i];
                    commit.Author = tmp[i + 1];
                    commit.Email = tmp[i + 2];
                    var dt = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt64(tmp[i + 3]));
                    commit.Time = dt.ToLocalTime();
                    ret.Add(commit);
                    i = i + 3;
                }
                else
                {
                    ret.Last().Changes.Add(new Change
                    {
                        Additions = Convert.ToInt64(splited[0] == "-" ? "0" : splited[0]),
                        Deletions = Convert.ToInt64(splited[1] == "-" ? "0" : splited[1]),
                        Path = splited[2],
                        CommitId = ret.Last().Id
                    });
                }
            }
            return ret;
        }

        private static List<Commit> GetLogs(string Path)
        {
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = Path;
            proc.StartInfo.FileName = "git";
            proc.StartInfo.Arguments = "--no-pager log --numstat --format=%H%n%an%n%ae%n%at";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            proc.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            proc.Start();
            var output = proc.StandardOutput.ReadToEnd();
            return Parse(output);
        }

        public static List<Commit> CloneAndGetLogs(string gitsrc, string cloneto)
        {
            var di = new System.IO.DirectoryInfo(cloneto);
            if (di.GetDirectories().Count() == 0)
            {
                GitClone.Clone(gitsrc, cloneto);
            }
            else
            {
                var proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WorkingDirectory = di.GetDirectories().First().FullName;
                proc.StartInfo.FileName = "git";
                proc.StartInfo.Arguments = "pull";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
            }
            di = new System.IO.DirectoryInfo(cloneto);
            cloneto = di.GetDirectories().First().FullName;
            var ret = GetLogs(cloneto);
            return ret;
        }
    }
}
