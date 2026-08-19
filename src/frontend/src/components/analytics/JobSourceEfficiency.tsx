import React from 'react';
import type { JobSourceMetric } from '@/api/analytics';
import { Share2, Trophy, Send, Users, Award, RefreshCw } from 'lucide-react';

interface JobSourceEfficiencyProps {
  sources: JobSourceMetric[];
}

export const JobSourceEfficiency: React.FC<JobSourceEfficiencyProps> = ({ sources }) => {
  const topSource = sources.length > 0 ? sources[0] : null;

  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-border/40 pb-4 gap-2">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-lg bg-blue-500/10 text-blue-500">
            <Share2 className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-bold text-foreground">Job Source Performance Comparison</h2>
            <p className="text-xs text-muted-foreground">
              Comparative analysis of application channels dynamically compiled from application history.
            </p>
          </div>
        </div>

        {topSource && topSource.submittedCount > 0 && (
          <div className="inline-flex items-center gap-2 rounded-xl bg-amber-500/10 border border-amber-500/20 px-3 py-1.5 text-xs text-amber-500 font-semibold self-start sm:self-auto">
            <Trophy className="h-4 w-4 shrink-0" />
            <span>Top Source: <strong className="text-foreground">{topSource.sourceName}</strong> ({topSource.hrInterviewRate}% HR Conv.)</span>
          </div>
        )}
      </div>

      {sources.length === 0 ? (
        <div className="p-8 text-center text-sm text-muted-foreground">
          No job source performance data available for the active filters.
        </div>
      ) : (
        <div className="space-y-4">
          {/* Channel Cards Grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {sources.map((src, idx) => (
              <div
                key={src.sourceName}
                className="rounded-xl border border-border/50 bg-accent/20 p-4 space-y-3 hover:border-primary/40 transition-all"
              >
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="flex h-6 w-6 items-center justify-center rounded-full bg-primary/10 text-primary text-xs font-bold">
                      #{idx + 1}
                    </span>
                    <h3 className="font-bold text-sm text-foreground">{src.sourceName}</h3>
                  </div>

                  <span className="text-xs font-semibold text-primary bg-primary/10 px-2.5 py-0.5 rounded-full">
                    {src.shareOfTotal}% share
                  </span>
                </div>

                {/* Metrics Row */}
                <div className="grid grid-cols-3 sm:grid-cols-6 gap-1.5 text-center bg-card/80 p-2.5 rounded-lg border border-border/40 text-xs">
                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">Applied</div>
                    <div className="font-bold text-foreground flex items-center justify-center gap-1 mt-0.5">
                      <Send className="h-3 w-3 text-blue-500" /> {src.submittedCount}
                    </div>
                  </div>

                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">Response</div>
                    <div className="font-bold text-purple-500 flex items-center justify-center gap-1 mt-0.5">
                      <RefreshCw className="h-3 w-3 text-purple-500" /> {src.responseRate}%
                    </div>
                  </div>

                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">No Resp.</div>
                    <div className="font-bold text-amber-500 mt-0.5">
                      {src.noResponseRate}%
                    </div>
                  </div>

                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">HR Round</div>
                    <div className="font-bold text-indigo-500 flex items-center justify-center gap-1 mt-0.5">
                      <Users className="h-3 w-3 text-indigo-500" /> {src.hrInterviewRate}%
                    </div>
                  </div>

                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">Tech Round</div>
                    <div className="font-bold text-violet-500 mt-0.5">
                      {src.techInterviewRate}%
                    </div>
                  </div>

                  <div>
                    <div className="text-muted-foreground text-[10px] uppercase font-semibold">Offers</div>
                    <div className="font-bold text-emerald-500 flex items-center justify-center gap-1 mt-0.5">
                      <Award className="h-3 w-3 text-emerald-500" /> {src.offerCount} ({src.offerRate}%)
                    </div>
                  </div>
                </div>

                {/* HR & Offer Conversion Bars */}
                <div className="space-y-1">
                  <div className="flex justify-between text-[11px] font-medium">
                    <span className="text-muted-foreground">HR Interview Conversion</span>
                    <span className="font-bold text-foreground">{src.hrInterviewRate}%</span>
                  </div>
                  <div className="h-2 w-full bg-secondary rounded-full overflow-hidden">
                    <div
                      className="h-full bg-primary rounded-full transition-all duration-500"
                      style={{ width: `${Math.max(src.hrInterviewRate, 2)}%` }}
                    />
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Detailed Performance Table */}
          <div className="overflow-x-auto rounded-xl border border-border/60 bg-card mt-4">
            <table className="w-full text-left text-xs">
              <thead className="border-b border-border/60 bg-accent/40 text-muted-foreground font-semibold">
                <tr>
                  <th className="px-4 py-3">Job Source</th>
                  <th className="px-4 py-3 text-center">Submitted</th>
                  <th className="px-4 py-3 text-center">Response Rate</th>
                  <th className="px-4 py-3 text-center">No Response</th>
                  <th className="px-4 py-3 text-center">HR Rate</th>
                  <th className="px-4 py-3 text-center">Tech Rate</th>
                  <th className="px-4 py-3 text-center">Offers</th>
                  <th className="px-4 py-3 text-center">Offer Rate</th>
                  <th className="px-4 py-3 text-right">Rejections</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border/40">
                {sources.map((src) => (
                  <tr key={src.sourceName} className="hover:bg-accent/20 transition-colors">
                    <td className="px-4 py-2.5 font-bold text-foreground">{src.sourceName}</td>
                    <td className="px-4 py-2.5 text-center font-semibold text-foreground">{src.submittedCount}</td>
                    <td className="px-4 py-2.5 text-center text-purple-500 font-semibold">{src.responseRate}%</td>
                    <td className="px-4 py-2.5 text-center text-amber-500 font-semibold">{src.noResponseRate}% ({src.noResponseCount})</td>
                    <td className="px-4 py-2.5 text-center text-indigo-500 font-semibold">{src.hrInterviewRate}%</td>
                    <td className="px-4 py-2.5 text-center text-violet-500 font-semibold">{src.techInterviewRate}%</td>
                    <td className="px-4 py-2.5 text-center text-emerald-500 font-bold">{src.offerCount}</td>
                    <td className="px-4 py-2.5 text-center text-emerald-500 font-bold">{src.offerRate}%</td>
                    <td className="px-4 py-2.5 text-right text-rose-500 font-semibold">{src.rejectedCount} ({src.rejectionRate}%)</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
};

export default JobSourceEfficiency;
