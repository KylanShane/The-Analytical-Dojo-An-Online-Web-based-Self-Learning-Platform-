using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
public static class DojoPractice {
 public const string Starter="SELECT course, ROUND(AVG(score), 1) AS avg_score\nFROM my_quiz_results\nGROUP BY course\nORDER BY avg_score DESC;";
 // Never execute editor input. Parse a deliberately small grammar and use fixed SQL.
 public static DataTable Run(int uid,string input){
  if(input==null||input.Length>2000)throw new ArgumentException("Please keep your query under 2,000 characters.");
  string s=Regex.Replace(input.Trim().TrimEnd(';'),@"\s+"," ");
  string avg=Regex.Replace(Starter.TrimEnd(';'),@"\s+"," ");
  if(string.Equals(s,avg,StringComparison.OrdinalIgnoreCase))return DojoData.Query("WITH my_quiz_results AS ("+DojoData.ResultsSql+") SELECT course,ROUND(AVG(score),1) AS avg_score FROM my_quiz_results WHERE quiz_type='OBJ' AND score IS NOT NULL GROUP BY course ORDER BY avg_score DESC",DojoData.P("@uid",uid));
  var m=Regex.Match(s,@"^SELECT (?<cols>\*|(?:course|quiz|score)(?:\s*,\s*(?:course|quiz|score))*) FROM my_quiz_results(?: WHERE score\s*(?<op>>=|<=|=|>|<)\s*(?<value>\d{1,3}(?:\.\d{1,2})?))?(?: ORDER BY score(?: (?<order>ASC|DESC))?)?$",RegexOptions.IgnoreCase);
  if(!m.Success)throw new ArgumentException("This query is outside the supported practice syntax. Choose a starter or open Supported SQL for examples.");
  string cols=m.Groups["cols"].Value.ToLowerInvariant();if(cols=="*")cols="course,quiz,score";
  decimal value=0;if(m.Groups["value"].Success){value=decimal.Parse(m.Groups["value"].Value,CultureInfo.InvariantCulture);if(value>100)throw new ArgumentException("Use a score between 0 and 100.");}
  string sql="WITH my_quiz_results AS ("+DojoData.ResultsSql+") SELECT "+cols+" FROM my_quiz_results WHERE quiz_type='OBJ' AND score IS NOT NULL";
  if(m.Groups["op"].Success)sql+=" AND score "+m.Groups["op"].Value+" @score";
  if(s.IndexOf("ORDER BY",StringComparison.OrdinalIgnoreCase)>=0)sql+=" ORDER BY score "+(m.Groups["order"].Value.ToUpperInvariant()=="DESC"?"DESC":"ASC");
  return DojoData.Query(sql,DojoData.P("@uid",uid),DojoData.P("@score",value));
 }
}
