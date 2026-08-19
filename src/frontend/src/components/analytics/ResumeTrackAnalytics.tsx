import React from 'react';
import type { ResumeRevisionMetric } from '@/api/analytics';
import { FileUser, Sparkles, ArrowUpRight, CheckCircle, Clock } from 'lucide-react';

interface ResumeTrackAnalyticsProps {
  revisions: ResumeRevisionMetric[];
}

export const ResumeTrackAnalytics: React.FC<ResumeTrackAnalyticsProps> = ({ revisions }) => {
  const topRevision = revisions.length > 0
    ? [...revisions].sort((a, b) => b.hrInterviewRate - a.hrInterviewRate)[0]
    : null;

  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-border/40 pb-4 gap-2">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-lg bg-purple-500/10 text-purple-500">
            <FileUser className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-bold text-foreground">Resume Revision Performance Comparison</h2>
            <p className="text-xs text-muted-foreground">
              Compare conversion rates, interviews, and offers across resume versions respecting ADR 005 snapshot immutability.
            </p>
          </div>
        </div>

        {topRevision && topRevision.submittedCount > 0 && (
          <div className="inline-flex items-center gap-2 rounded-xl bg-purple-500/10 border border-purple-500/20 px-3 py-1.5 text-xs text-purple-500 font-semibold self-start sm:self-auto">
            <Sparkles className="h-4 w-4 shrink-0" />
            <span>Top Revision: <strong className="text-foreground">v{topRevision.version}</strong> ({topRevision.hrInterviewRate}% HR Conv.)</span>
          </div>
        )}
      </div>

      {revisions.length === 0 ? (
        <div className="p-8 text-center text-sm text-muted-foreground">
          No resume revision history found.
        </div>
      ) : (
        <div className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
            {revisions.map((rev) => (
              <div
                key={rev.revisionId}
                className="rounded-xl border border-border/60 bg-accent/20 p-4 space-y-3 hover:border-primary/40 transition-all flex flex-col justify-between"
              >
                <div>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <span className="font-bold text-sm text-foreground">Version {rev.version}</span>
                      <span
                        className={`text-[11px] font-semibold px-2 py-0.5 rounded-full flex items-center gap-1 ${
                          rev.status === 'Applied'
                            ? 'bg-emerald-500/10 text-emerald-500 border border-emerald-500/20'
                            : 'bg-amber-500/10 text-amber-500 border border-amber-500/20'
                        }`}
                      >
                        {rev.status === 'Applied' ? (
                          <CheckCircle className="h-3 w-3" />
                        ) : (
                          <Clock className="h-3 w-3" />
                        )}
                        {rev.status}
                      </span>
                    </div>

                    <span className="text-xs text-muted-foreground font-medium">
                      {rev.skillsCount} skills
                    </span>
                  </div>

                  <div className="text-xs text-muted-foreground mt-1">
                    Candidate: <span className="font-semibold text-foreground">{rev.fullName}</span>
                  </div>

                  {/* Metrics Stats Grid */}
                  <div className="grid grid-cols-3 sm:grid-cols-6 gap-1 text-center bg-card/80 p-2 rounded-lg border border-border/40 text-xs mt-3">
                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">Applied</div>
                      <div className="font-bold text-foreground mt-0.5">{rev.submittedCount}</div>
                    </div>

                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">Response</div>
                      <div className="font-bold text-purple-500 mt-0.5">{rev.responseRate}%</div>
                    </div>

                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">No Resp.</div>
                      <div className="font-bold text-amber-500 mt-0.5">{rev.noResponseRate}%</div>
                    </div>

                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">HR Rate</div>
                      <div className="font-bold text-indigo-500 mt-0.5">{rev.hrInterviewRate}%</div>
                    </div>

                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">Tech Rate</div>
                      <div className="font-bold text-violet-500 mt-0.5">{rev.techInterviewRate}%</div>
                    </div>

                    <div>
                      <div className="text-muted-foreground text-[10px] uppercase font-semibold">Offers</div>
                      <div className="font-bold text-emerald-500 mt-0.5">{rev.offersCount} ({rev.offerRate}%)</div>
                    </div>
                  </div>
                </div>

                {/* Conversion and Offer Bars */}
                <div className="space-y-2 pt-2 border-t border-border/40">
                  <div className="space-y-1">
                    <div className="flex justify-between text-[11px] font-medium">
                      <span className="text-muted-foreground">HR Interview Conversion</span>
                      <span className="font-bold text-purple-500 flex items-center gap-0.5">
                        {rev.hrInterviewRate}% <ArrowUpRight className="h-3 w-3" />
                      </span>
                    </div>
                    <div className="h-1.5 w-full bg-secondary rounded-full overflow-hidden">
                      <div
                        className="h-full bg-purple-500 rounded-full transition-all duration-500"
                        style={{ width: `${Math.max(rev.hrInterviewRate, 2)}%` }}
                      />
                    </div>
                  </div>

                  <div className="space-y-1">
                    <div className="flex justify-between text-[11px] font-medium">
                      <span className="text-muted-foreground">Offer Win Rate</span>
                      <span className="font-bold text-emerald-500">
                        {rev.offerRate}%
                      </span>
                    </div>
                    <div className="h-1.5 w-full bg-secondary rounded-full overflow-hidden">
                      <div
                        className="h-full bg-emerald-500 rounded-full transition-all duration-500"
                        style={{ width: `${Math.max(rev.offerRate, 2)}%` }}
                      />
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Revision Performance Table */}
          <div className="overflow-x-auto rounded-xl border border-border/60 bg-card mt-4">
            <table className="w-full text-left text-xs">
              <thead className="border-b border-border/60 bg-accent/40 text-muted-foreground font-semibold">
                <tr>
                  <th className="px-4 py-3">Version</th>
                  <th className="px-4 py-3">Status</th>
                  <th className="px-4 py-3 text-center">Submitted</th>
                  <th className="px-4 py-3 text-center">Response Rate</th>
                  <th className="px-4 py-3 text-center">No Response</th>
                  <th className="px-4 py-3 text-center">HR Rate</th>
                  <th className="px-4 py-3 text-center">Tech Rate</th>
                  <th className="px-4 py-3 text-center">Offers</th>
                  <th className="px-4 py-3 text-right">Rejections</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border/40">
                {revisions.map((rev) => (
                  <tr key={rev.revisionId} className="hover:bg-accent/20 transition-colors">
                    <td className="px-4 py-2.5 font-bold text-foreground">v{rev.version} ({rev.fullName})</td>
                    <td className="px-4 py-2.5 font-medium text-muted-foreground">{rev.status}</td>
                    <td className="px-4 py-2.5 text-center font-semibold text-foreground">{rev.submittedCount}</td>
                    <td className="px-4 py-2.5 text-center text-purple-500 font-semibold">{rev.responseRate}%</td>
                    <td className="px-4 py-2.5 text-center text-amber-500 font-semibold">{rev.noResponseRate}% ({rev.noResponseCount})</td>
                    <td className="px-4 py-2.5 text-center text-indigo-500 font-semibold">{rev.hrInterviewRate}% ({rev.hrInterviewCount})</td>
                    <td className="px-4 py-2.5 text-center text-violet-500 font-semibold">{rev.techInterviewRate}% ({rev.techInterviewCount})</td>
                    <td className="px-4 py-2.5 text-center text-emerald-500 font-bold">{rev.offersCount} ({rev.offerRate}%)</td>
                    <td className="px-4 py-2.5 text-right text-rose-500 font-semibold">{rev.rejectionsCount} ({rev.rejectionRate}%)</td>
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

export default ResumeTrackAnalytics;
