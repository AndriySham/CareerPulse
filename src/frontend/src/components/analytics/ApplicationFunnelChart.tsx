import React from 'react';
import type { FunnelStageData, AnalyticsKpiData } from '@/api/analytics';
import { STATUS_CONFIG } from '@/components/applications/ApplicationStatusBadge';
import { Filter, ArrowRight, CheckCircle2, XCircle, Clock } from 'lucide-react';

interface ApplicationFunnelChartProps {
  funnelStages: FunnelStageData[];
  kpi: AnalyticsKpiData;
}

export const ApplicationFunnelChart: React.FC<ApplicationFunnelChartProps> = ({
  funnelStages,
  kpi,
}) => {
  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between border-b border-border/40 pb-4">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-lg bg-primary/10 text-primary">
            <Filter className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-bold text-foreground">Application Conversion Funnel</h2>
            <p className="text-xs text-muted-foreground">
              Stage-by-stage pipeline progression and drop-off conversion rates.
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2 text-xs">
          <span className="inline-flex items-center gap-1 font-semibold text-emerald-500 bg-emerald-500/10 px-2.5 py-1 rounded-full">
            <CheckCircle2 className="h-3.5 w-3.5" /> {kpi.offerCount} Offer(s)
          </span>
          <span className="inline-flex items-center gap-1 font-semibold text-rose-500 bg-rose-500/10 px-2.5 py-1 rounded-full">
            <XCircle className="h-3.5 w-3.5" /> {kpi.rejectedCount} Rejected
          </span>
          <span className="inline-flex items-center gap-1 font-semibold text-amber-500 bg-amber-500/10 px-2.5 py-1 rounded-full">
            <Clock className="h-3.5 w-3.5" /> {kpi.noResponseCount} No Response
          </span>
        </div>
      </div>

      {/* Visual Funnel Steps */}
      <div className="space-y-4">
        {funnelStages.map((stage, index) => {
          const config = STATUS_CONFIG[stage.status];
          const Icon = config ? config.icon : Filter;
          const isFinal = index === funnelStages.length - 1;

          return (
            <div key={stage.status} className="relative space-y-1.5">
              <div className="flex flex-col sm:flex-row sm:items-center justify-between text-xs gap-1">
                <div className="flex items-center gap-2 font-semibold text-foreground">
                  <div className={`p-1.5 rounded-md ${stage.color} text-white shrink-0`}>
                    <Icon className="h-3.5 w-3.5" />
                  </div>
                  <span>{stage.label}</span>
                </div>

                <div className="flex items-center gap-3 text-muted-foreground font-medium">
                  <span className="font-bold text-foreground">{stage.count} candidate app(s)</span>
                  <span className="text-primary font-bold">({stage.percentageOfSubmitted}% of submitted)</span>
                  {index > 0 && (
                    <span className="bg-accent px-2 py-0.5 rounded text-[11px] font-semibold text-foreground flex items-center gap-1">
                      <ArrowRight className="h-3 w-3 text-muted-foreground" /> {stage.conversionFromPrevious}% pass rate
                    </span>
                  )}
                </div>
              </div>

              {/* Funnel Progress Bar */}
              <div className="h-3.5 w-full bg-secondary/80 rounded-full overflow-hidden flex shadow-inner">
                <div
                  className={`h-full ${stage.color} transition-all duration-500 rounded-full`}
                  style={{ width: `${Math.max(stage.percentageOfSubmitted, 2)}%` }}
                />
              </div>

              {!isFinal && (
                <div className="pl-4 py-0.5 flex items-center">
                  <div className="h-3 w-0.5 bg-border/80 ml-2" />
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Pipeline Summary Insights Footer */}
      <div className="mt-4 pt-4 border-t border-border/40 grid grid-cols-1 sm:grid-cols-3 gap-3 text-xs">
        <div className="p-3 rounded-lg bg-accent/40 border border-border/40">
          <div className="font-semibold text-foreground">Top Funnel Drop-off</div>
          <div className="text-muted-foreground mt-0.5">
            {kpi.submittedApplications > 0
              ? `${kpi.ghostingRate}% of submitted applications received no response.`
              : 'No applications submitted yet.'}
          </div>
        </div>

        <div className="p-3 rounded-lg bg-accent/40 border border-border/40">
          <div className="font-semibold text-foreground">Interview Conversion</div>
          <div className="text-muted-foreground mt-0.5">
            {kpi.interviewConversionRate}% of applications passed screening into interviews.
          </div>
        </div>

        <div className="p-3 rounded-lg bg-accent/40 border border-border/40">
          <div className="font-semibold text-foreground">Offer Efficiency</div>
          <div className="text-muted-foreground mt-0.5">
            {kpi.interviewCount > 0
              ? `${Math.round((kpi.offerCount / kpi.interviewCount) * 100)}% of interviews converted to job offers.`
              : '0 offers from interviews currently.'}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ApplicationFunnelChart;
