import React from 'react';
import type { AnalyticsKpiData } from '@/api/analytics';
import {
  Send,
  Users,
  Code2,
  Award,
  RefreshCw,
  Link2,
  TrendingUp,
  FileCheck,
  Ghost,
  XCircle,
} from 'lucide-react';

interface AnalyticsKpiGridProps {
  kpi: AnalyticsKpiData;
}

export const AnalyticsKpiGrid: React.FC<AnalyticsKpiGridProps> = ({ kpi }) => {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-4">
      {/* 1. Total Applied (Submitted Applications) */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-blue-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Total Applied</span>
          <div className="p-2 rounded-lg bg-blue-500/10 text-blue-500 group-hover:bg-blue-500 group-hover:text-white transition-colors">
            <Send className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.submittedApplications}
          </div>
          <span className="text-[11px] text-muted-foreground flex items-center gap-1 font-medium">
            <FileCheck className="h-3 w-3 text-blue-500" /> {kpi.totalApplications} incl. drafts
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Excludes {kpi.draftApplications} draft preparation(s)
        </div>
      </div>

      {/* 2. Employer Response Rate */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-purple-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Employer Response Rate</span>
          <div className="p-2 rounded-lg bg-purple-500/10 text-purple-500 group-hover:bg-purple-500 group-hover:text-white transition-colors">
            <RefreshCw className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.responseRate}%
          </div>
          <span className="text-[11px] font-semibold text-purple-500 bg-purple-500/10 px-2 py-0.5 rounded-full">
            Feedback received
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Excludes No Response & Drafts
        </div>
      </div>

      {/* 3. Ghosting / No Response Rate */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-amber-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">No Response / Ghosting</span>
          <div className="p-2 rounded-lg bg-amber-500/10 text-amber-500 group-hover:bg-amber-500 group-hover:text-white transition-colors">
            <Ghost className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.ghostingRate}%
          </div>
          <span className="text-[11px] font-semibold text-amber-500 bg-amber-500/10 px-2 py-0.5 rounded-full">
            {kpi.noResponseCount} ghosted app(s)
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Unanswered submissions
        </div>
      </div>

      {/* 4. Rejections & Rejection Rate */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-rose-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Rejections</span>
          <div className="p-2 rounded-lg bg-rose-500/10 text-rose-500 group-hover:bg-rose-500 group-hover:text-white transition-colors">
            <XCircle className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.rejectedCount}
          </div>
          <span className="text-[11px] font-semibold text-rose-500 bg-rose-500/10 px-2 py-0.5 rounded-full">
            {kpi.rejectionRate}% Rejection Rate
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Formal rejection notices
        </div>
      </div>

      {/* 5. HR Interviews */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-indigo-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">HR Interviews</span>
          <div className="p-2 rounded-lg bg-indigo-500/10 text-indigo-500 group-hover:bg-indigo-500 group-hover:text-white transition-colors">
            <Users className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.hrInterviewCount}
          </div>
          <span className="text-[11px] font-semibold text-indigo-500 flex items-center gap-0.5">
            <TrendingUp className="h-3 w-3" /> {kpi.interviewConversionRate}% rate
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Screening rounds reached
        </div>
      </div>

      {/* 6. Technical Interviews */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-violet-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Technical Interviews</span>
          <div className="p-2 rounded-lg bg-violet-500/10 text-violet-500 group-hover:bg-violet-500 group-hover:text-white transition-colors">
            <Code2 className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.techInterviewCount}
          </div>
          <span className="text-[11px] font-semibold text-violet-500 bg-violet-500/10 px-2 py-0.5 rounded-full">
            {kpi.techRoundRate}% Tech Rate
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Coding & Tech assessments
        </div>
      </div>

      {/* 7. Total Offers */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-emerald-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Total Offers</span>
          <div className="p-2 rounded-lg bg-emerald-500/10 text-emerald-500 group-hover:bg-emerald-500 group-hover:text-white transition-colors">
            <Award className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.offerCount}
          </div>
          <span className="text-[11px] font-semibold text-emerald-500 bg-emerald-500/10 px-2 py-0.5 rounded-full">
            {kpi.offerConversionRate}% Offer Rate
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Job offer win count
        </div>
      </div>

      {/* 8. Vacancy Linkage Rate */}
      <div className="group rounded-xl border border-border/60 bg-card p-4 shadow-sm hover:border-cyan-500/40 hover:shadow-md transition-all">
        <div className="flex items-center justify-between">
          <span className="text-xs font-medium text-muted-foreground">Vacancy Linked</span>
          <div className="p-2 rounded-lg bg-cyan-500/10 text-cyan-500 group-hover:bg-cyan-500 group-hover:text-white transition-colors">
            <Link2 className="h-4 w-4" />
          </div>
        </div>
        <div className="mt-3 flex items-baseline justify-between">
          <div className="text-2xl font-bold tracking-tight text-foreground">
            {kpi.vacancyLinkageRate}%
          </div>
          <span className="text-[11px] text-cyan-500 font-medium bg-cyan-500/10 px-2 py-0.5 rounded-full">
            Structured vacancy
          </span>
        </div>
        <div className="mt-2 text-[11px] text-muted-foreground/80">
          Specific listing attached
        </div>
      </div>
    </div>
  );
};

export default AnalyticsKpiGrid;
